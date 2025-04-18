using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUIManager : MonoBehaviour
{
    [Header("Regarded_Complaint_UI")]
    // 오늘의 송신해야 할 민원 횟수 -> 줄어들 때마다 update필요
    [SerializeField] private Text outgoingCallValue;
    // 맵에 동적으로 띄울 민원 아이콘 -> 줄어들 때마다 update 필요 (오브젝트 풀링 쓸지 고민)
    [SerializeField] private GameObject complaintObjIcon;
    private int activeIconObjVal;
    // 송신해야할 민원 아이콘 누르면 띄울 민원 내용 및 유저 전화번호 담긴 오브젝트 -> 하나 클리어 할 때마다 내용 바뀌게
    [SerializeField] private GameObject complaintPaper;
    public static Text complaintPaper_content { get; private set; }
    private Text complaintPaper_number;
    //private int activePaperObjVal = -1; // 활성화 할 민원 페이퍼 오브젝트 개수
    //private int currOutGoingIdx = -1;// 현재 진행 중인 송신 민원 인덱스

    [Header("Regarded_State_UI")]
    [SerializeField] private Text userName;
    [SerializeField] private Text userDetermination;
    //[SerializeField] private GameObject userReputation;

    [Header("")]
    [SerializeField] private Text dayText;



    private void Awake()
    {
        GameManager.actionUpdatedDay += UpdateDayUI; // 새로운 day 시작될 때마다, 호출됨
        // 하루 치 수신 통화 수치 업데이트 시, 송신 전화 완료 시, 호출 됨
        OutGoingCallManager.actionEndedGoingCall += UpdateOutGoingCallValue;
        OutGoingCallManager.actionUpdatedScenario += CreateComplaintPapers; // 송신 시나리오 완성 시, 호출됨

        complaintPaper_content = complaintPaper.transform.GetChild(0).GetComponent<Text>();
        complaintPaper_number = complaintPaper.transform.GetChild(1).GetComponent<Text>();

        userName.text = $"안녕하세요, {UserData.Instance.userName} 님.";
        userDetermination.text = $"{UserData.Instance.userName}님의 한 마디, {UserData.Instance.userDetermination}";
    }

    /// <summary>
    /// 날짜 UI 변경
    /// </summary>
    private void UpdateDayUI()
    {
        dayText.text = $"- Day {GameManager.Instance.day} -";
    }

    /// <summary>
    /// 하루 치 민원량 (6개) 동적으로 맵에 랜덤위치로 띄우기 -> 오브젝트 풀 이용
    /// </summary>
    /// <param name="val"></param>
    private void SetCallIcon(int val)
    {
        //for (int i = 0; i < val; i++)
        //{
        //    Instantiate(complaintObjIcon);
        //}
    }

    /// <summary>
    /// 송신해야할 민원 횟수 UI 세팅 및 줄어들 때마다 업데이트
    /// </summary>
    private void UpdateOutGoingCallValue(int val)
    {
        if(complaintPaper.activeSelf)
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
    /// 송신할 민원 버튼 누를 경우 실행
    /// </summary>
    bool isOpenPaper = false;
    public void OpenCurrComplanitPaper()
    {
        isOpenPaper = !isOpenPaper;
        complaintPaper.SetActive(isOpenPaper);
    }

    /// <summary>
    /// 부서 전화번호부
    /// </summary>
    /// <param name="numb">전화번호</param>
    private void CreateNumberBookText(string numb)
    {

    }
}
