using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainUIManager : MonoBehaviour
{
    private void Awake()
    {
        GameManager.actionUpdatedDay += UpdateDayUI;
    }

    /// <summary>
    /// 날짜 UI 변경
    /// </summary>
    private void UpdateDayUI()
    {

    }
}
