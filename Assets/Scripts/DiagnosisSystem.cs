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

    // �ʱ� ���� Ȯ�ο� bool��
    private bool isFirstCheckEnded = false;
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


    public static Action OnTakeCall; // ���ܽý������κ��� ���� ��ȭ�� �޾��� ��� ConversationManager�� ��ȭ ����


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
    // �ʱ� ���� ���� ��ư(���� ���� ��ư)
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



    // ��ȭ �ޱ� ��ư ������ ���,
    public void TakeCall()
    {
        SoundManager.instance.Clear();
        StopAllCoroutines();
        isTakeCall = true;

        if (isFirstCheckEnded == false)
            CheckForFirstData(); // �ʱ� ���ܿ� ���� ��ȭ
        else
            OnTakeCall?.Invoke(); // ��ȭ�� �޾��� ��� ����Ǵ� ��������Ʈ
    }

    // ��ȭ ���� ��ư / ���߿� ���� ��ư ������ ���,
    public void UnTakeCall()
    {
        SoundManager.instance.Clear();
        StopAllCoroutines();
        preFactor++; //���� ���� ���� + 1

#if UNITY_EDITOR
        Debug.Log("PreFactor: " + preFactor);
#endif
        if (isFirstCheckEnded == false)
            CheckForFirstData(); // �ʱ� ���ܿ� ������ ����
    }


    // ���� ���� (���� üũ)
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

    // �ʱ� ���ܿ� ���� ó��
    private void CheckForFirstData()
    {
        // �ʱ� ���� ������ ��� return
        if (isFirstCheckEnded == true) return;

        // ��ȭ�� �޾��� ���
        if (isTakeCall)
        {
            if (preUserDataReply == 0)
            {
                TTSChanger.NormalSpeak(dialogs[++preUserDataReply]);
            }
        }
        // ��ȭ�� ���� �ʾ��� ���
        else
        {
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

        isFirstCheckEnded = true;
    }

    // �ʱ� ���� ���� ����
    private void OnTTSEnded()
    {
        if (dialogs.Length <= preUserDataReply) return;
        MicRecorder.StartRecording(); /// ����ũ ���� ����
    }
    // ����ũ ���� ���� �� ȣ��
    private void OnRecordEnded(string userComment)
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

    // �ʱ� ���� ���� ��, ��ǲ �ʵ� ���� üũ ��ư
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



    // ���� ��, ��������Ʈ ����
    private void OnDestroy()
    {
        TTSChanger.actionTTSEnded -= OnTTSEnded;
        MicRecorder.actionMicRecorded -= OnRecordEnded;
    }
}
