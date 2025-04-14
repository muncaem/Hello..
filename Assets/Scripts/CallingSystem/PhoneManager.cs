using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class PhoneManager : MonoBehaviour
{
    [Header("Screen")]
    /*[SerializeField]*/ private GameObject HomeScreen;
    /*[SerializeField]*/ private GameObject CallingScreen;
    /*[SerializeField]*/ private GameObject InCallScreen;
    /*[SerializeField]*/ private GameObject EmptyScreen;
    /*[SerializeField]*/ private GameObject SurveyScreen;

    private UnityEngine.UI.Text EmptyScreenDialogField;
    private GameObject EmptyScreenInputFields;
    private InputField nameField; // 이름 인풋필드
    private InputField determinationField; // 각오 인풋 필드


    private void Awake()
    {
        // 게임 시작 버튼 눌렀을 경우, 게임 진입 FadeIn 후 UI처리 대리함수 할당
        InteractiveButton.actionEndedFadeIn += OpenCallingScreen;


        // 초기 진단 시, 전화를 받지 않았을 경우 UI처리 대리함수 할당
        DiagnosisSystem.actionFirstTestUnCall += FirstTestUnCallSurvey;
        // 초기 진단 시, 전화를 받은 이후 설문 UI 대리함수 할당
        DiagnosisSystem.actionFirstCallEndedCall += FirstTestCallSurvey;

        // Main 치료 시, 송신 전화 올 경우, UI처리 대리함수 할당
        DiagnosisSystem.actionStartIncomingCall += OpenCallingScreen;
        // Main 치료 시, 전화 도중 끊겼을 경우 설문 UI 대리함수 할당
        ConversationManager.actionEndedCallbySilence += OpenUnCallbySilenceSurvey;
        // Main 치료 시, 하루 마무리 설문 UI 대리 함수 할당
        // -> 설문 하는 동안 시간 멈추기. 설문 완료하면 다음날 시작 UI 및 처리
        GameManager.actionEndedDayTime += OpenEndedDaySurvey;

        // Home Screen으로 콜백 할 델리게이트
        // 설문 이후
        CallSurvey.actionEndedSurvey += OpenHomeScreen;
        // 전화 거절 이후
        DiagnosisSystem.actionUnCall += OpenHomeScreen;
        // 전화 완료 이후
        ConversationManager.actionEndedCall += OpenHomeScreen;

        // UI 할당
        HomeScreen = transform.GetChild(0).gameObject;
        CallingScreen = transform.GetChild(1).gameObject;
        InCallScreen = transform.GetChild(2).gameObject;
        EmptyScreen = transform.GetChild(3).gameObject;
        SurveyScreen = transform.GetChild(4).gameObject;

        EmptyScreenDialogField = EmptyScreen.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>();
        EmptyScreenInputFields = EmptyScreen.transform.GetChild(1).gameObject;
        nameField = EmptyScreenInputFields.transform.GetChild(0).GetComponent<InputField>();
        determinationField = EmptyScreenInputFields.transform.GetChild(1).GetComponent<InputField>();
    }


    private void OpenHomeScreen()
    {
        HomeScreen.SetActive(true);
    }

    /// <summary>
    /// 전화 오는 화면 UI 활성화
    /// </summary>
    private void OpenCallingScreen()
    {
        CallingScreen.SetActive(true); // 오브젝트 활성화
        if (CallingScreen.TryGetComponent<UnityEngine.UI.Image>(out var img))
            StartCoroutine(GameManager.Instance.FadeIn(img, 0.5f, () => { })); // 페이드인

        if (InteractiveButton.actionEndedFadeIn != null)
            InteractiveButton.actionEndedFadeIn -= OpenCallingScreen;
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

            SurveyScreen.transform.GetChild(3).gameObject.SetActive(true);
        }));
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
    }


    private void OnDestroy()
    {
        DiagnosisSystem.actionFirstTestUnCall -= FirstTestUnCallSurvey;
        DiagnosisSystem.actionFirstCallEndedCall -= FirstTestCallSurvey;
        DiagnosisSystem.actionStartIncomingCall -= OpenCallingScreen;
        ConversationManager.actionEndedCallbySilence -= OpenUnCallbySilenceSurvey;
        GameManager.actionEndedDayTime -= OpenEndedDaySurvey;
        CallSurvey.actionEndedSurvey -= OpenHomeScreen;
        DiagnosisSystem.actionUnCall -= OpenHomeScreen;
        ConversationManager.actionEndedCall -= OpenHomeScreen;
    }
}
