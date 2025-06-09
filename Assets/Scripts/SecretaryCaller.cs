using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor.VersionControl;
using UnityEngine;

public class SecretaryCaller : MonoBehaviour
{
    private void OnEnable()
    {
        EventHub.actionUpdatedUserSpeaking += AddUserSpeaking;
        EventHub.actionUpdatedAISpeaking += AddAISpeaking;
        EventHub.actionGetSecretaryReply += PlayTTS;
    }

    private void OnDisable()
    {
        EventHub.actionUpdatedUserSpeaking -= AddUserSpeaking;
        EventHub.actionUpdatedAISpeaking -= AddAISpeaking;
        EventHub.actionGetSecretaryReply -= PlayTTS;
    }

    [SerializeField] private GptRequester GPT;
    [SerializeField] private TTSChanger TTS;

    // ��ȭ ���
    private int maxLogCount = 20;
    private List<string> conversationLog = new List<string>(20);

    /// <summary>
    /// ���� ��ȭ �߰�
    /// </summary>
    /// <param name="speak"></param>
    private void AddUserSpeaking(string speak)
    {
        if (conversationLog.Count >= maxLogCount)
            conversationLog.RemoveAt(0);

        conversationLog.Add($"[����] {speak}");
    }

    /// <summary>
    /// AI ��ȭ �߰�
    /// </summary>
    /// <param name="speak"></param>
    private void AddAISpeaking(string speak)
    {
        if (conversationLog.Count >= maxLogCount)
            conversationLog.RemoveAt(0);

        conversationLog.Add($"[AI �ֹ�] {speak}");
    }

    /// <summary>
    /// GTP ���޿� �α� ������Ʈ ���� ����
    /// </summary>
    /// <returns></returns>
    private string GetFormatConversationForAssist()
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("�Ʒ��� �÷��̾�� AI �ֹ��� ��ȭ ����Դϴ�.");
        sb.AppendLine("�÷��̾��� Ŀ�´����̼� �ɷ��� �м��ϰ�, ������ �������� �˷��ּ���.");
        sb.AppendLine();
        sb.AppendLine("<��ȭ ���>");

        foreach (var line in conversationLog)
        {
            sb.AppendLine(line);
        }

        sb.AppendLine("</��ȭ ���>");
        sb.AppendLine();
        sb.AppendLine("�� ��ȭ ����� ������� �÷��̾��� Ŀ�´����̼� �ɷ��� �����ּ���.");


        return sb.ToString();
    }


    /// <summary>
    /// AI�񼭿��� ��ȭ -> GPT ����
    /// </summary>
    public void CallToSecretary()
    {
#if UNITY_EDITOR
        print($"CallToSecretary");
#endif
        if (ConversationManager.GlobalCallState == false)
            GPT.RequestGPT(GetFormatConversationForAssist(), GptRequester.GPTRequestType.Secretary);
    }

    /// <summary>
    /// AI �� ���� TTS ����
    /// </summary>
    /// <param name="comment"></param>
    private void PlayTTS(string comment)
    {
        TTS.Speak(comment, "ko-KR-Neural2-C");
    }
}
