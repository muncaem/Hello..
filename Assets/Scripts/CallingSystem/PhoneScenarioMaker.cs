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
    private List<string> situations = new List<string>
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




    //������Ʈ ���޿� �ó�����
    public /*string*/ScenarioData ScenarioMaker()
    {
        //string role = roles[Random.Range(0, roles.Count)];
        //string situation = situations[Random.Range(0, situations.Count)];

        List<string> emotionKeys = new List<string>(emotionMap.Keys);
        string emotion = emotionKeys[Random.Range(0, emotionKeys.Count)];
        //string emotionTag = emotionMap[emotion];

        //string scenario = $"�ʴ� {role}. {situation}. {emotion} ���¾�. �׻� ��� �� �տ� {emotionTag}�±׸� �ٿ��� ������ ǥ����.";
        //Debug.Log("������ �ó�����: " + scenario);

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
