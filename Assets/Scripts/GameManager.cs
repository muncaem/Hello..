using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public int curSceneNumb { get; private set; }
    private float time = 0; // ������ �Ϸ�ġ �ð�
    private float gapTime = 0;
    private bool canPlayTime = false;
    private bool canUpdateDay = false;

    public int day { get; private set; } // ������ ��¥
    [SerializeField] private float secondsPerDay = 180; // �Ϸ翡 �־��� �ð�
    [SerializeField] private int maxDay = 3; // �ΰ��� ���� ��¥
    private float dayCallGap;
    private float[] dayCallGapRange = { 25, 35 }; // �Ϸ翡 ��ȭ ���� ����

    public static Action actionChangedScene; // Scene Change Follow

    /// <summary>
    /// �׽�Ʈ�� - Skip Day Button Ȱ��ȭ ����
    private int testButtonAccess = 0;
    private Button SkipButton;
    /// </summary>


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

        curSceneNumb = SceneManager.GetActiveScene().buildIndex;

        EventHub.actionFirstEndedSaveScore += MoveScene;
        EventHub.actionSurveyEnded += UpdateDayTimer;
    }

    // �� �̵�
    private void MoveScene()
    {
        SceneManager.LoadScene(++curSceneNumb);
        EventHub.actionSurveyEnded -= MoveScene;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // ���� ������ �ε� �Ǿ��� �� ȣ���
        if (scene.buildIndex == 1)
        {
            StartCoroutine(InitDay());
            actionChangedScene?.Invoke();
        }
    }

    private IEnumerator InitDay()
    {
        yield return new WaitForSeconds(1f);
        day = 1;
        EventHub.actionUpdatedDay?.Invoke();
        SetDayCallGap();
        canPlayTime = true;

        Debug.Log($"SetDayCallGap: {dayCallGap}");
    }

    private void Update()
    {
        if (curSceneNumb == 1 && canPlayTime)
            DayCount();

#if UNITY_EDITOR
        /// �׽�Ʈ�� - Skip Day Button Ȱ��ȭ ����
        TestAccess();
#endif
    }

    // ��¥ ī��Ʈ ���
    private void DayCount()
    {
        time += Time.deltaTime;

        // ��ȭ ���� �ƴϸ� gapTime �ױ�
        if (!ConversationManager.GlobalCallState)
        {
            gapTime += Time.deltaTime;
        }

        // ��ȭ ���� ���� ��
        if (gapTime >= dayCallGap)
        {
            Debug.Log("gapTime >= dayCallGap");
            EventHub.actionReachedCallGap?.Invoke();
            SetDayCallGap();
            gapTime = 0f;
        }

        // �Ϸ� ���� ����
        if (time >= secondsPerDay && ConversationManager.GlobalCallState == false)
        /*|| ConversationManager. // �Ϸ� ġ ��ȭ �Ҵ緮 �� ä���� ���*/
        {
            Debug.Log("time >= secondsPerDay && ConversationManager.GlobalCallState == false");
            // �־��� �Ϸ� �ð��� ��� ����ϰų� �Ҵ緮 ä���� ��� ������ ���� ����
            EventHub.actionEndedDayTime?.Invoke();
            canPlayTime = false;
        }
    }

    // CallSurvey���� �Ϸ� ������ survey �Ϸ��ϸ� ȣ�� => ���������� ���� ���� �Ѿ�� ��.
    private void UpdateDayTimer()
    {
        if (canPlayTime == false && curSceneNumb == 1)
        {
            time = 0;
            gapTime = 0;
            day += 1;

            if (day >= 4)
            {
                EventHub.actionEndGame?.Invoke();
#if UNITY_EDITOR
                print("<color=red>End Day</color>");
#endif
                return;
            }

            EventHub.actionUpdatedDay?.Invoke();

            canPlayTime = true;
            canUpdateDay = false;

            SetDayCallGap();
        }
    }

    // �Ϸ� �� ��ȭ ���� ���� ���� �� ����
    private void SetDayCallGap()
    {
        dayCallGap = UnityEngine.Random.Range(dayCallGapRange[0], dayCallGapRange[1] + 1);
    }


    // ������ �ڷ�ƾ
    public IEnumerator DelayTime(float delay, System.Action onComplete)
    {
        yield return new WaitForSeconds(delay);
        onComplete?.Invoke();
    }

    //���̵� �� �ڷ�ƾ
    public IEnumerator FadeIn(Image fadeImg, float duration, System.Action onComplete)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsed / duration);
            Color color = fadeImg.color;
            color.a = alpha;
            fadeImg.color = color;
            yield return null;
        }

        onComplete?.Invoke();
    }

    /// <summary>
    /// ���߿� DaySkip Tester
    /// </summary>
    public void SkipDay()
    {
        canPlayTime = false;
        UpdateDayTimer();
        //print($"���� ��¥ {day}");
    }
    private void TestAccess()
    {
        if (Input.GetKeyDown(KeyCode.F11))
        {
            if (!SkipButton)
                SkipButton = GameObject.FindWithTag("ForTest").transform.GetChild(0).GetComponent<Button>();
            testButtonAccess++;
            if (testButtonAccess >= 3 && SkipButton)
            {
                SkipButton.onClick.AddListener(SkipDay);
                SkipButton.gameObject.SetActive(true);
            }
        }
    }
}
