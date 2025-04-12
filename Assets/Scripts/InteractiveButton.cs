using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InteractiveButton : MonoBehaviour
{
    private ColorBlock colorBlock;
    private ColorBlock cbRed;
    private ColorBlock cbWhite;

    private bool isClicked = false;

    public static Action actionEndedFadeIn;

    private void Awake()
    {
        colorBlock.normalColor = Color.red;
        cbRed = colorBlock;
        colorBlock.normalColor = Color.white;
        cbWhite = colorBlock;
    }


    // 페이드 아웃 실행 버튼
    public void StartFadeIn(Image fade)
    {
        fade.gameObject.SetActive(true);
        StartCoroutine(GameManager.Instance.FadeIn(fade, 1.5f, () =>
        {
            actionEndedFadeIn?.Invoke();
        }));

    }

    // 할당한 이미지 생성 실행 버튼
    public void ActiveObject(GameObject obj)
    {
        obj.SetActive(true); // 오브젝트 활성화
        if (obj.TryGetComponent<Image>(out var img))
            StartCoroutine(GameManager.Instance.FadeIn(img, 0.5f, () => { })); // 페이드인

    }
}
