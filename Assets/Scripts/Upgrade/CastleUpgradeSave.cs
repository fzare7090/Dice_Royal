using UnityEngine;
using System.Collections.Generic;

public class CastleUpgradeSave : MonoBehaviour
{
    public static CastleUpgradeSave Instance { get; private set; }

    private Dictionary<string, int> castleLevels = new Dictionary<string, int>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveCastleLevel(string castleId, int level)
    {
        if (castleLevels.ContainsKey(castleId))
            castleLevels[castleId] = level;
        else
            castleLevels.Add(castleId, level);

        PlayerPrefs.SetInt($"Castle_{castleId}", level);
        PlayerPrefs.Save();
    }

    public int LoadCastleLevel(string castleId)
    {
        if (castleLevels.ContainsKey(castleId))
            return castleLevels[castleId];

        int savedLevel = PlayerPrefs.GetInt($"Castle_{castleId}", 1);
        castleLevels[castleId] = savedLevel;
        return savedLevel;
    }

    public void ResetAllCastles()
    {
        castleLevels.Clear();
    }
}
