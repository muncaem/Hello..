using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventHub : MonoBehaviour
{
    public static EventHub Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }


    /// <summary>
    /// ��ư ����
    /// </summary>
    public static Action actionEndedFadeIn; // ��ư�� FadeOut �� ���� ��ε�ĳ��Ʈ ��.
    public static Action actionConnectedComingCall; // ������ ��ȭ [ �ޱ� ��ư ]�� ������ ��
    public static Action actionDisconnectedComingCall; // ������ ��ȭ [ ���� ��ư ]�� ������ ��


    /// <summary>
    /// ��ȭ Communication ����
    /// </summary>
    public static Action<string, bool> actionGptReceived; // gpt ���� ���� �븮�� ȣ��
    public static Action actionTTSEnded; // TTS ���� �̺�Ʈ
    public static Action<string> actionMicRecorded; // stt ��ȯ ���� �븮�� ȣ��
    /// <summary>
    /// Out Going ��ȭ ����
    /// </summary>
    public static Action<int> actionUpdatedOutGoingValue; // ������ ������Ʈ �Ǵ� ���� ��ȭ��
    public static Action actionConnectedGoingCall; // ������ ��ȭ�� AI���� ���� ���� �°� ����Ǿ��� ��
    public static Action<ScenarioData> actionStartedGoingCall; // ���� Out Going ��ȭ ����
    public static Action<ScenarioData, string> actionUpdatedScenario; // ���� ���� ���� ���� �ó������� ������Ʈ
    public static Action<int> actionUpdatedGoingCallValue; // ��ȭ ���� �� Ƚ�� ������Ʈ
    /// <summary>
    /// In Coming ��ȭ ����
    /// </summary>
    public static Action actionStartIncomingCall;
    public static Action OnTakeCall; // ���ܽý������κ��� ���� ��ȭ�� �޾��� ��� ConversationManager�� ��ȭ ����
    /// <summary>
    /// ���� ��ȭ ����
    /// </summary>
    public static Action actionEndedCallBySpeak; // ���� ��ȭ ����Ǿ��� �� ȣ�� -> ConversationManger���� TTSEnded ����
    public static Action actionEndedCallBySelect; // ��ư���� ����Ǿ��� �� ȣ�� -> DiagnosisSystem���� ���� ��ư ������ ��
    public static Action<string> actionUpdateComplaintMsg; // ���������� ��ȭ ���� ���� �ο� �޽��� ó�� update

    /// <summary>
    /// �ʱ� ���ܿ� Conversation ����
    /// </summary>
    public static Action<string> actionFirstTestUnCall; // �ʱ� ���� ��ȭ �ȹ޾��� ���
    public static Action actionFirstCallEndedCall; // �ʱ� ���� ��ȭ ���������� Ended���
    public static Action actionFirstEndedSaveScore; // �ʱ� ���� ���� UserData�� ���� �Ϸ� �� �븮�� ȣ��

    /// <summary>
    /// ���� üũ ����
    /// </summary>
    public static Action actionEndedBySilence; // ħ���� ���� ��ȭ ���� ���� -> ���� ���� üũ��
    public static Action actionEndedRealCallbySilence; // silence ���� üũ ���� ���� ��ȭ ���� ó��
    public static Action<string> actionUpdatedSpeakSituationFactor; // ��ȭ ��Ȳ �� ������� ���� ������Ʈ �븮�� ȣ��
    public static Action<string> actionUpdatedSurvey; // Survey ��� ������Ʈ ��
    public static Action actionSurveyEnded; // Survey ������ ��� ȣ��


    /// <summary>
    /// ��¥/�ð� ����
    /// </summary>
    public static Action actionUpdatedDay; // �Ϸ簡 ������ �� ȣ��
    public static Action actionReachedCallGap; // Call Gap ���� �ø��� ȣ��
    public static Action actionEndedDayTime; // �Ϸ� ġ �ð� ��� �Ҹ��԰� ���ÿ� ��ȭ�� ������ ��� ȣ��


}
