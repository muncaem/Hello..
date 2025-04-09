using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScriptsController : MonoBehaviour
{
    private Text textUI;
    [SerializeField] private string[] dialogs;

    private void Awake()
    {
        textUI = transform.GetChild(0).GetComponent<Text>();
    }

    void Start()
    {
        //DiagnosisSystem.actionTakeCall += NormalQuestionVoice;
        //DiagnosisSystem.actionUnTakeCall += NormalQuestionText;
    }


    // 처음 시작 후
    // 전화 응답 시, 일반 질문 -> 기본적인 정보 수집
    void NormalQuestionVoice(int idx)
    {
        /// 질문에 대답하는 동안 심리체크 로직 추가 예정
        /// 해당 함수는 질문만 함.
        /// 질문하고 마이크 자동으로 활성화 돼서 대답 인식 => 마이크 추가할까
        /// "음성 인식 기능 스크립트 따로 생성"
        /// !대답 끝나는 포인트 체크 필요! 이후 씬 이동 인터랙티브버튼 onmike함수의 actionEndTalk?.Invoke()처럼
    }
    // 전화 미응답 시, 질문
    public void NormalQuestionText(int idx)
    {
        StartCoroutine(GameManager.Instance.DelayTime(1f,
            () =>
            {
                textUI.gameObject.SetActive(true);
                textUI.text = dialogs[idx];
            }
            ));
    }


    //// 종료 시, 델리게이트 해제
    //private void OnDestroy()
    //{
    //    DiagnosisSystem.actionTakeCall -= NormalQuestionVoice;
    //    DiagnosisSystem.actionUnTakeCall -= NormalQuestionText;
    //}
}
