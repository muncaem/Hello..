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
    /// ��¥ UI ����
    /// </summary>
    private void UpdateDayUI()
    {

    }
}
