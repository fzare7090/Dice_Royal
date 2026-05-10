using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private List<Unit> team1Units = new List<Unit>();
    [SerializeField] private List<Unit> team2Units = new List<Unit>();
    [SerializeField] private GameObject unitPrefab; 
    [SerializeField] private Button buttonP1;
    [SerializeField] private Button buttonP2;
    [SerializeField] private Transform backGroundTransform;
    // [SerializeField] private GameManager gameManager;
    GameObject spawnedUnit ;
    private List<Coroutine> moveCoroutinesP1 = new List<Coroutine>();
    private List<Coroutine> moveCoroutinesP2 = new List<Coroutine>();

    void Start()
    {
        buttonP1.onClick.AddListener(OnClickP1);
        buttonP2.onClick.AddListener(OnClickP2);
    }
    
    

    void Update()
    {
        Team1Handeler();
        Team2Handeler();
    }
    private void Team1Handeler()
    {
        bool canAttackTeam = false;
        int temp = -1 ;
        for (int i = 0; i < team1Units.Count; i++)
        {
            for (int j = 0; j < team2Units.Count; j++)
            {
            if (team1Units[i].CheckAttackCondition(team2Units[j].transform))
            {
                if (!canAttackTeam)
                {
                    StopCoroutine(moveCoroutinesP1[i]);
                    StartCoroutine(team1Units[i].AttackUnit(team2Units[j].transform));
                    canAttackTeam = true ;
                    temp = i ;   
                }
            }
            else
            {
                if (canAttackTeam && temp >= 0)
                {
                   moveCoroutinesP1[temp] = StartCoroutine(team1Units[temp].MoveUnit());
                }
                canAttackTeam = false ;
            }
            }
            
        }
    }
    private void Team2Handeler()
    {
        bool canAttackTeam = false;
        int temp = -1 ;
        for (int i = 0; i < team2Units.Count; i++)
        {
            for (int j = 0; j < team1Units.Count; j++)
            {
            if (team2Units[i].CheckAttackCondition(team1Units[j].transform))
            {
                if (!canAttackTeam)
                {
                    StopCoroutine(moveCoroutinesP2[i]);
                    StartCoroutine(team2Units[i].AttackUnit(team1Units[j].transform));
                    canAttackTeam = true ;
                    temp = i ;   
                }
            }
            else
            {
                if (canAttackTeam && temp >= 0)
                {
                   moveCoroutinesP2[temp] = StartCoroutine(team2Units[temp].MoveUnit());
                }
                canAttackTeam = false ;
            }
            }
            
        }
    }
    
    
    

    private void OnClickP1()
    {
        SpawnUnit(1);
    
    }

    private void OnClickP2()
    {
        SpawnUnit(2);
    }

    private void SpawnUnit(int teamId)
    {
        spawnedUnit = Instantiate(unitPrefab, backGroundTransform);
        Unit unit = spawnedUnit.GetComponent<Unit>();
        Image image = spawnedUnit.GetComponent<Image>();
        unit.SetTeamId(teamId);

        if (teamId == 1)
        {
            spawnedUnit.transform.localPosition = unit.GetMinBoard();
            image.color = Color.green;
            team1Units.Add(unit);
            moveCoroutinesP1.Add(StartCoroutine(unit.MoveUnit()));
            StartCoroutine(unit.MoveUnit());

        }

        if (teamId == 2)
        {
            spawnedUnit.transform.localPosition = unit.GetMaxBoard();
            image.color = Color.red;
            team2Units.Add(unit);
            moveCoroutinesP2.Add(StartCoroutine(unit.MoveUnit()));
            StartCoroutine(unit.MoveUnit());
        }
    }
}
