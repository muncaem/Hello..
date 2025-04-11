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

    public int firstPreFactor { get; set; }
    public int firstMidFactor { get; set; }
    public int firstPostFactor { get; set; }

    public int lastPreFactor { get; set; }
    public int lastMidFactor { get; set; }
    public int lastPostFactor { get; set; }
}
