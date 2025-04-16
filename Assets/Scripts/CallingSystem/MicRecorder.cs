using UnityEngine;
using System.IO;
using UnityEngine.Networking;
using System.Collections;
using System;
using System.Security.Cryptography;

public class MicRecorder : MonoBehaviour
{
    private AudioClip recordedClip;
    private bool isRecording = false;
    private float silenceTimer = 0f;
    private const float silenceThreshold = 0.01f; // ���� ���� ����
    private const float maxSilenceTime = 2f;      // ħ�� ���� �ð�
    private int silenceCount = 0; // ħ�� Þ��
    private int maxSilenceCount = 3; // �ִ� ħ�� ���� Ƚ��

    private string micDevice;
    private int sampleWindow = 128;

    public static Action<string> actionMicRecorded; // stt ��ȯ ���� �븮�� ȣ��
    public static Action<string> actionUpdatedFactor; // ������� ���� ������Ʈ �븮�� ȣ��
    public static Action actionEndedBySilence; // ħ���� ���� ��ȭ ���� ���� �� �븮�� ȣ��

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

                CheckPhobiaSymtom(res.text);
            }
            else
            {
                Debug.LogError("STT ���� ��û ����: " + www.error);
            }
        }
    }

    private void CheckPhobiaSymtom(string response)
    {
        if (string.IsNullOrWhiteSpace(response))
        {
            silenceCount++;
            if (silenceCount >= maxSilenceCount)
            {
#if UNITY_EDITOR
                Debug.Log("���� ħ�� ���� - ��ȭ ���� ����");
#endif
                //actionMicRecorded?.Invoke("������ ��� ħ���ϰ� �־�. �ʴ� ȭ�� ���� �������� ��ȭ�� ���ڴٰ� ����.");
                actionMicRecorded?.Invoke("__force_system__:������ �ƹ� ���� ���� �ʰ� ħ���ϰ� �ִ�. �ʴ� ȭ�� ���� ��ȭ�� ���ڴٰ� �Ѵ�.");

                // ȸ�� ���� �߰�
                actionUpdatedFactor?.Invoke("avoid_call");
                // ħ���� ���� ���� ���� üũ
                actionEndedBySilence?.Invoke();
            }
            else
            {
                // ��ȭ �Ҿ� ���� �߰�
                actionUpdatedFactor?.Invoke("hesitate_speaking");
                actionMicRecorded?.Invoke("__force_system__:������ �ƹ� ���� ���� �ʰ� ħ���ϰ� �ִ�. �ʴ� �� �鸮�� �ʴ´ٰ� �Ѵ�.");
            }
        }
        else
        {
            if (response.Contains("��") || response.Contains("��")
                || response.Contains("�𸣰ھ��") || response.Contains("�ۛ��")
                || response.Contains("�ٽ� ��") || response.Contains("�ٽ� �� �� ��"))
            {
                // ��ȭ �Ҿ� ���� �߰�
                actionUpdatedFactor?.Invoke("hesitate_speaking");
            }

            // ���� �Ϸ�. �븮 �Լ� ȣ��
            actionMicRecorded?.Invoke(response);
            silenceCount = 0;
        }
    }
}
