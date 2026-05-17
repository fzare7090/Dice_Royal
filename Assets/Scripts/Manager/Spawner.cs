using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
    [SerializeField] private List<Transform> team1Enemies = new List<Transform>();
    [SerializeField] private List<Transform> team2Enemies = new List<Transform>();

    [Header("Castle Data")]
    [SerializeField] private CastleData kingCastleDataP1;
    [SerializeField] private CastleData princessCastleDataP1;
    [SerializeField] private CastleData kingCastleDataP2;
    [SerializeField] private CastleData princessCastleDataP2;

    [Header("Castle")]
    [SerializeField] private KingCastleConfig kingCastleP1;
    [SerializeField] private List<PrinceCastleConfig> princessCastleP1 = new List<PrinceCastleConfig>();
    [SerializeField] private KingCastleConfig kingCastleP2;
    [SerializeField] private List<PrinceCastleConfig> princessCastleP2 = new List<PrinceCastleConfig>();

    [Header("Board")]
    [SerializeField] private List<Board> board;

    [Header("Spawn Parent")]
    [SerializeField] private Transform backGroundTransform;

    [Header("Drag Card")]
    [SerializeField] private Image drageCardP1;
    [SerializeField] private Image drageCardP2;

    [Header("Players")]
    [SerializeField] private List<Player> players = new List<Player>();
    private Player player1;
    private Player player2;

    [Header("Card UI")]
    [SerializeField] private Transform cardButtonParent;
    [SerializeField] private CardButtonUI buttonPrefab;

    private List<UnitData> deck;

    void Awake()
    {
        InitializeCastles();
    }

    void Start()
    {
        deck = GameDeckManager.Instance.GetPlayerDeck();
        

        player1 = players[0];
        player2 = players[1];

        SetPlayerTeamId();
        CreateCardButtons();
    }

    private void SpawnUnit(UnitData data, int teamId)
    {
        Player player;
        if (teamId == 1) {
            player = player1;
        } else {
            player = player2;
        }
        if (player.GetElexir().GetElexir() < data.elexir)
            return;

        GameObject spawnedUnit = Instantiate(data.Unitprefab, backGroundTransform);

        Unit unit = spawnedUnit.GetComponent<Unit>();
        Image image = spawnedUnit.GetComponent<Image>();

        unit.SetTeamId(teamId);
        unit.SetBoard(board);
        unit.SetData(data);

        if (teamId == 1)
        {
            spawnedUnit.transform.localPosition = drageCardP1.transform.localPosition;
            image.color = Color.green;

            team2Enemies.Add(unit.transform);
            unit.SetEnemies(team1Enemies);
        }
        else
        {
            spawnedUnit.transform.localPosition = drageCardP2.transform.localPosition;
            image.color = Color.red;

            team1Enemies.Add(unit.transform);
            unit.SetEnemies(team2Enemies);
        }

        player.GetElexir().UsingElexir(data.elexir);
    }

     private void CreateCardButtons()
    {
        foreach (UnitData data in deck)
        {
            CardButtonUI button = Instantiate(buttonPrefab, cardButtonParent);

            button.SetCard(data);

            button.GetButton().onClick.AddListener(OnButtonClick);
            void OnButtonClick()
            {
                SpawnUnit(data, 1);
            }
            
        }
    }



    private void InitializeCastles()
    {
        for (int i = 0; i < princessCastleP1.Count; i++)
        {
            princessCastleP1[i].SetTeamId(1);
            princessCastleP1[i].SetEnemies(team1Enemies);
            princessCastleP1[i].SetData(princessCastleDataP1);
            team2Enemies.Add(princessCastleP1[i].transform);
        }

        for (int i = 0; i < princessCastleP2.Count; i++)
        {
            princessCastleP2[i].SetTeamId(2);
            princessCastleP2[i].SetEnemies(team2Enemies);
            princessCastleP2[i].SetData(princessCastleDataP2);
            team1Enemies.Add(princessCastleP2[i].transform);
        }

        kingCastleP1.SetTeamId(1);
        kingCastleP2.SetTeamId(2);

        kingCastleP1.SetEnemies(team1Enemies);
        kingCastleP2.SetEnemies(team2Enemies);

        kingCastleP1.SetData(kingCastleDataP1);
        kingCastleP2.SetData(kingCastleDataP2);

        team2Enemies.Add(kingCastleP1.transform);
        team1Enemies.Add(kingCastleP2.transform);
    }

    private void SetPlayerTeamId()
    {
        for (int i = 0; i < players.Count; i++)
        {
            players[i].SetTeamId(i + 1);
        }
    }
}
