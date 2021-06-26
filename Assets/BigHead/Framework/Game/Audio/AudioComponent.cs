//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年6月26日   |   音效管理组件
//

using System;
using BigHead.Framework.Game;
using UniRx;
using UnityEngine;

public partial class AudioComponent : MonoBehaviour
{
    /// <summary> 音源 </summary>
    private AudioSource _audioSource;

    /// <summary> 监听器 </summary>
    private Listener _listener;

    /// <summary> 是否为音乐 </summary>
    private bool _isMusic;
    
    /// <summary>
    /// 唤醒
    /// </summary>
    private void Awake()
    {
        // 添加监听
        _listener = Listener.Instance;
        _listener?.Add(EnumListener.MusicVolume.ToString(), OnMusicVolumeChanged);
        _listener?.Add(EnumListener.SoundVolume.ToString(), OnSoundVolumeChanged);
        _listener?.Add(EnumListener.MusicMute.ToString(), OnMusicMute);
        _listener?.Add(EnumListener.SoundMute.ToString(), OnSoundMute);
        _listener?.Add(EnumListener.Pause.ToString(), OnPause);
        _listener?.Add(EnumListener.Resume.ToString(), OnResume);
        
        // 修改为音源为3D模式
        _audioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.spatialBlend = 1;
        _audioSource.dopplerLevel = 0;
        _audioSource.spread = 0;
        _audioSource.rolloffMode = AudioRolloffMode.Linear;
        _audioSource.minDistance = 0f;
    }

    /// <summary>
    /// 销毁
    /// </summary>
    private void OnDestroy()
    {
        _listener?.Remove(EnumListener.MusicVolume.ToString(), OnMusicVolumeChanged);
        _listener?.Remove(EnumListener.SoundVolume.ToString(), OnSoundVolumeChanged);
        _listener?.Remove(EnumListener.MusicMute.ToString(), OnMusicMute);
        _listener?.Remove(EnumListener.SoundMute.ToString(), OnSoundMute);
        _listener?.Remove(EnumListener.Pause.ToString(), OnPause);
        _listener?.Remove(EnumListener.Resume.ToString(), OnResume);
    }

    /// <summary>
    /// 音乐音量响应函数
    /// </summary>
    private void OnMusicVolumeChanged(object[] param)
    {
        if (_isMusic)_audioSource.volume = (float) param[0];
    }
    
    /// <summary>
    /// 音效音量响应函数
    /// </summary>
    private void OnSoundVolumeChanged(object[] param)
    {
        if (!_isMusic) _audioSource.volume = (float) param[0];
    }

    /// <summary>
    /// 音效静音响应函数
    /// </summary>
    private void OnSoundMute(object[] param)
    {
        if(!_isMusic) _audioSource.mute = (bool) param[0];
    }

    /// <summary>
    /// 音乐静音响应函数
    /// </summary>
    private void OnMusicMute(object[] param)
    {
        if(_isMusic) _audioSource.mute = (bool) param[0];
    }

    /// <summary>
    /// 暂停
    /// </summary>
    private void OnPause(object[] param)
    {
        _audioSource.Pause();
    }

    /// <summary>
    /// 恢复
    /// </summary>
    private void OnResume(object[] param)
    {
        _audioSource.UnPause();
    }

    /// <summary>
    /// 播放
    /// </summary>
    /// <param name="parent">跟随的物体</param>
    /// <param name="audioClip">播放的音频</param>
    /// <param name="isMusic">是否为音乐（音乐为循环）</param>
    /// <param name="maxDistance">最远播放距离</param>
    public void Play(Transform parent, AudioClip audioClip, bool isMusic, float maxDistance = 500f)
    {
        var t = transform;
        t.SetParent(parent);
        t.localPosition = Vector3.zero;
        gameObject.SetActive(true);

        _isMusic = isMusic;
        _audioSource.volume = isMusic ? AudioManager.MusicVolume : AudioManager.SoundVolume;
        _audioSource.mute = isMusic ? AudioManager.MusicMuteState : AudioManager.SoundMuteState;
        _audioSource.clip = audioClip;
        _audioSource.loop = isMusic;
        _audioSource.maxDistance = maxDistance;
        _audioSource.Play();

        if (isMusic) 
            return;
        
        Observable
            .Timer(TimeSpan.FromSeconds(audioClip.length))
            .Subscribe(_ => AudioManager.Recycle(this));
    }


    public void OnRecycle()
    {
        _audioSource.Stop();
        _audioSource.clip = null;
        gameObject.SetActive(false);
    }

    public void OnClear()
    {
        Destroy(gameObject);
    }
}