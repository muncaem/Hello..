using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class ConversationManager : MonoBehaviour
{
    [SerializeField] private MicRecorder micRecorder; // 마이크 리코더 클래스 연결
    [SerializeField] private GptRequester gptRequester; // 지피티 requester 클래스 연결
    [SerializeField] private PhoneScenarioMaker scenarioMaker; // AI 주민 배경 시나리오 maker 클래스 연결
    [SerializeField] private TTSChanger ttsChanger; // tts 체인저 클래스 연결

    private bool isEndCallbySilence = false;
    private bool isConversationEnded = false;
    private string emotionTag; ///ai 주민 감정 태그
    public static string GlobalEmotionTag { get; private set; } // 감정 태그 static 변수
    private string currentScenario; /// 현 ai 주민 배경 시나리오
    public static bool GlobalCallState { get; private set; }


    public static Action actionEndedCall;
    public static Action actionEndedCallbySilence;

    private void Awake()
    {
        DiagnosisSystem.OnTakeCall += StartComingConversation; // 진단 시스템에서 전화를 받았을 경우 대리자 호출

        MicRecorder.actionMicRecorded += OnUserSpeechRecognized; // 마이크 리코더에서 stt 변환 이후 대리자 호출
        GptRequester.actionGptReceived += OnGPTReplyReceived; // 지피티 requester에서 gpt 응답 이후 대리자 호출
        TTSChanger.actionTTSEnded += OnTTSEnded; // 지피티 응답 tts 처리 이후 작업 호출

        MicRecorder.actionEndedBySilence += OnEndCallbySilence;

        OutGoingCallManager.actionStartedGoingCall += StartGoingConversation;
    }

    /// <summary>
    /// 수신(Incoming) 전화 시작
    /// </summary>
    private void StartComingConversation()
    {
#if UNITY_EDITOR
        Debug.Log("StartComingConversation()");
#endif

        ScenarioData scenarioData = scenarioMaker.ScenarioMaker();
        currentScenario = $"너는 {scenarioData.role}. {scenarioData.situation}. {scenarioData.emotion} 상태야. " +
            $"항상 대답 맨 앞에 {scenarioData.emotionTag}태그를 붙여서 감정을 표시해.";

        // 1. GPT가 먼저 전화 시작 2. 유저 말 감지 루프는 GPT 응답 끝난 후에 시작되도록 GptRequester에서 처리
        gptRequester.RequestGPT(currentScenario); // GPT가 먼저 발화

        isConversationEnded = false;
        GlobalCallState = true;
    }

    /// <summary>
    /// 송신(Outgoing) 전화 시작
    /// </summary>
    private void StartGoingConversation(ScenarioData data)
    {
#if UNITY_EDITOR
        Debug.Log("StartGoingConversation()");
#endif
        currentScenario = $"너는 {data.role}. {data.situation}. {data.emotion} 상태야. " +
            $"항상 대답 맨 앞에 {data.emotionTag}태그를 붙여서 감정을 표시해.";
        gptRequester.RequestGPT(currentScenario);
        GlobalCallState = true;
    }

    // GPT 응답 후 호출됨
    private void OnGPTReplyReceived(string reply, bool isEnd)
    {
        // 감정과 대사 분리
        reply = AIEmotionParsing(reply);


        // 응답 텍스트를 파싱하거나, TTS로 넘기는 작업은 여기에서
        string voice = DetermineVoiceFromScenario(currentScenario);
        ttsChanger.Speak(reply, voice);

        if (isEnd)
        {
            Debug.Log("전화 종료 처리중");

            isConversationEnded = true; // 대화 종료 포인트 경우
//            GlobalCallState = false;

//            if (isEndCallbySilence)
//            {
//                actionEndedCallbySilence?.Invoke();
//                isEndCallbySilence = false;
//#if UNITY_EDITOR
//                Debug.Log("전화 종료 처리 - 침묵에 의해");
//#endif
//            }
//            else
//            {
//                actionEndedCall?.Invoke();
//#if UNITY_EDITOR
//                Debug.Log("전화 종료 처리!");
//#endif
//            }

//            return;
        }
    }
    /// <summary>
    /// 침묵에 의해 전화 끊김 체크 <- MicRecorder에서 델리게이트 호출해서 받음
    /// </summary>
    private void OnEndCallbySilence()
    {
        isEndCallbySilence = true;
    }

    // STT 결과 → GPT 요청
    private void OnUserSpeechRecognized(string userText)
    {
        if (isConversationEnded) return; // 대화 종료 경우 return

        gptRequester.RequestGPT(userText); // 유저 말 GPT에 전송
    }

    // GPT 응답 TTS 이후 마이크 녹음 재시작
    private void OnTTSEnded()
    {
        if (isConversationEnded)
        {
            GlobalCallState = false; // AI의 마지막 말로 끝까지 끝났을 때 체크

            if (isEndCallbySilence)
            {
                actionEndedCallbySilence?.Invoke();
                isEndCallbySilence = false;
#if UNITY_EDITOR
                Debug.Log("전화 종료 처리 - 침묵에 의해");
#endif
            }
            else
            {
                actionEndedCall?.Invoke();
#if UNITY_EDITOR
                Debug.Log("전화 종료 처리!");
#endif
            }

            return;
        }

        // Conversation 끝나지 않았을 경우
        micRecorder.StartRecording(); // TTS 끝나면 마이크 시작
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
                GlobalEmotionTag = emotionTag;
                return reply.Substring(endIdx + 1).Trim();
            }
        }

        return reply;
    }


    // 시나리오 기반 목소리 지정
    public string DetermineVoiceFromScenario(string scenario)
    {
        if (scenario.Contains("남성") || scenario.Contains("할아버지") || scenario.Contains("아빠") || scenario.Contains("남자"))
            return "ko-KR-Neural2-C"/*"ko-KR-Chirp3-HD-Charon"*/; // 남성 음성 - chirp 계열은 음높이, 속도 조절 안됨..

        if (scenario.Contains("여성") || scenario.Contains("할머니") || scenario.Contains("엄마") || scenario.Contains("여자"))
            return "ko-KR-Neural2-B"/*"ko-KR-Chirp3-HD-Zephyr"*/; // 여성 음성 - wavenet로 할지도 고민..

        return "ko-KR-Wavenet-D"/*"ko-KR-Neural2-B"*/; // 기본값
    }


    // 종료 시, 델리게이트 해제
    private void OnDestroy()
    {
        MicRecorder.actionMicRecorded -= OnUserSpeechRecognized;
        GptRequester.actionGptReceived -= OnGPTReplyReceived;
        TTSChanger.actionTTSEnded -= OnTTSEnded;
    }
}

