using UnityEngine;
using System.Collections.Generic;

public class UpgradeSaveManager : MonoBehaviour
{
    public static UpgradeSaveManager Instance { get; private set; }

    // ذخیره سطح‌ها (key: شناسه یکتا, value: سطح)
    private Dictionary<string, int> savedLevels = new Dictionary<string, int>();

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

    // ذخیره سطح (بعد از هر آپگرید صدا بزن)
    public void SaveLevel(string id, int level)
    {
        if (savedLevels.ContainsKey(id))
            savedLevels[id] = level;
        else
            savedLevels.Add(id, level);

        // ذخیره در PlayerPrefs برای ماندگاری بعد از بستن بازی
        PlayerPrefs.SetInt(id, level);
        PlayerPrefs.Save();
    }

    // دریافت سطح ذخیره شده
    public int LoadLevel(string id, int defaultLevel = 1)
    {
        if (savedLevels.ContainsKey(id))
            return savedLevels[id];

        int savedLevel = PlayerPrefs.GetInt(id, defaultLevel);
        savedLevels[id] = savedLevel;
        return savedLevel;
    }
}