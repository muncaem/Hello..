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


    public static Action OnTakeCall; // ���ܽý������κ��� ���� ��ȭ�� �޾��� ��� ConversationManager�� ��ȭ ����
    public static Action<string> actionFirstTestUnCall;
    public static Action actionFirstCallEndedCall;
    public static Action actionEndedSaveScore; // �ʱ� ���� ���� UserData�� ���� �Ϸ� �� �븮�� ȣ��
    public static Action actionUnCall;
    public static Action<int> actionUpdatedOutGoingValue; // ������ ������Ʈ �Ǵ� ���� ��ȭ��
    public static Action actionStartIncomingCall;

    private Coroutine waitCallCoroutine;

    private void Awake()
    {
        if (Instance == null) Instance = this;

        //isFirstScene = SceneManager.GetActiveScene().buildIndex == 0 ? true : false;
        //if (SceneManager.GetActiveScene().name.Contains("Start"))
        //    isFirstScene = true;

        if (GameManager.Instance.curSceneNumb == 0)
        {
            TTSChanger.actionTTSEnded += OnTTSEnded;
            MicRecorder.actionMicRecorded += OnRecordEnded;
        }
        CallSurvey.actionUpdatedSurvey += UpdateScoreBySituation;
        CallSurvey.actionEndedSurvey += ReturnFinalScore;
        MicRecorder.actionUpdatedFactor += UpdateScoreBySituation;
        GameManager.actionUpdatedDay += StartMainTherapy; // �Ϸ簡 ���������� �۽�/���ŷ� ����
        GameManager.actionUpdatedCall += InComingCall; // n�ʸ��� ��ȭ ���� ��
        ConversationManager.actionEndedCall += RefreshCallState;
    }

    /// <summary>
    /// �ʱ� ���� ���� ��ư(���� ���� ��ư)
    /// </summary>
    public void StartDiagnosis()
    {
        //�ʱ� ��ȭ �ޱ�/���� �׽�Ʈ
        StartCoroutine(InitCheck());
    }
    private IEnumerator InitCheck()
    {
        yield return new WaitForSeconds(1.5f);

        for (int i = 0; i < TakeCallChance; i++)
        {
            if (!isCalled)
            {
                SoundManager.instance.Play("bell");

                yield return new WaitForSeconds(4);
            }
        }
        yield return null;

        // 3�� �� �︰ ����, ��ȭ ���� ���� ������ ����
        if (GameManager.Instance.curSceneNumb == 0) // �ʱ� ������ ���
            actionFirstTestUnCall?.Invoke(dialogs[0]);
        // Main ġ�� ���
        else
        {
            actionUnCall?.Invoke();
        }

        UnTakeCall();
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

        actionUpdatedOutGoingValue?.Invoke(outGoingCall);

        //InComingCall();
        // ù ���� call
        if (!isCalled)
        {
            actionStartIncomingCall?.Invoke();
            if (waitCallCoroutine != null)
            {
                StopCoroutine(waitCallCoroutine);
                waitCallCoroutine = null;
            }
            waitCallCoroutine = StartCoroutine(WaitForUserCallResponse());
        }
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
        actionStartIncomingCall?.Invoke();
        // �޴����� ��ȭ���� UI ����

        if (waitCallCoroutine != null)
        {
            StopCoroutine(waitCallCoroutine);
            waitCallCoroutine = null;
        }
        waitCallCoroutine = StartCoroutine(WaitForUserCallResponse());
    }
    private IEnumerator WaitForUserCallResponse() /// incomming call ���� ������ ���
    {
        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < TakeCallChance; i++)
        {
            if (!isCalled)
            {
                SoundManager.instance.Play("bell");
                yield return new WaitForSeconds(4);
            }
        }
        //actionUnCall?.Invoke();
        UnTakeCall();
    }


    /// <summary>
    /// ��ȭ �ޱ� ��ư ������ ���
    /// </summary>
    public void TakeCall()
    {
        SoundManager.instance.Clear();
        StopAllCoroutines();
        isCalled = true;

        if (GameManager.Instance.curSceneNumb == 0)
            CheckForFirstData(); // �ʱ� ���ܿ� ���� ��ȭ
        else
            OnTakeCall?.Invoke(); // ��ȭ�� �޾��� ��� ����Ǵ� ��������Ʈ => StartComingConversation()
    }

    /// <summary>
    /// ��ȭ ���� ��ư / ���߿� ���� ��ư ������ ���
    /// </summary>
    public void UnTakeCall()
    {
        StopAllCoroutines();
        if (waitCallCoroutine != null)
        {
            StopCoroutine(waitCallCoroutine);
            waitCallCoroutine = null;
        }
        SoundManager.instance.Clear();
        preFactor++; //���� ���� ���� + 1

#if UNITY_EDITOR
        Debug.Log("PreFactor: " + preFactor);
#endif
        if (GameManager.Instance.curSceneNumb == 0)
        {
            actionFirstTestUnCall?.Invoke(dialogs[0]); // �ʱ� ���� ��ȭ ������ �ؽ�Ʈ�� ����

            TTSChanger.actionTTSEnded -= OnTTSEnded;
            MicRecorder.actionMicRecorded -= OnRecordEnded;
        }
        else
        {
            outGoingCall++; // ���� ��, ���� ��ȭ �߰�
            // ���� �ý��� ���� ���� => UI ���� �� ��������Ʈ�� �ű�� ���
            UserData.Instance.userReputation--;
            actionUnCall?.Invoke();
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
            CallSurvey.actionEndedSurvey -= ReturnFinalScore;
            actionEndedSaveScore?.Invoke();
            Destroy(MicRecorder);
            //Destroy(TTSChanger);
            Destroy(transform.GetChild(0).gameObject);
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
            actionFirstTestUnCall?.Invoke(dialogs[0]);
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

            actionFirstCallEndedCall?.Invoke();

            isCalled = false; // �ʱ� ���� ��ȭ ����

            // ��������Ʈ ����
            TTSChanger.actionTTSEnded -= OnTTSEnded;
            MicRecorder.actionMicRecorded -= OnRecordEnded;
            return;
        }
    }
}
