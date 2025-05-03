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
    /// 민원전화 분류 및 내선전화번호 연결
    /// </summary>
    /// <param name="content"></param>
    private void ClassifyComplaintAndCreateInNumber(string content)
    {
        int matchInNumb;

        if (Regex.IsMatch(content, "가로등|소음|음주"))
        {
            // 1 안전총괄과
            matchInNumb = 1;
        }
        else if (Regex.IsMatch(content, "버스|불법 주차|횡단보도|과속 차량"))
        {
            // 2 교통행정과
            matchInNumb = 2;
        }
        else if (Regex.IsMatch(content, "포트홀|하수구"))
        {
            // 3 도로과
            matchInNumb = 3;
        }
        else if (Regex.IsMatch(content, "길 고양이|쓰레기|담배"))
        {
            // 4 위생 안전과
            matchInNumb = 4;
        }
        else if (Regex.IsMatch(content, "벤치|화장실"))
        {
            // 5 공원녹지과
            matchInNumb = 5;
        }
        else if (Regex.IsMatch(content, "보건소"))
        {
            // 6 보건정책과
            matchInNumb = 6;
        }
        else if (Regex.IsMatch(content, "단수"))
        {
            // 7 수도과
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
    /// 키패드 버튼과 연결
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
