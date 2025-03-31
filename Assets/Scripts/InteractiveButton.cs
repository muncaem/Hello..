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

    public static Action actionEndTalk;

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
            if (fade.transform.childCount > 0)
                ActiveObject(fade.transform.GetChild(0).gameObject);
        }));

    }

    // 할당한 이미지 생성 실행 버튼
    public void ActiveObject(GameObject obj)
    {
        StartCoroutine(GameManager.Instance.FadeIn(obj.GetComponent<Image>(), 0.5f, () => { }));
        obj.SetActive(true);
    }


    // 마이크 눌렀을 경우 실행 버튼 => 추후 음성 인식 도입 예정
    public void OnMike(Sprite sprite)
    {
        Button currButton = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        ColorBlock cb = currButton.colors;

        if (!isClicked)
        {
            // 마이크 색 클릭 시, 활성화 색 변경
            cb.normalColor = Color.red;
            cb.highlightedColor = Color.red; // 마우스 올라갈 때
            cb.selectedColor = Color.red;    // 선택된 상태일 때
            currButton.colors = cb;

            /// 음성 인식 기능으로 사용자 응답 기록 추가 예정
            /// "음성 인식 기능 스크립트 따로 생성"
            


            isClicked = true;
        }
        else
        {
            // 마이크 비활성화
            currButton.gameObject.GetComponent<Image>().sprite = sprite;
            currButton.enabled = false;
            actionEndTalk?.Invoke();
        }
    }
}
