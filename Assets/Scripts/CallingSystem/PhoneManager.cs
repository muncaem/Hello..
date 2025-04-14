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
    private InputField nameField; // �̸� ��ǲ�ʵ�
    private InputField determinationField; // ���� ��ǲ �ʵ�


    private void Awake()
    {
        // ���� ���� ��ư ������ ���, ���� ���� FadeIn �� UIó�� �븮�Լ� �Ҵ�
        InteractiveButton.actionEndedFadeIn += OpenCallingScreen;


        // �ʱ� ���� ��, ��ȭ�� ���� �ʾ��� ��� UIó�� �븮�Լ� �Ҵ�
        DiagnosisSystem.actionFirstTestUnCall += FirstTestUnCallSurvey;
        // �ʱ� ���� ��, ��ȭ�� ���� ���� ���� UI �븮�Լ� �Ҵ�
        DiagnosisSystem.actionFirstCallEndedCall += FirstTestCallSurvey;

        // Main ġ�� ��, �۽� ��ȭ �� ���, UIó�� �븮�Լ� �Ҵ�
        DiagnosisSystem.actionStartIncomingCall += OpenCallingScreen;
        // Main ġ�� ��, ��ȭ ���� ������ ��� ���� UI �븮�Լ� �Ҵ�
        ConversationManager.actionEndedCallbySilence += OpenUnCallbySilenceSurvey;
        // Main ġ�� ��, �Ϸ� ������ ���� UI �븮 �Լ� �Ҵ�
        // -> ���� �ϴ� ���� �ð� ���߱�. ���� �Ϸ��ϸ� ������ ���� UI �� ó��
        GameManager.actionEndedDayTime += OpenEndedDaySurvey;

        // Home Screen���� �ݹ� �� ��������Ʈ
        // ���� ����
        CallSurvey.actionEndedSurvey += OpenHomeScreen;
        // ��ȭ ���� ����
        DiagnosisSystem.actionUnCall += OpenHomeScreen;
        // ��ȭ �Ϸ� ����
        ConversationManager.actionEndedCall += OpenHomeScreen;

        // UI �Ҵ�
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
