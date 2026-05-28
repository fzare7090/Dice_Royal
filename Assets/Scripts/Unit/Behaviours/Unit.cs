using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour
{
    [SerializeField] protected int teamId;
    [SerializeField] protected UnitData data;
    [SerializeField] protected TMP_Text hpText;
    [SerializeField] protected List<Board> board;

    [Header("Waypoints (assigned or auto‑generated)")]
    [SerializeField] private List<Transform> predefinedWaypoints;

    protected NavMeshAgent agent;
    protected List<Transform> enemies;
    protected Transform attackTarget;


    protected List<Transform> waypoints = new List<Transform>();
    protected int currentWaypointIndex = 0;

    protected float currentHP;

    protected enum UnitState
    {
        Moving,
        MovingToEnemy,
        Attacking
    }
    protected UnitState currentState;

    public void SetTeamId(int newTeamId)
    {
        teamId = newTeamId;
    }
    public int GetTeamId()
    {
        return teamId;
    }

    public void SetBoard(List<Board> newBoard)
    {
        board = newBoard;
    }

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
            agent = gameObject.AddComponent<NavMeshAgent>();


        agent.updateRotation = false;
        agent.autoBraking = true;
        agent.stoppingDistance = 0.1f;
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
        agent.speed = data.moveSpeed;


        if (!agent.isOnNavMesh)
            agent.Warp(transform.position);


        BuildWaypoints();

        if (waypoints.Count > 0)
        {
            currentWaypointIndex = 0;
            agent.SetDestination(waypoints[0].position);
        }

        currentState = UnitState.Moving;
    }


    protected virtual void BuildWaypoints()
    {
        waypoints.Clear();


        if (predefinedWaypoints != null && predefinedWaypoints.Count > 0)
        {
            waypoints.AddRange(predefinedWaypoints);
            return;
        }


        List<PrinceCastleConfig> princeCastleP1 = board[0].GetPrinceCastles();
        List<PrinceCastleConfig> princeCastleP2 = board[1].GetPrinceCastles();
        KingCastleConfig kingCastleP1 = board[0].GetKingCastle();
        KingCastleConfig kingCastleP2 = board[1].GetKingCastle();


        bool useRightLane = transform.position.x > 0;

        if (teamId == 1)
        {

            Transform bridgeEntry = useRightLane ?
                GameObject.Find("BridgeEntry_Right_Team1")?.transform :
                GameObject.Find("BridgeEntry_Left_Team1")?.transform;


            PrinceCastleConfig prince = useRightLane ? princeCastleP2[0] : princeCastleP2[1];
            Transform kingTarget = kingCastleP2?.transform;

            if (bridgeEntry != null)
                waypoints.Add(bridgeEntry);
            if (prince != null && prince.gameObject.activeInHierarchy)
                waypoints.Add(prince.transform);
            if (kingTarget != null)
                waypoints.Add(kingTarget);
        }
        else
        {
            Transform bridgeEntry = useRightLane ?
                GameObject.Find("BridgeEntry_Right_Team2")?.transform :
                GameObject.Find("BridgeEntry_Left_Team2")?.transform;

            PrinceCastleConfig prince = useRightLane ? princeCastleP1[0] : princeCastleP1[1];
            Transform kingTarget = kingCastleP1?.transform;

            if (bridgeEntry != null)
                waypoints.Add(bridgeEntry);
            if (prince != null && prince.gameObject.activeInHierarchy)
                waypoints.Add(prince.transform);
            if (kingTarget != null)
                waypoints.Add(kingTarget);
        }
    }

    protected void UpdateHPText()
    {
        hpText.text = currentHP.ToString();
    }

    public void SetEnemies(List<Transform> newEnemies)
    {
        enemies = newEnemies;
    }

    protected float GetDistanceToTarget(Transform target)
    {
        if (target == null) return float.MaxValue;
        Collider targetCollider = null;

        if (target.TryGetComponent<Castle>(out Castle castle))
        {
            targetCollider = castle.GetCastleCollider();
        }
        else
        {
            targetCollider = target.GetComponent<Collider>();
        }
        if (targetCollider != null)
        {
            Vector3 closestPoint = targetCollider.ClosestPoint(transform.position);
            return Vector3.Distance(transform.position, closestPoint);
        }

        return Vector3.Distance(transform.position, target.position);
    }

    protected Vector3 GetClosestPointOnTarget(Transform target)
    {
        if (target == null) return transform.position;
        Collider targetCollider = null;
        if (target.TryGetComponent<Castle>(out Castle castle))
        {
            targetCollider = castle.GetCastleCollider();
        }
        else
        {
            targetCollider = target.GetComponent<Collider>();
        }
        if (targetCollider != null)
        {
            return targetCollider.ClosestPoint(transform.position);
        }
        return target.position;
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

                    agent.SetDestination(GetClosestPointOnTarget(attackTarget));
                    CheckAttackRange();
                }
                else
                {
                    ReturnToOriginalPath();
                }
                break;
            case UnitState.Attacking:
                if (attackTarget == null)
                {
                    ReturnToOriginalPath();
                }
                else
                {

                    float dist = GetDistanceToTarget(attackTarget);
                    if (dist > data.attackRange)
                    {
                        currentState = UnitState.MovingToEnemy;
                        ChaseEnemy();
                    }
                }
                break;
        }
    }

    protected void ContinuouslyCheckEnemy()
    {
        if (attackTarget != null && currentState != UnitState.Attacking)
        {
            float distance = GetDistanceToTarget(attackTarget);

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


    protected virtual void MoveForward()
    {
        if (waypoints == null || waypoints.Count == 0)
            return;

        while (currentWaypointIndex < waypoints.Count && waypoints[currentWaypointIndex] == null)
            currentWaypointIndex++;
        if (currentWaypointIndex >= waypoints.Count)
        {
            Destroy(gameObject);
            return;
        }

        if (agent.isStopped) agent.isStopped = false;
        if (agent.stoppingDistance > 0.2f) agent.stoppingDistance = 0.1f;

        Vector3 targetPos = waypoints[currentWaypointIndex].position;
        if (Vector3.Distance(agent.destination, targetPos) > 0.5f)
        {
            agent.SetDestination(targetPos);
        }
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            currentWaypointIndex++;
            while (currentWaypointIndex < waypoints.Count && waypoints[currentWaypointIndex] == null)
                currentWaypointIndex++;
            if (currentWaypointIndex >= waypoints.Count)
            {
                Destroy(gameObject);
                return;
            }
            agent.SetDestination(waypoints[currentWaypointIndex].position);
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

            float distance = GetDistanceToTarget(enemy);
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
                    ChaseEnemy();
                }
            }
            else if (currentState == UnitState.MovingToEnemy)
            {
                ChaseEnemy();
            }
        }
        else if (attackTarget != null && currentState != UnitState.Attacking)
        {
            ReturnToOriginalPath();
        }
    }

    protected virtual void CheckAttackRange()
    {
        if (attackTarget == null) return;

        float distanceToTarget = GetDistanceToTarget(attackTarget);
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
                ChaseEnemy();
            }
        }
        else
        {
            ReturnToOriginalPath();
        }
    }


    protected void ChaseEnemy()
    {
        if (attackTarget == null) return;
        agent.isStopped = false;
        agent.stoppingDistance = data.attackRange * 0.9f;
        agent.SetDestination(GetClosestPointOnTarget(attackTarget));
    }


    protected virtual IEnumerator AttackRoutine()
    {
        agent.isStopped = true;
        while (attackTarget != null &&
                   GetDistanceToTarget(attackTarget) <= data.attackRange)
        {
            DealDamage();

            if (attackTarget == null)
                break;
            yield return new WaitForSeconds(data.attackCooldown);
            if (attackTarget == null ||
                        GetDistanceToTarget(attackTarget) > data.attackRange)
            {
                break;
            }
        }
        agent.isStopped = false;

        if (attackTarget == null)
        {
            ReturnToOriginalPath();
            yield break;
        }
        if (GetDistanceToTarget(attackTarget) <= data.enemyFindingRange)
        {
            currentState = UnitState.MovingToEnemy;
            ChaseEnemy();
        }
        else
        {
            ReturnToOriginalPath();
        }
    }


    protected void ReturnToOriginalPath()
    {
        attackTarget = null;
        agent.isStopped = false;
        agent.stoppingDistance = 0.1f;

        waypoints.RemoveAll(w => w == null);
        if (waypoints.Count == 0)
        {
            currentState = UnitState.Moving;
            return;
        }

        if (currentWaypointIndex >= waypoints.Count)
            currentWaypointIndex = waypoints.Count - 1;

        float minDist = float.MaxValue;
        int closestIndex = currentWaypointIndex;
        for (int i = 0; i < waypoints.Count; i++)
        {
            if (waypoints[i] == null) continue;
            float dist = Vector3.Distance(transform.position, waypoints[i].position);
            if (dist < minDist)
            {
                minDist = dist;
                closestIndex = i;
            }
        }
        currentWaypointIndex = closestIndex;
        agent.SetDestination(waypoints[currentWaypointIndex].position);
        currentState = UnitState.Moving;
    }

    protected virtual void DealDamage()
    {
        if (attackTarget == null) return;

        if (attackTarget.TryGetComponent<Unit>(out Unit targetUnit))
            targetUnit.TakeDamage(data.attackDamage);
        else if (attackTarget.TryGetComponent<Castle>(out Castle targetCastle))
            targetCastle.TakeDamage(data.attackDamage);
    }

    public virtual void TakeDamage(float damage)
    {
        currentHP -= damage;
        UpdateHPText();
        if (currentHP <= 0)
            Die();
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }
}
