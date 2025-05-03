using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserData : MonoBehaviour
{
    public static UserData Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
    }

    public string userName { get; set; }
    public string userDetermination { get; set; }
    private float _userReputation = 100;
    public float userReputation
    {
        get => _userReputation;
        set => _userReputation = Mathf.Clamp(value, 0, 100);
    }

    public int firstPreFactor { get; set; }
    public int firstMidFactor { get; set; }
    public int firstPostFactor { get; set; }

    public int lastPreFactor { get; set; }
    public int lastMidFactor { get; set; }
    public int lastPostFactor { get; set; }


    /// <summary>
    /// 민원 처리 완료되었을 경우, 평판 관리
    /// 하나 처리 완료 시, 16씩 +
    /// 하루 넘어갈 때 해결하지 못한 민원 갯수 세서 16 x n개만큼 -
    /// UnTakeCall 클릭 시 혹은 전화하다가 끊었을 때, 5씩 -
    /// </summary>
    private void ManageUserReputation()
    {

    }
}
