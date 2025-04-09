using UnityEngine;
using System.IO;
using UnityEngine.Networking;
using System.Collections;
using System;

public class MicRecorder : MonoBehaviour
{
    private AudioClip recordedClip;
    private bool isRecording = false;
    private float silenceTimer = 0f;
    private const float silenceThreshold = 0.01f; // 감지 기준 볼륨
    private const float maxSilenceTime = 2f;      // 침묵 지속 시간

    private string micDevice;
    private int sampleWindow = 128;

    public static Action<string> actionMicRecorded; // stt 변환 이후 대리자 호출

    [System.Serializable]
    public class SttResponse
    {
        public string text;
    }

    void Update()
    {
        if (isRecording)
        {
            float volume = GetMicVolume();

            if (volume < silenceThreshold)
            {
                silenceTimer += Time.deltaTime;

                if (silenceTimer >= maxSilenceTime)
                {
                    StopAndSendRecording(); // maxSilenceTime초 이상 조용하면 자동 종료
                }
            }
            else
            {
                silenceTimer = 0f; // 소리 감지되면 타이머 초기화
            }
        }
    }

    /// <summary>
    /// 마이크 녹음 시작 (최대 10초, 16kHz)
    /// </summary>
    public void StartRecording()
    {
        micDevice = Microphone.devices[0];
        recordedClip = Microphone.Start(micDevice, false, 10, 16000); // 최대 10초 녹음
        isRecording = true;
        silenceTimer = 0f;
        Debug.Log("녹음 시작!");
    }

    float GetMicVolume()
    {
        // 현재 마이크 위치에서 샘플을 가져오기 위해 위치 계산
        int micPosition = Microphone.GetPosition(null) - sampleWindow;
        if (micPosition < 0) return 0f; // 위치가 음수면 아무것도 안함

        // sampleWindow 크기만큼 오디오 데이터를 받아올 버퍼 생성
        float[] samples = new float[sampleWindow];
        recordedClip.GetData(samples, micPosition); // 오디오 샘플 데이터 받아옴

        float sum = 0f;
        for (int i = 0; i < sampleWindow; i++)
        {
            // RMS (Root Mean Square) 계산을 위한 제곱합
            sum += samples[i] * samples[i];
        }

        // 제곱합 평균에 루트를 씌워 RMS(Root Mean Square) 계산 → "소리 크기"의 척도
        return Mathf.Sqrt(sum / sampleWindow);
    }

    /// <summary>
    /// 마이크 녹음 종료 및 서버로 전송
    /// </summary>
    public void StopAndSendRecording()
    {
        if (!isRecording) return;

        isRecording = false;
        Microphone.End(micDevice);
        Debug.Log("녹음 끝!");

        // AudioClip → WAV 바이트 변환 (임시 저장 안함)
        byte[] wavBytes = WavUtility.FromAudioClip(recordedClip, out _, false);

        // 바로 서버로 업로드
        StartCoroutine(UploadWavBytes(wavBytes));
    }

    /// <summary>
    /// Flask 서버로 byte[] 전송 (HTTP POST)
    /// </summary>
    IEnumerator UploadWavBytes(byte[] wavData)
    {
        WWWForm form = new WWWForm();
        form.AddBinaryData("audio", wavData, "recorded.wav", "audio/wav");

        using (UnityWebRequest www = UnityWebRequest.Post("http://127.0.0.1:5000/stt", form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                //Debug.Log("STT 응답: " + www.downloadHandler.text);
                string json = www.downloadHandler.text;
                SttResponse res = JsonUtility.FromJson<SttResponse>(json);
                Debug.Log("인식된 텍스트: " + res.text);

                actionMicRecorded?.Invoke(res.text);
                //OnUserSpeechRecognized(res.text);
            }
            else
            {
                Debug.LogError("STT 서버 요청 실패: " + www.error);
            }
        }
    }
}
