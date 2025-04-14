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

    public static Action<string, string> actionUpdatedScenario; // ���� ���� ���� ���� �ó������� ������Ʈ
    public static Action<int> actionEndedGoingCall;

    private void Awake()
    {
        DiagnosisSystem.actionUpdatedOutGoingValue += CreateOutGoingValue;
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
            setPhoneNumber[i] = $"326-{UnityEngine.Random.Range(111, 1000).ToString()}-{UnityEngine.Random.Range(111, 1000).ToString()}";

#if UNITY_EDITOR
            Debug.Log(todayScenarios[i].situation + " " + setPhoneNumber[i]);
#endif
        }

        actionEndedGoingCall?.Invoke(count);
        actionUpdatedScenario?.Invoke(todayScenarios[0].situation, setPhoneNumber[0]);
    }

    /// <summary>
    /// �ο� �ذ� ���� ��ȭ ��� ��Ͽ� �ִ� ���� ��ȭ ��ư ������ ��� Conversation ���� ��ư 
    /// => �̰� OutGoingCallManager�� �Űܾ��� �� ������
    /// </summary>
    public void OutGoingCall()
    {
        // ��ȭ ���̸� return
        if (DiagnosisSystem.isCalled) return;


        // �ó����� �̸� �������־ �װ� GPT���� Request���� �ؾ��� ��
        // ��ȭ �ɾ�� �ϴ� Ƚ����ŭ �ó����� ����Ŀ ������
        // UI ���� UI ��ư ������ �ش� ������Ʈ ��� StartConversation�ǰ� �ϱ�
    }


    /// <summary>
    /// ���� ��ȭ �ο� ó�� �Ϸ� ��, ȣ�� ����
    /// </summary>
    private void EndedGoingComplaint()
    {
        currFinishedGoingCall++; // ���� ��ȭ �Ϸ�
        actionEndedGoingCall?.Invoke(MaxGoingCall - currFinishedGoingCall); // ���� ���� ��ȭ�� ������Ʈ
        actionUpdatedScenario?.Invoke(todayScenarios[currFinishedGoingCall].situation, setPhoneNumber[currFinishedGoingCall]); // ���� �ó����� ������Ʈ
    }
}
