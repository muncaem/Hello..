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
    private float time = 0; // 누적된 하루치 시간
    private bool canPlayTime = true;
    private bool canUpdateDay = false;

    public int day { get; private set; } // 누적된 날짜
    [SerializeField] private float secondsPerDay = 180; // 하루에 주어진 시간
    private float dayCallGap;
    private float[] dayCallGapRange = { 25, 35 }; // 하루에 전화 오는 간격

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

    // 씬 이동
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
    // 날짜 카운트 기능
    private void DayCount()
    {
        time += Time.deltaTime;

        if (time >= dayCallGap)
            // 하루 당 수신 call gap 지날 때마다 시간 알림 델리게이트
            actionUpdatedCall?.Invoke();

        if ((time >= secondsPerDay && ConversationManager.GlobalCallState == false) 
            /*|| ConversationManager. // 하루 치 통화 할당량 다 채웠을 경우*/)
        {
            // 주어진 하루 시간을 모두 사용하거나 할당량 채웠을 경우 마무리 설문 진행
            actionEndedDayTime?.Invoke();
            canPlayTime = false;
        }
    }

    // CallSurvey에서 하루 마무리 survey 완료하면 호출 => 실질적으로 다음 날로 넘어가게 함.
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

    // 하루 당 전화 오는 간격 랜덤 값 조절
    private void SetDayCallGap()
    {
        dayCallGap = UnityEngine.Random.Range(dayCallGapRange[0], dayCallGapRange[1] + 1);
    }

    // 딜레이 코루틴
    public IEnumerator DelayTime(float delay, System.Action onComplete)
    {
        yield return new WaitForSeconds(delay);
        onComplete?.Invoke();
    }

    //페이드 인 코루틴
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
