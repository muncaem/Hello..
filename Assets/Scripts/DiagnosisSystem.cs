using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiagnosisSystem : MonoBehaviour
{
    // 기회 내 전화 받았는지 여부 체크 => 회피 요인 +1
    private bool isTakeCall = false; 
    // 전화 받을 기회
    private int TakeCallChance = 3;

    // 마이크 이미지 및 버튼
    [SerializeField] private GameObject MikeOnBtn;
    [SerializeField] private Image MikeOffImg;
    [SerializeField] private GameObject EmptyScreen;

    /// 유저 요인 정보 임시 할당
    private int preFactor = 0;
    private int postFactor = 0;
    /// 유저 요인 정보 임시 할당

    // 전화 받았을/거절했을 경우 스크립트 활성화 델리게이트
    public static Action<int> actionTakeCall;
    public static Action<int> actionUnTakeCall;

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

        // 3번 벨 울린 이후, 전화 받지 않은 것으로 간주
        EmptyScreen.SetActive(true);
        UnTakeCall();
    }

    // 전화 받기 버튼 눌렀을 경우, 스크립트 실행 및 마이크
    public void TakeCall()
    {
        SoundManager.instance.Clear();
        isTakeCall = true;

        actionTakeCall?.Invoke(0); // 진단용 음성 대화
    }

    // 전화 끊기 버튼 / 나중에 보기 버튼 눌렀을 경우, 스크립트 실행 및 마이크
    public void UnTakeCall()
    {
        SoundManager.instance.Clear();
        StopAllCoroutines();
        preFactor++; //사전 증세 요인 + 1

#if UNITY_EDITOR
        Debug.Log("PreFactor: " + preFactor);
#endif

        actionUnTakeCall?.Invoke(0); // 진단용 대답 체크

        // 마이크 버튼 on
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
