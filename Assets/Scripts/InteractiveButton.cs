using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InteractiveButton : MonoBehaviour
{
    // 페이드 아웃 실행 버튼
    public void StartFadeIn(Image fade)
    {
        fade.gameObject.SetActive(true);
        StartCoroutine(FadeIn(fade, 1.5f, () =>
        {
            if (fade.transform.childCount > 0)
                ActiveObject(fade.transform.GetChild(0).gameObject);
        }));

    }

    // 이미지 생성 실행 버튼
    void ActiveObject(GameObject obj)
    {
        StartCoroutine(FadeIn(obj.GetComponent<Image>(), 0.5f, () => { }));
        obj.SetActive(true);
    }



    //페이드 인 로직 구현
    IEnumerator FadeIn(Image fadeImg, float duration, System.Action onComplete)
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
