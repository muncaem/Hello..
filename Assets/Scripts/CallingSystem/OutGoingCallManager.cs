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

    public static Action<string, string> actionUpdatedScenario; // 현재 진행 중인 수신 시나리오로 업데이트
    public static Action<int> actionEndedGoingCall;

    private void Awake()
    {
        DiagnosisSystem.actionUpdatedOutGoingValue += CreateOutGoingValue;
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
            setPhoneNumber[i] = $"326-{UnityEngine.Random.Range(111, 1000).ToString()}-{UnityEngine.Random.Range(111, 1000).ToString()}";

#if UNITY_EDITOR
            Debug.Log(todayScenarios[i].situation + " " + setPhoneNumber[i]);
#endif
        }

        actionEndedGoingCall?.Invoke(count);
        actionUpdatedScenario?.Invoke(todayScenarios[0].situation, setPhoneNumber[0]);
    }

    /// <summary>
    /// 민원 해결 위해 전화 대기 목록에 있는 수신 전화 버튼 눌렀을 경우 Conversation 시작 버튼 
    /// => 이거 OutGoingCallManager로 옮겨야할 것 같은데
    /// </summary>
    public void OutGoingCall()
    {
        // 전화 중이면 return
        if (DiagnosisSystem.isCalled) return;


        // 시나리오 미리 정해져있어서 그걸 GPT한테 Request따로 해야할 듯
        // 전화 걸어야 하는 횟수만큼 시나리오 메이커 돌려서
        // UI 띄우고 UI 버튼 누르면 해당 프롬프트 들고 StartConversation되게 하기
    }


    /// <summary>
    /// 수신 전화 민원 처리 완료 시, 호출 예정
    /// </summary>
    private void EndedGoingComplaint()
    {
        currFinishedGoingCall++; // 수신 전화 완료
        actionEndedGoingCall?.Invoke(MaxGoingCall - currFinishedGoingCall); // 남은 수신 전화량 업데이트
        actionUpdatedScenario?.Invoke(todayScenarios[currFinishedGoingCall].situation, setPhoneNumber[currFinishedGoingCall]); // 다음 시나리오 업데이트
    }
}
