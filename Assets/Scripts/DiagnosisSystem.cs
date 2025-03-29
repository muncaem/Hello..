using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiagnosisSystem : MonoBehaviour
{
    // 기회 내 전화 받았는지 여부 체크 => 회피 요인 +1
    private bool isTakeCall = false; 
    // 전화 받을 기회
    private int TakeCallChance = 3;

    // Start is called before the first frame update
    void Start()
    {
        //초기 전화 받기/거절 테스트
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

    // 전화 받기 버튼
    public void TakeCall()
    {
        SoundManager.instance.Clear();
        isTakeCall = true;
    }

    // 전화 끊기 버튼
    public void UnTakeCall()
    {
        SoundManager.instance.Clear();
        StopAllCoroutines();
    }

}
