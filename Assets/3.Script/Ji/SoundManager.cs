 using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }
    
    [field: SerializeField] public AudioSource bgmSource { get; set; }
    [field: SerializeField] public AudioSource sfxSource { get; set; }

    [Header("Audio Clips")]
    public AudioClip[] bgmClips;
    public AudioClip[] sfxClips;

    private Dictionary<string, AudioClip> bgmDict;
    private Dictionary<string, AudioClip> sfxDict;

    [Header("Default Volumes")]
    [Range(0f, 1f)] public float defaultBgmVolume = 0.4f;
    [Range(0f, 1f)] public float defaultSfxVolume = 0.7f;

    private const string BgmVolumeKey = "BGM_VOLUME";
    private const string SfxVolumeKey = "SFX_VOLUME";

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadVolumes();
        CacheClips();
    }

    private void LoadVolumes()
    {
        float bgmVol = PlayerPrefs.GetFloat(BgmVolumeKey, defaultBgmVolume);
        float sfxVol = PlayerPrefs.GetFloat(SfxVolumeKey, defaultSfxVolume);

        SetBgmVolume(bgmVol);
        SetSfxVolume(sfxVol);
    }

    private void CacheClips()
    {
        bgmDict = new Dictionary<string, AudioClip>();
        sfxDict = new Dictionary<string, AudioClip>();
        
        foreach (var clip in bgmClips)
        {
            if (clip != null)
                bgmDict.TryAdd(clip.name, clip);
        }

        foreach (var clip in sfxClips)
        {
            if (clip != null)
                sfxDict.TryAdd(clip.name, clip);
        }
        
        Debug.Log($"bgmDict : {bgmDict.Count}");
    }

    public void SetBgmVolume(float volume)
    {
        bgmSource.volume = volume;
        PlayerPrefs.SetFloat(BgmVolumeKey, volume);
    }

    public void SetSfxVolume(float volume)
    {
        sfxSource.volume = volume;
        PlayerPrefs.SetFloat(SfxVolumeKey, volume);
    }

    public void PlaySfx(string clipName) //Sfx -> 짧은 효과음
    {
        if (sfxDict.TryGetValue(clipName, out var clip))
        {
            sfxSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"SFX clip not found: {clipName}");
        }
    }

    public void PlayBgm(string clipName, bool loop = true) //Bgm -> 배경음악 등 긴 것
    {
        foreach (var clip1 in bgmDict)
        {
            Debug.Log($"clip1 : {clip1.Key}");
        }
        if (bgmDict.TryGetValue(clipName, out var clip))
        {
            if (bgmSource.clip != clip)
            {
                bgmSource.clip = clip;
                bgmSource.loop = loop; //반복 여부
                bgmSource.Play();
            }
        }
        else
        {
            Debug.LogWarning($"BGM clip not found: {clipName}");
        }
    }

    public void StopBgm()
    {
        bgmSource.Stop();
        bgmSource.clip = null;
    }
}