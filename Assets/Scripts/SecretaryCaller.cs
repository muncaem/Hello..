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

    // 대화 기록
    private int maxLogCount = 20;
    private List<string> conversationLog = new List<string>(20);

    /// <summary>
    /// 유저 발화 추가
    /// </summary>
    /// <param name="speak"></param>
    private void AddUserSpeaking(string speak)
    {
        if (conversationLog.Count >= maxLogCount)
            conversationLog.RemoveAt(0);

        conversationLog.Add($"[유저] {speak}");
    }

    /// <summary>
    /// AI 발화 추가
    /// </summary>
    /// <param name="speak"></param>
    private void AddAISpeaking(string speak)
    {
        if (conversationLog.Count >= maxLogCount)
            conversationLog.RemoveAt(0);

        conversationLog.Add($"[AI 주민] {speak}");
    }

    /// <summary>
    /// GTP 전달용 로그 프롬프트 포맷 생성
    /// </summary>
    /// <returns></returns>
    private string GetFormatConversationForAssist()
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("아래는 플레이어와 AI 주민의 통화 기록입니다.");
        sb.AppendLine("플레이어의 커뮤니케이션 능력을 분석하고, 강점과 개선점을 알려주세요.");
        sb.AppendLine();
        sb.AppendLine("<대화 기록>");

        foreach (var line in conversationLog)
        {
            sb.AppendLine(line);
        }

        sb.AppendLine("</대화 기록>");
        sb.AppendLine();
        sb.AppendLine("이 대화 기록을 기반으로 플레이어의 커뮤니케이션 능력을 평가해주세요.");


        return sb.ToString();
    }


    /// <summary>
    /// AI비서에게 전화 -> GPT 연결
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
    /// AI 비서 응답 TTS 실행
    /// </summary>
    /// <param name="comment"></param>
    private void PlayTTS(string comment)
    {
        TTS.Speak(comment, "ko-KR-Neural2-C");
    }
}
