using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class Unit : MonoBehaviour
{
    [SerializeField] protected int teamId;
    [SerializeField] protected UnitData data;
    [SerializeField] protected TMP_Text hpText;
    [SerializeField] protected List<Board> board;

    protected List<Transform> enemies;
    protected Transform attackTarget;

    protected List<Vector3> pathPoints = new List<Vector3>();
    protected int currentPoint = 0;


    protected float currentHP;
    private Vector3 minRightBridge = new Vector3(355, -200, 0);
    private Vector3 maxRightBridge = new Vector3(355, 200, 0);
    private Vector3 minLeftBridge = new Vector3(-355, -200, 0);
    private Vector3 maxLeftBridge = new Vector3(-355, 200, 0);
    private float midelBoardX = 0;


    protected enum UnitState
    {
        Moving,
        Attacking
    }

    public void SetTeamId(int newTeamId)
    {
        teamId = newTeamId;
    }
    public void SetBoard(List<Board> newBoard)
    {
        board = newBoard;
    }

    protected UnitState currentState;

    protected virtual void Start()
    {
        if (data != null)
        {
            InitializeUnit();
        }
    }

    public void SetData(UnitData newData)
    {
        data = newData;
    }

    
    protected virtual void InitializeUnit()
    {
        currentHP = data.hp;
        UpdateHPText();
        SetPath();
        currentState = UnitState.Moving;
        StartCoroutine(MoveUnit());

    }

    protected void UpdateHPText()
    {
        hpText.text = currentHP.ToString();
    }

    public void SetEnemies(List<Transform> newEnemies)
    {
        enemies = newEnemies;
    }
    private void SetPath()
    {
        pathPoints.Clear();
        currentPoint = 0;


        Vector3 targetPos1;
        Vector3 targetPos2;
        Vector3 targetPos3;
        Vector3 targetPos4;
        
        List<PrinceCastleConfig> princeCastleP1 = board[0].GetPrinceCastles();
        List<PrinceCastleConfig> princeCastleP2 = board[1].GetPrinceCastles();
        KingCastleConfig kingCastleP1 = board[0].GetKingCastle();
        KingCastleConfig kingCastleP2 = board[1].GetKingCastle();
        if (teamId == 1)
        {
            if (transform.localPosition.x > midelBoardX)
            {
                targetPos1 = minRightBridge;
                targetPos2 = maxRightBridge;

                targetPos3 = princeCastleP2[0].transform.localPosition;
                targetPos4 = kingCastleP2.transform.localPosition;
            }
            else
            {
                targetPos1 = minLeftBridge;
                targetPos2 = maxLeftBridge;

                targetPos3 = princeCastleP2[1].transform.localPosition;
                targetPos4 = kingCastleP2.transform.localPosition;
            }
        }
        else
        {
            if (transform.localPosition.x > midelBoardX)
            {
                targetPos1 = maxRightBridge;
                targetPos2 = minRightBridge;

                targetPos3 = princeCastleP1[0].transform.localPosition;
                targetPos4 = kingCastleP1.transform.localPosition;
            }
            else
            {
                targetPos1 = maxLeftBridge;
                targetPos2 = minLeftBridge;

                targetPos3 = princeCastleP1[1].transform.localPosition;
                targetPos4 = kingCastleP1.transform.localPosition;
            }
        }

        pathPoints.Add(targetPos1);
        pathPoints.Add(targetPos2);
        pathPoints.Add(targetPos3);
        pathPoints.Add(targetPos4);
    }

    protected IEnumerator MoveUnit()
    {
        while (true)
        {
            if (currentState == UnitState.Moving)
            {
                
                CheckForEnemyInRange();
                MoveForward();
            }

            yield return null;
        }
        
    }

    protected virtual void MoveForward()
    {
        
        if (currentPoint >= pathPoints.Count) return;
        
        transform.localPosition = Vector3.MoveTowards(
            transform.localPosition,
            pathPoints[currentPoint],
            data.moveSpeed * Time.deltaTime
        );
        

        if (Vector3.Distance(transform.localPosition, pathPoints[currentPoint]) < 0.1f)
        {
            currentPoint++;
        }
    }

    protected virtual void CheckForEnemyInRange()
    {
        if (enemies == null) return;

        foreach (Transform enemy in enemies)
        {
            if (enemy == null) continue;

            float distance = Vector3.Distance(
                transform.localPosition,
                enemy.localPosition
            );

            if (distance <= data.attackRange && attackTarget == null)
            {
                attackTarget = enemy;
                StartCoroutine(AttackUnit());
                break;
            }
        }
    }

    protected virtual IEnumerator AttackUnit()
    {
        currentState = UnitState.Attacking;

        while (attackTarget != null &&
               Vector3.Distance(transform.localPosition, attackTarget.localPosition) <= data.attackRange)
        {
            DealDamage();

            yield return new WaitForSeconds(data.attackCooldown);
        }

        attackTarget = null;
        currentState = UnitState.Moving;
    }

    protected virtual void DealDamage()
    {
        if (attackTarget == null) return;

        if (attackTarget.TryGetComponent<Unit>(out Unit targetUnit))
        {
            targetUnit.TakeDamage(data.attackDamage);
        }
        else if (attackTarget.TryGetComponent<Castle>(out Castle targetCastle))
        {
            targetCastle.TakeDamage(data.attackDamage);
        }
    }

    public virtual void TakeDamage(float damage)
    {
        currentHP -= damage;
        UpdateHPText();

        if (currentHP <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }
}
