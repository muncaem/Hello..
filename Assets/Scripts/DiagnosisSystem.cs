using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class DiagnosisSystem : MonoBehaviour
{
    public static DiagnosisSystem Instance;
    ///  <summary>
    /// ���� ���� ���� �ӽ� �Ҵ�
    private int preFactor = 0; // ȸ�� ���� - ��ȭ�� �� �ްų� ����
    private int midFactor = 0; // ��ȭ �Ҿ� - ��ȭ �� ���� �Ҿ�
    private int postFactor = 0; // ���� ���� - ��ȭ�� ���� �� ��ȸ �� �Ҿ�

    private int preUserDataReply = 0; // �ʱ� ���� ��, ���� ��� Ƚ�� üũ
    /// </summary>

    //// �ʱ� ���� Ȯ�ο� bool��
    //private bool isFirstScene;
    // ��ȸ �� ��ȭ �޾Ҵ��� ���� üũ �� ���� ��ȭ ������ üũ => ȸ�� ���� +1
    public static bool isCalled { get; private set; } = false;
    // ��ȭ ���� ��ȸ
    private int TakeCallChance = 3;

    // Main ���� ġ�� ��, �Ϸ� �� ��ȭ��
    private int totalCallValue = 6;
    // Main ���� ġ�� ��, �Ϸ�� ������ �ɾ�� �ϴ� ��ȭ Ƚ��
    private int outGoingCall;
    // Main ���� ġ�� ��, �Ϸ�� ������ �޾ƾ� �ϴ� ��ȭ Ƚ��
    private int inCompingCall;

    // ����ũ �̹��� �� ��ư
    [Header("Function")]
    [SerializeField] private MicRecorder MicRecorder;
    [SerializeField] private TTSChanger TTSChanger;
    //[SerializeField] private OutGoingCallManager OutGoingCallManager;

    [Header("DiagnosisText")]
    [SerializeField] private string[] dialogs; // ���̾�α�

    //public static Action OnTakeCall; // ���ܽý������κ��� ���� ��ȭ�� �޾��� ��� ConversationManager�� ��ȭ ����
    //public static Action<string> actionFirstTestUnCall;
    //public static Action actionFirstCallEndedCall;
    //public static Action actionEndedSaveScore; // �ʱ� ���� ���� UserData�� ���� �Ϸ� �� �븮�� ȣ��
    //public static Action actionUnCall;
    //public static Action<int> actionUpdatedOutGoingValue; // ������ ������Ʈ �Ǵ� ���� ��ȭ��
    //public static Action actionStartIncomingCall;

    private Coroutine waitCallCoroutine;

    private void Awake()
    {
        if (Instance == null) Instance = this;

        //isFirstScene = SceneManager.GetActiveScene().buildIndex == 0 ? true : false;
        //if (SceneManager.GetActiveScene().name.Contains("Start"))
        //    isFirstScene = true;

        if (GameManager.Instance.curSceneNumb == 0)
        {
            EventHub.actionTTSEnded += OnTTSEnded;
            EventHub.actionMicRecorded += OnRecordEnded;
        }
        EventHub.actionUpdatedSurvey += UpdateScoreBySituation;
        EventHub.actionSurveyEnded += ReturnFinalScore;
        EventHub.actionUpdatedSpeakSituationFactor += UpdateScoreBySituation;

        EventHub.actionUpdatedDay += StartMainTherapy; // �Ϸ簡 ���������� �۽�/���ŷ� ����
        EventHub.actionReachedCallGap += InComingCall; // n�ʸ��� ��ȭ ���� ��
        GameManager.actionChangedScene += OnChangedScene;

        EventHub.actionEndedCallBySpeak += RefreshCallState;

        EventHub.actionConnectedComingCall += TakeCallDiagnosis;
        EventHub.actionDisconnectedComingCall += UnTakeCallDiagnosis;
    }

    private void OnChangedScene()
    {
        // ��������Ʈ ����
        EventHub.actionTTSEnded -= OnTTSEnded;
        EventHub.actionMicRecorded -= OnRecordEnded;

        EventHub.actionConnectedComingCall -= TakeCallDiagnosis;
        EventHub.actionDisconnectedComingCall -= UnTakeCallDiagnosis;

        StartCoroutine(RebindPhoneManagerActions());
    }
    private IEnumerator RebindPhoneManagerActions()
    {
        Debug.Log("RebindPhoneManagerActions In");
        yield return new WaitUntil(() => FindObjectOfType<PhoneManager>() != null);
        Debug.Log("RebindPhoneManagerActions end wait");
        EventHub.actionConnectedComingCall += TakeCallDiagnosis;
        EventHub.actionDisconnectedComingCall += UnTakeCallDiagnosis;
    }

    /// <summary>
    /// �ʱ� ���� ���� ��ư(���� ���� ��ư)
    /// </summary>
    public void StartDiagnosis()
    {
        //�ʱ� ��ȭ �ޱ�/���� �׽�Ʈ
        //StartCoroutine(InitCheck());
        StartCoroutine(GameManager.Instance.DelayTime(1.5f, () =>
        {
            EventHub.actionStartIncomingCall?.Invoke();
        }));
    }


    /// <summary>
    /// Main ������ �۽�/���ŷ� ���� ����
    /// �Ϸ� ���� ������ ���
    /// pre�� avg(mid + post)���� ũ�� �۽��� 50% + 1�̻�. ������ ������ 50% + 1�̻� �� ���� ��
    /// </summary>
    private void StartMainTherapy()
    {
        StartCoroutine(DelayedMainTherapy());
    }
    private IEnumerator DelayedMainTherapy()
    {
        yield return null;

        int firstPre = UserData.Instance.firstPreFactor;
        int firstMid = UserData.Instance.firstMidFactor;
        int firstPost = UserData.Instance.firstPostFactor;
        Debug.Log($"{firstPre} {firstMid} {firstPost}");

        if (firstPre > (firstMid + firstPost) / 2)
        {
            inCompingCall = UnityEngine.Random.Range(totalCallValue / 2 + 1, totalCallValue);
            outGoingCall = totalCallValue - inCompingCall;
        }
        else if (firstPre < (firstMid + firstPost) / 2)
        {
            outGoingCall = UnityEngine.Random.Range(totalCallValue / 2 + 1, totalCallValue);
            inCompingCall = totalCallValue - outGoingCall;
        }
        else
        {
            outGoingCall = totalCallValue / 2;
            inCompingCall = totalCallValue - outGoingCall;
        }

        Debug.Log($"outgoingcall: {outGoingCall}, incomingcall: {inCompingCall}");


        EventHub.actionUpdatedOutGoingValue?.Invoke(outGoingCall);

        //// ù ���� call
        //actionStartIncomingCall?.Invoke();

        //StartCoroutine(GameManager.Instance.DelayTime(1.5f, () =>
        //{
        //    if (!isCalled)
        //    {
        //        actionStartIncomingCall?.Invoke();
        //        if (waitCallCoroutine != null)
        //        {
        //            StopCoroutine(waitCallCoroutine);
        //            waitCallCoroutine = null;
        //        }
        //        waitCallCoroutine = StartCoroutine(WaitForUserCallResponse());
        //    }
        //}));
    }


    private void RefreshCallState()
    {
        // ConversationManager���� ��ȭ ������ ȣ��Ǿ� ��ȭ ���� false�� ����
        isCalled = false;
    }

    /// <summary>
    /// n�ʸ��� ��ȭ�� ���� �۽� ��ȭ Conversation ���� - GameManager�� n�� ���� �븮�� ����
    /// </summary>
    private void InComingCall()
    {
        // ��ȭ ���̸� return
        if (isCalled) return;

        // �Ϸ� �������ڸ��� n�� �� ��ȭ ���� ��
        EventHub.actionStartIncomingCall?.Invoke();
    }


    /// <summary>
    /// ��ȭ �ޱ� ��ư ������ ���
    /// </summary>
    public void TakeCallDiagnosis()
    {
        isCalled = true;

        if (GameManager.Instance.curSceneNumb == 0)
            CheckForFirstData(); // �ʱ� ���ܿ� ���� ��ȭ
        else
            EventHub.OnTakeCall?.Invoke(); // ��ȭ�� �޾��� ��� ����Ǵ� ��������Ʈ => StartComingConversation()
    }

    /// <summary>
    /// ��ȭ ���� ��ư / ���߿� ���� ��ư ������ ���
    /// </summary>
    public void UnTakeCallDiagnosis()
    {
        preFactor++; //���� ���� ���� + 1

#if UNITY_EDITOR
        Debug.Log("PreFactor: " + preFactor);
#endif
        if (GameManager.Instance.curSceneNumb == 0)
        {
            EventHub.actionFirstTestUnCall?.Invoke(dialogs[0]); // �ʱ� ���� ��ȭ ������ �ؽ�Ʈ�� ����
        }
        else
        {
            outGoingCall++; // ���� ��, ���� ��ȭ �߰�
            // ���� �ý��� ���� ���� => UI ���� �� ��������Ʈ�� �ű�� ���
            UserData.Instance.userReputation -= 5;
            EventHub.actionUpdateReputation?.Invoke();
            EventHub.actionEndedCallBySelect?.Invoke();
            isCalled = false;
        }
    }


    /// <summary>
    /// ��Ȳ�� ���� ���� �߰�
    /// </summary>
    /// <param name="situationId"> ���� ���� </param>
    private void UpdateScoreBySituation(string situationId)
    {
#if UNITY_EDITOR
        //Debug.Log($"UpdateScoreBySituation: {situationId}");
#endif
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

        EventHub.actionUpdatePhobiaBar?.Invoke(preFactor, midFactor, postFactor);
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
            EventHub.actionSurveyEnded -= ReturnFinalScore;
            EventHub.actionFirstEndedSaveScore?.Invoke();
            Destroy(MicRecorder); // �ʱ� ���ܿ� STT ����
            Destroy(transform.GetChild(0).gameObject); // �ʱ� ���ܿ� TTS ����

            // ���� ���� ġ�� ���� �ʱ� ���� ��� �ʱ�ȭ
            preFactor = 0;
            midFactor = 0;
            postFactor = 0;
        }
    }

    /// <summary>
    /// �ʱ� ���ܿ� ���� ó��
    /// </summary>
    private void CheckForFirstData()
    {
        // ��ȭ�� �޾��� ���
        if (isCalled)
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
            EventHub.actionFirstTestUnCall?.Invoke(dialogs[0]);
        }
    }

    /// <summary>
    /// �ʱ� ���� ���� ����
    /// </summary>
    private void OnTTSEnded()
    {
        if (dialogs.Length <= preUserDataReply) return;
        MicRecorder.StartRecording(); /// ����ũ ���� ����
    }
    /// <summary>
    /// ����ũ ���� ���� �� ȣ�� ��
    /// </summary>
    /// <param name="userComment"> ���� ���� </param>
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

            TTSChanger.NormalSpeak($"�����մϴ�, {UserData.Instance.userName}��. ����ǥ�μ� ������ �� ��Ź�帳�ϴ�~");
#if UNITY_EDITOR
            Debug.Log($"�ʱ� ���� ���� - ���� �̸�: {UserData.Instance.userName}, ���� �Ѹ���: {UserData.Instance.userDetermination}");
#endif
            preUserDataReply += 1;

            EventHub.actionFirstCallEndedCall?.Invoke();

            isCalled = false; // �ʱ� ���� ��ȭ ����

            //// ��������Ʈ ����
            //TTSChanger.actionTTSEnded -= OnTTSEnded;
            //MicRecorder.actionMicRecorded -= OnRecordEnded;
            return;
        }
    }
}
