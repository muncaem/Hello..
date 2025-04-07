using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneScenarioMaker : MonoBehaviour
{
    private List<string> roles = new List<string> 
    { 
        "70�� ���� ����ȸ���̾�",
        "���� �Ʊ� ������",
        "���� ����л��̾�",
        "���� ����л��̾�",
        "���� ���л��̾�",
        "���� ���л��̾�",
        "���� ���� �ڿ����ھ�",
        "�п� ���� �������̾�",
        "���� ���� ������",
        "���� �������̾�"
    };
    private List<string> situations  = new List<string> 
    {
        "���� �뼱�� �پ�� �� ���� ��� �־�",
        "�� �� ���ε��� ���峪�� �� ���� ���� �����̾ �����ٴ� ���� ���� ��� �־�",
        "�߼�ȭ ���� ���� �� ����� ������ �ο��� �־���",
        "��Ȱ�ϴ� ���� ���� ���� ������ ������ �ʹ� ū ���� ���� ��� �־�",
        "�ε��� ħ���ϰ� �ִ� �ҹ� ���� ������ ����� �ϰ� �־� �ҹ� ������ ���� �ذ� ����� �����϶�� �ο��� �ְ� �־�",
        "�氡�� ��Ȱ �����Ⱑ �������� �η� �ִ� ���� ���� ��� �־�",
        "������ ���̰� ��ĥ° �� ġ������ �ο��� �־���",
    };
    //private List<string> emotions = new List<string> 
    //{ 
    //    "�ſ� �г���",
    //    "�ӻ��ϰ� ����",
    //    "�Ҹ��� ū",
    //    "ȥ��������",
    //    "�����",
    //    "�Ҹ��� ������ ������"
    //};
    //private List<string> emotionTags = new List<string>
    //{
    //    "[ȭ��]",
    //    "[����]",
    //    "[�߸�]"
    //};

    // ���� ����� �±׸� 1:1�� ����
    private Dictionary<string, string> emotionMap = new Dictionary<string, string>
    {
        { "�ſ� �г���", "[ȭ��]" },
        { "�ӻ��ϰ� ��ģ", "[����]" },
        { "�Ҹ��� ������ ������", "[�߸�]" },
        { "ȥ�������� �����", "[ȭ��]" },
        { "ȥ��������", "[ȭ��]" },
        { "�����", "����" },
        { "�߸�����", "[�߸�]" }
    };

    //// ��ư Ŭ���̵� �Ϸ簡 ���۵Ǹ� �ٷ� �������� �ؼ� �������� �ó����� �����ϵ����Ѵ�.
    ////string selected = scenarios[Random.Range(0, scenarios.Count)];



    //������Ʈ ���޿� �ó�����
    public string ScenarioMaker()
    {
        string role = roles[Random.Range(0, roles.Count)];
        string situation = situations[Random.Range(0, situations.Count)];

        List<string> emotionKeys = new List<string>(emotionMap.Keys);
        string emotion = emotionKeys[Random.Range(0, emotionKeys.Count)];
        string emotionTag = emotionMap[emotion];

        string scenario = $"�ʴ� {role}. {situation}. {emotion} ���¾�. �׻� ��� �� �տ� {emotionTag}�±׸� �ٿ��� ������ ǥ����.";
        Debug.Log("������ �ó�����: " + scenario);

        return scenario;
            //"�ʴ� 70�� ���� ����ȸ���̾�. ���� �뼱�� �پ�� �� ���� ��� �־�. �ſ� �г��� ���¾�. �׻� ��� �� �տ� [ȭ��] �±׸� �ٿ��� ������ ǥ����.";
    }
}
