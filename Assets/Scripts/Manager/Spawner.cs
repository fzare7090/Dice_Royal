using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private RectTransform drageCardP1;
    [SerializeField] private RectTransform drageCardP2;

    [Header("Players")]
    [SerializeField] private List<Player> players = new List<Player>();
    private Player player1;
    private Player player2;

    [Header("Card UI")]
    [SerializeField] private Transform cardButtonParentP1;
    [SerializeField] private Transform cardButtonParentP2;
    [SerializeField] private CardButtonUI buttonPrefab;


    [SerializeField] private AreaManager areaManager;
    [Header("Upgrade Manager")]
    [SerializeField] private SceneManagerDataSaver gameSceneManager;


    private List<UnitData> deck;

    RectTransform currentSpawner;

    void Awake()
    {
        InitializeCastles();

    }

    void Start()
    {

        deck = GameDeckManager.Instance.GetPlayerDeck(); //

        player1 = players[0];
        player2 = players[1];

        SetPlayerTeamId();
        CreateCardButtons(); //

        //SetBoardsInfo();
    }

    private void SpawnUnit(UnitData data, int teamId)
    {
        Player player;
        List<Vector3> positions;
        if (teamId == 0)
        {
            player = player1;
            positions = SetUnitsPositions(data.spawnCount, drageCardP1.transform.localPosition);
            currentSpawner = drageCardP1;
        }
        else
        {
            player = player2;
            positions = SetUnitsPositions(data.spawnCount, drageCardP2.transform.localPosition);
            currentSpawner = drageCardP2;
        }
        Debug.Log("unit position set");

        if (player.GetElexir().GetElexir() < data.elexir)
        {
            Debug.Log("Elexir is not enough");
            return;
        }

        if (!areaManager.CanPlayerSpawnAtPosition(currentSpawner, teamId))
        {
            Debug.Log("CanPlayerSpawnAtPosition is false");
            return;
        }

        // ========== متغیر رو اینجا تعریف کن (بیرون حلقه) ==========
        CardUpgrade cardUpgrade = null;

        for (int i = 0; i < data.spawnCount; i++)
        {
            GameObject spawnedUnit = Instantiate(data.Unitprefab, backGroundTransform.parent);

            Unit unit = spawnedUnit.GetComponent<Unit>();
            Image image = spawnedUnit.GetComponent<Image>();

            unit.SetTeamId(teamId);
            unit.SetBoard(board);
            unit.SetData(data);
            Vector3 position = new Vector3(0, 0, 0);

            if (teamId == 0)
            {
                spawnedUnit.transform.localPosition = positions[i];
                image.color = Color.green;

                Debug.Log("unit swaned");

                team2Enemies.Add(unit.transform);
                unit.SetEnemies(team1Enemies);

                Debug.Log("unit set as enemi");
            }
            else
            {
                spawnedUnit.transform.localPosition = positions[i];
                image.color = Color.red;

                Debug.Log("unit swaned");

                team1Enemies.Add(unit.transform);
                unit.SetEnemies(team2Enemies);

                Debug.Log("unit set as enemi");
            }

            // ========== فقط برای اولین یونیت CardUpgrade رو ثبت کن (اختیاری) ==========
            if (cardUpgrade == null)
            {
                cardUpgrade = spawnedUnit.GetComponent<CardUpgrade>();
                if (cardUpgrade == null)
                {
                    cardUpgrade = spawnedUnit.AddComponent<CardUpgrade>();
                }
            }
        }

        // حالا اینجا cardUpgrade تعریف شده و در دسترس هست
        if (gameSceneManager != null && cardUpgrade != null)
        {
            gameSceneManager.RegisterCard(cardUpgrade);
        }

        player.GetElexir().UsingElexir(data.elexir);
    }
    private List<Vector3> SetUnitsPositions(int count, Vector3 center)
    {
        List<Vector3> positions = new List<Vector3>();
        float radius = 100f;
        if (count < 7)
        {
            if (count == 1)
            {
                positions.Add(center);
                return positions;
            }
            for (int i = 0; i < count; i++)
            {
                float angle = Mathf.PI * 2f * i / count;

                Vector3 offset = new Vector3(
                    Mathf.Cos(angle) * radius,
                    Mathf.Sin(angle) * radius,
                    0
                );
                positions.Add(center + offset);
            }
            return positions;
        }
        else if (count == 15)
        {
            int countReduced = 9;
            float radiusReduced = 100;

            for (int j = 0; j < 3; j++)
            {
                countReduced -= 2;
                radiusReduced -= 10;
                for (int i = 0; i < countReduced; i++)
                {
                    float angle = Mathf.PI * 2f * i / count;

                    Vector3 offset = new Vector3(
                        Mathf.Cos(angle) * radiusReduced,
                        Mathf.Sin(angle) * radiusReduced,
                        0
                    );
                    positions.Add(center + offset);
                }
            }

            return positions;
        }
        return positions;

    }


    private void CreateCardButtons()
    {
        foreach (UnitData data in deck)
        {
            CardButtonUI buttonP1 = Instantiate(buttonPrefab, cardButtonParentP1);
            CardButtonUI buttonP2 = Instantiate(buttonPrefab, cardButtonParentP2);


            buttonP1.SetCard(data);
            buttonP2.SetCard(data);

            buttonP1.GetButton().onClick.AddListener(OnButtonClickP1);
            buttonP2.GetButton().onClick.AddListener(OnButtonClickP2);

            void OnButtonClickP1()
            {
                SpawnUnit(data, 0);
            }
            void OnButtonClickP2()
            {
                SpawnUnit(data, 1);
            }
            Debug.Log("card buttons instatiate");


        }
    }



    private void InitializeCastles()
    {
        for (int i = 0; i < princessCastleP1.Count; i++)
        {
            princessCastleP1[i].SetTeamId(0);
            princessCastleP1[i].SetEnemies(team1Enemies);
            princessCastleP1[i].SetData(princessCastleDataP1);
            team2Enemies.Add(princessCastleP1[i].transform);


        }

        for (int i = 0; i < princessCastleP2.Count; i++)
        {
            princessCastleP2[i].SetTeamId(1);
            princessCastleP2[i].SetEnemies(team2Enemies);
            princessCastleP2[i].SetData(princessCastleDataP2);
            team1Enemies.Add(princessCastleP2[i].transform);
        }

        kingCastleP1.SetTeamId(0);
        kingCastleP2.SetTeamId(1);

        kingCastleP1.SetEnemies(team1Enemies);
        kingCastleP2.SetEnemies(team2Enemies);

        kingCastleP1.SetData(kingCastleDataP1);
        kingCastleP2.SetData(kingCastleDataP2);

        team2Enemies.Add(kingCastleP1.transform);
        team1Enemies.Add(kingCastleP2.transform);

        Debug.Log("castles initialize");
    }

    private void SetPlayerTeamId()
    {
        for (int i = 0; i < players.Count; i++)
        {
            players[i].SetTeamId(i + 1);
        }
    }
    

}