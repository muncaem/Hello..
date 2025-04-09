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
    private const float silenceThreshold = 0.01f; // ���� ���� ����
    private const float maxSilenceTime = 2f;      // ħ�� ���� �ð�

    private string micDevice;
    private int sampleWindow = 128;

    public static Action<string> actionMicRecorded; // stt ��ȯ ���� �븮�� ȣ��

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
                    StopAndSendRecording(); // maxSilenceTime�� �̻� �����ϸ� �ڵ� ����
                }
            }
            else
            {
                silenceTimer = 0f; // �Ҹ� �����Ǹ� Ÿ�̸� �ʱ�ȭ
            }
        }
    }

    /// <summary>
    /// ����ũ ���� ���� (�ִ� 10��, 16kHz)
    /// </summary>
    public void StartRecording()
    {
        micDevice = Microphone.devices[0];
        recordedClip = Microphone.Start(micDevice, false, 10, 16000); // �ִ� 10�� ����
        isRecording = true;
        silenceTimer = 0f;
        Debug.Log("���� ����!");
    }

    float GetMicVolume()
    {
        // ���� ����ũ ��ġ���� ������ �������� ���� ��ġ ���
        int micPosition = Microphone.GetPosition(null) - sampleWindow;
        if (micPosition < 0) return 0f; // ��ġ�� ������ �ƹ��͵� ����

        // sampleWindow ũ�⸸ŭ ����� �����͸� �޾ƿ� ���� ����
        float[] samples = new float[sampleWindow];
        recordedClip.GetData(samples, micPosition); // ����� ���� ������ �޾ƿ�

        float sum = 0f;
        for (int i = 0; i < sampleWindow; i++)
        {
            // RMS (Root Mean Square) ����� ���� ������
            sum += samples[i] * samples[i];
        }

        // ������ ��տ� ��Ʈ�� ���� RMS(Root Mean Square) ��� �� "�Ҹ� ũ��"�� ô��
        return Mathf.Sqrt(sum / sampleWindow);
    }

    /// <summary>
    /// ����ũ ���� ���� �� ������ ����
    /// </summary>
    public void StopAndSendRecording()
    {
        if (!isRecording) return;

        isRecording = false;
        Microphone.End(micDevice);
        Debug.Log("���� ��!");

        // AudioClip �� WAV ����Ʈ ��ȯ (�ӽ� ���� ����)
        byte[] wavBytes = WavUtility.FromAudioClip(recordedClip, out _, false);

        // �ٷ� ������ ���ε�
        StartCoroutine(UploadWavBytes(wavBytes));
    }

    /// <summary>
    /// Flask ������ byte[] ���� (HTTP POST)
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
                //Debug.Log("STT ����: " + www.downloadHandler.text);
                string json = www.downloadHandler.text;
                SttResponse res = JsonUtility.FromJson<SttResponse>(json);
                Debug.Log("�νĵ� �ؽ�Ʈ: " + res.text);

                actionMicRecorded?.Invoke(res.text);
                //OnUserSpeechRecognized(res.text);
            }
            else
            {
                Debug.LogError("STT ���� ��û ����: " + www.error);
            }
        }
    }
}
