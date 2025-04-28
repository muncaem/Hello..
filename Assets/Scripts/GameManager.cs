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
    private float dayCallGap;
    private float[] dayCallGapRange = { 25, 35 }; // �Ϸ翡 ��ȭ ���� ����

    //public static Action actionUpdatedDay;
    //public static Action actionUpdatedCall;
    //public static Action actionEndedDayTime;
    public static Action actionChangedScene; // Scene Change Follow

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
    }
    // ��¥ ī��Ʈ ���
    private void DayCount()
    {
        time += Time.deltaTime;

        //gapTime += Time.deltaTime;

        //if (gapTime >= dayCallGap && !ConversationManager.GlobalCallState)
        //{
        //    // �Ϸ� �� ���� call gap ���� ������ �ð� �˸� ��������Ʈ
        //    actionUpdatedCall?.Invoke();

        //    // ���� call gap ����
        //    SetDayCallGap();
        //    gapTime = 0;
        //}

        // ��ȭ ���� �ƴϸ� gapTime �ױ�
        if (!ConversationManager.GlobalCallState)
            gapTime += Time.deltaTime;

        // ��ȭ ���� ���� ��
        if (gapTime >= dayCallGap)
        {
            EventHub.actionReachedCallGap?.Invoke();
            SetDayCallGap();
            gapTime = 0f;
        }

        // �Ϸ� ���� ����
        if ((time >= secondsPerDay && ConversationManager.GlobalCallState == false) 
            /*|| ConversationManager. // �Ϸ� ġ ��ȭ �Ҵ緮 �� ä���� ���*/)
        {
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
            EventHub.actionUpdatedDay?.Invoke();

            time = 0;
            gapTime = 0;
            day += 1;

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
}
