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
    public int day { get; private set; } // ������ ��¥
    [SerializeField] private float secondsPerDay = 300;
    [SerializeField] private float secondsPerCall = 35;

    public static Action actionUpdatedDay;
    public static Action actionUpdatedCall;

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
        if (curSceneNumb == 1)
            DayCount();
    }
    // ��¥ ī��Ʈ ���
    private void DayCount()
    {
        time += Time.deltaTime;
        if (time >= secondsPerCall)
            actionUpdatedCall?.Invoke();
        if (time >= secondsPerDay)
        {
            actionUpdatedDay?.Invoke();
            time = 0;
            day += 1;
        }
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
