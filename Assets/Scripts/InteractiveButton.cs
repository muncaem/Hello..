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


    // ���̵� �ƿ� ���� ��ư
    public void StartFadeIn(Image fade)
    {
        fade.gameObject.SetActive(true);
        StartCoroutine(GameManager.Instance.FadeIn(fade, 1.5f, () =>
        {
            if (fade.transform.childCount > 0)
                ActiveObject(fade.transform.GetChild(0).gameObject);
        }));

    }

    // �Ҵ��� �̹��� ���� ���� ��ư
    public void ActiveObject(GameObject obj)
    {
        StartCoroutine(GameManager.Instance.FadeIn(obj.GetComponent<Image>(), 0.5f, () => { }));
        obj.SetActive(true);
    }


    // ����ũ ������ ��� ���� ��ư => ���� ���� �ν� ���� ����
    public void OnMike(Sprite sprite)
    {
        Button currButton = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        ColorBlock cb = currButton.colors;

        if (!isClicked)
        {
            // ����ũ �� Ŭ�� ��, Ȱ��ȭ �� ����
            cb.normalColor = Color.red;
            cb.highlightedColor = Color.red; // ���콺 �ö� ��
            cb.selectedColor = Color.red;    // ���õ� ������ ��
            currButton.colors = cb;

            /// ���� �ν� ������� ����� ���� ��� �߰� ����
            /// "���� �ν� ��� ��ũ��Ʈ ���� ����"
            


            isClicked = true;
        }
        else
        {
            // ����ũ ��Ȱ��ȭ
            currButton.gameObject.GetComponent<Image>().sprite = sprite;
            currButton.enabled = false;
            actionEndTalk?.Invoke();
        }
    }
}
