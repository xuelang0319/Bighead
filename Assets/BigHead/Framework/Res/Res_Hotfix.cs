//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年7月11日   |   Addressable 热更新
//

using System;
using System.Collections;
using System.Collections.Generic;
using BigHead.Framework.Core;
using BigHead.Framework.Game;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;

/*
 * 开启热更功能步骤：
 * 1、打开Group的设置。
 * 2、勾选Build Remote Catalog.
 * 3、勾选Disable Catalog Update.
 * 4、设置BuildPath = RemoteBuildPath, 并且设置远程打包目标地址。
 * 5、设置LoadPath = RemoteLoadPath, 并且设置远程加载目标地址。
 * 6、如果使用虚拟服务器，则打开Addressables Hosting. 直接开启Enable，系统会默认设置端口(Port)。同时可以设置UploadSpeed等限速设置。
 * 7、创建不同的Addressables Profiles， 测试远端需要将Play Mode Script设置为 Use Existing Build(requires built groups),
 * 8、测试期间可以不修改BuildPath，如果为本机虚拟服务器远端测试，则需要将Profiles中RemoteLoadPath设置为：http://[PrivateIpAddress]:[HostingServicePort]宏。
 * 注意： [PrivateIpAddress] 和 [HostingServicePort] 在Addressables Hosting中VariableName可以看见。
 */


public static class AddressableHotfix
{
    public enum EnumHotfix
    {
        /// <summary> 检查更新失败 </summary>
        CheckFailed,
        /// <summary> 获取更新列表失败 </summary>
        UpdateFailed,
        /// <summary> 下载失败 </summary>
        DownloadFailed,
        /// <summary> 获取更新列表成功 </summary>
        UpdateSuccess,
        /// <summary> 更新完成 </summary>
        Finish,
        /// <summary> 更新进度 </summary>
        Downloading,
        /// <summary> 更新大小 </summary>
        DownloadSize,
    }
    
    private static List<object> _catalogs = new List<object>();

    public static IEnumerator CheckCatalogs()
    {
        yield return Addressables.InitializeAsync();
        
        var catalogs = new List<string>();
        var checkHandler = Addressables.CheckForCatalogUpdates(false);
        checkHandler.Completed += op => catalogs.AddRange(op.Result);
        yield return checkHandler;
        Addressables.Release(checkHandler);
        
        if (Equals(catalogs.Count, 0))
        {
            Broadcast(EnumHotfix.Finish);
            yield break;
        }

        
        var locators = new List<IResourceLocator>();
        var updateHandle = Addressables.UpdateCatalogs(catalogs, false);
        updateHandle.Completed += op => locators.AddRange(op.Result);
        yield return updateHandle;
        Addressables.Release(updateHandle);

        if (Equals(locators.Count, 0))
        {
            Broadcast(EnumHotfix.Finish);
            yield break;
        }

        foreach (var locator in locators)
            _catalogs.AddRange(locator.Keys);
        
        Broadcast(EnumHotfix.UpdateSuccess);
    }

    public static IEnumerator DownloadDependencies()
    {
        var sizeHandle = Addressables.GetDownloadSizeAsync(_catalogs);
        yield return sizeHandle;

        var downloadSize = sizeHandle.Result;
        Addressables.Release(sizeHandle);
        
        if (Equals((int) downloadSize, 0))
        {
            Broadcast(EnumHotfix.Finish);
            yield break;
        }
        
        Broadcast(EnumHotfix.DownloadSize, downloadSize);
        
        var downloadHandle = Addressables.DownloadDependenciesAsync(_catalogs, Addressables.MergeMode.Union);
        while (!downloadHandle.IsDone)
        {
            Broadcast(EnumHotfix.Downloading, downloadHandle.PercentComplete);
            yield return 0;
        }

        yield return downloadHandle;
        var downloadState = downloadHandle.Status;
        Addressables.Release(downloadHandle);

        if (Equals(downloadState, AsyncOperationStatus.Failed))
        {
            Broadcast(EnumHotfix.DownloadFailed);
            yield break;
        }

        Broadcast(EnumHotfix.Finish);
    }
    
    private static void Broadcast(params object[] param) => Listener.Instance.Broadcast(EnumListener.HotfixResponse.ToString(), param);
}

public partial class Res
{
    partial void Awake_Hotfix()
    {
        _listener.Add(EnumListener.HotfixResponse.ToString(), HotfixListener);
    }

    partial void OnDestroy_Hotfix()
    {
    }

    public void StartHotfix()
    {
        switch (_hotfixState)
        {
            case HotfixState.None:
            case HotfixState.Catalogs:
                StartCatalogs();
                break;
            case HotfixState.Download:
                StartDownload();
                break;
            case HotfixState.Finish:
                OnHotfixSuccess?.Invoke();
                "Hotfix has already update success, please do not start more than twice.".Exception();
                break;
            default:
                $"Hotfix found unknow response : {_hotfixState}, please check.".Exception();
                break;
        }
    }

    private void StartCatalogs()
    {
        _hotfixState = HotfixState.Catalogs;
        StartCoroutine(AddressableHotfix.CheckCatalogs());
    }

    private void StartDownload()
    {
        _hotfixState = HotfixState.Download;
        StartCoroutine(AddressableHotfix.DownloadDependencies());
    }

    private enum HotfixState
    {
        None,
        Catalogs,
        Download,
        Finish
    }

    /// <summary>
    /// 默认状态
    /// </summary>
    private HotfixState _hotfixState = HotfixState.None;

    private void HotfixListener(object[] param)
    {
        var hotfixResponse = (AddressableHotfix.EnumHotfix) param[0];
        switch (hotfixResponse)
        {
            case AddressableHotfix.EnumHotfix.UpdateSuccess:
                StartDownload();
                break;
            case AddressableHotfix.EnumHotfix.CheckFailed:
            case AddressableHotfix.EnumHotfix.UpdateFailed:
            case AddressableHotfix.EnumHotfix.DownloadFailed:
                OnHotfixFailed?.Invoke(hotfixResponse);
                break;
            case AddressableHotfix.EnumHotfix.Finish:
                _listener.Remove(EnumListener.HotfixResponse.ToString(), HotfixListener);
                _hotfixState = HotfixState.Finish;
                OnHotfixSuccess?.Invoke();
                break;
            case AddressableHotfix.EnumHotfix.DownloadSize:
                OnHotfixGetDownloadSize?.Invoke((long) param[1]);
                break;
            case AddressableHotfix.EnumHotfix.Downloading:
                OnHotfixDownloadProgressChanged?.Invoke((float) param[1]);
                break;
        }
    }

    /// <summary>
    /// 热更失败监听事件
    /// </summary>
    public event Action<AddressableHotfix.EnumHotfix> OnHotfixFailed;
    /// <summary>
    /// 热更成功监听事件
    /// </summary>
    public event Action OnHotfixSuccess;
    /// <summary>
    /// 热更进度监听事件
    /// </summary>
    public event Action<float> OnHotfixDownloadProgressChanged;
    /// <summary>
    /// 热更获取下载大小监听事件
    /// </summary>
    public event Action<long> OnHotfixGetDownloadSize;
}