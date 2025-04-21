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
    private InputField nameField; // �̸� ��ǲ�ʵ�
    private InputField determinationField; // ���� ��ǲ �ʵ�

    [Header("Regarded_Complaint")]
    [SerializeField] private GameObject complaintMsgIcon;
    [SerializeField] private UnityEngine.UI.Text complaintScreenText;
    private string complainContent;
    public static bool isProcessedComplain { get; private set; } = false;

    public static Action actionConnectedGoingCall;


    private void Awake()
    {

        if (GameManager.Instance.curSceneNumb == 0)
        {
            // ���� ���� ��ư ������ ���, ���� ���� FadeIn �� UIó�� �븮�Լ� �Ҵ�
            InteractiveButton.actionEndedFadeIn += OpenCallingScreen;
            // �ʱ� ���� ��, ��ȭ�� ���� �ʾ��� ��� UIó�� �븮�Լ� �Ҵ�
            DiagnosisSystem.actionFirstTestUnCall += FirstTestUnCallSurvey;
            // �ʱ� ���� ��, ��ȭ�� ���� ���� ���� UI �븮�Լ� �Ҵ�
            DiagnosisSystem.actionFirstCallEndedCall += FirstTestCallSurvey;
        }

        // Main ġ�� ��, �۽� ��ȭ �� ���, UIó�� �븮�Լ� �Ҵ�
        DiagnosisSystem.actionStartIncomingCall += OpenCallingScreen;
        // Main ġ�� ��, ��ȭ ���� ������ ��� ���� UI �븮�Լ� �Ҵ�
        ConversationManager.actionEndedCallbySilence += OpenUnCallbySilenceSurvey;
        // Main ġ�� ��, �Ϸ� ������ ���� UI �븮 �Լ� �Ҵ�
        // -> ���� �ϴ� ���� �ð� ���߱�. ���� �Ϸ��ϸ� ������ ���� UI �� ó��
        GameManager.actionEndedDayTime += OpenEndedDaySurvey;

        // Home Screen���� �ݹ� �� ��������Ʈ
        // ���� ���� Ȩ �̵�
        CallSurvey.actionEndedSurvey += OpenHomeScreen;
        // ��ȭ ���� ���� Ȩ �̵�
        DiagnosisSystem.actionUnCall += OpenHomeScreen;
        // ��ȭ �Ϸ� ���� Ȩ �̵�
        ConversationManager.actionEndedCall += OpenHomeScreen;

        // ���� ���� ���� ��ȭ ���� ������Ʈ �ø��� ȣ��
        OutGoingCallManager.actionUpdatedScenario += UpdatedCurrentOutgoingCallContent;
        //ConversationManager.actionEndedCall += UpdatedEndCall;

        // UI �Ҵ�
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


    private void OpenHomeScreen()
    {
        HomeScreen.SetActive(true);
        SurveyScreen.SetActive(false);
        CallingScreen.SetActive(false);
        InCallScreen.SetActive(false);
        KeypadScreen.SetActive(false);

        if (GameManager.Instance.curSceneNumb == 0) return;

        if (isProcessedComplain)
        {
            // �ο�ó�� �Ϸ� �� ��Ȱ��ȭ �ʿ�
            complaintMsgIcon.SetActive(true);
            complaintScreenText.text = complainContent;
        }
    }


    /// <summary>
    /// ��ȭ ���� ȭ�� UI Ȱ��ȭ
    /// </summary>
    private void OpenCallingScreen()
    {
        CallingScreen.SetActive(true); // ������Ʈ Ȱ��ȭ
        if (CallingScreen.TryGetComponent<UnityEngine.UI.Image>(out var img))
            StartCoroutine(GameManager.Instance.FadeIn(img, 0.5f, () => { })); // ���̵���

        if (InteractiveButton.actionEndedFadeIn != null)
            InteractiveButton.actionEndedFadeIn -= OpenCallingScreen;
    }

    /// <summary>
    /// ���� �۽� ��ȭ�� �ɾ���ϴ� ��ȭ��ȣ ������Ʈ
    /// </summary>
    /// <param name="content"></param>
    /// <param name="number">�۽� ��ȭ��ȣ</param>
    private void UpdatedCurrentOutgoingCallContent(ScenarioData data, string number)
    {
        outgoingNumber = number.Replace("-", "");
        complainContent = MainUIManager.complaintPaper_content.text;
    }

    /// <summary>
    /// �Ϸ簡 ������ �� ��, ��ȭ�� ���� �������� UI Ȱ��ȭ
    /// </summary>
    private void OpenEndedDaySurvey()
    {
        StartCoroutine(GameManager.Instance.DelayTime(2f, () =>  // 2�� ������
        {
            SurveyScreen.gameObject.SetActive(true); // ������Ʈ Ȱ��ȭ
            if (SurveyScreen.TryGetComponent<UnityEngine.UI.Image>(out var img))
                StartCoroutine(GameManager.Instance.FadeIn(img, 0.5f, () => { })); // ���̵���

            SurveyScreen.transform.GetChild(2).gameObject.SetActive(true);
        }));
    }

    /// <summary>
    /// ��ȭ ���� ������ ħ���� ���� ������ ���, �̿� ���� �������� UI Ȱ��ȭ
    /// </summary>
    private void OpenUnCallbySilenceSurvey()
    {
        StartCoroutine(GameManager.Instance.DelayTime(2f, () =>  // 2�� ������
        {
            SurveyScreen.gameObject.SetActive(true); // ������Ʈ Ȱ��ȭ
            if (SurveyScreen.TryGetComponent<UnityEngine.UI.Image>(out var img))
                StartCoroutine(GameManager.Instance.FadeIn(img, 0.5f, () => { })); // ���̵���

            SurveyScreen.transform.GetChild(3).gameObject.SetActive(true);
        }));
    }

    /// <summary>
    /// Ű�е� ��ư�� ����
    /// </summary>
    public void PushedKeypad()
    {
        if (inputNumber.Length >= 9)
            return;

        inputNumber += EventSystem.current.currentSelectedGameObject.name;
        numberField.text = inputNumber;
    }

    /// <summary>
    /// Ű�е��� ��ȭ ��ư�� ����, inputNumber �����ͼ� �ο� ��ȭ��ȣ�� ������ ��ȭ ����
    /// </summary>
    public void KeypadCallButton()
    {
        // ��ȭ ���� - �ο��� ����� �����ؼ� ��������Ʈ ȣ��
        if (outgoingNumber == inputNumber)
        {
            isProcessedComplain = true;

            InCallScreen.gameObject.SetActive(true);
            KeypadScreen.gameObject.SetActive(false);
            actionConnectedGoingCall?.Invoke();
            inputNumber = "";
            numberField.text = "";
        }
        // ��ȭ ��ȣ �ٸ� ���
        else { }
    }

    /// <summary>
    /// Ű�е��� ���� ��ư�� ����, inputNumber�� ������ ���ڸ� ����
    /// </summary>
    public void KeypadDelButton()
    {
        if (inputNumber.Length == 0)
            return;

        inputNumber = inputNumber.Remove(inputNumber.Length - 1);
        numberField.text = inputNumber;
    }



    /// <summary>
    /// �ʱ� ���� ��ȭ ���� ����, �̿� ���� �������� UI Ȱ��ȭ
    /// </summary>
    private void FirstTestCallSurvey()
    {
        StartCoroutine(GameManager.Instance.DelayTime(2f, () =>  // 2�� ������
        {
            SurveyScreen.gameObject.SetActive(true);
            if (SurveyScreen.TryGetComponent<UnityEngine.UI.Image>(out var img))
                StartCoroutine(GameManager.Instance.FadeIn(img, 0.5f, () => { })); // ���̵���

            SurveyScreen.transform.GetChild(0).gameObject.SetActive(true);

            InteractiveButton.actionEndedFadeIn -= OpenCallingScreen;
            DiagnosisSystem.actionFirstTestUnCall -= FirstTestUnCallSurvey;
            DiagnosisSystem.actionFirstCallEndedCall -= FirstTestCallSurvey;
        }));
    }

    /// <summary>
    /// �ʱ� ���� ��ȭ ���� �ʾ��� ���, �ؽ�Ʈ ���� ���� ������ ���� ���� ȣ���.
    /// </summary>
    /// <param name="dialog"></param>
    private void FirstTestUnCallSurvey(string dialog)
    {
        EmptyScreen.SetActive(true);

        // �ʱ� ���� ������ ���� ���� UI Ȱ��ȭ
        StartCoroutine(GameManager.Instance.DelayTime(1f, () =>
        {
            EmptyScreenDialogField.gameObject.SetActive(true);
            EmptyScreenDialogField.text = dialog;
        }));

        // ��ǲ�ʵ� on
        StartCoroutine(GameManager.Instance.DelayTime(1.5f, () =>
        {
            EmptyScreenInputFields.SetActive(true);
        }));
    }

    /// <summary>
    /// �ʱ� ���� ���� ��, ��ǲ �ʵ� ���� üũ ��ư
    /// </summary>
    public void FillCheckInputField()
    {
        if (nameField.text == "" || nameField.text.Contains(" "))
        {
            UnityEngine.UI.Text placeholder = nameField.placeholder as UnityEngine.UI.Text;
            placeholder.text = "���� �Ұ�";
            return;
        }
        if (determinationField.text == "")
        {
            UnityEngine.UI.Text placeholder = determinationField.placeholder as UnityEngine.UI.Text;
            placeholder.text = "���� �Ұ�";
            return;
        }

        UserData.Instance.userName = nameField.text;
        UserData.Instance.userDetermination = determinationField.text;

#if UNITY_EDITOR
        Debug.Log($"�ʱ� ���� ���� - ���� �̸�: {UserData.Instance.userName}, ���� �Ѹ���: {UserData.Instance.userDetermination}");
#endif
        InteractiveButton.actionEndedFadeIn -= OpenCallingScreen;
        DiagnosisSystem.actionFirstTestUnCall -= FirstTestUnCallSurvey;
        DiagnosisSystem.actionFirstCallEndedCall -= FirstTestCallSurvey;
    }


    private void OnDestroy()
    {
        if (GameManager.Instance.curSceneNumb == 0)
        {
            InteractiveButton.actionEndedFadeIn -= OpenCallingScreen;
            DiagnosisSystem.actionFirstTestUnCall -= FirstTestUnCallSurvey;
            DiagnosisSystem.actionFirstCallEndedCall -= FirstTestCallSurvey;
        }
        DiagnosisSystem.actionStartIncomingCall -= OpenCallingScreen;
        ConversationManager.actionEndedCallbySilence -= OpenUnCallbySilenceSurvey;
        GameManager.actionEndedDayTime -= OpenEndedDaySurvey;
        CallSurvey.actionEndedSurvey -= OpenHomeScreen;
        DiagnosisSystem.actionUnCall -= OpenHomeScreen;
        ConversationManager.actionEndedCall -= OpenHomeScreen;
    }
}
