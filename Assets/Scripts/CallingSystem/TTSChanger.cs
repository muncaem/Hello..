using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TTSChanger : MonoBehaviour
{
    //public static Action actionTTSEnded; // TTS ���� �̺�Ʈ
    private AudioSource audioSource; // ����� �÷��� �ҽ�

    private string latestReplyCache; // ������ ��� ĳ��

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Speak(string text, string voice)
    {
        if (text == latestReplyCache)
        {
            Debug.Log("[TTS] ���� �����Դϴ�. Google ��û �����մϴ�.");
            EventHub.actionTTSEnded?.Invoke();
            return;
        }

        latestReplyCache = text;

        // ���� ��� ���� ����
        float pitch, speakingRate;
        GetTTSStyleFromEmotion(ConversationManager.GlobalEmotionTag, out pitch, out speakingRate);

        StartCoroutine(RequestTTS(text, voice, pitch, speakingRate));
    }
    public void NormalSpeak(string text, bool isJustTalk = false)
    {
        StartCoroutine(RequestTTS(text, "ko-KR-Neural2-C", 1, 1, isJustTalk));
    }
    private IEnumerator RequestTTS(string text, string voice, float pitch, float speakingRate, bool isJustTalk = false)
    {
        WWWForm form = new WWWForm();
        form.AddField("text", text);
        form.AddField("voice", voice);  // ��: "ko-KR-Wavenet-A"
        form.AddField("pitch", pitch.ToString());
        form.AddField("speakingRate", speakingRate.ToString());

        using (UnityWebRequest www = UnityWebRequest.Post("http://127.0.0.1:5000/tts", form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                byte[] data = www.downloadHandler.data;
                AudioClip clip = ToAudioClip(data);
                audioSource.clip = clip;
                audioSource.Play();

                yield return new WaitWhile(() => audioSource.isPlaying);

                if (!isJustTalk) // ��ȭ ��Ȳ�� ��츸 TTSEnded ó�� (�� ��ȭ ��, ����)
                    EventHub.actionTTSEnded?.Invoke();
            }
            else
            {
                Debug.LogError("TTS ��û ����: " + www.error);
                // fallback: ������ �״�� ����
                EventHub.actionTTSEnded?.Invoke(); //���� UI ������뵵
            }
        }
    }


    public static AudioClip ToAudioClip(byte[] data)
    {
        try
        {
            WAV wav = new WAV(data);
            AudioClip audioClip = AudioClip.Create("tts_clip", wav.SampleCount, 1, wav.Frequency, false);
            audioClip.SetData(wav.LeftChannel, 0);
            return audioClip;
        }
        catch (Exception e)
        {
            Debug.LogError("WAV ��ȯ ����: " + e.Message);
            return null;
        }
    }

    private void GetTTSStyleFromEmotion(string emotion, out float pitch, out float speakingRate)
    {
        switch (emotion)
        {
            case "ȭ��":
                pitch = 0.9f;
                speakingRate = 1.3f;
                break;
            case "����":
                pitch = 0.9f;
                speakingRate = 1f;
                break;
            default:
                pitch = 1.0f;
                speakingRate = 1.0f;
                break;
        }
    }
}
