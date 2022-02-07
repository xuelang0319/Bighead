//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年6月26日   |   音效管理器
//

using BigHead.Framework.Game;
using BigHead.Framework.Pool;
using UniRx;
using UnityEngine;

public static class AudioManager
{
    /// <summary> 音效池 </summary>
    private static MonoPool<AudioComponent> _audioPool;

    /// <summary> 监听器 </summary>
    private static GameListener _listener;

    /// <summary> 音效音量 </summary>
    private static IReactiveProperty<float> _soundVolume;

    /// <summary> 音乐音量 </summary>
    private static IReactiveProperty<float> _musicVolume;

    /// <summary> 音效静音状态 </summary>
    private static IReactiveProperty<bool> _soundMuteState;

    /// <summary> 音乐静音状态 </summary>
    private static IReactiveProperty<bool> _musicMuteState;

    /// <summary> 音效音量接口 </summary>
    public static float SoundVolume => _soundVolume.Value;

    /// <summary> 音乐音量接口 </summary>
    public static float MusicVolume => _musicVolume.Value;

    /// <summary> 音效静音状态接口 </summary>
    public static bool SoundMuteState => _soundMuteState.Value;

    /// <summary> 音乐状态接口 </summary>
    public static bool MusicMuteState => _musicMuteState.Value;
    
    /// <summary> 音乐控件 </summary>
    private static AudioComponent _musicComponent;

    /// <summary> 音效收听器 </summary>
    private static AudioListener _audioListener;

    
    /// <summary> 音效音量本地存储键 </summary>
    private const string PrefSoundVolume = "Bighead_SoundVolume";
    /// <summary> 音乐音量本地存储键 </summary>
    private const string PrefMusicVolume = "Bighead_MusicVolume";
    /// <summary> 音效静音状态本地存储键 </summary>
    private const string PrefSoundMute = "Bighead_SoundMute";
    /// <summary> 音乐静音状态本地存储键 </summary>
    private const string PrefMusicMute = "Bighead_MusicMute";

    /// <summary>
    /// 构造状态
    /// </summary>
    static AudioManager()
    {
        _listener = GameListener.Instance;
        
        _audioPool = PoolAssistant.GetMonoGlobalPool(
            "AudioManager",
            () => new GameObject("AudioComponent").AddComponent<AudioComponent>(),
            component => component.OnRecycle(),
            component => component.OnClear());

        var soundVolume = PlayerPrefs.HasKey(PrefSoundVolume) ? PlayerPrefs.GetFloat(PrefSoundVolume) : 1;
        _soundVolume = new ReactiveProperty<float>(soundVolume);
        _soundVolume.Subscribe(_ =>
        {
            _listener.Broadcast(SystemListenerKeys.SoundVolume, _soundVolume.Value);
            PlayerPrefs.SetFloat(PrefSoundVolume, _soundVolume.Value);
        });
        
        var musicVolume = PlayerPrefs.HasKey(PrefMusicVolume) ? PlayerPrefs.GetFloat(PrefMusicVolume) : 1;
        _musicVolume = new ReactiveProperty<float>(musicVolume);
        _musicVolume.Subscribe(_ =>
        {
            _listener.Broadcast(SystemListenerKeys.MusicVolume, _musicVolume.Value);
            PlayerPrefs.SetFloat(PrefMusicVolume,_musicVolume.Value);
        });
        
        var soundMute = PlayerPrefs.HasKey(PrefSoundMute) ? PlayerPrefs.GetInt(PrefSoundMute) : 0;
        _soundMuteState = new ReactiveProperty<bool>(Equals(soundMute, 1));
        _soundMuteState.Subscribe(_ =>
        {
            _listener.Broadcast(SystemListenerKeys.SoundMute, _soundMuteState.Value);
            PlayerPrefs.SetInt(PrefSoundMute, Equals(_soundMuteState.Value, true) ? 1 : 0);
        });
        
        var musicMute = PlayerPrefs.HasKey(PrefMusicMute) ? PlayerPrefs.GetInt(PrefMusicMute) : 0;
        _musicMuteState = new ReactiveProperty<bool>(Equals(musicMute, 1));
        _musicMuteState.Subscribe(_ =>
        {
            _listener.Broadcast(SystemListenerKeys.MusicMute, _musicMuteState.Value);
            PlayerPrefs.SetInt(PrefMusicMute, Equals(_musicMuteState.Value, true) ? 1 : 0);
        });
    }

    /// <summary>
    /// 播放3D音效
    /// </summary>
    /// <param name="parent">跟随父物体</param>
    /// <param name="audioClip">音效</param>
    /// <param name="isMusic">是否为音乐（音乐为循环且唯一）</param>
    /// <param name="maxDistance">最远播放距离</param>
    public static void Play(Transform parent, AudioClip audioClip, bool isMusic, float maxDistance = 500f)
    {
        if (isMusic)
        {
            if (!_musicComponent) 
                _musicComponent = _audioPool.Get();

            if (!_audioListener)
            {
                _audioListener = Object.FindObjectOfType<AudioListener>();
                if (!_audioListener)
                {
                    "检查到场景不否存在声音接收器。".Exception();
                    return;
                }
            }

            _musicComponent.Play(_audioListener.transform, audioClip, true, maxDistance);
        }
        else
        {
            _audioPool.Get().Play(parent, audioClip, false, maxDistance);
        }
    }

    /// <summary>
    /// 播放音效,可用于播放2D音效或UI音效
    /// </summary>
    /// <param name="audioClip">音效</param>
    /// <param name="isMusic">是否为音乐（音乐为循环且唯一）</param>
    public static void Play(AudioClip audioClip, bool isMusic)
    {
        if (!_audioListener)
        {
            _audioListener = Object.FindObjectOfType<AudioListener>();
            if (!_audioListener)
            {
                "检查到场景不否存在声音接收器。".Exception();
                return;
            }
        }
        
        if(isMusic && !_musicComponent)
            _musicComponent = _audioPool.Get();

        var audioComponent = isMusic ? _musicComponent : _audioPool.Get();
        audioComponent.Play(_audioListener.transform, audioClip, isMusic);
    }

    /// <summary>
    /// 回收音源组件
    /// </summary>
    public static void Recycle(AudioComponent audioComponent)
    {
        _audioPool.Recycle(audioComponent);
    }

    /// <summary>
    /// 设置音效静音状态
    /// </summary>
    public static void SetSoundMute(bool isMute) =>
        _soundMuteState.Value = isMute;

    /// <summary>
    /// 设置音乐静音状态
    /// </summary>
    public static void SetMusicMute(bool isMute) =>
        _musicMuteState.Value = isMute;

    /// <summary>
    /// 设置音效音量
    /// </summary>
    public static void SetSoundVolume(int value) =>
        _soundVolume.Value = Mathf.Clamp(value, 0, 100) / 100f;

    /// <summary>
    /// 设置音乐音量
    /// </summary>
    public static void SetMusicVolume(int value) =>
        _musicVolume.Value = Mathf.Clamp(value, 0, 100) / 100f;

    /// <summary>
    /// 设置所有音量
    /// </summary>
    public static void SetAllVolume(int value)
    {
        SetSoundVolume(value);
        SetMusicVolume(value);
    }
}