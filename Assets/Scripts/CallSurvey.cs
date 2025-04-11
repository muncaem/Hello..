using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallSurvey : MonoBehaviour
{
    [SerializeField] private GameObject[] CheckBoxes;

    public static Action<string> actionUpdatedSurvey; // 조사 결과 업데이트 시 사용 델리게이트
    public static Action actionEndedSurvey; // 조사 끝 사용 델리게이트

    // 조사 결과 DiagnosisSystem 대리자 함수로 전달
    public void SendCallSurvey()
    {
        if (CheckBoxes[0].activeSelf)
            actionUpdatedSurvey?.Invoke("hesitate_speaking");
        if (CheckBoxes[1].activeSelf)
            actionUpdatedSurvey?.Invoke("regret_after_call");
        if (CheckBoxes[2].activeSelf)
            actionUpdatedSurvey?.Invoke("regret_after_call");
        if (CheckBoxes[3].activeSelf)
            actionUpdatedSurvey?.Invoke("regret_after_call");

        actionEndedSurvey?.Invoke();
    }

    // 조사 결과 DiagnosisSystem 대리자 함수로 전달
    public void SendMsgSurvey()
    {
        if (CheckBoxes[0].activeSelf)
            actionUpdatedSurvey?.Invoke("avoid_call");
        if (CheckBoxes[1].activeSelf)
            actionUpdatedSurvey?.Invoke("avoid_call");
        if (CheckBoxes[2].activeSelf)
            actionUpdatedSurvey?.Invoke("avoid_call");
        if (CheckBoxes[3].activeSelf)
            actionUpdatedSurvey?.Invoke("avoid_call");

        actionEndedSurvey?.Invoke();
    }


    //case "avoid_call":
    //   preFactor++;
    //   break;
    //case "hesitate_speaking":
    //   midFactor++;
    //   break;
    //case "regret_after_call":
    //   postFactor++;
    //   break;
}
