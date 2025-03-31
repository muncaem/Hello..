using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiagnosisSystem : MonoBehaviour
{
    // ��ȸ �� ��ȭ �޾Ҵ��� ���� üũ => ȸ�� ���� +1
    private bool isTakeCall = false; 
    // ��ȭ ���� ��ȸ
    private int TakeCallChance = 3;

    // ����ũ �̹��� �� ��ư
    [SerializeField] private GameObject MikeOnBtn;
    [SerializeField] private Image MikeOffImg;
    [SerializeField] private GameObject EmptyScreen;

    /// ���� ���� ���� �ӽ� �Ҵ�
    private int preFactor = 0;
    private int postFactor = 0;
    /// ���� ���� ���� �ӽ� �Ҵ�

    // ��ȭ �޾���/�������� ��� ��ũ��Ʈ Ȱ��ȭ ��������Ʈ
    public static Action<int> actionTakeCall;
    public static Action<int> actionUnTakeCall;

    // Start is called before the first frame update
    void Start()
    {
        //�ʱ� ��ȭ �ޱ�/���� �׽�Ʈ
        StartCoroutine(InitCheck());
    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator InitCheck()
    {
        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < TakeCallChance; i++)
        {
            if (!isTakeCall)
            {
                SoundManager.instance.Play("bell");

                yield return new WaitForSeconds(4);
            }
        }
        yield return null;

        // 3�� �� �︰ ����, ��ȭ ���� ���� ������ ����
        EmptyScreen.SetActive(true);
        UnTakeCall();
    }

    // ��ȭ �ޱ� ��ư ������ ���, ��ũ��Ʈ ���� �� ����ũ
    public void TakeCall()
    {
        SoundManager.instance.Clear();
        isTakeCall = true;

        actionTakeCall?.Invoke(0); // ���ܿ� ���� ��ȭ
    }

    // ��ȭ ���� ��ư / ���߿� ���� ��ư ������ ���, ��ũ��Ʈ ���� �� ����ũ
    public void UnTakeCall()
    {
        SoundManager.instance.Clear();
        StopAllCoroutines();
        preFactor++; //���� ���� ���� + 1

#if UNITY_EDITOR
        Debug.Log("PreFactor: " + preFactor);
#endif

        actionUnTakeCall?.Invoke(0); // ���ܿ� ��� üũ

        // ����ũ ��ư on
        StartCoroutine(GameManager.Instance.FadeIn(MikeOffImg, 0.5f, () => { }));
        StartCoroutine(GameManager.Instance.DelayTime(2f,
            () =>
            {
                MikeOffImg.gameObject.SetActive(false);
                MikeOnBtn.SetActive(true);
            }
            ));
    }

}
