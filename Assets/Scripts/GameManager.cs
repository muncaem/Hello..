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
    private float gapTime = 0;
    private bool canPlayTime = false;
    private bool canUpdateDay = false;

    public int day { get; private set; } // 누적된 날짜
    [SerializeField] private float secondsPerDay = 180; // 하루에 주어진 시간
    [SerializeField] private int maxDay = 3; // 인게임 최종 날짜
    private float dayCallGap;
    private float[] dayCallGapRange = { 25, 35 }; // 하루에 전화 오는 간격

    public static Action actionChangedScene; // Scene Change Follow

    /// <summary>
    /// 테스트용 - Skip Day Button 활성화 관련
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

    // 씬 이동
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
        // 씬이 실제로 로드 되었을 때 호출됨
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
        /// 테스트용 - Skip Day Button 활성화 관련
        TestAccess();
#endif
    }

    // 날짜 카운트 기능
    private void DayCount()
    {
        time += Time.deltaTime;

        // 전화 중이 아니면 gapTime 쌓기
        if (!ConversationManager.GlobalCallState)
        {
            gapTime += Time.deltaTime;
        }

        // 전화 간격 도달 시
        if (gapTime >= dayCallGap)
        {
            Debug.Log("gapTime >= dayCallGap");
            EventHub.actionReachedCallGap?.Invoke();
            SetDayCallGap();
            gapTime = 0f;
        }

        // 하루 종료 조건
        if (time >= secondsPerDay && ConversationManager.GlobalCallState == false)
        /*|| ConversationManager. // 하루 치 통화 할당량 다 채웠을 경우*/
        {
            Debug.Log("time >= secondsPerDay && ConversationManager.GlobalCallState == false");
            // 주어진 하루 시간을 모두 사용하거나 할당량 채웠을 경우 마무리 설문 진행
            EventHub.actionEndedDayTime?.Invoke();
            canPlayTime = false;
        }
    }

    // CallSurvey에서 하루 마무리 survey 완료하면 호출 => 실질적으로 다음 날로 넘어가게 함.
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

    /// <summary>
    /// 개발용 DaySkip Tester
    /// </summary>
    public void SkipDay()
    {
        canPlayTime = false;
        UpdateDayTimer();
        //print($"오늘 날짜 {day}");
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
