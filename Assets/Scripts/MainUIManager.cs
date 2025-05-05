using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUIManager : MonoBehaviour
{
    [Header("Regarded_Complaint_UI")]
    // ������ �۽��ؾ� �� �ο� Ƚ�� -> �پ�� ������ update�ʿ�
    [SerializeField] private Text outgoingCallValue;
    // �ʿ� �������� ��� �ο� ������ -> �پ�� ������ update �ʿ� (������Ʈ Ǯ�� ���� ���)
    //[SerializeField] private GameObject complaintObjIcon;
    //private int activeIconObjVal;
    // �۽��ؾ��� �ο� ������ ������ ��� �ο� ���� �� ���� ��ȭ��ȣ ��� ������Ʈ -> �ϳ� Ŭ���� �� ������ ���� �ٲ��
    [SerializeField] private GameObject complaintPaper;
    public static Text complaintPaper_content { get; private set; }
    private Text complaintPaper_number;
    //private int activePaperObjVal = -1; // Ȱ��ȭ �� �ο� ������ ������Ʈ ����
    //private int currOutGoingIdx = -1;// ���� ���� ���� �۽� �ο� �ε���

    [Header("Regarded_State_UI")]
    [SerializeField] private Text userName;
    [SerializeField] private Text userDetermination;
    [SerializeField] private Scrollbar userReputation;

    [Header("Regarded_PhobiaState_UI")]
    [SerializeField] private Scrollbar avoidCallBar;
    [SerializeField] private Scrollbar hesitateBar;
    [SerializeField] private Scrollbar afterRegretBar;

    [Header("")]
    [SerializeField] private Text dayText;



    private void Awake()
    {
        EventHub.actionUpdatedDay += UpdateDayUI; // ���ο� day ���۵� ������, ȣ���
        // �Ϸ� ġ ���� ��ȭ ��ġ ������Ʈ ��, �۽� ��ȭ �Ϸ� ��, ȣ�� ��
        EventHub.actionUpdatedGoingCallValue += UpdateOutGoingCallValue;
        EventHub.actionUpdatedScenario += CreateComplaintPapers; // �۽� �ó����� �ϼ� ��, ȣ���
        EventHub.actionUpdateReputation += ManageReputationBar; // �ο� ó�� ��, up/down
        EventHub.actionUpdatePhobiaBar += ManageCallPhobiaBar;

        complaintPaper_content = complaintPaper.transform.GetChild(0).GetComponent<Text>();
        complaintPaper_number = complaintPaper.transform.GetChild(1).GetComponent<Text>();

        userName.text = $"�ȳ��ϼ���, {UserData.Instance.userName} ��.";
        userDetermination.text = $"{UserData.Instance.userName}���� �� ����, {UserData.Instance.userDetermination}";
    }

    /// <summary>
    /// ��¥ UI ����
    /// </summary>
    private void UpdateDayUI()
    {
        dayText.text = $"- Day {GameManager.Instance.day} -";
    }

    /// <summary>
    /// �Ϸ� ġ �ο��� (6��) �������� �ʿ� ������ġ�� ���� -> ������Ʈ Ǯ �̿�
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
    /// �۽��ؾ��� �ο� Ƚ�� UI ���� �� �پ�� ������ ������Ʈ
    /// </summary>
    private void UpdateOutGoingCallValue(int val)
    {
        if(complaintPaper.activeSelf)
            complaintPaper.SetActive(false);

        outgoingCallValue.text = val.ToString();
        //currOutGoingIdx++;
    }

    /// <summary>
    /// �۽� ��ȭ�� �ο� ��� ������Ʈ ����
    /// </summary>
    private void CreateComplaintPapers(ScenarioData scenario, string phoneNumb)
    {
        string content = scenario.situation;
        if (content.EndsWith("�־���"))
            content = content.Replace("�־���", "�־����ϴ�.");
        else if (content.EndsWith("�־�"))
            content = content.Replace("�־�", "�ֽ��ϴ�.");

        complaintPaper_content.text = content;
        complaintPaper_number.text = phoneNumb;
    }


    /// <summary>
    /// �۽��� �ο� ��ư ���� ��� ����
    /// </summary>
    bool isOpenPaper = false;
    public void OpenCurrComplanitPaper()
    {
        isOpenPaper = !isOpenPaper;
        complaintPaper.SetActive(isOpenPaper);
    }


    /// <summary>
    /// �ο� ó�� �Ϸ�Ǿ��� ���, ���� ����
    /// ( �ϳ� ó�� �Ϸ� ��, 16�� +
    /// �Ϸ� �Ѿ �� �ذ����� ���� �ο� ���� ���� 16 x n����ŭ - (���� outgoingcall ����, complaint ���� ����_
    /// UnTakeCall Ŭ�� �� Ȥ�� ��ȭ�ϴٰ� �������� ������ ��, 5�� - )
    /// </summary>
    private void ManageReputationBar()
    {
        userReputation.size = 100 / UserData.Instance.userReputation;
    }


    private void ManageCallPhobiaBar(int avoid, int hesitate, int after)
    {
        avoidCallBar.size = 100 / avoid;
        hesitateBar.size = 100 / hesitate;
        afterRegretBar.size = 100 / after;
    }
}
