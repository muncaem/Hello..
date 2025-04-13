using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallSurvey : MonoBehaviour
{
    [SerializeField] private GameObject[] CheckBoxes;

    public static Action<string> actionUpdatedSurvey; // ���� ��� ������Ʈ �� ��� ��������Ʈ
    public static Action actionEndedSurvey; // ���� �� ��� ��������Ʈ

    // �ʱ� ���� ���� ��� DiagnosisSystem �븮�� �Լ��� ����
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

    // �ʱ� ���� ��� DiagnosisSystem �븮�� �Լ��� ����
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

    public void SendDaySurvey()
    {
        if (CheckBoxes[0].activeSelf)
            actionUpdatedSurvey?.Invoke("avoid_call");
        if (CheckBoxes[1].activeSelf)
            actionUpdatedSurvey?.Invoke("avoid_call");
        if (CheckBoxes[2].activeSelf)
            actionUpdatedSurvey?.Invoke("regret_after_call");
        if (CheckBoxes[3].activeSelf)
            actionUpdatedSurvey?.Invoke("hesitate_speaking");
        if (CheckBoxes[4].activeSelf)
            actionUpdatedSurvey?.Invoke("regret_after_call");

        actionEndedSurvey?.Invoke();
    }

    public void SendSpecialSurvey()
    {
        if (CheckBoxes[1].activeSelf)
            actionUpdatedSurvey?.Invoke("avoid_call");
        if (CheckBoxes[2].activeSelf)
            actionUpdatedSurvey?.Invoke("avoid_call");
        if (CheckBoxes[3].activeSelf)
            actionUpdatedSurvey?.Invoke("avoid_call");
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
