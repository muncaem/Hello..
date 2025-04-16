using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ScenarioData
{
    public string role { get; set; }
    public string situation { get; set; }
    public string emotion { get; set; }
    public string emotionTag { get; set; }

}

public class PhoneScenarioMaker : MonoBehaviour
{
    private List<string> roles = new List<string>
    {
        "70대 남성 노인회장이야",
        "젊은 아기 엄마야",
        "남자 고등학생이야",
        "여자 고등학생이야",
        "남자 대학생이야",
        "여자 대학생이야",
        "동네 남자 자영업자야",
        "학원 여자 선생님이야",
        "대학 남성 교수야",
        "여성 직장인이야"
    };
    private List<string> situations = new List<string>
    {
        "버스 노선이 줄어든 걸 문제 삼고 있어",
        "집 앞 가로등이 고장나서 집 가는 길이 암흑이어서 무섭다는 것을 문제 삼고 있어",
        "중성화 되지 않은 길 고양이 문제로 민원을 넣었어",
        "생활하는 공간 앞의 공사 현장의 소음이 너무 큰 것을 문제 삼고 있어",
        "집 문 앞을 막고 있는 불법 주차 때문에 힘들어 하고 있어 불법 주차에 대한 해결 방안을 제시하라고 민원을 넣고 있어",
        "길가에 생활 쓰레기가 어지럽게 널려 있는 것을 문제 삼고 있어",
        "쓰레기 더미가 며칠째 안 치워져서 민원을 넣었어",
        "근처 공연장의 소음이 너무 커서 집중이 안된다는 점을 문제 삼고 있어",
        "횡단보도의 신호가 너무 짧은 것을 문제 삼고 있어",
        "버스가 버스 정류장에 서지도 않고 출발해 약속에 늦어서 피해를 본 것을 문제 삼고 있어",
        "골목에서 담배를 너무 많이 피워서 지나다니기 무섭다는 것을 문제 삼고 있어",
        "걸어다니면서 담배 피는 사람들이 너무 많아 간접흡연 피해자가 되고 있다는 것을 문제 삼고 있어",
        "우리 집 앞에 다른 집 사람들이 쓰레기를 무단 투기하는 것을 문제 삼고 있어",
        "이번 장마로 하수구에 물이 넘쳐서 악취가 나는 것을 문제 삼고 있어",
        "버스 정류장의 안내판이 훼손되어 버스가 언제오는지 알 수 없다는 것을 문제 삼고 있어",
        "도로에 생긴 포트홀로 운전하기 무서워 민원을 넣고 있어",
        "스쿨존 내 과속 차량이 너무 많고 단속이 미흡하다는 것을 문제 삼고 있어",
        "공원에 벤치가 파손되어 쉴 곳이 없다는 것을 문제 삼고 있어",
        "공원 내 화장실들이 관리가 안돼서 악취가 나는 것을 문제 삼고 있어",
        "공원을 산책하는데 공원에서 불법으로 음주하고 취한 사람들이 많아 불쾌해서 민원을 넣고 있어",
        "허리가 아파서 보건소를 갔는데 대기 시간이 너무 긴 것을 문제 삼고 있어",
        "예고 없이 동네 전체에 단수가 되어서 씻지 못한 것에 피해를 봐서 민원을 넣고 있어",
        "가로등 주위에 전선이 노출된 것을 문제 삼고 있어"
    };
    //private List<string> emotions = new List<string> 
    //{ 
    //    "매우 분노한",
    //    "속상하고 지찬",
    //    "불만이 큰",
    //    "혼란스러운",
    //    "답답한",
    //    "불만이 있지만 차분한"
    //};
    //private List<string> emotionTags = new List<string>
    //{
    //    "[화남]",
    //    "[슬픔]",
    //    "[중립]"
    //};

    // 감정 설명과 태그를 1:1로 매핑
    private Dictionary<string, string> emotionMap = new Dictionary<string, string>
    {
        { "매우 분노한", "[화남]" },
        { "속상하고 지친", "[슬픔]" },
        { "불만이 있지만 차분한", "[중립]" },
        { "혼란스럽고 답답한", "[화남]" },
        { "혼란스러운", "[화남]" },
        { "답답한", "슬픔" },
        { "중립적인", "[중립]" }
    };




    //프롬프트 전달용 시나리오
    public /*string*/ScenarioData ScenarioMaker()
    {
        //string role = roles[Random.Range(0, roles.Count)];
        //string situation = situations[Random.Range(0, situations.Count)];

        List<string> emotionKeys = new List<string>(emotionMap.Keys);
        string emotion = emotionKeys[Random.Range(0, emotionKeys.Count)];
        //string emotionTag = emotionMap[emotion];

        //string scenario = $"너는 {role}. {situation}. {emotion} 상태야. 항상 대답 맨 앞에 {emotionTag}태그를 붙여서 감정을 표시해.";
        //Debug.Log("생성된 시나리오: " + scenario);

        return new ScenarioData
        {
            role = roles[Random.Range(0, roles.Count)],
            situation = situations[Random.Range(0, situations.Count)],
            emotion = emotion,
            emotionTag = emotionMap[emotion]
        };
        //return scenario;
    }
}
