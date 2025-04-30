using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventHub : MonoBehaviour
{
    public static EventHub Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }


    /// <summary>
    /// 버튼 관련
    /// </summary>
    public static Action actionEndedFadeIn; // 버튼이 FadeOut 된 이후 브로드캐스트 됨.
    public static Action actionConnectedComingCall; // 유저가 전화 [ 받기 버튼 ]을 눌렀을 때
    public static Action actionDisconnectedComingCall; // 유저가 전화 [ 끊기 버튼 ]을 눌렀을 때


    /// <summary>
    /// 통화 Communication 관련
    /// </summary>
    public static Action<string, bool> actionGptReceived; // gpt 응답 이후 대리자 호출
    public static Action actionTTSEnded; // TTS 종료 이벤트
    public static Action<string> actionMicRecorded; // stt 변환 이후 대리자 호출
    /// <summary>
    /// Out Going 전화 관련
    /// </summary>
    public static Action<int> actionUpdatedOutGoingValue; // 날마다 업데이트 되는 수신 통화량
    public static Action actionConnectedGoingCall; // 유저가 전화를 AI에게 걸은 이후 맞게 연결되었을 때
    public static Action<ScenarioData> actionStartedGoingCall; // 실제 Out Going 전화 시작
    public static Action<ScenarioData, string> actionUpdatedScenario; // 현재 진행 중인 수신 시나리오로 업데이트
    public static Action<int> actionUpdatedGoingCallValue; // 전화 결어야 할 횟수 업데이트
    /// <summary>
    /// In Coming 전화 관련
    /// </summary>
    public static Action actionStartIncomingCall;
    public static Action OnTakeCall; // 진단시스템으로부터 오는 전화를 받았을 경우 ConversationManager의 대화 시작
    /// <summary>
    /// 공통 전화 관련
    /// </summary>
    public static Action actionEndedCallBySpeak; // 말로 통화 종료되었을 때 호출 -> ConversationManger에서 TTSEnded 이후
    public static Action actionEndedCallBySelect; // 버튼으로 종료되었을 때 호출 -> DiagnosisSystem에서 끊기 버튼 눌렀을 때
    public static Action<string> actionUpdateComplaintMsg; // 정상적으로 통화 종료 이후 민원 메시지 처리 update

    /// <summary>
    /// 초기 진단용 Conversation 관련
    /// </summary>
    public static Action<string> actionFirstTestUnCall; // 초기 진단 전화 안받았을 경우
    public static Action actionFirstCallEndedCall; // 초기 진단 전화 정상적으로 Ended경우
    public static Action actionFirstEndedSaveScore; // 초기 진단 점수 UserData에 저장 완료 후 대리자 호출

    /// <summary>
    /// 요인 체크 관련
    /// </summary>
    public static Action actionEndedBySilence; // 침묵에 의한 전화 종료 유발 -> 끊김 유발 체크용
    public static Action actionEndedRealCallbySilence; // silence 유발 체크 이후 실제 전화 끊김 처리
    public static Action<string> actionUpdatedSpeakSituationFactor; // 발화 상황 중 콜포비아 요인 업데이트 대리자 호출
    public static Action<string> actionUpdatedSurvey; // Survey 결과 업데이트 시
    public static Action actionSurveyEnded; // Survey 끝났을 경우 호출


    /// <summary>
    /// 날짜/시간 관련
    /// </summary>
    public static Action actionUpdatedDay; // 하루가 끝났을 때 호출
    public static Action actionReachedCallGap; // Call Gap 도달 시마다 호출
    public static Action actionEndedDayTime; // 하루 치 시간 모두 소모함과 동시에 전화가 끝났을 경우 호출


}
