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
    protected float pendingDamage = 0f;

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

    public void SetData(UnitData newData)
    {
        data = newData;
    }

    // گرفتن نوع نیرو برای تشخیص توسط سایر نیروها
    public virtual UnitData.UnitType GetUnitType()
    {
        return data != null ? data.unitType : UnitData.UnitType.Ground;
    }

    // بررسی اینکه آیا این نیرو مجاز به حمله به هدف مشخص شده هست یا خیر
    public virtual bool CanAttackTarget(Transform target)
    {
        UnitData.UnitType targetType;

        // تشخیص نوع هدف
        if (target.TryGetComponent<Unit>(out Unit targetUnit))
        {
            targetType = targetUnit.data.unitType; // یا targetUnit.unitType بستگی به ساختارت دارد
        }
        else if (target.TryGetComponent<Castle>(out Castle _))
        {
            targetType = UnitData.UnitType.Building;
        }
        else
        {
            return false;
        }

        // بررسی اجازه حمله بر اساس اولویت هدفِ این نیرو
        switch (data.targetPreference)
        {
            case UnitData.TargetPreference.GroundOnly:
                return targetType == UnitData.UnitType.Ground || targetType == UnitData.UnitType.Building; // معمولا نیروهای زمینی‌زن، ساختمان هم می‌زنند
            case UnitData.TargetPreference.AirOnly:
                return targetType == UnitData.UnitType.Air;
            case UnitData.TargetPreference.BuildingsOnly:
                return targetType == UnitData.UnitType.Building;
            case UnitData.TargetPreference.GroundAndAir:
                return true; // به همه چیز می‌تواند حمله کند
            default:
                return false;
        }
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

        PrinceCastleConfig princeCastleP1Right = board[0].GetRightPrinceCastles();
        PrinceCastleConfig princeCastleP1Left = board[0].GetLeftPrinceCastles();
        KingCastleConfig kingCastleP1 = board[0].GetKingCastle();

        PrinceCastleConfig princeCastleP2Right = board[1].GetRightPrinceCastles();
        PrinceCastleConfig princeCastleP2Left = board[1].GetLeftPrinceCastles();
        KingCastleConfig kingCastleP2 = board[1].GetKingCastle();

        bool useRightLane = transform.localPosition.x > 0;

        if (teamId == 0)
        {
            PrinceCastleConfig prince = useRightLane ? princeCastleP2Right : princeCastleP2Left;
            Transform kingTarget = kingCastleP2?.transform;

            if (prince != null && prince.gameObject.activeInHierarchy) waypoints.Add(prince.transform);
            if (kingTarget != null) waypoints.Add(kingTarget);
        }
        else
        {
            PrinceCastleConfig prince = useRightLane ? princeCastleP1Right : princeCastleP1Left;
            Transform kingTarget = kingCastleP1?.transform;

            if (prince != null && prince.gameObject.activeInHierarchy) waypoints.Add(prince.transform);
            if (kingTarget != null) waypoints.Add(kingTarget);
        }
    }

    protected void UpdateHPText()
    {
        if (hpText != null)
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
        CheckForEnemyInRange();

        switch (currentState)
        {
            case UnitState.Moving:
                MoveForward();
                break;
            case UnitState.MovingToEnemy:
                if (attackTarget != null)
                {
                    agent.SetDestination(attackTarget.position);
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
                break;
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

        float distanceToTarget = Vector3.Distance(transform.position, targetPos);
        bool physicallyClose = distanceToTarget <= agent.stoppingDistance + 2f;
        bool pathComplete = !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance && agent.pathStatus == NavMeshPathStatus.PathComplete;

        if (physicallyClose || pathComplete)
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

    protected float GetTargetEffectiveHP(Transform target)
    {
        if (target == null) return 0;
        if (target.TryGetComponent<Unit>(out Unit unit)) return unit.GetEffectiveHP();
        if (target.TryGetComponent<Castle>(out Castle castle)) return castle.GetEffectiveHP();
        return 1;
    }

    protected void AddTargetPendingDamage(Transform target, float damage)
    {
        if (target == null) return;
        if (target.TryGetComponent<Unit>(out Unit unit)) unit.AddPendingDamage(damage);
        else if (target.TryGetComponent<Castle>(out Castle castle)) castle.AddPendingDamage(damage);
    }

    protected virtual void CheckForEnemyInRange()
    {
        if (enemies == null) return;

        // بررسی اعتبار هدف فعلی با استفاده از فیلتر جدید CanAttackTarget
        if (attackTarget != null)
        {
            if (!attackTarget.gameObject.activeInHierarchy || GetTargetEffectiveHP(attackTarget) <= 0 || !CanAttackTarget(attackTarget))
            {
                attackTarget = null;
            }
        }

        if (currentState == UnitState.Attacking && attackTarget != null)
        {
            float dist = GetDistanceToTarget(attackTarget);
            if (dist > data.attackRange)
            {
                currentState = UnitState.MovingToEnemy;
                ChaseEnemy();
            }
            return;
        }

        Transform closestEnemy = null;
        float closestDistance = float.MaxValue;

        foreach (Transform enemy in enemies)
        {
            // اضافه شدن شرط CanAttackTarget برای نادیده گرفتن اهداف غیرمرتبط
            if (enemy == null || !enemy.gameObject.activeInHierarchy || GetTargetEffectiveHP(enemy) <= 0 || !CanAttackTarget(enemy)) continue;

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
            }

            if (closestDistance <= data.attackRange)
            {
                if (currentState != UnitState.Attacking)
                {
                    currentState = UnitState.Attacking;
                    StopAllCoroutines();
                    StartCoroutine(AttackRoutine());
                }
            }
            else
            {
                if (currentState != UnitState.MovingToEnemy)
                {
                    currentState = UnitState.MovingToEnemy;
                    ChaseEnemy();
                }
                else
                {
                    agent.SetDestination(attackTarget.position);
                }
            }
        }
        else
        {
            attackTarget = null;
            if (currentState != UnitState.Moving)
            {
                ReturnToOriginalPath();
            }
        }
    }

    protected void ChaseEnemy()
    {
        if (attackTarget == null) return;
        agent.isStopped = false;
        agent.stoppingDistance = 0f;
        agent.SetDestination(attackTarget.position);
    }

    protected virtual IEnumerator AttackRoutine()
    {
        agent.isStopped = true;
        // بررسی مداوم فیلتر CanAttackTarget هنگام حمله
        while (attackTarget != null && CanAttackTarget(attackTarget) && GetDistanceToTarget(attackTarget) <= data.attackRange)
        {
            DealDamage();

            if (attackTarget == null)
                break;

            yield return new WaitForSeconds(data.attackCooldown);

            if (attackTarget == null || !CanAttackTarget(attackTarget) || GetDistanceToTarget(attackTarget) > data.attackRange)
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
        pendingDamage -= damage;
        if (pendingDamage < 0) pendingDamage = 0;

        UpdateHPText();
        if (currentHP <= 0)
            Die();
    }

    public void AddPendingDamage(float damage)
    {
        pendingDamage += damage;
    }

    public float GetEffectiveHP()
    {
        return currentHP - pendingDamage;
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }
}