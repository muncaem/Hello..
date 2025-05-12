using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class ConversationManager : MonoBehaviour
{
    [SerializeField] private MicRecorder micRecorder; // ����ũ ���ڴ� Ŭ���� ����
    [SerializeField] private GptRequester gptRequester; // ����Ƽ requester Ŭ���� ����
    [SerializeField] private PhoneScenarioMaker scenarioMaker; // AI �ֹ� ��� �ó����� maker Ŭ���� ����
    [SerializeField] private TTSChanger ttsChanger; // tts ü���� Ŭ���� ����

    private bool isEndCallbySilence = false;
    private bool isConversationEnded = false;
    private string emotionTag; ///ai �ֹ� ���� �±�
    public static string GlobalEmotionTag { get; private set; } // ���� �±� static ����
    private string currentScenario; /// �� ai �ֹ� ��� �ó�����
    private string currentOnlyScenarioContext;
    public static bool GlobalCallState { get; private set; }



    //public static Action actionEndedCall;
    //public static Action actionEndedCallbySilence;

    private void Awake()
    {
        EventHub.OnTakeCall += StartComingConversation; // ���� �ý��ۿ��� ��ȭ�� �޾��� ��� �븮�� ȣ��

        EventHub.actionMicRecorded += OnUserSpeechRecognized; // ����ũ ���ڴ����� stt ��ȯ ���� �븮�� ȣ��
        EventHub.actionGptReceived += OnGPTReplyReceived; // ����Ƽ requester���� gpt ���� ���� �븮�� ȣ��
        EventHub.actionTTSEnded += OnTTSEnded; // ����Ƽ ���� tts ó�� ���� �۾� ȣ��

        EventHub.actionEndedBySilence += OnEndCallbySilence;

        EventHub.actionStartedGoingCall += StartGoingConversation;
    }

    /// <summary>
    /// ����(Incoming) ��ȭ ����
    /// </summary>
    private void StartComingConversation()
    {
#if UNITY_EDITOR
        Debug.Log("StartComingConversation()");
#endif

        ScenarioData data = scenarioMaker.ScenarioMaker();
        currentScenario = $"�ʴ� {data.role}. {data.situation}. {data.emotion} ���¾�. " +
            $"{GetReputationAndSendFriendness()} " + 
            $"�׻� ��� �� �տ� {data.emotionTag}�±׸� �ٿ��� ������ ǥ����.";

        // 1. GPT�� ���� ��ȭ ���� 2. ���� �� ���� ������ GPT ���� ���� �Ŀ� ���۵ǵ��� GptRequester���� ó��
        gptRequester.RequestGPT(currentScenario); // GPT�� ���� ��ȭ
        currentOnlyScenarioContext = data.situation;

        isConversationEnded = false;
        GlobalCallState = true;
    }

    /// <summary>
    /// �۽�(Outgoing) ��ȭ ����
    /// </summary>
    private void StartGoingConversation(ScenarioData data)
    {
#if UNITY_EDITOR
        Debug.Log("StartGoingConversation()");
#endif
        currentScenario = $"�ʴ� {data.role}. {data.situation}. {data.emotion} ���¾�. " +
            $"{GetReputationAndSendFriendness()} " +
            $"�׻� ��� �� �տ� {data.emotionTag}�±׸� �ٿ��� ������ ǥ����.";
        
        gptRequester.RequestGPT(currentScenario);
        currentOnlyScenarioContext = data.situation;

        isConversationEnded = false;
        GlobalCallState = true;
    }

    private string GetReputationAndSendFriendness()
    {
        float currentReputation = UserData.Instance.userReputation;

        if (currentReputation > 50)
            return "������ ������ǥ�� ���� �����ϰ� �� ������� ������ �ణ ���� ������ ������ �־�.";
        else if (currentReputation > 30)
            return "�� ������ ����ϰ� �����߱� ������ Ư���� ���������, �Ǹ������� �ʾ�.";
        else
            return "�������� ������ �ο��� ����� �ذ����� ���ؼ� �ణ �Ҹ��� ������ �־�. �� ���ϰ� �̾߱��Ϸ� ��.";
    }

    // GPT ���� �� ȣ���
    private void OnGPTReplyReceived(string reply, bool isEnd)
    {
        // ������ ��� �и�
        reply = AIEmotionParsing(reply);


        // ���� �ؽ�Ʈ�� �Ľ��ϰų�, TTS�� �ѱ�� �۾��� ���⿡��
        string voice = DetermineVoiceFromScenario(currentScenario);
        ttsChanger.Speak(reply, voice);

        if (isEnd)
        {
            Debug.Log("��ȭ ���� ó����");

            isConversationEnded = true; // ��ȭ ���� ����Ʈ ���
        }
    }
    /// <summary>
    /// ħ���� ���� ��ȭ ���� üũ <- MicRecorder���� ��������Ʈ ȣ���ؼ� ����
    /// </summary>
    private void OnEndCallbySilence()
    {
        isEndCallbySilence = true;
    }

    // STT ��� �� GPT ��û
    private void OnUserSpeechRecognized(string userText)
    {
        if (isConversationEnded) return; // ��ȭ ���� ��� return

        gptRequester.RequestGPT(userText); // ���� �� GPT�� ����
    }

    // GPT ���� TTS ���� ����ũ ���� �����
    private void OnTTSEnded()
    {
        if (isConversationEnded)
        {
            if (isEndCallbySilence)
            {
                EventHub.actionEndedRealCallbySilence?.Invoke();
                UserData.Instance.userReputation -= 5;
                EventHub.actionUpdateReputation?.Invoke();
                isEndCallbySilence = false;
#if UNITY_EDITOR
                Debug.Log("��ȭ ���� ó�� - ħ���� ����");
#endif
            }
            else
            {
                EventHub.actionEndedCallBySpeak?.Invoke();
                EventHub.actionUpdateComplaintMsg?.Invoke(currentOnlyScenarioContext);
#if UNITY_EDITOR
                Debug.Log("��ȭ ���� ó��!");
#endif
            }

            GlobalCallState = false; // AI�� ������ ���� ������ ������ �� üũ
            isConversationEnded = true;
            return;
        }

        // Conversation ������ �ʾ��� ���
        micRecorder.StartRecording(); // TTS ������ ����ũ ����
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
                GlobalEmotionTag = emotionTag;
                return reply.Substring(endIdx + 1).Trim();
            }
        }

        return reply;
    }


    // �ó����� ��� ��Ҹ� ����
    public string DetermineVoiceFromScenario(string scenario)
    {
        if (scenario.Contains("����") || scenario.Contains("�Ҿƹ���") || scenario.Contains("�ƺ�") || scenario.Contains("����"))
            return "ko-KR-Neural2-C"/*"ko-KR-Chirp3-HD-Charon"*/; // ���� ���� - chirp �迭�� ������, �ӵ� ���� �ȵ�..

        if (scenario.Contains("����") || scenario.Contains("�ҸӴ�") || scenario.Contains("����") || scenario.Contains("����"))
            return "ko-KR-Neural2-B"/*"ko-KR-Chirp3-HD-Zephyr"*/; // ���� ���� - wavenet�� ������ ���..

        return "ko-KR-Wavenet-D"/*"ko-KR-Neural2-B"*/; // �⺻��
    }


    // ���� ��, ��������Ʈ ����
    private void OnDestroy()
    {
        EventHub.actionMicRecorded -= OnUserSpeechRecognized;
        EventHub.actionGptReceived -= OnGPTReplyReceived;
        EventHub.actionTTSEnded -= OnTTSEnded;
    }
}

