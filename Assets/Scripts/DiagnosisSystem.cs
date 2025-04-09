using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class DiagnosisSystem : MonoBehaviour
{
    ///  <summary>
    /// 유저 요인 정보 임시 할당
    private int preFactor = 0; // 회피 증세 - 전화를 안 받거나 피함
    private int midFactor = 0; // 발화 불안 - 발화 중 대응 불안
    private int postFactor = 0; // 사후 반추 - 전화를 끊은 뒤 후회 및 불안

    private int preUserDataReply = 0;
    private string userName;
    private string userDetermination;
    /// </summary>


    // 기회 내 전화 받았는지 여부 체크 => 회피 요인 +1
    private bool isTakeCall = false;
    // 전화 받을 기회
    private int TakeCallChance = 3;

    // 마이크 이미지 및 버튼
    [Header("Function")]
    [SerializeField] private MicRecorder MicRecorder;
    [SerializeField] private TTSChanger TTSChanger;
    [Header("UI")]
    [SerializeField] private GameObject EmptyScreen;
    [SerializeField] private GameObject InputFieldUI;
    private GameObject MikeOnBtn; // 유저 이름 입력하는 inputField로 바꾸기
    private UnityEngine.UI.Image MikeOffImg; // 각오 입력하는 inputField로 바꾸기
    private UnityEngine.UI.Text textUI; // 유저 정보 수집 위한 안내 텍스트
    private InputField nameField; // 이름 인풋필드
    private InputField determinationField; // 각오 인풋 필드

    [SerializeField] private string[] dialogs; // 다이얼로그


    private void Awake()
    {
        MikeOffImg = EmptyScreen.transform.GetChild(0).GetComponent<UnityEngine.UI.Image>();
        MikeOnBtn = EmptyScreen.transform.GetChild(1).gameObject;
        textUI = EmptyScreen.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>();
        nameField = InputFieldUI.transform.GetChild(0).GetComponent<InputField>();
        determinationField = InputFieldUI.transform.GetChild(1).GetComponent<InputField>();
    }

    void Start()
    {
        TTSChanger.actionTTSEnded += OnTTSEnded;
        MicRecorder.actionMicRecorded += OnRecordEnded;
    }
    public void StartDiagnosis()
    {
        //초기 전화 받기/거절 테스트
        StartCoroutine(InitCheck());
    }
    private IEnumerator InitCheck()
    {
        yield return new WaitForSeconds(1f);

        for (int i = 0; i < TakeCallChance; i++)
        {
            if (!isTakeCall)
            {
                SoundManager.instance.Play("bell");

                yield return new WaitForSeconds(4);
            }
        }
        yield return null;

        // 3번 벨 울린 이후, 전화 받지 않은 것으로 간주
        EmptyScreen.SetActive(true);
        UnTakeCall();
    }



    // 전화 받기 버튼 눌렀을 경우, 스크립트 실행 및 마이크
    public void TakeCall()
    {
        SoundManager.instance.Clear();
        StopAllCoroutines();
        isTakeCall = true;

        // 진단용 음성 대화
        if (preUserDataReply == 0)
        {
            TTSChanger.NormalSpeak(dialogs[++preUserDataReply]);
        }
    }
    private void OnTTSEnded() // 진단 질문 시작
    {
        if (dialogs.Length <= preUserDataReply) return;
        MicRecorder.StartRecording(); /// 마이크 녹음 시작
    }
    private void OnRecordEnded(string userComment) // 마이크 녹음 종료 시 호출
    {
        if (preUserDataReply == 1) // 첫번째 질문 응답 종료 경우
        {
            userName = userComment; /// 유저 이름 데이터 저장
            TTSChanger.NormalSpeak(dialogs[++preUserDataReply]); // 두번째 진단 질문 시작
        }
        else if (preUserDataReply == 2) // 두번째 질문 응답 종료 경우
        {
            userDetermination = userComment; /// 유저 각오 데이터 저장

            //전화 종료 UI추가 예정

            TTSChanger.NormalSpeak($"감사합니다, {userName}님. 동대표로서 앞으로 잘 부탁드립니다~");
#if UNITY_EDITOR
            Debug.Log($"초기 진단 종료 - 유저 이름: {userName}, 각오 한마디: {userDetermination}");
#endif
            preUserDataReply += 1;

            DoSurvey();
            return;
        }
    }

    // 전화 끊기 버튼 / 나중에 보기 버튼 눌렀을 경우, 스크립트 실행 및 마이크
    public void UnTakeCall()
    {
        SoundManager.instance.Clear();
        StopAllCoroutines();
        preFactor++; //사전 증세 요인 + 1

#if UNITY_EDITOR
        Debug.Log("PreFactor: " + preFactor);
#endif

        // 진단용 대답 체크
        StartCoroutine(GameManager.Instance.DelayTime(1f,
        () =>
        {
            textUI.gameObject.SetActive(true);
            textUI.text = dialogs[0];
        }
            ));

        // 인풋필드 on
        StartCoroutine(GameManager.Instance.DelayTime(1.5f,
            () =>
            {
                InputFieldUI.SetActive(true);
            }
            ));
    }

    // 인풋 필드 정보 체크
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

        userName = nameField.text;
        userDetermination = determinationField.text;

#if UNITY_EDITOR
        Debug.Log($"초기 진단 종료 - 유저 이름: {userName}, 각오 한마디: {userDetermination}");
#endif

        DoSurvey();
    }

    // 조사 진행 (전화 문자 선호도에 대해)
    private void DoSurvey()
    {

    }

    // 상황별 증상 점수 추가
    public void UpdateScoreBySituation(string situationId)
    {
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
    }
}
