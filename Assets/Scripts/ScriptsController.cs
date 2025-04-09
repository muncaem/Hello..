using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScriptsController : MonoBehaviour
{
    private Text textUI;
    [SerializeField] private string[] dialogs;

    private void Awake()
    {
        textUI = transform.GetChild(0).GetComponent<Text>();
    }

    void Start()
    {
        //DiagnosisSystem.actionTakeCall += NormalQuestionVoice;
        //DiagnosisSystem.actionUnTakeCall += NormalQuestionText;
    }


    // ó�� ���� ��
    // ��ȭ ���� ��, �Ϲ� ���� -> �⺻���� ���� ����
    void NormalQuestionVoice(int idx)
    {
        /// ������ ����ϴ� ���� �ɸ�üũ ���� �߰� ����
        /// �ش� �Լ��� ������ ��.
        /// �����ϰ� ����ũ �ڵ����� Ȱ��ȭ �ż� ��� �ν� => ����ũ �߰��ұ�
        /// "���� �ν� ��� ��ũ��Ʈ ���� ����"
        /// !��� ������ ����Ʈ üũ �ʿ�! ���� �� �̵� ���ͷ�Ƽ���ư onmike�Լ��� actionEndTalk?.Invoke()ó��
    }
    // ��ȭ ������ ��, ����
    public void NormalQuestionText(int idx)
    {
        StartCoroutine(GameManager.Instance.DelayTime(1f,
            () =>
            {
                textUI.gameObject.SetActive(true);
                textUI.text = dialogs[idx];
            }
            ));
    }


    //// ���� ��, ��������Ʈ ����
    //private void OnDestroy()
    //{
    //    DiagnosisSystem.actionTakeCall -= NormalQuestionVoice;
    //    DiagnosisSystem.actionUnTakeCall -= NormalQuestionText;
    //}
}
