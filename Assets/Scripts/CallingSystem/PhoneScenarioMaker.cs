using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneScenarioMaker : MonoBehaviour
{
    // ������Ʈ ���޿� �ó����� ����
    //List<string> scenarios = new List<string>
    //{
    //      "�ʴ� 70�� ���� ����ȸ���̾�. ���� �뼱�� �پ�� �� ���� ��� �־�. �ſ� �г��� ���¾�. �׻� ��� �� �տ� [ȭ��] �±׸� �ٿ��� ������ ǥ����.",
    //      "�ʴ� ���� �̱۸��̾�. ����Ʈ ���������� ���� ������ �������� �ް� �־�. �ӻ��ϰ� ��ģ ���¾�. �׻� ��� �� �տ� [����] �±׸� �ٿ��� ������ ǥ����.",
    //      "�ʴ� ����̸� Ű��� 30�� �����̾�. ���� ������ ������ �ο��� �־���. �Ҹ��� ������ �����ϰ� ���ϴ� �����̾�. �׻� ��� �� �տ� [�߸�] �±׸� �ٿ�."
    //};
    // ��ư Ŭ���̵� �Ϸ簡 ���۵Ǹ� �ٷ� �������� �ؼ� �������� �ó����� �����ϵ����Ѵ�.
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
        return "�ʴ� 70�� ���� ����ȸ���̾�. ���� �뼱�� �پ�� �� ���� ��� �־�. �ſ� �г��� ���¾�. �׻� ��� �� �տ� [ȭ��] �±׸� �ٿ��� ������ ǥ����.";
        ///ex: ������ ������ �ʹ� ���Ƽ� �����~
        /// �ʴ� 60�� ���� ���� �ֹ��̾�. ���� ���� ��ǥ���� ��ȭ�ؼ� ������ ���̰� ��ĥ° �� ġ�����ٰ� ȭ���� �־�. ù���� ����.
        /// 
    }
}
