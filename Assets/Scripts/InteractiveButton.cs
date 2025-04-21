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


    // ���̵� �ƿ� ���� ��ư
    public void StartFadeIn(Image fade)
    {
        fade.gameObject.SetActive(true);
        StartCoroutine(GameManager.Instance.FadeIn(fade, 1.5f, () =>
        {
            actionEndedFadeIn?.Invoke();
        }));

    }

    // �Ҵ��� �̹��� ���� ���� ��ư
    public void ActiveObject(GameObject obj)
    {
        obj.SetActive(true); // ������Ʈ Ȱ��ȭ
        if (obj.TryGetComponent<Image>(out var img) && this.gameObject.activeSelf)
            StartCoroutine(GameManager.Instance.FadeIn(img, 0.5f, () => { })); // ���̵���

    }

    public void SetUnTakeCallButton(Button btn)
    {
        //btn.onClick.RemoveAllListeners(); // �ڵ忡�� ���� �� AddListener() ȣ��� ���ɼ� ���� ��� ���
        btn.onClick.AddListener(() =>
        {
            if (DiagnosisSystem.Instance != null)
                DiagnosisSystem.Instance.UnTakeCall();
        });
    }

    public void SetTakeCallButton(Button btn)
    {
        //btn.onClick.RemoveAllListeners(); // �ڵ忡�� ���� �� AddListener() ȣ��� ���ɼ� ���� ��� ���
        btn.onClick.AddListener(() =>
        {
            if (DiagnosisSystem.Instance != null)
                DiagnosisSystem.Instance.TakeCall();
        });
    }
}
