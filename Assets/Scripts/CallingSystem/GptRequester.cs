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
    public static Action<string, bool> actionGptReceived; // gpt 응답 이후 대리자 호출

    /// Flask에서 실행 중인 서버의 주소
    private string apiUrl = "http://127.0.0.1:5000/generate";


    // AI 전화 시작 함수
    /// 외부에서 호출할 함수 (프롬프트를 받아 코루틴 실행)
    /// <param name="prompt">AI에게 보낼 시나리오 또는 질문 내용</param>
    public void RequestGPT(string prompt)
    {
        // 코루틴으로 GPT에 요청 시작
        StartCoroutine(SendPrompt(prompt));
    }
    /// 실제로 GPT 서버에 POST 요청을 보내는 함수
    /// <param name="prompt">GPT에게 전달할 메시지</param>
    IEnumerator SendPrompt(string prompt)
    {
        // JSON 형식으로 prompt 감싸기
        string json = "{\"prompt\":\"" + prompt + "\"}";

        // 문자열을 byte 배열로 변환
        byte[] postData = System.Text.Encoding.UTF8.GetBytes(json);

        // POST 방식으로 UnityWebRequest 생성
        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        request.uploadHandler = new UploadHandlerRaw(postData);        // 전송할 데이터 설정
        request.downloadHandler = new DownloadHandlerBuffer();         // 응답 받을 핸들러 설정
        request.SetRequestHeader("Content-Type", "application/json");  // 요청 헤더 설정 (JSON)

        // 요청을 서버에 보내고 응답 대기
        yield return request.SendWebRequest();

        // 결과 처리
        if (request.result == UnityWebRequest.Result.Success)
        {
            // 요청 성공: GPT 응답
            string comment = request.downloadHandler.text;

            // 종료 키워드 체크 및 매니저에게 델리게이트
            actionGptReceived?.Invoke(comment, AITalkEndCheck(comment));

            jsonResponse res = JsonUtility.FromJson<jsonResponse>(comment);
            Debug.Log("GPT 응답 받음: " + res.reply);
        }
        else
        {
            // 요청 실패: 에러 메시지 출력
            Debug.LogError("GPT 요청 실패: " + request.error);
        }
    }

    [System.Serializable]
    public class jsonResponse
    {
        public string reply;
    }


    // AI 대화 종료 포인트 체크
    private bool AITalkEndCheck(string reply)
    {
        // 전화 종료 키워드 체크
        if (reply.Contains("감사합니다") || reply.Contains("알겠습니다") || reply.Contains("그러세요"))
        {
            return true;
        }
        else return false;
    }
}
