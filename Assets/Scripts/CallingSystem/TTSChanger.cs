using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TTSChanger : MonoBehaviour
{
    //public static Action actionTTSEnded; // TTS 종료 이벤트
    private AudioSource audioSource; // 오디오 플레이 소스

    private string latestReplyCache; // 마지막 대답 캐싱

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Speak(string text, string voice)
    {
        if (text == latestReplyCache)
        {
            Debug.Log("[TTS] 같은 문장입니다. Google 요청 생략합니다.");
            EventHub.actionTTSEnded?.Invoke();
            return;
        }

        latestReplyCache = text;

        // 감정 기반 말투 조정
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
        form.AddField("voice", voice);  // 예: "ko-KR-Wavenet-A"
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

                if (!isJustTalk) // 대화 상황일 경우만 TTSEnded 처리 (비서 통화 시, 예외)
                    EventHub.actionTTSEnded?.Invoke();
            }
            else
            {
                Debug.LogError("TTS 요청 실패: " + www.error);
                // fallback: 녹음은 그대로 진행
                EventHub.actionTTSEnded?.Invoke(); //실패 UI 날리기용도
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
            Debug.LogError("WAV 변환 실패: " + e.Message);
            return null;
        }
    }

    private void GetTTSStyleFromEmotion(string emotion, out float pitch, out float speakingRate)
    {
        switch (emotion)
        {
            case "화남":
                pitch = 0.9f;
                speakingRate = 1.3f;
                break;
            case "슬픔":
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
