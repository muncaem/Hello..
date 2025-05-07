using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class DiagnosisSystem : MonoBehaviour
{
    public static DiagnosisSystem Instance;
    ///  <summary>
    /// 유저 요인 정보 임시 할당
    private int preFactor = 0; // 회피 증세 - 전화를 안 받거나 피함
    private int midFactor = 0; // 발화 불안 - 발화 중 대응 불안
    private int postFactor = 0; // 사후 반추 - 전화를 끊은 뒤 후회 및 불안

    private int preUserDataReply = 0; // 초기 진단 시, 유저 대답 횟수 체크
    /// </summary>

    //// 초기 진단 확인용 bool값
    //private bool isFirstScene;
    // 기회 내 전화 받았는지 여부 체크 및 현재 전화 중인지 체크 => 회피 요인 +1
    public static bool isCalled { get; private set; } = false;
    // 전화 받을 기회
    private int TakeCallChance = 3;

    // Main 노출 치료 시, 하루 총 전화량
    private int totalCallValue = 6;
    // Main 노출 치료 시, 하루당 유저가 걸어야 하는 전화 횟수
    private int outGoingCall;
    // Main 노출 치료 시, 하루당 유저가 받아야 하는 전화 횟수
    private int inCompingCall;

    // 마이크 이미지 및 버튼
    [Header("Function")]
    [SerializeField] private MicRecorder MicRecorder;
    [SerializeField] private TTSChanger TTSChanger;
    //[SerializeField] private OutGoingCallManager OutGoingCallManager;

    [Header("DiagnosisText")]
    [SerializeField] private string[] dialogs; // 다이얼로그

    //public static Action OnTakeCall; // 진단시스템으로부터 오는 전화를 받았을 경우 ConversationManager의 대화 시작
    //public static Action<string> actionFirstTestUnCall;
    //public static Action actionFirstCallEndedCall;
    //public static Action actionEndedSaveScore; // 초기 진단 점수 UserData에 저장 완료 후 대리자 호출
    //public static Action actionUnCall;
    //public static Action<int> actionUpdatedOutGoingValue; // 날마다 업데이트 되는 수신 통화량
    //public static Action actionStartIncomingCall;

    private Coroutine waitCallCoroutine;

    private void Awake()
    {
        if (Instance == null) Instance = this;

        //isFirstScene = SceneManager.GetActiveScene().buildIndex == 0 ? true : false;
        //if (SceneManager.GetActiveScene().name.Contains("Start"))
        //    isFirstScene = true;

        if (GameManager.Instance.curSceneNumb == 0)
        {
            EventHub.actionTTSEnded += OnTTSEnded;
            EventHub.actionMicRecorded += OnRecordEnded;
        }
        EventHub.actionUpdatedSurvey += UpdateScoreBySituation;
        EventHub.actionSurveyEnded += ReturnFinalScore;
        EventHub.actionUpdatedSpeakSituationFactor += UpdateScoreBySituation;

        EventHub.actionUpdatedDay += StartMainTherapy; // 하루가 지날때마다 송신/수신량 결정
        EventHub.actionReachedCallGap += InComingCall; // n초마다 전화 오게 함
        GameManager.actionChangedScene += OnChangedScene;

        EventHub.actionEndedCallBySpeak += RefreshCallState;

        EventHub.actionConnectedComingCall += TakeCallDiagnosis;
        EventHub.actionDisconnectedComingCall += UnTakeCallDiagnosis;
    }

    private void OnChangedScene()
    {
        // 델리게이트 해제
        EventHub.actionTTSEnded -= OnTTSEnded;
        EventHub.actionMicRecorded -= OnRecordEnded;

        EventHub.actionConnectedComingCall -= TakeCallDiagnosis;
        EventHub.actionDisconnectedComingCall -= UnTakeCallDiagnosis;

        StartCoroutine(RebindPhoneManagerActions());
    }
    private IEnumerator RebindPhoneManagerActions()
    {
        Debug.Log("RebindPhoneManagerActions In");
        yield return new WaitUntil(() => FindObjectOfType<PhoneManager>() != null);
        Debug.Log("RebindPhoneManagerActions end wait");
        EventHub.actionConnectedComingCall += TakeCallDiagnosis;
        EventHub.actionDisconnectedComingCall += UnTakeCallDiagnosis;
    }

    /// <summary>
    /// 초기 진단 시작 버튼(게임 시작 버튼)
    /// </summary>
    public void StartDiagnosis()
    {
        //초기 전화 받기/거절 테스트
        //StartCoroutine(InitCheck());
        StartCoroutine(GameManager.Instance.DelayTime(1.5f, () =>
        {
            EventHub.actionStartIncomingCall?.Invoke();
        }));
    }


    /// <summary>
    /// Main 씬에서 송신/수신량 랜덤 결정
    /// 하루 지날 때마다 계산
    /// pre가 avg(mid + post)보다 크면 송신이 50% + 1이상. 작으면 수신이 50% + 1이상 중 랜덤 값
    /// </summary>
    private void StartMainTherapy()
    {
        StartCoroutine(DelayedMainTherapy());
    }
    private IEnumerator DelayedMainTherapy()
    {
        yield return null;

        int firstPre = UserData.Instance.firstPreFactor;
        int firstMid = UserData.Instance.firstMidFactor;
        int firstPost = UserData.Instance.firstPostFactor;
        Debug.Log($"{firstPre} {firstMid} {firstPost}");

        if (firstPre > (firstMid + firstPost) / 2)
        {
            inCompingCall = UnityEngine.Random.Range(totalCallValue / 2 + 1, totalCallValue);
            outGoingCall = totalCallValue - inCompingCall;
        }
        else if (firstPre < (firstMid + firstPost) / 2)
        {
            outGoingCall = UnityEngine.Random.Range(totalCallValue / 2 + 1, totalCallValue);
            inCompingCall = totalCallValue - outGoingCall;
        }
        else
        {
            outGoingCall = totalCallValue / 2;
            inCompingCall = totalCallValue - outGoingCall;
        }

        Debug.Log($"outgoingcall: {outGoingCall}, incomingcall: {inCompingCall}");


        EventHub.actionUpdatedOutGoingValue?.Invoke(outGoingCall);

        //// 첫 시작 call
        //actionStartIncomingCall?.Invoke();

        //StartCoroutine(GameManager.Instance.DelayTime(1.5f, () =>
        //{
        //    if (!isCalled)
        //    {
        //        actionStartIncomingCall?.Invoke();
        //        if (waitCallCoroutine != null)
        //        {
        //            StopCoroutine(waitCallCoroutine);
        //            waitCallCoroutine = null;
        //        }
        //        waitCallCoroutine = StartCoroutine(WaitForUserCallResponse());
        //    }
        //}));
    }


    private void RefreshCallState()
    {
        // ConversationManager에서 전화 끝나면 호출되어 전화 상태 false로 변경
        isCalled = false;
    }

    /// <summary>
    /// n초마다 전화가 오고 송신 전화 Conversation 시작 - GameManager의 n초 지남 대리자 연결
    /// </summary>
    private void InComingCall()
    {
        // 전화 중이면 return
        if (isCalled) return;

        // 하루 시작하자마자 n초 후 전화 오게 함
        EventHub.actionStartIncomingCall?.Invoke();
    }


    /// <summary>
    /// 전화 받기 버튼 눌렀을 경우
    /// </summary>
    public void TakeCallDiagnosis()
    {
        isCalled = true;

        if (GameManager.Instance.curSceneNumb == 0)
            CheckForFirstData(); // 초기 진단용 음성 대화
        else
            EventHub.OnTakeCall?.Invoke(); // 전화를 받았을 경우 실행되는 델리게이트 => StartComingConversation()
    }

    /// <summary>
    /// 전화 끊기 버튼 / 나중에 보기 버튼 눌렀을 경우
    /// </summary>
    public void UnTakeCallDiagnosis()
    {
        preFactor++; //사전 증세 요인 + 1

#if UNITY_EDITOR
        Debug.Log("PreFactor: " + preFactor);
#endif
        if (GameManager.Instance.curSceneNumb == 0)
        {
            EventHub.actionFirstTestUnCall?.Invoke(dialogs[0]); // 초기 진단 전화 거절로 텍스트로 진행
        }
        else
        {
            outGoingCall++; // 거절 시, 수신 전화 추가
            // 평판 시스템 평판 감소 => UI 연결 시 델리게이트로 옮기기 고려
            UserData.Instance.userReputation -= 5;
            EventHub.actionUpdateReputation?.Invoke();
            EventHub.actionEndedCallBySelect?.Invoke();
            isCalled = false;
        }
    }


    /// <summary>
    /// 상황별 증상 점수 추가
    /// </summary>
    /// <param name="situationId"> 증상 형태 </param>
    private void UpdateScoreBySituation(string situationId)
    {
#if UNITY_EDITOR
        //Debug.Log($"UpdateScoreBySituation: {situationId}");
#endif
        switch (situationId)
        {
            case "avoid_call":
                preFactor++;
                break;
            case "hesitate_speaking":
                midFactor++;
                break;
            case "regret_after_call":
                postFactor++;
                break;
            default:
#if UNITY_EDITOR
                Debug.LogWarning("알 수 없는 상황 ID: " + situationId);
#endif
                break;
        }

        EventHub.actionUpdatePhobiaBar?.Invoke(preFactor, midFactor, postFactor);
    }
    private void ReturnFinalScore()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            UserData.Instance.firstPreFactor = preFactor;
            UserData.Instance.firstMidFactor = midFactor;
            UserData.Instance.firstPostFactor = postFactor;

#if UNITY_EDITOR
            Debug.Log($"preFactor: {preFactor}, midFactor: {midFactor}, postFactor: {postFactor}");
#endif
            EventHub.actionSurveyEnded -= ReturnFinalScore;
            EventHub.actionFirstEndedSaveScore?.Invoke();
            Destroy(MicRecorder); // 초기 진단용 STT 제거
            Destroy(transform.GetChild(0).gameObject); // 초기 진단용 TTS 제거

            // 메인 노출 치료 위해 초기 진단 결과 초기화
            preFactor = 0;
            midFactor = 0;
            postFactor = 0;
        }
    }

    /// <summary>
    /// 초기 진단용 로직 처리
    /// </summary>
    private void CheckForFirstData()
    {
        // 전화를 받았을 경우
        if (isCalled)
        {
            if (preUserDataReply == 0)
            {
                TTSChanger.NormalSpeak(dialogs[++preUserDataReply]);
            }
        }
        // 전화를 받지 않았을 경우
        else
        {
            // 진단용 대답 체크
            EventHub.actionFirstTestUnCall?.Invoke(dialogs[0]);
        }
    }

    /// <summary>
    /// 초기 진단 질문 시작
    /// </summary>
    private void OnTTSEnded()
    {
        if (dialogs.Length <= preUserDataReply) return;
        MicRecorder.StartRecording(); /// 마이크 녹음 시작
    }
    /// <summary>
    /// 마이크 녹음 종료 시 호출 됨
    /// </summary>
    /// <param name="userComment"> 유저 응답 </param>
    private void OnRecordEnded(string userComment)
    {
        if (preUserDataReply == 1) // 첫번째 질문 응답 종료 경우
        {
            UserData.Instance.userName = userComment; /// 유저 이름 데이터 저장
            TTSChanger.NormalSpeak(dialogs[++preUserDataReply]); // 두번째 진단 질문 시작
        }
        else if (preUserDataReply == 2) // 두번째 질문 응답 종료 경우
        {
            UserData.Instance.userDetermination = userComment; /// 유저 각오 데이터 저장

            TTSChanger.NormalSpeak($"감사합니다, {UserData.Instance.userName}님. 동대표로서 앞으로 잘 부탁드립니다~");
#if UNITY_EDITOR
            Debug.Log($"초기 진단 종료 - 유저 이름: {UserData.Instance.userName}, 각오 한마디: {UserData.Instance.userDetermination}");
#endif
            preUserDataReply += 1;

            EventHub.actionFirstCallEndedCall?.Invoke();

            isCalled = false; // 초기 진단 전화 종료

            //// 델리게이트 해제
            //TTSChanger.actionTTSEnded -= OnTTSEnded;
            //MicRecorder.actionMicRecorded -= OnRecordEnded;
            return;
        }
    }
}
