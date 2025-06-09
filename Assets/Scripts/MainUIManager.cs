using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainUIManager : MonoBehaviour
{
    [Header("Regarded_Complaint_UI")]
    // 오늘의 송신해야 할 민원 횟수 -> 줄어들 때마다 update필요
    [SerializeField] private Text outgoingCallValue;
    // 송신해야할 민원 아이콘 누르면 띄울 민원 내용 및 유저 전화번호 담긴 오브젝트 -> 하나 클리어 할 때마다 내용 바뀌게
    [SerializeField] private GameObject complaintPaper;

    public static Text complaintPaper_content { get; private set; }
    private Text complaintPaper_number;

    [Header("Regarded_State_UI")]
    [SerializeField] private Text userName;
    [SerializeField] private Text userDetermination;
    [SerializeField] private Scrollbar userReputation;

    [Header("Regarded_PhobiaState_UI")]
    [SerializeField] private Scrollbar avoidCallBar;
    [SerializeField] private Scrollbar hesitateBar;
    [SerializeField] private Scrollbar afterRegretBar;
    [SerializeField] private Scrollbar firstAvoidCallBar;
    [SerializeField] private Scrollbar firstHesitateBar;
    [SerializeField] private Scrollbar firstAfterRegretBar;
    [SerializeField] private Scrollbar finalAvoidCallBar;
    [SerializeField] private Scrollbar finalHesitateBar;
    [SerializeField] private Scrollbar finalAfterRegretBar;

    [Header("")]
    [SerializeField] private Text dayText;
    [SerializeField] private GameObject GameFadeOutObj;
    [SerializeField] private GameObject FinalMentObj;



    private void Awake()
    {
        EventHub.actionUpdatedDay += UpdateDayUI; // 새로운 day 시작될 때마다, 호출됨
        // 하루 치 수신 통화 수치 업데이트 시, 송신 전화 완료 시, 호출 됨
        EventHub.actionUpdatedGoingCallValue += UpdateOutGoingCallValue;
        EventHub.actionUpdatedScenario += CreateComplaintPapers; // 송신 시나리오 완성 시, 호출됨
        EventHub.actionUpdateReputation += ManageReputationBar; // 민원 처리 시, up/down
        EventHub.actionUpdatePhobiaBar += ManageCallPhobiaBar;
        EventHub.actionSetFinalStatus += SetFinalStatus; // 게임 종료 시 결과 Status 표시

        complaintPaper_content = complaintPaper.transform.GetChild(0).GetComponent<Text>();
        complaintPaper_number = complaintPaper.transform.GetChild(1).GetComponent<Text>();

        userName.text = $"안녕하세요, {UserData.Instance.userName} 님.";
        userDetermination.text = $"{UserData.Instance.userName}님의 한 마디, {UserData.Instance.userDetermination}";

        ManageReputationBar(); // 시작 시, 평판 초기화
    }

    /// <summary>
    /// 날짜 UI 변경
    /// </summary>
    private void UpdateDayUI()
    {
        dayText.text = $"- Day {GameManager.Instance.day} -";
    }

    /// <summary>
    /// 송신해야할 민원 횟수 UI 세팅 및 줄어들 때마다 업데이트
    /// </summary>
    private void UpdateOutGoingCallValue(int val)
    {
        if (complaintPaper.activeSelf)
            complaintPaper.SetActive(false);

        outgoingCallValue.text = val.ToString();
        //currOutGoingIdx++;
    }

    /// <summary>
    /// 송신 전화할 민원 목록 오브젝트 생성
    /// </summary>
    private void CreateComplaintPapers(ScenarioData scenario, string phoneNumb)
    {
        string content = scenario.situation;
        if (content.EndsWith("넣었어"))
            content = content.Replace("넣었어", "넣었습니다.");
        else if (content.EndsWith("있어"))
            content = content.Replace("있어", "있습니다.");

        complaintPaper_content.text = content;
        complaintPaper_number.text = phoneNumb;
    }

    /// <summary>
    /// 민원 처리 완료되었을 경우, 평판 관리
    /// ( 하나 처리 완료 시, 16씩 +
    /// 하루 넘어갈 때 해결하지 못한 민원 갯수 세서 16 x n개만큼 - (남은 outgoingcall 개수, complaint 문자 개수_
    /// UnTakeCall 클릭 시 혹은 전화하다가 무음으로 끊었을 때, 5씩 - )
    /// </summary>
    private void ManageReputationBar()
    {
        float normalized = (float)UserData.Instance.userReputation / 100f;
        userReputation.size = Mathf.Clamp01(normalized);
    }


    private void ManageCallPhobiaBar(int avoid, int hesitate, int after)
    {
        if (avoid != 0)
        {
            float normalized = (float)avoid / 100f;
            avoidCallBar.size = Mathf.Clamp01(normalized);

        }
        if (hesitate != 0)
        {
            float normalized = (float)hesitate / 100f;
            hesitateBar.size = Mathf.Clamp01(normalized);
        }
        if (after != 0)
        {
            float normalized = (float)after / 100f;
            afterRegretBar.size = Mathf.Clamp01(normalized);
        }
    }

    private void SetFinalStatus(int a1, int m1, int p1, int a2, int m2, int p2)
    {
        GameFadeOutObj.SetActive(true);
        StartCoroutine(GameManager.Instance.DelayTime(2f, () =>
        {
            FinalMentObj.SetActive(true);
            if (a1 != 0)
            {
                float normalized = (float)a1 / 100f;
                firstAvoidCallBar.size = Mathf.Clamp01(normalized);

            }
            if (m1 != 0)
            {
                float normalized = (float)m1 / 100f;
                firstHesitateBar.size = Mathf.Clamp01(normalized);
            }
            if (p1 != 0)
            {
                float normalized = (float)p1 / 100f;
                firstAfterRegretBar.size = Mathf.Clamp01(normalized);
            }
            if (a2 != 0)
            {
                float normalized = (float)a2 / 100f;
                finalAvoidCallBar.size = Mathf.Clamp01(normalized);

            }
            if (m2 != 0)
            {
                float normalized = (float)m2 / 100f;
                finalHesitateBar.size = Mathf.Clamp01(normalized);
            }
            if (p2 != 0)
            {
                float normalized = (float)p2 / 100f;
                finalAfterRegretBar.size = Mathf.Clamp01(normalized);
            }
        }));
    }
}
