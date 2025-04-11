using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class DiagnosisSystem : MonoBehaviour
{
    ///  <summary>
    /// ���� ���� ���� �ӽ� �Ҵ�
    private int preFactor = 0; // ȸ�� ���� - ��ȭ�� �� �ްų� ����
    private int midFactor = 0; // ��ȭ �Ҿ� - ��ȭ �� ���� �Ҿ�
    private int postFactor = 0; // ���� ���� - ��ȭ�� ���� �� ��ȸ �� �Ҿ�

    private int preUserDataReply = 0; // �ʱ� ���� ��, ���� ��� Ƚ�� üũ
    /// </summary>

    // �ʱ� ���� Ȯ�ο� bool��
    private bool isFirstCheckEnded = false;
    // ��ȸ �� ��ȭ �޾Ҵ��� ���� üũ => ȸ�� ���� +1
    private bool isTakeCall = false;
    // ��ȭ ���� ��ȸ
    private int TakeCallChance = 3;
    // Main ���� ġ�� ��, �Ϸ�� ������ �ɾ�� �ϴ� ��ȭ Ƚ��
    private int outGoingCall;
    // Main ���� ġ�� ��, �Ϸ�� ������ �޾ƾ� �ϴ� ��ȭ Ƚ��
    private int inCompingCall;

    // ����ũ �̹��� �� ��ư
    [Header("Function")]
    [SerializeField] private MicRecorder MicRecorder;
    [SerializeField] private TTSChanger TTSChanger;
    [Header("UI")]
    [SerializeField] private GameObject EmptyScreen;
    [SerializeField] private GameObject InputFieldUI;
    [SerializeField] private GameObject SurveyScreen;
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
        CallSurvey.actionUpdatedSurvey += UpdateScoreBySituation;
        CallSurvey.actionEndedSurvey += ReturnFinalScore;
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


    // Main ������ �۽�/���ŷ� ���� ����
    private void StartMainTherapy()
    {
        // �Ϸ� ���� ������ ��� pre�� avg(mid + post)���� ũ�� �۽��� 60%�̻� ������ ������ 60% �̻� �� ���� ��
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



    // ��Ȳ�� ���� ���� �߰�
    private void UpdateScoreBySituation(string situationId)
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
    private void ReturnFinalScore()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            UserData.Instance.firstPreFactor = preFactor;
            UserData.Instance.firstMidFactor = midFactor;
            UserData.Instance.firstPostFactor = postFactor;

#if UNITY_EDITOR
            Debug.Log($"preFactor: {preFactor}, midFactor: {midFactor}, postFactor: {postFactor}");
#endif
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
            UserData.Instance.userName = userComment; /// ���� �̸� ������ ����
            TTSChanger.NormalSpeak(dialogs[++preUserDataReply]); // �ι�° ���� ���� ����
        }
        else if (preUserDataReply == 2) // �ι�° ���� ���� ���� ���
        {
            UserData.Instance.userDetermination = userComment; /// ���� ���� ������ ����

            //��ȭ ���� UI�߰� ����

            TTSChanger.NormalSpeak($"�����մϴ�, {UserData.Instance.userName}��. ����ǥ�μ� ������ �� ��Ź�帳�ϴ�~");
#if UNITY_EDITOR
            Debug.Log($"�ʱ� ���� ���� - ���� �̸�: {UserData.Instance.userName}, ���� �Ѹ���: {UserData.Instance.userDetermination}");
#endif
            preUserDataReply += 1;

            //DoSurvey();

            //��ȭ ���� UI ���� ���� ������ ������ ����
            SurveyScreen.gameObject.SetActive(true);
            SurveyScreen.transform.GetChild(0).gameObject.SetActive(true);
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

        UserData.Instance.userName = nameField.text;
        UserData.Instance.userDetermination = determinationField.text;

#if UNITY_EDITOR
        Debug.Log($"�ʱ� ���� ���� - ���� �̸�: {UserData.Instance.userName}, ���� �Ѹ���: {UserData.Instance.userDetermination}");
#endif

        //DoSurvey();
        if (SurveyScreen.transform.childCount > 1)
        {
            SurveyScreen.gameObject.SetActive(true);
            SurveyScreen.transform.GetChild(1).gameObject.SetActive(true);
        }
    }



    // ���� ��, ��������Ʈ ����
    private void OnDestroy()
    {
        TTSChanger.actionTTSEnded -= OnTTSEnded;
        MicRecorder.actionMicRecorded -= OnRecordEnded;
    }
}
