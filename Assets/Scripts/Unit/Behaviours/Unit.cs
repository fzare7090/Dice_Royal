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
    protected List<Vector3> originalPathPoints = new List<Vector3>();
    protected bool isReturningToOriginalPath = false;
    protected int originalPathIndex = 0;

    protected float currentHP;
    private Vector3 minRightBridge = new Vector3(355, -100, 0);
    private Vector3 maxRightBridge = new Vector3(355, 50, 0);
    private Vector3 minLeftBridge = new Vector3(-361, -100, 0);
    private Vector3 maxLeftBridge = new Vector3(-361, 50, 0);
    private float midelBoardX = 0;

    protected enum UnitState
    {
        Moving,
        Attacking,
        MovingToEnemy
    }

    protected UnitState currentState;

    public void SetTeamId(int newTeamId)
    {
        teamId = newTeamId;
    }

    public void SetBoard(List<Board> newBoard)
    {
        board = newBoard;
    }

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
        SetMovingPath();
        originalPathPoints = new List<Vector3>(pathPoints);
        currentState = UnitState.Moving;
    }

    protected void UpdateHPText()
    {
        hpText.text = currentHP.ToString();
    }

    public void SetEnemies(List<Transform> newEnemies)
    {
        enemies = newEnemies;
    }

    protected virtual void Update()
    {

        ContinuouslyCheckEnemy();

        switch (currentState)
        {
            case UnitState.Moving:
                MoveForward();
                CheckForEnemyInRange();
                break;

            case UnitState.MovingToEnemy:

                if (attackTarget != null)
                {
                    UpdateEnemyPathContinuously();
                    CheckAttackRange();
                }
                else
                {
                    ReturnToOriginalPath();
                }
                MoveForward();
                break;

            case UnitState.Attacking:

                if (attackTarget == null)
                {
                    ReturnToOriginalPath();
                }
                else
                {

                    float distance = Vector3.Distance(transform.localPosition, attackTarget.localPosition);
                    if (distance > data.attackRange)
                    {
                        currentState = UnitState.MovingToEnemy;
                        SetEnemyPath();
                    }
                }
                break;
        }
    }

    protected void ContinuouslyCheckEnemy()
    {
        if (attackTarget != null && currentState != UnitState.Attacking)
        {
            float distance = Vector3.Distance(transform.localPosition, attackTarget.localPosition);


            if (distance > data.enemyFindingRange)
            {
                ReturnToOriginalPath();
            }

            else if (distance <= data.attackRange && currentState == UnitState.MovingToEnemy)
            {
                currentState = UnitState.Attacking;
                StopAllCoroutines();
                StartCoroutine(AttackRoutine());
            }
        }
    }

    protected void SetMovingPath()
    {
        pathPoints.Clear();
        currentPoint = 0;

        Vector3 pos = transform.localPosition;
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
            if (pos.x > midelBoardX)
            {
                if (pos.y <= minRightBridge.y)
                {
                    targetPos1 = minRightBridge;
                    targetPos2 = maxRightBridge;
                    targetPos3 = princeCastleP2[0].transform.localPosition;
                    targetPos4 = kingCastleP2.transform.localPosition;
                    pathPoints.Add(targetPos1);
                    pathPoints.Add(targetPos2);
                    pathPoints.Add(targetPos3);
                    pathPoints.Add(targetPos4);
                    return;
                }
                else if (pos.y <= maxRightBridge.y)
                {
                    targetPos1 = maxRightBridge;
                    targetPos2 = princeCastleP2[0].transform.localPosition;
                    targetPos3 = kingCastleP2.transform.localPosition;
                    pathPoints.Add(targetPos1);
                    pathPoints.Add(targetPos2);
                    pathPoints.Add(targetPos3);
                    return;
                }
                else if (pos.y <= princeCastleP2[0].transform.localPosition.y)
                {
                    targetPos1 = princeCastleP2[0].transform.localPosition;
                    targetPos2 = kingCastleP2.transform.localPosition;
                    pathPoints.Add(targetPos1);
                    pathPoints.Add(targetPos2);
                    return;
                }
                else
                {
                    targetPos1 = kingCastleP2.transform.localPosition;
                    pathPoints.Add(targetPos1);
                    return;
                }
            }
            else
            {
                if (pos.y <= minLeftBridge.y)
                {
                    targetPos1 = minLeftBridge;
                    targetPos2 = maxLeftBridge;
                    targetPos3 = princeCastleP2[1].transform.localPosition;
                    targetPos4 = kingCastleP2.transform.localPosition;
                    pathPoints.Add(targetPos1);
                    pathPoints.Add(targetPos2);
                    pathPoints.Add(targetPos3);
                    pathPoints.Add(targetPos4);
                    return;
                }
                else if (pos.y <= maxLeftBridge.y)
                {
                    targetPos1 = maxLeftBridge;
                    targetPos2 = princeCastleP2[1].transform.localPosition;
                    targetPos3 = kingCastleP2.transform.localPosition;
                    pathPoints.Add(targetPos1);
                    pathPoints.Add(targetPos2);
                    pathPoints.Add(targetPos3);
                    return;
                }
                else if (pos.y <= princeCastleP2[1].transform.localPosition.y)
                {
                    targetPos1 = princeCastleP2[1].transform.localPosition;
                    targetPos2 = kingCastleP2.transform.localPosition;
                    pathPoints.Add(targetPos1);
                    pathPoints.Add(targetPos2);
                    return;
                }
                else
                {
                    targetPos1 = kingCastleP2.transform.localPosition;
                    pathPoints.Add(targetPos1);
                    return;
                }
            }
        }
        else
        {
            if (pos.x > midelBoardX)
            {
                if (pos.y >= maxRightBridge.y)
                {
                    targetPos1 = maxRightBridge;
                    targetPos2 = minRightBridge;
                    targetPos3 = princeCastleP1[0].transform.localPosition;
                    targetPos4 = kingCastleP1.transform.localPosition;
                    pathPoints.Add(targetPos1);
                    pathPoints.Add(targetPos2);
                    pathPoints.Add(targetPos3);
                    pathPoints.Add(targetPos4);
                    return;
                }
                else if (pos.y >= minRightBridge.y)
                {
                    targetPos1 = minRightBridge;
                    targetPos2 = princeCastleP1[0].transform.localPosition;
                    targetPos3 = kingCastleP1.transform.localPosition;
                    pathPoints.Add(targetPos1);
                    pathPoints.Add(targetPos2);
                    pathPoints.Add(targetPos3);
                    return;
                }
                else if (pos.y >= princeCastleP1[0].transform.localPosition.y)
                {
                    targetPos1 = princeCastleP1[0].transform.localPosition;
                    targetPos2 = kingCastleP1.transform.localPosition;
                    pathPoints.Add(targetPos1);
                    pathPoints.Add(targetPos2);
                    return;
                }
                else
                {
                    targetPos1 = kingCastleP1.transform.localPosition;
                    pathPoints.Add(targetPos1);
                    return;
                }
            }
            else
            {
                if (pos.y >= maxLeftBridge.y)
                {
                    targetPos1 = maxLeftBridge;
                    targetPos2 = minLeftBridge;
                    targetPos3 = princeCastleP1[1].transform.localPosition;
                    targetPos4 = kingCastleP1.transform.localPosition;
                    pathPoints.Add(targetPos1);
                    pathPoints.Add(targetPos2);
                    pathPoints.Add(targetPos3);
                    pathPoints.Add(targetPos4);
                    return;
                }
                else if (pos.y >= minLeftBridge.y)
                {
                    targetPos1 = minLeftBridge;
                    targetPos2 = princeCastleP1[1].transform.localPosition;
                    targetPos3 = kingCastleP1.transform.localPosition;
                    pathPoints.Add(targetPos1);
                    pathPoints.Add(targetPos2);
                    pathPoints.Add(targetPos3);
                    return;
                }
                else if (pos.y >= princeCastleP1[1].transform.localPosition.y)
                {
                    targetPos1 = princeCastleP1[1].transform.localPosition;
                    targetPos2 = kingCastleP1.transform.localPosition;
                    pathPoints.Add(targetPos1);
                    pathPoints.Add(targetPos2);
                    return;
                }
                else
                {
                    targetPos1 = kingCastleP1.transform.localPosition;
                    pathPoints.Add(targetPos1);
                    return;
                }
            }
        }
    }

    protected void SetEnemyPath()
    {
        if (attackTarget == null)
        {
            return;
        }

        pathPoints.Clear();

        pathPoints.Add(attackTarget.localPosition);
        currentPoint = 0;
    }

    protected void UpdateEnemyPathContinuously()
    {
        if (attackTarget == null || currentState != UnitState.MovingToEnemy) return;


        if (pathPoints.Count > 0)
        {

            if (Vector3.Distance(pathPoints[pathPoints.Count - 1], attackTarget.localPosition) > 1f)
            {
                SetEnemyPath();
            }
        }
    }



    protected virtual void MoveForward()
    {
        if (currentPoint >= pathPoints.Count)
        {

            if (currentState == UnitState.MovingToEnemy && attackTarget != null)
            {

                float distanceToTarget = Vector3.Distance(transform.localPosition, attackTarget.localPosition);

                if (distanceToTarget <= data.attackRange)
                {
                    currentState = UnitState.Attacking;
                    StopAllCoroutines();
                    StartCoroutine(AttackRoutine());
                }
                else
                {
                    SetEnemyPath();
                }
            }
            else if (currentState == UnitState.Moving)
            {
                Destroy(gameObject);
            }
            return;
        }


        transform.localPosition = Vector3.MoveTowards(
            transform.localPosition,
            pathPoints[currentPoint],
            data.moveSpeed * Time.deltaTime
        );


        if (Vector3.Distance(transform.localPosition, pathPoints[currentPoint]) < 1f)
        {
            currentPoint++;


            if (currentState == UnitState.MovingToEnemy && attackTarget != null)
            {
                SetEnemyPath();
            }
        }
    }

    protected virtual void CheckForEnemyInRange()
    {
        if (enemies == null) return;

        Transform closestEnemy = null;
        float closestDistance = float.MaxValue;

        foreach (Transform enemy in enemies)
        {
            if (enemy == null) continue;

            float distance = Vector3.Distance(
                transform.localPosition,
                enemy.localPosition
            );


            if (distance <= data.enemyFindingRange && distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }

        if (closestEnemy != null)
        {

            if (attackTarget != closestEnemy)
            {
                attackTarget = closestEnemy;


                if (closestDistance <= data.attackRange)
                {
                    currentState = UnitState.Attacking;
                    StopAllCoroutines();
                    StartCoroutine(AttackRoutine());
                }
                else
                {

                    currentState = UnitState.MovingToEnemy;
                    SetEnemyPath();
                }
            }
            else if (currentState == UnitState.MovingToEnemy)
            {

                SetEnemyPath();
            }
        }
        else if (attackTarget != null && currentState != UnitState.Attacking)
        {

            ReturnToOriginalPath();
        }
    }

    protected virtual void CheckAttackRange()
    {
        if (attackTarget == null)
        {
            ReturnToOriginalPath();
            return;
        }

        float distanceToTarget = Vector3.Distance(
            transform.localPosition,
            attackTarget.localPosition
        );


        if (distanceToTarget <= data.attackRange)
        {
            if (currentState != UnitState.Attacking)
            {
                currentState = UnitState.Attacking;
                StopAllCoroutines();
                StartCoroutine(AttackRoutine());
            }
        }

        else if (distanceToTarget <= data.enemyFindingRange)
        {
            if (currentState != UnitState.MovingToEnemy)
            {
                currentState = UnitState.MovingToEnemy;
                SetEnemyPath();
            }
        }

        else
        {
            ReturnToOriginalPath();
        }
    }

    protected virtual IEnumerator AttackRoutine()
    {
        while (attackTarget != null &&
               Vector3.Distance(transform.localPosition, attackTarget.localPosition) <= data.attackRange)
        {
            DealDamage();
            yield return new WaitForSeconds(data.attackCooldown);


            if (attackTarget == null ||
                Vector3.Distance(transform.localPosition, attackTarget.localPosition) > data.attackRange)
            {
                break;
            }
        }


        if (attackTarget != null &&
            Vector3.Distance(transform.localPosition, attackTarget.localPosition) <= data.enemyFindingRange)
        {

            currentState = UnitState.MovingToEnemy;
            SetEnemyPath();
        }
        else
        {

            ReturnToOriginalPath();
        }
    }

    protected void ReturnToOriginalPath()
    {
        if (originalPathPoints.Count == 0)
        {
            return;
        }

        float minDistance = float.MaxValue;
        int closestIndex = 0;

        for (int i = 0; i < originalPathPoints.Count; i++)
        {
            float distance = Vector3.Distance(transform.localPosition, originalPathPoints[i]);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestIndex = i;
            }
        }

        pathPoints.Clear();
        pathPoints.AddRange(originalPathPoints);
        currentPoint = closestIndex;
        currentState = UnitState.Moving;
        attackTarget = null;
        isReturningToOriginalPath = false;
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
