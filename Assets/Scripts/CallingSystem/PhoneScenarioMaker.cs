using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneScenarioMaker : MonoBehaviour
{
    // 프롬프트 전달용 시나리오 예시
    //List<string> scenarios = new List<string>
    //{
    //      "너는 70대 남성 노인회장이야. 버스 노선이 줄어든 걸 문제 삼고 있어. 매우 분노한 상태야. 항상 대답 맨 앞에 [화남] 태그를 붙여서 감정을 표시해.",
    //      "너는 젊은 싱글맘이야. 아파트 엘리베이터 고장 문제로 불편함을 겪고 있어. 속상하고 지친 상태야. 항상 대답 맨 앞에 [슬픔] 태그를 붙여서 감정을 표시해.",
    //      "너는 고양이를 키우는 30대 여성이야. 동네 길고양이 문제로 민원을 넣었어. 불만이 있지만 차분하게 말하는 성격이야. 항상 대답 맨 앞에 [중립] 태그를 붙여."
    //};
    // 버튼 클릭이든 하루가 시작되면 바로 나오든지 해서 랜덤으로 시나리오 선택하도록한다.
    //string selected = scenarios[Random.Range(0, scenarios.Count)];
    //gptRequester.RequestGPT(selected);

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string ScenarioMaker()
    {
        return "너는 70대 남성 노인회장이야. 버스 노선이 줄어든 걸 문제 삼고 있어. 매우 분노한 상태야. 항상 대답 맨 앞에 [화남] 태그를 붙여서 감정을 표시해.";
        ///ex: 마을에 쓰레기 너무 많아서 힘들다~
        /// 너는 60대 여성 마을 주민이야. 지금 마을 대표에게 전화해서 쓰레기 더미가 며칠째 안 치워졌다고 화내고 있어. 첫마디를 해줘.
        /// 
    }
}
