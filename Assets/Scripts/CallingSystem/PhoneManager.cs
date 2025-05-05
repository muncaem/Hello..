using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class PhoneManager : MonoBehaviour
{
    [Header("Screen")]
    private GameObject HomeScreen;
    private GameObject CallingScreen;
    private GameObject InCallScreen;
    private GameObject EmptyScreen;
    private GameObject SurveyScreen;
    private GameObject KeypadScreen;

    [Header("Regarded_Keypad")]
    [SerializeField] private UnityEngine.UI.Text numberField;
    private string inputNumber = "";
    private string outgoingNumber;

    private UnityEngine.UI.Text EmptyScreenDialogField;
    private GameObject EmptyScreenInputFields;
    private InputField nameField; // 이름 인풋필드
    private InputField determinationField; // 각오 인풋 필드

    [Header("Regarded_Complaint")]
    [SerializeField] private GameObject complaintMsgIcon;
    [SerializeField] private UnityEngine.UI.Text[] complaintScreenText;
    private int activeComplaintText = 0;
    //private string complainContent;
    //public static bool isProcessedComplain { get; private set; } = false;

    // 전화 받을 기회
    private int TakeCallChance = 3;


    private void Awake()
    {
        if (GameManager.Instance.curSceneNumb == 0)
        {
            // 게임 시작 버튼 눌렀을 경우, 게임 진입 FadeIn 후 UI처리 대리함수 할당
            EventHub.actionEndedFadeIn += OpenCallingScreen;
            // 초기 진단 시, 전화를 받지 않았을 경우 UI처리 대리함수 할당
            EventHub.actionFirstTestUnCall += FirstTestUnCallSurvey;
            // 초기 진단 시, 전화를 받은 이후 설문 UI 대리함수 할당
            EventHub.actionFirstCallEndedCall += FirstTestCallSurvey;
        }

        // Main 치료 시, 송신 전화 올 경우, 사운드 및 UI처리 대리함수 할당
        EventHub.actionStartIncomingCall += RingingCall;
        EventHub.actionStartIncomingCall += OpenCallingScreen;
        // Main 치료 시, 전화 도중 끊겼을 경우 설문 UI 대리함수 할당
        EventHub.actionEndedRealCallbySilence += OpenUnCallbySilenceSurvey;
        // Main 치료 시, 하루 마무리 설문 UI 대리 함수 할당
        // -> 설문 하는 동안 시간 멈추기. 설문 완료하면 다음날 시작 UI 및 처리
        EventHub.actionEndedDayTime += OpenEndedDaySurvey;

        // Home Screen으로 콜백 할 델리게이트
        // 설문 이후 홈 이동
        EventHub.actionSurveyEnded += OpenHomeScreen;
        // 전화 거절 이후 홈 이동
        EventHub.actionEndedCallBySelect += OpenHomeScreen;
        // 전화 완료 이후 홈 이동
        EventHub.actionEndedCallBySpeak += OpenHomeScreen;
        // 전화 종료 후 컴플레인 텍스트 활성화 (정상적으로 종료 경우만)
        EventHub.actionUpdateComplaintMsg += ActiveComplaintMessage;

        // 진행 중인 수신 전화 내용 업데이트 시마다 호출
        EventHub.actionUpdatedScenario += UpdatedCurrentOutgoingCallContent;

        // 민원 처리 완료 시 호출되어 complaint text 제어
        EventHub.actionSolvedComplaint += DeactiveComplaintMessage;

        // 날짜 바뀔 때 UI 초기화
        EventHub.actionEndedDayTime += RefreshComplaintUI;

        // UI 할당
        HomeScreen = transform.GetChild(0).gameObject;
        CallingScreen = transform.GetChild(1).gameObject;
        InCallScreen = transform.GetChild(2).gameObject;
        EmptyScreen = transform.GetChild(3).gameObject;
        SurveyScreen = transform.GetChild(4).gameObject;
        KeypadScreen = transform.GetChild(5).gameObject;

        EmptyScreenDialogField = EmptyScreen.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>();
        EmptyScreenInputFields = EmptyScreen.transform.GetChild(1).gameObject;
        nameField = EmptyScreenInputFields.transform.GetChild(0).GetComponent<InputField>();
        determinationField = EmptyScreenInputFields.transform.GetChild(1).GetComponent<InputField>();
    }

    private void OnEnable()
    {
        if (GameManager.Instance.curSceneNumb == 1)
        {
            RingingCall();
            OpenCallingScreen();
        }
    }

    /// <summary>
    /// 수신 전화로 벨소리 울릴 경우 호출됨
    /// </summary>
    private void RingingCall()
    {
        StartCoroutine(GameManager.Instance.DelayTime(1.5f, () =>
        {
            if (!DiagnosisSystem.isCalled && !ConversationManager.GlobalCallState)
            {
                StartCoroutine(WaitForUserCallResponse());
            }
        }));
    }
    private IEnumerator WaitForUserCallResponse()
    {
        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < TakeCallChance; i++)
        {
            if (!DiagnosisSystem.isCalled)
            {
                SoundManager.instance.Play("bell");
                yield return new WaitForSeconds(4);
            }
        }

        UnTakeCallButton();
    }

    /// <summary>
    /// 전화 끊기 버튼
    /// </summary>
    public void UnTakeCallButton()
    {
        SoundManager.instance.Clear();
        StopAllCoroutines();
        EventHub.actionDisconnectedComingCall?.Invoke();
    }

    /// <summary>
    /// 전화 끊은 이후 HomeScreen으로 전환
    /// </summary>
    private void OpenHomeScreen()
    {
        HomeScreen.SetActive(true); // 오브젝트 활성화
        if (HomeScreen.TryGetComponent<UnityEngine.UI.Image>(out var img))
            StartCoroutine(GameManager.Instance.FadeIn(img, 0.5f, () => { })); // 페이드인

        SurveyScreen.SetActive(false);
        CallingScreen.SetActive(false);
        InCallScreen.SetActive(false);
        KeypadScreen.SetActive(false);

        if (GameManager.Instance.curSceneNumb == 0)
        {
            Debug.Log("<color=yellow>여기 아예 안들어갈 것 같은데 OpenHomeScreen 2번째 if문 </color>");
            EventHub.actionStartIncomingCall -= RingingCall;
            return;
        }
    }


    private void ActiveComplaintMessage(string content)
    {
        // 민원처리 완료 시 비활성화 필요
        complaintMsgIcon.SetActive(true);
        complaintScreenText[activeComplaintText].gameObject.SetActive(true);
        complaintScreenText[activeComplaintText++].text = content;
    }
    private void DeactiveComplaintMessage(int index)
    {
        complaintScreenText[index].gameObject.SetActive(false);

        // 모두 비활성화 경우, complaintMsg 아이콘 비활성화
        for (int i = 0; i < complaintScreenText.Length; i++)
        {
            // 활성화 오브젝트가 하나라도 있을 경우 return
            if (complaintScreenText[i].gameObject.activeSelf)
                return;
        }
        complaintMsgIcon.SetActive(false);
    }
    private void RefreshComplaintUI()
    {
        int notSolveCallVal = 0;
        for (int i = 0; i < complaintScreenText.Length; i++)
        {
            if (complaintScreenText[i].gameObject.activeSelf)
            {
                notSolveCallVal++;
                complaintScreenText[i].gameObject.SetActive(false);
            }
            complaintScreenText[i].text = "";
        }
        complaintMsgIcon.SetActive(false);

        // 해결하지 않은 outgoing call 개수만큼 평판 차감
        UserData.Instance.userReputation -= 16 * notSolveCallVal;
        EventHub.actionUpdateReputation?.Invoke();
    }


    /// <summary>
    /// 전화 오는 화면 UI 활성화
    /// </summary>
    private void OpenCallingScreen()
    {
        CallingScreen.SetActive(true); // 오브젝트 활성화
        if (CallingScreen.TryGetComponent<UnityEngine.UI.Image>(out var img))
            StartCoroutine(GameManager.Instance.FadeIn(img, 0.5f, () => { })); // 페이드인

        //if (EventHub.actionEndedFadeIn != null)
        //    EventHub.actionEndedFadeIn -= OpenCallingScreen;
    }

    /// <summary>
    /// 현재 송신 전화로 걸어야하는 전화번호 업데이트
    /// </summary>
    /// <param name="content"></param>
    /// <param name="number">송신 전화번호</param>
    private void UpdatedCurrentOutgoingCallContent(ScenarioData data, string number)
    {
        outgoingNumber = number.Replace("-", "");
        //complainContent = MainUIManager.complaintPaper_content.text;
    }

    /// <summary>
    /// 하루가 마무리 될 때, 전화에 대한 설문조사 UI 활성화
    /// </summary>
    private void OpenEndedDaySurvey()
    {
        StartCoroutine(GameManager.Instance.DelayTime(2f, () =>  // 2초 딜레이
        {
            SurveyScreen.gameObject.SetActive(true); // 오브젝트 활성화
            if (SurveyScreen.TryGetComponent<UnityEngine.UI.Image>(out var img))
                StartCoroutine(GameManager.Instance.FadeIn(img, 0.5f, () => { })); // 페이드인

            SurveyScreen.transform.GetChild(2).gameObject.SetActive(true);
        }));
    }

    /// <summary>
    /// 전화 도중 유저의 침묵에 의해 끊겼을 경우, 이에 대한 설문조사 UI 활성화
    /// </summary>
    private void OpenUnCallbySilenceSurvey()
    {
        StartCoroutine(GameManager.Instance.DelayTime(2f, () =>  // 2초 딜레이
        {
            SurveyScreen.gameObject.SetActive(true); // 오브젝트 활성화
            if (SurveyScreen.TryGetComponent<UnityEngine.UI.Image>(out var img))
                StartCoroutine(GameManager.Instance.FadeIn(img, 0.5f, () => { })); // 페이드인

            SurveyScreen.transform.GetChild(1).gameObject.SetActive(true);
        }));
    }

    /// <summary>
    /// 키패드 버튼과 연결
    /// </summary>
    public void PushedKeypad()
    {
        if (inputNumber.Length >= 9)
            return;

        inputNumber += EventSystem.current.currentSelectedGameObject.name;
        numberField.text = inputNumber;
    }

    /// <summary>
    /// 키패드의 전화 버튼과 연결, inputNumber 가져와서 민원 전화번호랑 같으면 전화 연결
    /// </summary>
    public void KeypadCallButton()
    {
        // 전화 연결 - 민원의 내용과 연결해서 델리게이트 호출
        if (outgoingNumber == inputNumber)
        {
            //isProcessedComplain = true;

            InCallScreen.gameObject.SetActive(true);
            KeypadScreen.gameObject.SetActive(false);
            EventHub.actionConnectedGoingCall?.Invoke();
            inputNumber = "";
            numberField.text = "";
        }
        // 전화 번호 다를 경우
        else 
        { 
        
        }
    }

    /// <summary>
    /// 키패드의 삭제 버튼과 연결, inputNumber의 마지막 숫자를 지움
    /// </summary>
    public void KeypadDelButton()
    {
        if (inputNumber.Length == 0)
            return;

        inputNumber = inputNumber.Remove(inputNumber.Length - 1);
        numberField.text = inputNumber;
    }



    /// <summary>
    /// 초기 진단 전화 받은 이후, 이에 대한 설문조사 UI 활성화
    /// </summary>
    private void FirstTestCallSurvey()
    {
        StartCoroutine(GameManager.Instance.DelayTime(2f, () =>  // 2초 딜레이
        {
            SurveyScreen.gameObject.SetActive(true);
            if (SurveyScreen.TryGetComponent<UnityEngine.UI.Image>(out var img))
                StartCoroutine(GameManager.Instance.FadeIn(img, 0.5f, () => { })); // 페이드인

            SurveyScreen.transform.GetChild(0).gameObject.SetActive(true);

            //EventHub.actionEndedFadeIn -= OpenCallingScreen;
            //DiagnosisSystem.actionFirstTestUnCall -= FirstTestUnCallSurvey;
            //DiagnosisSystem.actionFirstCallEndedCall -= FirstTestCallSurvey;
        }));
    }

    /// <summary>
    /// 초기 진단 전화 받지 않았을 경우, 텍스트 통해 유저 데이터 수집 위해 호출됨.
    /// </summary>
    /// <param name="dialog"></param>
    private void FirstTestUnCallSurvey(string dialog)
    {
        EmptyScreen.SetActive(true);

        // 초기 유저 데이터 수집 위한 UI 활성화
        StartCoroutine(GameManager.Instance.DelayTime(1f, () =>
        {
            EmptyScreenDialogField.gameObject.SetActive(true);
            EmptyScreenDialogField.text = dialog;
        }));

        // 인풋필드 on
        StartCoroutine(GameManager.Instance.DelayTime(1.5f, () =>
        {
            EmptyScreenInputFields.SetActive(true);
        }));
    }

    /// <summary>
    /// 초기 문자 진단 시, 인풋 필드 정보 체크 버튼
    /// </summary>
    public void FillCheckInputField()
    {
        if (nameField.text == "" || nameField.text.Contains(" "))
        {
            UnityEngine.UI.Text placeholder = nameField.placeholder as UnityEngine.UI.Text;
            placeholder.text = "공백 불가";
            return;
        }
        if (determinationField.text == "")
        {
            UnityEngine.UI.Text placeholder = determinationField.placeholder as UnityEngine.UI.Text;
            placeholder.text = "공백 불가";
            return;
        }

        UserData.Instance.userName = nameField.text;
        UserData.Instance.userDetermination = determinationField.text;

#if UNITY_EDITOR
        Debug.Log($"초기 진단 종료 - 유저 이름: {UserData.Instance.userName}, 각오 한마디: {UserData.Instance.userDetermination}");
#endif
        //EventHub.actionEndedFadeIn -= OpenCallingScreen;
        //DiagnosisSystem.actionFirstTestUnCall -= FirstTestUnCallSurvey;
        //DiagnosisSystem.actionFirstCallEndedCall -= FirstTestCallSurvey;
    }


    private void OnDestroy()
    {
        EventHub.actionEndedFadeIn -= OpenCallingScreen;

        EventHub.actionFirstTestUnCall -= FirstTestUnCallSurvey;
        EventHub.actionFirstCallEndedCall -= FirstTestCallSurvey;
        EventHub.actionStartIncomingCall -= OpenCallingScreen;
        EventHub.actionEndedCallBySelect -= OpenHomeScreen;
        EventHub.actionStartIncomingCall -= RingingCall;

        EventHub.actionEndedRealCallbySilence -= OpenUnCallbySilenceSurvey;
        EventHub.actionEndedCallBySpeak -= OpenHomeScreen;

        EventHub.actionEndedDayTime -= OpenEndedDaySurvey;
        EventHub.actionSurveyEnded -= OpenHomeScreen;
    }
}
