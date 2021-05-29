//
// = The script is part of BigHead and the framework is individually owned by Eric Lee.
// = Cannot be commercially used without the authorization.
//
//  Author  |  UpdateTime     |   Desc  
//  Eric    |  2021年5月29日   |  音效管理器
//

using System;
using System.Collections.Generic;
using BigHead.Framework.Core;
using BigHead.Framework.Pool;
using UniRx;
using UnityEngine;
using Object = UnityEngine.Object;

public static class AudioManager
{
    /// <summary>  音乐播放器  </summary>
    private static AudioSource _music;

    /// <summary>  正在播放的声效列表  </summary>
    private static List<AudioSource> _playingSources;

    /// <summary>  声效对象池  </summary>
    private static MonoPool<AudioSource> _sourcesPool;

    /// <summary>  Audio 父节点  </summary>
    private static Transform _parent;

    /// <summary>  独立ID  </summary>
    private static SingleId _id;

    /// <summary> 音乐静音状态 </summary>
    private static bool _musicMute = false;

    /// <summary> 声效静音状态 </summary>
    private static bool _soundMute = false;

    /// <summary> 音乐音量 </summary>
    private static float _musicVolume = 1f;

    /// <summary> 音效音量 </summary>
    private static float _soundVolume = 1f;

    /// <summary>  音乐静音状态  </summary>
    public static bool MusicMute => _musicMute;

    /// <summary>  音效静音状态  </summary>
    public static bool SoundMute => _soundMute;

    /// <summary>  音乐音量  </summary>
    public static float MusicVolume => _musicVolume;

    /// <summary>  音效音量  </summary>
    public static float SoundVolume => _soundVolume;

    /// <summary>
    /// 构造方法
    /// </summary>
    static AudioManager()
    {
        _parent = new GameObject("AudioNode").transform;

        _music = _GetNewAudioSource("MusicSource");
        _music.transform.SetParent(_parent);

        _sourcesPool = PoolAssistant.GetMonoPool("AudioPool",
            () => _GetNewAudioSource(),
            source =>
            {
                source.Stop();
                source.clip = null;
                source.gameObject.SetActive(false);
            },
            source => Object.Destroy(source.gameObject));

        _playingSources = new List<AudioSource>();

        _id = SingleId.New();

        Object.DontDestroyOnLoad(_parent);
        Object.DontDestroyOnLoad(_sourcesPool.Parent);
    }

    /// <summary>
    /// 获取新的 AudioSource 组件方法
    /// </summary>
    static AudioSource _GetNewAudioSource(string name = "")
    {
        name = string.IsNullOrEmpty(name) ? $"Audio_{_id.Get()}" : name;
        return new GameObject(name).AddComponent<AudioSource>();
    }

    /// <summary>
    /// 播放
    /// </summary>
    public static void Play(AudioClip clip, bool isMusic)
    {
        if (isMusic)
        {
            _music.clip = clip;
            _music.loop = true;
            _music.Play();
        }
        else
        {
            var sound = _sourcesPool.Get();
            sound.mute = _soundMute;
            sound.volume = _soundVolume;
            sound.loop = false;
            sound.clip = clip;
            sound.Play();

            Observable
                .Timer(TimeSpan.FromSeconds(clip.length))
                .Subscribe(_ => _sourcesPool.Recycle(sound));
        }
    }

    /// <summary>
    /// 设置静音
    /// </summary>
    public static void SetMute(bool isMusic, bool isMute)
    {
        if (isMusic)
        {
            _musicMute = isMute;
            _music.mute = isMute;
        }
        else
        {
            _soundMute = isMute;
            foreach (var audioSource in _playingSources)
            {
                audioSource.mute = isMute;
            }
        }
    }

    /// <summary>
    /// 设置音量 Range 0f - 1f
    /// </summary>
    public static void SetVolume(bool isMusic, float value)
    {
        if (value > 1f) value = 1f;
        if (value < 0f) value = 0f;

        if (isMusic)
        {
            _musicVolume = value;
            _music.volume = value;
        }
        else
        {
            _soundVolume = value;
            foreach (var audioSource in _playingSources)
            {
                audioSource.volume = value;
            }
        }
    }

    /// <summary>
    /// 设置音量 Range 0 - 100
    /// </summary>
    public static void SetVolume(bool isMusic, int value)
    {
        SetVolume(isMusic, (float) value / 100f);
    }

    /// <summary>
    /// 暂停音乐
    /// </summary>
    public static void PauseMusic() =>
        _music.Pause();

    /// <summary>
    /// 继续播放音乐
    /// </summary>
    public static void UnPauseMusic()
    {
        _music.mute = _musicMute;
        _music.volume = _musicVolume;
        _music.UnPause();
    }
}