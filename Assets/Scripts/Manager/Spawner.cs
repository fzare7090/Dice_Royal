using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spawner : MonoBehaviour
{
    [SerializeField] private List<Transform> team2Units = new List<Transform>();
    [SerializeField] private List<Transform> team1Units = new List<Transform>();

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
    


    private List<UnitData> deck;

    
    private List<UnitData> p1Hand = new List<UnitData>();
    private Queue<UnitData> p1Queue = new Queue<UnitData>();

    private List<UnitData> p2Hand = new List<UnitData>();
    private Queue<UnitData> p2Queue = new Queue<UnitData>();

    RectTransform currentSpawner;

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

    private bool SpawnUnit(UnitData data, int teamId)
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


        if (player.GetElexir().GetElexir() < data.elexir)
        {
            return false;
        }

        if (!areaManager.CanPlayerSpawnAtPosition(currentSpawner, teamId))
        {
            Debug.Log("CanPlayerSpawnAtPosition is false");
            return false;
        }


        CardUpgrade cardUpgrade = null;

        for (int i = 0; i < data.spawnCount; i++)
        {
            GameObject spawnedUnit = Instantiate(data.Unitprefab, backGroundTransform.parent);

            Unit unit = spawnedUnit.GetComponent<Unit>();
            Renderer renderer = spawnedUnit.GetComponent<Renderer>();

            unit.SetTeamId(teamId);
            unit.SetBoard(board);
            unit.SetData(data);

            if (teamId == 0)
            {
                spawnedUnit.transform.localPosition = positions[i];
                renderer.material.color = Color.green;

                team1Units.Add(unit.transform);
                unit.SetEnemies(team2Units);
            }
            else
            {
                spawnedUnit.transform.localPosition = positions[i];
                renderer.material.color = Color.red;

                team2Units.Add(unit.transform);
                unit.SetEnemies(team1Units);
            }

            
            if (cardUpgrade == null)
            {
                cardUpgrade = spawnedUnit.GetComponent<CardUpgrade>();
                if (cardUpgrade == null)
                {
                    cardUpgrade = spawnedUnit.AddComponent<CardUpgrade>();
                }
            }
        }

        

        player.GetElexir().UsingElexir(data.elexir);

        return true;
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
                    float angle = Mathf.PI * 2f * i / countReduced;

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
        
        for (int i = 0; i < deck.Count; i++)
        {
            if (i < 4) p1Hand.Add(deck[i]);
            else p1Queue.Enqueue(deck[i]);
        }

       
        for (int i = 0; i < deck.Count; i++)
        {
            if (i < 4) p2Hand.Add(deck[i]);
            else p2Queue.Enqueue(deck[i]);
        }

        
        for (int i = 0; i < p1Hand.Count; i++)
        {
            CardButtonUI buttonP1 = Instantiate(buttonPrefab, cardButtonParentP1);
            buttonP1.SetCard(p1Hand[i], 0, this);

            buttonP1.OnDragStartEvent += HandleDragStart;
            buttonP1.OnDragEvent += HandleDrag;
            buttonP1.OnDragEndEvent += HandleDragEnd;
        }

        
        for (int i = 0; i < p2Hand.Count; i++)
        {
            CardButtonUI buttonP2 = Instantiate(buttonPrefab, cardButtonParentP2);
            buttonP2.SetCard(p2Hand[i], 1, this);

            buttonP2.OnDragStartEvent += HandleDragStart;
            buttonP2.OnDragEvent += HandleDrag;
            buttonP2.OnDragEndEvent += HandleDragEnd;
        }

        
        drageCardP1.gameObject.SetActive(false);
        drageCardP2.gameObject.SetActive(false);
    }

    private void HandleDragStart(CardButtonUI button, UnitData data, int teamId, Vector2 screenPos)
    {
        RectTransform dragger = (teamId == 0) ? drageCardP1 : drageCardP2;
        dragger.gameObject.SetActive(true);
        PositionDragger(dragger, screenPos);

        
        if (areaManager != null)
        {
            areaManager.ShowValidAreas(teamId, true);
        }
    }

    private void HandleDrag(Vector2 screenPos)
    {
        if (drageCardP1.gameObject.activeSelf) PositionDragger(drageCardP1, screenPos);
        if (drageCardP2.gameObject.activeSelf) PositionDragger(drageCardP2, screenPos);
    }

    private void PositionDragger(RectTransform dragger, Vector2 screenPos)
    {
        Camera mainCam = Camera.main;
        Canvas parentCanvas = dragger.GetComponentInParent<Canvas>();

        if (parentCanvas != null && parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            dragger.position = screenPos;
        }
        else if (parentCanvas != null)
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(
                dragger.parent as RectTransform,
                screenPos,
                parentCanvas.worldCamera,
                out Vector3 worldPoint);
            dragger.position = worldPoint;
        }
    }

    private void HandleDragEnd(CardButtonUI button, UnitData data, int teamId)
    {
        RectTransform dragger = (teamId == 0) ? drageCardP1 : drageCardP2;

        dragger.gameObject.SetActive(false); 

        
        if (areaManager != null)
        {
            areaManager.ShowValidAreas(teamId, false);
        }

        
        bool spawned = SpawnUnit(data, teamId);

        if (spawned)
        {
            CycleCard(button, data, teamId);
        }
    }

    private void CycleCard(CardButtonUI button, UnitData playedData, int teamId)
    {
        if (teamId == 0)
        {
            
            p1Hand.Remove(playedData);
            p1Queue.Enqueue(playedData);

            
            UnitData nextCard = p1Queue.Dequeue();
            p1Hand.Add(nextCard);

            
            button.AnimateCardCycle(nextCard);
        }
        else
        {
            p2Hand.Remove(playedData);
            p2Queue.Enqueue(playedData);

            UnitData nextCard = p2Queue.Dequeue();
            p2Hand.Add(nextCard);

            button.AnimateCardCycle(nextCard);
        }
    }

    private void InitializeCastles()
    {
        for (int i = 0; i < princessCastleP1.Count; i++)
        {
            princessCastleP1[i].SetTeamId(0);
            princessCastleP1[i].SetEnemies(team2Units);
            princessCastleP1[i].SetData(princessCastleDataP1);
            team1Units.Add(princessCastleP1[i].transform);
        }

        for (int i = 0; i < princessCastleP2.Count; i++)
        {
            princessCastleP2[i].SetTeamId(1);
            princessCastleP2[i].SetEnemies(team1Units);
            princessCastleP2[i].SetData(princessCastleDataP2);
            team2Units.Add(princessCastleP2[i].transform);
        }

        kingCastleP1.SetTeamId(0);
        kingCastleP2.SetTeamId(1);

        kingCastleP1.SetEnemies(team2Units);
        kingCastleP2.SetEnemies(team1Units);

        kingCastleP1.SetData(kingCastleDataP1);
        kingCastleP2.SetData(kingCastleDataP2);

        team1Units.Add(kingCastleP1.transform);
        team2Units.Add(kingCastleP2.transform);
    }

    private void SetPlayerTeamId()
    {
        for (int i = 0; i < players.Count; i++)
        {
            players[i].SetTeamId(i);
        }
    }
}
