using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiagnosisSystem : MonoBehaviour
{
    // ��ȸ �� ��ȭ �޾Ҵ��� ���� üũ => ȸ�� ���� +1
    private bool isTakeCall = false; 
    // ��ȭ ���� ��ȸ
    private int TakeCallChance = 3;

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
    }

    // ��ȭ �ޱ� ��ư
    public void TakeCall()
    {
        SoundManager.instance.Clear();
        isTakeCall = true;
    }

    // ��ȭ ���� ��ư
    public void UnTakeCall()
    {
        SoundManager.instance.Clear();
        StopAllCoroutines();
    }

}
