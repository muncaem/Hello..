using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class SaveData
{
    public int userValue;
    public int avoidFactor;
    public int afterRegretFactor;
    public int traumaFactor;
}

public class CollectExamination : MonoBehaviour
{
    [SerializeField] private List<GameObject> checkBoxLists;
    private int[] avoidIndex = { 1, 2, 3, 5, 6, 7, 11, 12, 13, 18, 21, 22, 23 };
    private int[] afterRegretIndex = { 4, 8, 9, 10, 14, 15, 19, 24 };
    private int[] traumaIndex = { 16, 17, 20 };

    private int avoidFactor = 0;
    private int afterRegretFactor = 0;
    private int traumaFactor = 0;

    // 데이터 저장 객체
    private SaveData saveData = new SaveData();
    // 파일 이름
    private string SAVE_FILENAME = "/SaveFile.txt";

    private void OnEnable()
    {
        if (!Directory.Exists(Application.persistentDataPath + "/Saves"))
            Directory.CreateDirectory(Application.persistentDataPath + "/Saves");

        if (File.Exists(Application.persistentDataPath + "/Saves" + SAVE_FILENAME))
        {
            string loadJSon = File.ReadAllText(Application.persistentDataPath + "/Saves" + SAVE_FILENAME);
            saveData = JsonUtility.FromJson<SaveData>(loadJSon);
#if UNITY_EDITOR
            print($"Save ExaminationData Loaded: avoid-{saveData.avoidFactor}, regret-{saveData.afterRegretFactor}, trauma-{saveData.traumaFactor}");
#endif
        }
    }

    public void CollectCheckBox()
    {
        for (int i = 0; i < checkBoxLists.Count; i++)
        {
            if (checkBoxLists[i].activeSelf)
                SortFactor(i + 1);
        }

        SaveDataToLocal(); // 로컬에 측정 결과 저장
        // 추후 서버에 저장
    }

    private void SortFactor(int idx)
    {
        if (Array.Exists(avoidIndex, x => x == idx))
            avoidFactor++;
        else if (Array.Exists(afterRegretIndex, x => x == idx))
            afterRegretFactor++;
        else
            traumaFactor++;
    }

    private void SaveDataToLocal()
    {
        saveData.userValue++;
        saveData.avoidFactor = avoidFactor;
        saveData.afterRegretFactor = afterRegretFactor;
        saveData.traumaFactor = traumaFactor;

        // 저장
        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(Application.persistentDataPath + "/Saves" + SAVE_FILENAME, json);
    }
}
