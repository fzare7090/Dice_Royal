using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private PlayerResourceData playerResource;
    [SerializeField] private CastleSaveData castleSave;
    [SerializeField] private DiceSaveData diceSave;

    private bool initialized;

    public bool IsInitialized => initialized;

    #region Unity

    private void LoadGame()
    {
        string json = File.ReadAllText(SaveSettings.SavePath);
        GameSaveData data = JsonUtility.FromJson<GameSaveData>(json);

        if (data == null)
            return;

        // Resources
        playerResource.SetCoin(data.coin);
        playerResource.SetGem(data.gem);

        // Levels
        castleSave.SetLevel(data.castleLevel);
        diceSave.SetLevel(data.diceLevel);

        // Cards
        playerResource.ClearCards();

        foreach (var c in data.cards)
        {
            CardData card = playerResource.GetOrCreateCard(c.cardId);
            card.level = c.level;
            card.ownedCopies = c.ownedCopies;
        }

        Debug.Log("Game Loaded");
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        Debug.Log("SaveManager Awake");

        Initialize();
    }

    #endregion

    #region Initialize

    private void Initialize()
    {
        if (initialized)
            return;

        SaveGame();

        ValidateReferences();

        if (HasSave())
        {
            LoadGame();
        }
        else
        {
            CreateDefaultSave();
            SaveGame();
        }

        initialized = true;

        Debug.Log("Initialize");

    }

    private void ValidateReferences()
    {
        if (playerResource == null)
            Debug.LogError("PlayerResourceData reference missing.");

        if (castleSave == null)
            Debug.LogError("CastleSaveData reference missing.");

        if (diceSave == null)
            Debug.LogError("DiceSaveData reference missing.");
    }

    #endregion

    #region Save

    public void SaveGame()
    {
        Debug.Log("SaveGame Called");

        GameSaveData saveData = BuildSaveData();

        saveData.saveVersion = SaveSettings.SaveVersion;
        saveData.saveTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

        string json = JsonUtility.ToJson(saveData, true);

        File.WriteAllText(SaveSettings.SavePath, json);

        Debug.Log(SaveSettings.SavePath);
        Debug.Log("Game Saved");
    }

    private GameSaveData BuildSaveData()
    {
        GameSaveData data = new GameSaveData();

        //-----------------------------
        // Resources
        //-----------------------------

        data.coin = playerResource.Coin;
        data.gem = playerResource.Gem;

        //-----------------------------
        // Levels
        //-----------------------------

        data.castleLevel = castleSave.Level;
        data.diceLevel = diceSave.Level;

        //-----------------------------
        // Cards
        //-----------------------------

        data.cards.Clear();

        foreach (CardData card in playerResource.Cards)
        {
            CardSaveInfo info = new CardSaveInfo();

            info.cardId = card.cardId;
            info.level = card.level;
            info.ownedCopies = card.ownedCopies;

            data.cards.Add(info);
        }

        return data;
    }

    #endregion

    #region Utility

    public bool HasSave()
    {
        return File.Exists(SaveSettings.SavePath);
    }

    private void CreateDefaultSave()
    {
        playerResource.SetCoin(0);
        playerResource.SetGem(0);

        castleSave.SetLevel(1);
        diceSave.SetLevel(1);

        playerResource.ClearCards();
    }

    private void OnEnable()
    {
        Debug.Log("SaveManager Enabled");

        AutoSaveEvents.OnCoinChanged += SaveGame;
        AutoSaveEvents.OnGemChanged += SaveGame;
        AutoSaveEvents.OnCardChanged += SaveGame;
        AutoSaveEvents.OnCastleChanged += SaveGame;
        AutoSaveEvents.OnDiceChanged += SaveGame;
    }

    private void OnDisable()
    {
        AutoSaveEvents.OnCoinChanged -= SaveGame;
        AutoSaveEvents.OnGemChanged -= SaveGame;
        AutoSaveEvents.OnCardChanged -= SaveGame;
        AutoSaveEvents.OnCastleChanged -= SaveGame;
        AutoSaveEvents.OnDiceChanged -= SaveGame;
    }
    #endregion
}
