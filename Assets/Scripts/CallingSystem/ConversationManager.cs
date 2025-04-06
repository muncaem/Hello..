using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConversationManager : MonoBehaviour
{
    [SerializeField] private MicRecorder micRecorder; // ����ũ ���ڴ� Ŭ���� ����
    [SerializeField] private GptRequester gptRequester; // ����Ƽ requester Ŭ���� ����
    [SerializeField] private PhoneScenarioMaker scenarioMaker; // AI �ֹ� ��� �ó����� maker Ŭ���� ����
    [SerializeField] private TTSChanger ttsChanger; // tts ü���� Ŭ���� ����

    private bool isConversationEnded = false;
    private string emotionTag; ///ai �ֹ� ���� �±�
    public static string GlobalEmotionTag { get; private set; } // ���� �±� static ����
    private string currentScenario; /// �� ai �ֹ� ��� �ó�����

    private void Awake()
    {
        MicRecorder.actionMicRecorded += OnUserSpeechRecognized; // ����ũ ���ڴ����� stt ��ȯ ���� �븮�� ȣ��
        GptRequester.actionGptReceived += OnGPTReplyReceived; // ����Ƽ requester���� gpt ���� ���� �븮�� ȣ��
        TTSChanger.actionTTSEnded += OnTTSEnded; // ����Ƽ ���� tts ó�� ���� �۾� ȣ��
    }

    // Start is called before the first frame update
    void Start()
    {
        currentScenario = scenarioMaker.ScenarioMaker();

        //// 1. GPT�� ���� ��ȭ ���� 2. ���� �� ���� ������ GPT ���� ���� �Ŀ� ���۵ǵ��� GptRequester���� ó��
        //gptRequester.RequestGPT(scenarioMaker.ScenarioMaker()); // GPT�� ���� ��ȭ
    }

    public void StartConversation()
    {
        Debug.Log("StartConversation()");
        // 1. GPT�� ���� ��ȭ ���� 2. ���� �� ���� ������ GPT ���� ���� �Ŀ� ���۵ǵ��� GptRequester���� ó��
        gptRequester.RequestGPT(currentScenario); // GPT�� ���� ��ȭ
    }

    // GPT ���� �� ȣ���
    private void OnGPTReplyReceived(string reply, bool isEnd)
    {
        // ������ ��� �и�
        reply = AIEmotionParsing(reply);


        // ���� �ؽ�Ʈ�� �Ľ��ϰų�, TTS�� �ѱ�� �۾��� ���⿡��
        string voice = DetermineVoiceFromScenario(currentScenario);
        ttsChanger.Speak(reply, voice);


        // ��ȭ ���� �� UIó��
        if (isEnd)
        {
            isConversationEnded = true; // ��ȭ ���� ���
            Debug.Log("��ȭ ���� ó��!");
            // ��ȭ UI ����, �ִϸ��̼� ���� ��
            return;
        }
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
        if (isConversationEnded) return;

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
        if (scenario.Contains("����") || scenario.Contains("�Ҿƹ���"))
            return "ko-KR-Wavenet-C"; // ���� ����

        if (scenario.Contains("����") || scenario.Contains("�ҸӴ�"))
            return "ko-KR-Wavenet-A"; // ���� ����

        return "ko-KR-Wavenet-B"; // �⺻��
    }


    // ���� ��, ��������Ʈ ����
    private void OnDestroy()
    {
        MicRecorder.actionMicRecorded -= OnUserSpeechRecognized;
        GptRequester.actionGptReceived -= OnGPTReplyReceived;
        TTSChanger.actionTTSEnded -= OnTTSEnded;
    }
}

