using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private int curSceneNumb;

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
        InteractiveButton.actionEndTalk += MoveScene;
    }

    // �� �̵�
    private void MoveScene()
    {
        if (curSceneNumb > 0) return;
        StartCoroutine(DelayTime(1f, () => SceneManager.LoadScene(++curSceneNumb)));
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
