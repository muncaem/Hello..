using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneManager : MonoBehaviour
{
    private void Awake()
    {
        GptRequester.actionGptReceived += EndCallUI;
    }
    
    private void EndCallUI(string c, bool isEnd)
    {
        if (isEnd)
        {
            // ��ȭ ���� UI
        }
    }
}
