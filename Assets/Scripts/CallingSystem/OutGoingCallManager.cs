using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutGoingCallManager : MonoBehaviour
{
    [SerializeField] private PhoneScenarioMaker ScenarioMaker;

    private ScenarioData[] todayScenarios = new ScenarioData[10];
    private Dictionary<int, string> setPhoneNumber = new Dictionary<int, string>(); // 전화번호 생성
    private int currFinishedGoingCall = 0;
    private int MaxGoingCall;

    public static Action<ScenarioData, string> actionUpdatedScenario; // 현재 진행 중인 수신 시나리오로 업데이트
    public static Action<int> actionEndedGoingCall;
    public static Action<ScenarioData> actionStartedGoingCall;

    private void Awake()
    {
        DiagnosisSystem.actionUpdatedOutGoingValue += CreateOutGoingValue; // 진단 결과 기반하여 수신 전화 시나리오 구성
        PhoneManager.actionConnectedGoingCall += OutGoingCall; // 현재 전화 걸어야 하는 번호로 맞게 눌렀을 경우 호출됨
        PhoneManager.actionConnectedGoingCall += EndedGoingComplaint; // 현재 송신 전화 임무는 완료
    }

    /// <summary>
    /// 날마다 업데이트 되는 수신 통화량
    /// </summary>
    /// <param name="count">수신 통화량</param>
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
    /// 민원 해결 위해 전화 대기 목록에 있는 수신 전화 버튼 눌렀을 경우 Conversation 시작
    /// </summary>
    public void OutGoingCall()
    {
        // 전화 중이면 return
        if (DiagnosisSystem.isCalled) return;

        actionStartedGoingCall?.Invoke(todayScenarios[currFinishedGoingCall]);
    }


    /// <summary>
    /// 송신 전화 완료 시, 호출
    /// </summary>
    private void EndedGoingComplaint()
    {
        currFinishedGoingCall++; // 송신 전화 완료
        actionEndedGoingCall?.Invoke(MaxGoingCall - currFinishedGoingCall); // 남은 송신 전화량 업데이트
        if (currFinishedGoingCall >= MaxGoingCall) return;
        actionUpdatedScenario?.Invoke(todayScenarios[currFinishedGoingCall], setPhoneNumber[currFinishedGoingCall]); // 다음 시나리오 업데이트
    }
}
