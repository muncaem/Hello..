using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;

public class SolveComplaint : MonoBehaviour
{
    private void OnEnable()
    {
        EventHub.actionUpdateComplaintMsg += ClassifyComplaintAndCreateInNumber;
    }
    private void OnDisable()
    {
        EventHub.actionUpdateComplaintMsg -= ClassifyComplaintAndCreateInNumber;
    }

    [SerializeField] private GameObject OkSign;
    List<Tuple<int, int>> complaintNumberPair = new List<Tuple<int, int>>();
    private int currIndex = 0;

    /// <summary>
    /// �ο���ȭ �з� �� ������ȭ��ȣ ����
    /// </summary>
    /// <param name="content"></param>
    private void ClassifyComplaintAndCreateInNumber(string content)
    {
        int matchInNumb;

        if (Regex.IsMatch(content, "���ε�|����|����"))
        {
            // 1 �����Ѱ���
            matchInNumb = 1;
        }
        else if (Regex.IsMatch(content, "����|�ҹ� ����|Ⱦ�ܺ���|���� ����"))
        {
            // 2 ����������
            matchInNumb = 2;
        }
        else if (Regex.IsMatch(content, "��ƮȦ|�ϼ���"))
        {
            // 3 ���ΰ�
            matchInNumb = 3;
        }
        else if (Regex.IsMatch(content, "�� �����|������|���"))
        {
            // 4 ���� ������
            matchInNumb = 4;
        }
        else if (Regex.IsMatch(content, "��ġ|ȭ���"))
        {
            // 5 ����������
            matchInNumb = 5;
        }
        else if (Regex.IsMatch(content, "���Ǽ�"))
        {
            // 6 ������å��
            matchInNumb = 6;
        }
        else if (Regex.IsMatch(content, "�ܼ�"))
        {
            // 7 ������
            matchInNumb = 7;
        }
        else
        {
#if UNITY_EDITOR
            Debug.Log("No Match - ClassifyComplaintAndCreateInNumber");
#endif
            return;
        }

        complaintNumberPair.Add(Tuple.Create(currIndex++, matchInNumb));
    }

    /// <summary>
    /// Ű�е� ��ư�� ����
    /// </summary>
    public void PushedKeypad()
    {
        int inputNumber = int.Parse(EventSystem.current.currentSelectedGameObject.name);
        
        for (int i = 0; i < complaintNumberPair.Count; i++)
        {
            if (complaintNumberPair[i].Item1 == inputNumber)
            {
                EventHub.actionSolvedComplaint?.Invoke(i);
                OkSign.SetActive(true);
                StartCoroutine(GameManager.Instance.DelayTime(1.5f, () =>
                {
                    OkSign.SetActive(false);
                }));
                return;
            }
        }
    }
}
