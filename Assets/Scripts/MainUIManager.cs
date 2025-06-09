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
    // ������ �۽��ؾ� �� �ο� Ƚ�� -> �پ�� ������ update�ʿ�
    [SerializeField] private Text outgoingCallValue;
    // �۽��ؾ��� �ο� ������ ������ ��� �ο� ���� �� ���� ��ȭ��ȣ ��� ������Ʈ -> �ϳ� Ŭ���� �� ������ ���� �ٲ��
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
        EventHub.actionUpdatedDay += UpdateDayUI; // ���ο� day ���۵� ������, ȣ���
        // �Ϸ� ġ ���� ��ȭ ��ġ ������Ʈ ��, �۽� ��ȭ �Ϸ� ��, ȣ�� ��
        EventHub.actionUpdatedGoingCallValue += UpdateOutGoingCallValue;
        EventHub.actionUpdatedScenario += CreateComplaintPapers; // �۽� �ó����� �ϼ� ��, ȣ���
        EventHub.actionUpdateReputation += ManageReputationBar; // �ο� ó�� ��, up/down
        EventHub.actionUpdatePhobiaBar += ManageCallPhobiaBar;
        EventHub.actionSetFinalStatus += SetFinalStatus; // ���� ���� �� ��� Status ǥ��

        complaintPaper_content = complaintPaper.transform.GetChild(0).GetComponent<Text>();
        complaintPaper_number = complaintPaper.transform.GetChild(1).GetComponent<Text>();

        userName.text = $"�ȳ��ϼ���, {UserData.Instance.userName} ��.";
        userDetermination.text = $"{UserData.Instance.userName}���� �� ����, {UserData.Instance.userDetermination}";

        ManageReputationBar(); // ���� ��, ���� �ʱ�ȭ
    }

    /// <summary>
    /// ��¥ UI ����
    /// </summary>
    private void UpdateDayUI()
    {
        dayText.text = $"- Day {GameManager.Instance.day} -";
    }

    /// <summary>
    /// �۽��ؾ��� �ο� Ƚ�� UI ���� �� �پ�� ������ ������Ʈ
    /// </summary>
    private void UpdateOutGoingCallValue(int val)
    {
        if (complaintPaper.activeSelf)
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
    /// �ο� ó�� �Ϸ�Ǿ��� ���, ���� ����
    /// ( �ϳ� ó�� �Ϸ� ��, 16�� +
    /// �Ϸ� �Ѿ �� �ذ����� ���� �ο� ���� ���� 16 x n����ŭ - (���� outgoingcall ����, complaint ���� ����_
    /// UnTakeCall Ŭ�� �� Ȥ�� ��ȭ�ϴٰ� �������� ������ ��, 5�� - )
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
