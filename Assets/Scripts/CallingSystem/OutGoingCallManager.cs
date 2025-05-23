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

    //public static Action<ScenarioData, string> actionUpdatedScenario; // 현재 진행 중인 수신 시나리오로 업데이트
    //public static Action<int> actionEndedGoingCall;
    //public static Action<ScenarioData> actionStartedGoingCall;

    private void Awake()
    {
        EventHub.actionUpdatedOutGoingValue += CreateOutGoingValue; // 진단 결과 기반하여 수신 전화 시나리오 구성

        EventHub.actionConnectedGoingCall += OutGoingCall; // 현재 전화 걸어야 하는 번호로 맞게 눌렀을 경우 호출됨
        EventHub.actionConnectedGoingCall += EndedGoingComplaint; // 현재 송신 전화 임무는 완료
        EventHub.actionEndedDayTime += EndDayAndSendleftCallValue; // 날짜 바뀌기 직전 남은 송신량 평판에 반영
    }

    /// <summary>
    /// 날마다 업데이트 되는 out going call 통화량
    /// </summary>
    /// <param name="count">out going call 통화량</param>
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

        EventHub.actionUpdatedGoingCallValue?.Invoke(count);
        EventHub.actionUpdatedScenario?.Invoke(todayScenarios[0], setPhoneNumber[0]);
    }

    /// <summary>
    /// 민원 해결 위해 전화 대기 목록에 있는 수신 전화 버튼 눌렀을 경우 Conversation 시작
    /// </summary>
    public void OutGoingCall()
    {
        // 전화 중이면 return
        if (DiagnosisSystem.isCalled) return;

        EventHub.actionStartedGoingCall?.Invoke(todayScenarios[currFinishedGoingCall]);
    }


    /// <summary>
    /// 송신 전화 완료 시, 호출
    /// </summary>
    private void EndedGoingComplaint()
    {
        currFinishedGoingCall++; // 송신 전화 완료
        EventHub.actionUpdatedGoingCallValue?.Invoke(MaxGoingCall - currFinishedGoingCall); // 남은 송신 전화량 업데이트
        if (currFinishedGoingCall >= MaxGoingCall) return;
        EventHub.actionUpdatedScenario?.Invoke(todayScenarios[currFinishedGoingCall], setPhoneNumber[currFinishedGoingCall]); // 다음 시나리오 업데이트
    }


    /// <summary>
    /// 하루가 끝날 때 남은 out going call value 에 따른 평판 업데이트
    /// </summary>
    private void EndDayAndSendleftCallValue()
    {
        UserData.Instance.userReputation -= 16 * (MaxGoingCall - currFinishedGoingCall);
        EventHub.actionUpdateReputation?.Invoke();
    }
}
