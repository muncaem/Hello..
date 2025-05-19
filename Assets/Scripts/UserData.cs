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
    private float _userReputation = 50;
    public float userReputation
    {
        get => _userReputation;
        set => _userReputation = Mathf.Clamp(value, 0, 100);
    }

    public int firstPreFactor { get; set; }
    public int firstMidFactor { get; set; }
    public int firstPostFactor { get; set; }


    public int mainFirstDayPre { get; set; }
    public int mainFirstDayMid { get; set; }
    public int mainFirstDayPost { get; set; }


    public int lastPreFactor { get; set; }
    public int lastMidFactor { get; set; }
    public int lastPostFactor { get; set; }
}
