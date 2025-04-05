using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConversationManager : MonoBehaviour
{
    [SerializeField] private MicRecorder micRecorder; // ����ũ ���ڴ� Ŭ���� ����
    [SerializeField] private GptRequester gptRequester; // ����Ƽ requester Ŭ���� ����
    [SerializeField] private PhoneScenarioMaker scenarioMaker; // AI �ֹ� ��� �ó����� maker Ŭ���� ����

    private bool isConversationEnded = false;
    protected string emotionTag { get; set; } ///ai �ֹ� ���� �±�

    private void Awake()
    {
        MicRecorder.actionMicRecorded += OnUserSpeechRecognized; // ����ũ ���ڴ����� stt ��ȯ ���� �븮�� ȣ��
        GptRequester.actionGptReceived += OnGPTReplyReceived; // ���� requester���� gpt ���� ���� �븮�� ȣ��
    }

    // Start is called before the first frame update
    void Start()
    {
        //// 1. GPT�� ���� ��ȭ ���� 2. ���� �� ���� ������ GPT ���� ���� �Ŀ� ���۵ǵ��� GptRequester���� ó��
        //gptRequester.RequestGPT(scenarioMaker.ScenarioMaker()); // GPT�� ���� ��ȭ
    }

    public void StartReQuestGPT()
    {
        // 1. GPT�� ���� ��ȭ ���� 2. ���� �� ���� ������ GPT ���� ���� �Ŀ� ���۵ǵ��� GptRequester���� ó��
        gptRequester.RequestGPT(scenarioMaker.ScenarioMaker()); // GPT�� ���� ��ȭ
    }

    // GPT ���� �� ȣ���
    private void OnGPTReplyReceived(string reply, bool isEnd)
    {
        // ���� �ؽ�Ʈ�� �Ľ��ϰų�, TTS�� �ѱ�� �۾��� ���⿡��


        // ������ ��� �и�
        reply = AIEmotionParsing(reply);


        // ��ȭ ���� �� UIó��
        if (isEnd)
        {
            isConversationEnded = true; // ��ȭ ���� ���
            Debug.Log("��ȭ ���� ó��!");
            // ��ȭ UI ����, �ִϸ��̼� ���� ��
            return;
        }
        // GPT ��� ��� �� �� ���� ���� ��� ����
        micRecorder.StartRecording(); // ���� ����ũ ��� ����
    }

    // STT ��� �� GPT ��û
    private void OnUserSpeechRecognized(string userText)
    {
        if (isConversationEnded) return; // ��ȭ ���� ��� return

        gptRequester.RequestGPT(userText); // ���� �� GPT�� ����
    }



    // AI ���� �Ľ�
    private string AIEmotionParsing(string reply)
    {
        // ai ���� ����
        if (reply.StartsWith("["))
        {
            int endIdx = reply.IndexOf("]");
            if (endIdx > 0)
            {
                emotionTag = reply.Substring(1, endIdx - 1);
                return reply.Substring(endIdx + 1).Trim();
            }
        }

        return "ai�� ������ �Էµ��� �ʾҽ��ϴ�.";
    }


    // ���� ��, ��������Ʈ ����
    private void OnDestroy()
    {
        MicRecorder.actionMicRecorded -= OnUserSpeechRecognized;
        GptRequester.actionGptReceived -= OnGPTReplyReceived;
    }
}

