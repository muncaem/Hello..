using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConversationManager : MonoBehaviour
{
    [SerializeField] private MicRecorder micRecorder; // 마이크 리코더 클래스 연결
    [SerializeField] private GptRequester gptRequester; // 지피티 requester 클래스 연결
    [SerializeField] private PhoneScenarioMaker scenarioMaker; // AI 주민 배경 시나리오 maker 클래스 연결

    private bool isConversationEnded = false;
    protected string emotionTag { get; set; } ///ai 주민 감정 태그

    private void Awake()
    {
        MicRecorder.actionMicRecorded += OnUserSpeechRecognized; // 마이크 리코더에서 stt 변환 이후 대리자 호출
        GptRequester.actionGptReceived += OnGPTReplyReceived; // 지피 requester에서 gpt 응답 이후 대리자 호출
    }

    // Start is called before the first frame update
    void Start()
    {
        //// 1. GPT가 먼저 전화 시작 2. 유저 말 감지 루프는 GPT 응답 끝난 후에 시작되도록 GptRequester에서 처리
        //gptRequester.RequestGPT(scenarioMaker.ScenarioMaker()); // GPT가 먼저 발화
    }

    public void StartReQuestGPT()
    {
        // 1. GPT가 먼저 전화 시작 2. 유저 말 감지 루프는 GPT 응답 끝난 후에 시작되도록 GptRequester에서 처리
        gptRequester.RequestGPT(scenarioMaker.ScenarioMaker()); // GPT가 먼저 발화
    }

    // GPT 응답 후 호출됨
    private void OnGPTReplyReceived(string reply, bool isEnd)
    {
        // 응답 텍스트를 파싱하거나, TTS로 넘기는 작업은 여기에서


        // 감정과 대사 분리
        reply = AIEmotionParsing(reply);


        // 전화 종료 시 UI처리
        if (isEnd)
        {
            isConversationEnded = true; // 대화 종료 경우
            Debug.Log("전화 종료 처리!");
            // 전화 UI 끄기, 애니메이션 종료 등
            return;
        }
        // GPT 대사 출력 후 → 유저 응답 대기 시작
        micRecorder.StartRecording(); // 유저 마이크 대기 시작
    }

    // STT 결과 → GPT 요청
    private void OnUserSpeechRecognized(string userText)
    {
        if (isConversationEnded) return; // 대화 종료 경우 return

        gptRequester.RequestGPT(userText); // 유저 말 GPT에 전송
    }



    // AI 감정 파싱
    private string AIEmotionParsing(string reply)
    {
        // ai 감정 추출
        if (reply.StartsWith("["))
        {
            int endIdx = reply.IndexOf("]");
            if (endIdx > 0)
            {
                emotionTag = reply.Substring(1, endIdx - 1);
                return reply.Substring(endIdx + 1).Trim();
            }
        }

        return "ai의 감정이 입력되지 않았습니다.";
    }


    // 종료 시, 델리게이트 해제
    private void OnDestroy()
    {
        MicRecorder.actionMicRecorded -= OnUserSpeechRecognized;
        GptRequester.actionGptReceived -= OnGPTReplyReceived;
    }
}

