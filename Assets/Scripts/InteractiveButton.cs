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
        if (obj.TryGetComponent<Image>(out var img) && this.gameObject.activeSelf)
            StartCoroutine(GameManager.Instance.FadeIn(img, 0.5f, () => { })); // 페이드인

    }

    public void SetUnTakeCallButton(Button btn)
    {
        //btn.onClick.RemoveAllListeners(); // 코드에서 여러 번 AddListener() 호출될 가능성 있을 경우 사용
        btn.onClick.AddListener(() =>
        {
            if (DiagnosisSystem.Instance != null)
                DiagnosisSystem.Instance.UnTakeCall();
        });
    }

    public void SetTakeCallButton(Button btn)
    {
        //btn.onClick.RemoveAllListeners(); // 코드에서 여러 번 AddListener() 호출될 가능성 있을 경우 사용
        btn.onClick.AddListener(() =>
        {
            if (DiagnosisSystem.Instance != null)
                DiagnosisSystem.Instance.TakeCall();
        });
    }
}
