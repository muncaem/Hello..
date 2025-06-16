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
    public AudioClip clip; // ����� Ŭ��
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    [SerializeField]
    AudioSource[] audioSources
        = new AudioSource[System.Enum.GetValues(typeof(SoundType)).Length]; // ���� ���� ī��Ʈ
    [SerializeField] private List<AudioClips> BgmClip; // BGM
    [SerializeField] private List<AudioClips> SfxClip; // ȿ����

    public void Awake()
    {
        // Dont Destroy ����
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        // BGM loop ����
        audioSources[(int)SoundType.BGM].loop = true;
    }

    // ����� �ҽ� ����
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

    // ����� �ҽ� ��� ������ Ȯ��
    public bool IsPlayAudio()
    {
        if (audioSources[(int)SoundType.SFX].isPlaying) return true;
        else return false;
    }

    // ������ ����� �ҽ� ��������
    public AudioClip GetAudioClip(string name, SoundType type = SoundType.SFX)
    {
        List<AudioClips> currentClipList = null;

        // SoundType Ȯ���ؼ� �����Ŭ�� ����Ʈ ����
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

    // ����� �ҽ� �÷���
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