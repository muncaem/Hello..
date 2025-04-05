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
    public static Action<string, bool> actionGptReceived; // gpt ���� ���� �븮�� ȣ��

    /// Flask���� ���� ���� ������ �ּ�
    private string apiUrl = "http://127.0.0.1:5000/generate";


    // AI ��ȭ ���� �Լ�
    /// �ܺο��� ȣ���� �Լ� (������Ʈ�� �޾� �ڷ�ƾ ����)
    /// <param name="prompt">AI���� ���� �ó����� �Ǵ� ���� ����</param>
    public void RequestGPT(string prompt)
    {
        // �ڷ�ƾ���� GPT�� ��û ����
        StartCoroutine(SendPrompt(prompt));
    }
    /// ������ GPT ������ POST ��û�� ������ �Լ�
    /// <param name="prompt">GPT���� ������ �޽���</param>
    IEnumerator SendPrompt(string prompt)
    {
        // JSON �������� prompt ���α�
        string json = "{\"prompt\":\"" + prompt + "\"}";

        // ���ڿ��� byte �迭�� ��ȯ
        byte[] postData = System.Text.Encoding.UTF8.GetBytes(json);

        // POST ������� UnityWebRequest ����
        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        request.uploadHandler = new UploadHandlerRaw(postData);        // ������ ������ ����
        request.downloadHandler = new DownloadHandlerBuffer();         // ���� ���� �ڵ鷯 ����
        request.SetRequestHeader("Content-Type", "application/json");  // ��û ��� ���� (JSON)

        // ��û�� ������ ������ ���� ���
        yield return request.SendWebRequest();

        // ��� ó��
        if (request.result == UnityWebRequest.Result.Success)
        {
            // ��û ����: GPT ����
            string comment = request.downloadHandler.text;

            // ���� Ű���� üũ �� �Ŵ������� ��������Ʈ
            actionGptReceived?.Invoke(comment, AITalkEndCheck(comment));

            jsonResponse res = JsonUtility.FromJson<jsonResponse>(comment);
            Debug.Log("GPT ���� ����: " + res.reply);
        }
        else
        {
            // ��û ����: ���� �޽��� ���
            Debug.LogError("GPT ��û ����: " + request.error);
        }
    }

    [System.Serializable]
    public class jsonResponse
    {
        public string reply;
    }


    // AI ��ȭ ���� ����Ʈ üũ
    private bool AITalkEndCheck(string reply)
    {
        // ��ȭ ���� Ű���� üũ
        if (reply.Contains("�����մϴ�") || reply.Contains("�˰ڽ��ϴ�") || reply.Contains("�׷�����"))
        {
            return true;
        }
        else return false;
    }
}
