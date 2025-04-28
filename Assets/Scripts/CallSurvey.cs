using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallSurvey : MonoBehaviour
{
    [SerializeField] private GameObject[] CheckBoxes;

    //public static Action<string> actionUpdatedSurvey; // ���� ��� ������Ʈ �� ��� ��������Ʈ
    //public static Action actionEndedSurvey; // ���� �� ��� ��������Ʈ

    // �ʱ� ���� ���� ��� DiagnosisSystem �븮�� �Լ��� ����
    public void SendCallSurvey()
    {
        if (CheckBoxes[0].activeSelf)
            EventHub.actionUpdatedSurvey?.Invoke("hesitate_speaking");
        if (CheckBoxes[1].activeSelf)
            EventHub.actionUpdatedSurvey?.Invoke("regret_after_call");
        if (CheckBoxes[2].activeSelf)
            EventHub.actionUpdatedSurvey?.Invoke("regret_after_call");
        if (CheckBoxes[3].activeSelf)
            EventHub.actionUpdatedSurvey?.Invoke("regret_after_call");

        EventHub.actionSurveyEnded?.Invoke();
    }

    // �ʱ� ���� ��� DiagnosisSystem �븮�� �Լ��� ����
    public void SendMsgSurvey()
    {
        if (CheckBoxes[0].activeSelf)
            EventHub.actionUpdatedSurvey?.Invoke("avoid_call");
        if (CheckBoxes[1].activeSelf)
            EventHub.actionUpdatedSurvey?.Invoke("avoid_call");
        if (CheckBoxes[2].activeSelf)
            EventHub.actionUpdatedSurvey?.Invoke("avoid_call");
        if (CheckBoxes[3].activeSelf)
            EventHub.actionUpdatedSurvey?.Invoke("avoid_call");

        EventHub.actionSurveyEnded?.Invoke();
    }

    public void SendDaySurvey()
    {
        if (CheckBoxes[0].activeSelf)
            EventHub.actionUpdatedSurvey?.Invoke("avoid_call");
        if (CheckBoxes[1].activeSelf)
            EventHub.actionUpdatedSurvey?.Invoke("avoid_call");
        if (CheckBoxes[2].activeSelf)
            EventHub.actionUpdatedSurvey?.Invoke("regret_after_call");
        if (CheckBoxes[3].activeSelf)
            EventHub.actionUpdatedSurvey?.Invoke("hesitate_speaking");
        if (CheckBoxes[4].activeSelf)
            EventHub.actionUpdatedSurvey?.Invoke("regret_after_call");

        EventHub.actionSurveyEnded?.Invoke();
    }

    public void SendSpecialSurvey()
    {
        if (CheckBoxes[1].activeSelf)
            EventHub.actionUpdatedSurvey?.Invoke("avoid_call");
        if (CheckBoxes[2].activeSelf)
            EventHub.actionUpdatedSurvey?.Invoke("avoid_call");
        if (CheckBoxes[3].activeSelf)
            EventHub.actionUpdatedSurvey?.Invoke("avoid_call");

        EventHub.actionSurveyEnded?.Invoke();
    }
}
