using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Save Data/Castle Save")]
public class CastleSaveData : ScriptableObject
{
    [SerializeField] private int level = 1;

    public event Action<int> OnLevelChanged;

    public int Level => level;

    public void SetLevel(int value)
    {
        level = Mathf.Max(1, value);

        OnLevelChanged?.Invoke(level);
    }

    public bool LevelUp(int maxLevel)
    {
        if (level >= maxLevel)
            return false;

        level++;

        OnLevelChanged?.Invoke(level);

        return true;
    }

    public void ResetData()
    {
        level = 1;

        OnLevelChanged?.Invoke(level);
    }

    public int GetLevel()
    {
        return level;
    }
}