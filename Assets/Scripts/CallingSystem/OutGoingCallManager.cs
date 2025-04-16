using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutGoingCallManager : MonoBehaviour
{
    [SerializeField] private PhoneScenarioMaker ScenarioMaker;

    private ScenarioData[] todayScenarios = new ScenarioData[10];
    private Dictionary<int, string> setPhoneNumber = new Dictionary<int, string>(); // ��ȭ��ȣ ����
    private int currFinishedGoingCall = 0;
    private int MaxGoingCall;

    public static Action<ScenarioData, string> actionUpdatedScenario; // ���� ���� ���� ���� �ó������� ������Ʈ
    public static Action<int> actionEndedGoingCall;
    public static Action<ScenarioData> actionStartedGoingCall;

    private void Awake()
    {
        DiagnosisSystem.actionUpdatedOutGoingValue += CreateOutGoingValue; // ���� ��� ����Ͽ� ���� ��ȭ �ó����� ����
        PhoneManager.actionConnectedGoingCall += OutGoingCall; // ���� ��ȭ �ɾ�� �ϴ� ��ȣ�� �°� ������ ��� ȣ���
        PhoneManager.actionConnectedGoingCall += EndedGoingComplaint; // ���� �۽� ��ȭ �ӹ��� �Ϸ�
    }

    /// <summary>
    /// ������ ������Ʈ �Ǵ� ���� ��ȭ��
    /// </summary>
    /// <param name="count">���� ��ȭ��</param>
    public void CreateOutGoingValue(int count)
    {
        MaxGoingCall = count;

        for (int i = 0; i < MaxGoingCall; i++)
        {
            todayScenarios[i] = ScenarioMaker.ScenarioMaker();
            setPhoneNumber[i] = $"326-{UnityEngine.Random.Range(100, 1000).ToString()}-{UnityEngine.Random.Range(100, 1000).ToString()}";

#if UNITY_EDITOR
            Debug.Log(todayScenarios[i].situation + " " + setPhoneNumber[i]);
#endif
        }

        actionEndedGoingCall?.Invoke(count);
        actionUpdatedScenario?.Invoke(todayScenarios[0], setPhoneNumber[0]);
    }

    /// <summary>
    /// �ο� �ذ� ���� ��ȭ ��� ��Ͽ� �ִ� ���� ��ȭ ��ư ������ ��� Conversation ����
    /// </summary>
    public void OutGoingCall()
    {
        // ��ȭ ���̸� return
        if (DiagnosisSystem.isCalled) return;

        actionStartedGoingCall?.Invoke(todayScenarios[currFinishedGoingCall]);
    }


    /// <summary>
    /// �۽� ��ȭ �Ϸ� ��, ȣ��
    /// </summary>
    private void EndedGoingComplaint()
    {
        currFinishedGoingCall++; // �۽� ��ȭ �Ϸ�
        actionEndedGoingCall?.Invoke(MaxGoingCall - currFinishedGoingCall); // ���� �۽� ��ȭ�� ������Ʈ
        if (currFinishedGoingCall >= MaxGoingCall) return;
        actionUpdatedScenario?.Invoke(todayScenarios[currFinishedGoingCall], setPhoneNumber[currFinishedGoingCall]); // ���� �ó����� ������Ʈ
    }
}
