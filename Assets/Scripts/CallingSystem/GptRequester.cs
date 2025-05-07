using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using static MicRecorder;

// Flask�� GPT ������ ������Ʈ�� ������ ������ �޾ƿ��� Ŭ����
public class GptRequester : MonoBehaviour
{
    //public static Action<string, bool> actionGptReceived; // gpt ���� ���� �븮�� ȣ��
    private string userId = "user_001";  // ���� ���� ID (���� �α��� �ý��۰� ���� ����)

    public enum GPTRequestType
    {
        Resident,
        Secretary
    }

    /// Flask���� ���� ���� ������ �ּ�
    private string residentApiUrl = "http://127.0.0.1:5000/generate_resident";
    private string secretaryApiUrl = "http://127.0.0.1:5000/generate_secretary";

    // ���� ��û Ŭ����
    [Serializable]
    public class GPTRequest
    {
        public string user_id;
        public string prompt;
    }
    // ���� Ŭ����
    [System.Serializable]
    public class GPTResponse
    {
        public string reply; //json �Ľ̿�
    }

    // AI ��ȭ ���� �Լ�
    /// �ܺο��� ȣ���� �Լ� (������Ʈ�� �޾� �ڷ�ƾ ����)
    /// <param name="prompt">AI���� ���� �ó����� �Ǵ� ���� ����</param>
    public void RequestGPT(string prompt, GPTRequestType type = GPTRequestType.Resident)
    {
        if (string.IsNullOrWhiteSpace(prompt))
        {
            Debug.LogWarning("GPT ��û ����: �� prompt");
            // ����ũ �ν� ���� ��, UI ���� MicRecorder ������ǵ��� ���� ����
            return;
        }

        // �ڷ�ƾ���� GPT�� ��û ����
        string apiUrl = type == GPTRequestType.Resident ? residentApiUrl : secretaryApiUrl;
        StartCoroutine(SendPrompt(prompt, apiUrl));
    }
    /// ������ GPT ������ POST ��û�� ������ �Լ�
    /// <param name="prompt">GPT���� ������ �޽���</param>
    IEnumerator SendPrompt(string prompt, string apiUrl)
    {
        // JSON �������� prompt ���α�
        //string json = "{\"prompt\":\"" + prompt + "\"}";
        // JSON ����: user_id�� prompt
        string json = JsonUtility.ToJson(new GPTRequest
        {
            user_id = userId,
            prompt = prompt
        });

        // ���ڿ��� byte �迭�� ��ȯ
        byte[] postData = System.Text.Encoding.UTF8.GetBytes(json);

        // POST ������� UnityWebRequest ����
        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        request.method = UnityWebRequest.kHttpVerbPOST;
        request.uploadHandler = new UploadHandlerRaw(postData);        // ������ ������ ����
        request.downloadHandler = new DownloadHandlerBuffer();         // ���� ���� �ڵ鷯 ����
        request.SetRequestHeader("Content-Type", "application/json");  // ��û ��� ���� (JSON)

        Debug.Log("���� JSON: " + json);

        // ��û�� ������ ������ ���� ���
        yield return request.SendWebRequest();

        // ��� ó��
        if (request.result == UnityWebRequest.Result.Success)
        {
            // ��û ����: GPT ����
            string rawJson = request.downloadHandler.text;
            GPTResponse res = JsonUtility.FromJson<GPTResponse>(rawJson); // json ��ȯ

            string comment = res.reply; // ��ȯ�� ����

            if (comment.StartsWith("[��]"))
            {
                EventHub.actionGetSecretaryReply?.Invoke(comment);
                Debug.Log("<color=orange>�� GPT ����</color> ����: " + comment);
            }
            else
            {
                // ���� Ű���� üũ �� �Ŵ������� ��������Ʈ
                EventHub.actionGptReceived?.Invoke(comment, AITalkEndCheck(comment));

                Debug.Log("<color=blue>�ֹ� GPT ����</color> ����: " + comment);

                // AI ��ȭ Log ������Ʈ
                EventHub.actionUpdatedAISpeaking?.Invoke(comment);
            }
        }
        else
        {
            // ��û ����: ���� �޽��� ���
            Debug.LogError("GPT ��û ����: " + request.error);
        }
    }



    // AI ��ȭ ���� ����Ʈ üũ
    private bool AITalkEndCheck(string reply)
    {
        // ��ȭ ���� Ű���� üũ
        if (reply.Contains("�����մϴ�") || reply.Contains("����") || reply.Contains("�׷�����") || reply.Contains("�׷� �̸�") || reply.Contains("����"))
        {
            return true;
        }
        else return false;
    }
}
