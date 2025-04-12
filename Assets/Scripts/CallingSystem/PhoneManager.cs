using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class PhoneManager : MonoBehaviour
{
    [Header("Screen")]
    [SerializeField] private GameObject HomeScreen;
    [SerializeField] private GameObject CallingScreen;
    [SerializeField] private GameObject InCallScreen;
    [SerializeField] private GameObject EmptyScreen;
    [SerializeField] private GameObject SurveyScreen;

    private UnityEngine.UI.Text EmptyScreenDialogField;
    private GameObject EmptyScreenInputFields;
    private InputField nameField; // 이름 인풋필드
    private InputField determinationField; // 각오 인풋 필드

    [Header("Button")]
    [SerializeField] private Button CallBtn;
    [SerializeField] private Button UnCallBtn;
    //[SerializeField] private Button SendBtn;

    private void Awake()
    {
        // 게임 시작 버튼 눌렀을 경우, 게임 진입 FadeIn 후 UI처리 대리함수 할당
        InteractiveButton.actionEndedFadeIn += OpenCallingScreen;

        // 초기 진단 시, 전화를 받지 않았을 경우 UI처리 대리함수 할당
        DiagnosisSystem.actionFirstTestUnCall += FirstTestUnCall;
        // 초기 진단 시, 전화를 받은 이후 설문 UI 대리함수 할당
        DiagnosisSystem.actionFirstCallEndedCall += OpenCallSurvey;

        // Main 치료 시, 전화를 받지 않았을 경우 설문 UI 대리함수 할당
        DiagnosisSystem.actionUnCall += OpenUnCallSurvey;
        // Main 치료 시, 전화를 받은 이후 설문 UI 대리함수 할당
        ConversationManager.actionEndedCall += OpenCallSurvey;

        // 설문 이후 홈 스크린으로 call back 대리함수 할당
        CallSurvey.actionEndedSurvey += OpenHomeScreen;

        // UI 할당
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
    /// 전화를 받지 않았을 경우, 이에 대한 설문조사 UI 활성화
    /// </summary>
    private void OpenUnCallSurvey()
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
    /// 전화 받은 이후, 이에 대한 설문조사 UI 활성화
    /// </summary>
    private void OpenCallSurvey()
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
    private void FirstTestUnCall(string dialog)
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
        DiagnosisSystem.actionFirstTestUnCall -= FirstTestUnCall;
    }
}
