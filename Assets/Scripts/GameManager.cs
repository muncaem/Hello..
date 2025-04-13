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
    private bool canPlayTime = true;
    private bool canUpdateDay = false;

    public int day { get; private set; } // ������ ��¥
    [SerializeField] private float secondsPerDay = 180; // �Ϸ翡 �־��� �ð�
    private float dayCallGap;
    private float[] dayCallGapRange = { 25, 35 }; // �Ϸ翡 ��ȭ ���� ����

    public static Action actionUpdatedDay;
    public static Action actionUpdatedCall;
    public static Action actionEndedDayTime;

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
        CallSurvey.actionEndedSurvey += MoveScene;
        CallSurvey.actionEndedSurvey += UpdateDayTimer;

        SetDayCallGap();
    }

    // �� �̵�
    private void MoveScene()
    {
        //if (curSceneNumb > 0) return;
        StartCoroutine(DelayTime(1f, () => SceneManager.LoadScene(++curSceneNumb)));
        CallSurvey.actionEndedSurvey -= MoveScene;
        day = 1;
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

        if (time >= dayCallGap)
            // �Ϸ� �� ���� call gap ���� ������ �ð� �˸� ��������Ʈ
            actionUpdatedCall?.Invoke();

        if ((time >= secondsPerDay && ConversationManager.GlobalCallState == false) 
            /*|| ConversationManager. // �Ϸ� ġ ��ȭ �Ҵ緮 �� ä���� ���*/)
        {
            // �־��� �Ϸ� �ð��� ��� ����ϰų� �Ҵ緮 ä���� ��� ������ ���� ����
            actionEndedDayTime?.Invoke();
            canPlayTime = false;
        }
    }

    // CallSurvey���� �Ϸ� ������ survey �Ϸ��ϸ� ȣ�� => ���������� ���� ���� �Ѿ�� ��.
    private void UpdateDayTimer()
    {
        if (canPlayTime == false)
        {
            actionUpdatedDay?.Invoke();

            time = 0;
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
