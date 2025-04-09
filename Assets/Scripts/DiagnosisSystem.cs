using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class DiagnosisSystem : MonoBehaviour
{
    ///  <summary>
    /// ���� ���� ���� �ӽ� �Ҵ�
    private int preFactor = 0; // ȸ�� ���� - ��ȭ�� �� �ްų� ����
    private int midFactor = 0; // ��ȭ �Ҿ� - ��ȭ �� ���� �Ҿ�
    private int postFactor = 0; // ���� ���� - ��ȭ�� ���� �� ��ȸ �� �Ҿ�

    private int preUserDataReply = 0;
    private string userName;
    private string userDetermination;
    /// </summary>


    // ��ȸ �� ��ȭ �޾Ҵ��� ���� üũ => ȸ�� ���� +1
    private bool isTakeCall = false;
    // ��ȭ ���� ��ȸ
    private int TakeCallChance = 3;

    // ����ũ �̹��� �� ��ư
    [Header("Function")]
    [SerializeField] private MicRecorder MicRecorder;
    [SerializeField] private TTSChanger TTSChanger;
    [Header("UI")]
    [SerializeField] private GameObject EmptyScreen;
    [SerializeField] private GameObject InputFieldUI;
    private GameObject MikeOnBtn; // ���� �̸� �Է��ϴ� inputField�� �ٲٱ�
    private UnityEngine.UI.Image MikeOffImg; // ���� �Է��ϴ� inputField�� �ٲٱ�
    private UnityEngine.UI.Text textUI; // ���� ���� ���� ���� �ȳ� �ؽ�Ʈ
    private InputField nameField; // �̸� ��ǲ�ʵ�
    private InputField determinationField; // ���� ��ǲ �ʵ�

    [SerializeField] private string[] dialogs; // ���̾�α�


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
        //�ʱ� ��ȭ �ޱ�/���� �׽�Ʈ
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

        // 3�� �� �︰ ����, ��ȭ ���� ���� ������ ����
        EmptyScreen.SetActive(true);
        UnTakeCall();
    }



    // ��ȭ �ޱ� ��ư ������ ���, ��ũ��Ʈ ���� �� ����ũ
    public void TakeCall()
    {
        SoundManager.instance.Clear();
        StopAllCoroutines();
        isTakeCall = true;

        // ���ܿ� ���� ��ȭ
        if (preUserDataReply == 0)
        {
            TTSChanger.NormalSpeak(dialogs[++preUserDataReply]);
        }
    }
    private void OnTTSEnded() // ���� ���� ����
    {
        if (dialogs.Length <= preUserDataReply) return;
        MicRecorder.StartRecording(); /// ����ũ ���� ����
    }
    private void OnRecordEnded(string userComment) // ����ũ ���� ���� �� ȣ��
    {
        if (preUserDataReply == 1) // ù��° ���� ���� ���� ���
        {
            userName = userComment; /// ���� �̸� ������ ����
            TTSChanger.NormalSpeak(dialogs[++preUserDataReply]); // �ι�° ���� ���� ����
        }
        else if (preUserDataReply == 2) // �ι�° ���� ���� ���� ���
        {
            userDetermination = userComment; /// ���� ���� ������ ����

            //��ȭ ���� UI�߰� ����

            TTSChanger.NormalSpeak($"�����մϴ�, {userName}��. ����ǥ�μ� ������ �� ��Ź�帳�ϴ�~");
#if UNITY_EDITOR
            Debug.Log($"�ʱ� ���� ���� - ���� �̸�: {userName}, ���� �Ѹ���: {userDetermination}");
#endif
            preUserDataReply += 1;

            DoSurvey();
            return;
        }
    }

    // ��ȭ ���� ��ư / ���߿� ���� ��ư ������ ���, ��ũ��Ʈ ���� �� ����ũ
    public void UnTakeCall()
    {
        SoundManager.instance.Clear();
        StopAllCoroutines();
        preFactor++; //���� ���� ���� + 1

#if UNITY_EDITOR
        Debug.Log("PreFactor: " + preFactor);
#endif

        // ���ܿ� ��� üũ
        StartCoroutine(GameManager.Instance.DelayTime(1f,
        () =>
        {
            textUI.gameObject.SetActive(true);
            textUI.text = dialogs[0];
        }
            ));

        // ��ǲ�ʵ� on
        StartCoroutine(GameManager.Instance.DelayTime(1.5f,
            () =>
            {
                InputFieldUI.SetActive(true);
            }
            ));
    }

    // ��ǲ �ʵ� ���� üũ
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

        userName = nameField.text;
        userDetermination = determinationField.text;

#if UNITY_EDITOR
        Debug.Log($"�ʱ� ���� ���� - ���� �̸�: {userName}, ���� �Ѹ���: {userDetermination}");
#endif

        DoSurvey();
    }

    // ���� ���� (��ȭ ���� ��ȣ���� ����)
    private void DoSurvey()
    {

    }

    // ��Ȳ�� ���� ���� �߰�
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
                Debug.LogWarning("�� �� ���� ��Ȳ ID: " + situationId);
#endif
                break;
        }
    }
}
