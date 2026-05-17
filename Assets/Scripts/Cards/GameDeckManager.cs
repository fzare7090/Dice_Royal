using System.Collections.Generic;
using UnityEngine;

public class GameDeckManager : MonoBehaviour
{
    public static GameDeckManager Instance;

    private List<UnitData> playerDeck = new List<UnitData>();

    void Awake()
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

    public void SetPlayerDeck(List<UnitData> unitData)
    {
        playerDeck = new List<UnitData>(unitData);
    }

    public List<UnitData> GetPlayerDeck()
    {
        return playerDeck;
    }
}
