using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundType
{
    BGM,
    SFX,
}

[System.Serializable]
public struct AudioClips
{
    public AudioClip clip; // 오디오 클립
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    [SerializeField]
    AudioSource[] audioSources
        = new AudioSource[System.Enum.GetValues(typeof(SoundType)).Length]; // 사운드 종류 카운트
    [SerializeField] private List<AudioClips> BgmClip; // BGM
    [SerializeField] private List<AudioClips> SfxClip; // 효과음

    public void Awake()
    {
        // Dont Destroy 설정
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        // BGM loop 설정
        audioSources[(int)SoundType.BGM].loop = true;
    }

    // 오디오 소스 비우기
    public void Clear()
    {
        foreach (AudioSource audioSource in audioSources)
        {
            if (audioSource.name == "bell")
            {
                audioSource.clip = null;
                audioSource.Stop();
            }
        }
    }

    // 오디오 소스 재생 중인지 확인
    public bool IsPlayAudio()
    {
        if (audioSources[(int)SoundType.SFX].isPlaying) return true;
        else return false;
    }

    // 설정된 오디오 소스 가져오기
    public AudioClip GetAudioClip(string name, SoundType type = SoundType.SFX)
    {
        List<AudioClips> currentClipList = null;

        // SoundType 확인해서 오디오클립 리스트 선택
        if (type == SoundType.BGM)
        {
            currentClipList = BgmClip;
        }
        else if (type == SoundType.SFX)
        {
            currentClipList = SfxClip;
        }
        else
        {
            Debug.LogWarning("Audio clip selection error!!!");
        }

        AudioClip currentClip = currentClipList.Find(x => x.clip.name.Equals(name)).clip;
        return currentClip;
    }

    // 오디오 소스 플레이
    public void Play(string name, float volume = 0.5f, SoundType type = SoundType.SFX)
    {
        AudioClip audioClip;
        try
        {
            audioClip = GetAudioClip(name, type);
        }
        catch
        {
            Debug.LogError("Can not get Audio Clip");
            return;
        }

        // Bgm
        if (type == SoundType.BGM)
        {
            AudioSource audioSource = audioSources[(int)SoundType.BGM];
            if (audioSource.isPlaying) audioSource.Stop();

            audioSource.volume = volume;
            audioSource.clip = audioClip;
            audioSource.Play();
        }
        // Sfx
        else if (type == SoundType.SFX)
        {
            AudioSource audioSource = audioSources[(int)SoundType.SFX];
            if (audioSource.isPlaying) audioSource.Stop();

            audioSource.volume = volume;
            audioSource.clip = audioClip;
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("Error Playing an Audio Clip");
        }
    }
}