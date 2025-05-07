using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using static MicRecorder;

// Flask의 GPT 서버에 프롬프트를 보내고 응답을 받아오는 클래스
public class GptRequester : MonoBehaviour
{
    //public static Action<string, bool> actionGptReceived; // gpt 응답 이후 대리자 호출
    private string userId = "user_001";  // 유저 고유 ID (추후 로그인 시스템과 연동 예정)

    public enum GPTRequestType
    {
        Resident,
        Secretary
    }

    /// Flask에서 실행 중인 서버의 주소
    private string residentApiUrl = "http://127.0.0.1:5000/generate_resident";
    private string secretaryApiUrl = "http://127.0.0.1:5000/generate_secretary";

    // 전송 요청 클래스
    [Serializable]
    public class GPTRequest
    {
        public string user_id;
        public string prompt;
    }
    // 응답 클래스
    [System.Serializable]
    public class GPTResponse
    {
        public string reply; //json 파싱용
    }

    // AI 전화 시작 함수
    /// 외부에서 호출할 함수 (프롬프트를 받아 코루틴 실행)
    /// <param name="prompt">AI에게 보낼 시나리오 또는 질문 내용</param>
    public void RequestGPT(string prompt, GPTRequestType type = GPTRequestType.Resident)
    {
        if (string.IsNullOrWhiteSpace(prompt))
        {
            Debug.LogWarning("GPT 요청 차단: 빈 prompt");
            // 마이크 인식 오류 시, UI 띄우고 MicRecorder 재녹음되도록 유도 예정
            return;
        }

        // 코루틴으로 GPT에 요청 시작
        string apiUrl = type == GPTRequestType.Resident ? residentApiUrl : secretaryApiUrl;
        StartCoroutine(SendPrompt(prompt, apiUrl));
    }
    /// 실제로 GPT 서버에 POST 요청을 보내는 함수
    /// <param name="prompt">GPT에게 전달할 메시지</param>
    IEnumerator SendPrompt(string prompt, string apiUrl)
    {
        // JSON 형식으로 prompt 감싸기
        //string json = "{\"prompt\":\"" + prompt + "\"}";
        // JSON 포맷: user_id와 prompt
        string json = JsonUtility.ToJson(new GPTRequest
        {
            user_id = userId,
            prompt = prompt
        });

        // 문자열을 byte 배열로 변환
        byte[] postData = System.Text.Encoding.UTF8.GetBytes(json);

        // POST 방식으로 UnityWebRequest 생성
        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        request.method = UnityWebRequest.kHttpVerbPOST;
        request.uploadHandler = new UploadHandlerRaw(postData);        // 전송할 데이터 설정
        request.downloadHandler = new DownloadHandlerBuffer();         // 응답 받을 핸들러 설정
        request.SetRequestHeader("Content-Type", "application/json");  // 요청 헤더 설정 (JSON)

        Debug.Log("보낼 JSON: " + json);

        // 요청을 서버에 보내고 응답 대기
        yield return request.SendWebRequest();

        // 결과 처리
        if (request.result == UnityWebRequest.Result.Success)
        {
            // 요청 성공: GPT 응답
            string rawJson = request.downloadHandler.text;
            GPTResponse res = JsonUtility.FromJson<GPTResponse>(rawJson); // json 변환

            string comment = res.reply; // 변환한 응답

            if (comment.StartsWith("[비서]"))
            {
                EventHub.actionGetSecretaryReply?.Invoke(comment);
                Debug.Log("<color=orange>비서 GPT 응답</color> 받음: " + comment);
            }
            else
            {
                // 종료 키워드 체크 및 매니저에게 델리게이트
                EventHub.actionGptReceived?.Invoke(comment, AITalkEndCheck(comment));

                Debug.Log("<color=blue>주민 GPT 응답</color> 받음: " + comment);

                // AI 발화 Log 업데이트
                EventHub.actionUpdatedAISpeaking?.Invoke(comment);
            }
        }
        else
        {
            // 요청 실패: 에러 메시지 출력
            Debug.LogError("GPT 요청 실패: " + request.error);
        }
    }



    // AI 대화 종료 포인트 체크
    private bool AITalkEndCheck(string reply)
    {
        // 전화 종료 키워드 체크
        if (reply.Contains("감사합니다") || reply.Contains("끊겠") || reply.Contains("그러세요") || reply.Contains("그럼 이만") || reply.Contains("끊을"))
        {
            return true;
        }
        else return false;
    }
}
