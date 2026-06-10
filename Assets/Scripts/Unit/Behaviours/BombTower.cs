using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BombTower : Unit
{
    private readonly Dictionary<GameObject, List<Transform>> activeProjectiles = new Dictionary<GameObject, List<Transform>>();

    public override UnitData.UnitType GetUnitType()
    {
        return UnitData.UnitType.Building;
    }

    protected override void BuildWaypoints()
    {
        waypoints.Clear();
    }

    protected override void MoveForward()
    {
    }

    protected override void CheckForEnemyInRange()
    {
        if (enemies == null || data == null) return;

        if (attackTarget != null)
        {
            if (!attackTarget.gameObject.activeInHierarchy ||
                GetTargetEffectiveHP(attackTarget) <= 0 ||
                !CanAttackTarget(attackTarget) ||
                GetDistanceToTarget(attackTarget) > data.attackRange)
            {
                attackTarget = null;
            }
        }

        if (attackTarget == null)
        {
            attackTarget = FindClosestAttackableTarget();
        }

        if (attackTarget != null && currentState != UnitState.Attacking)
        {
            currentState = UnitState.Attacking;
            StopAllCoroutines();
            StartCoroutine(AttackRoutine());
        }
        else if (attackTarget == null)
        {
            currentState = UnitState.Moving;
        }
    }

    protected override IEnumerator AttackRoutine()
    {
        agent.isStopped = true;

        while (attackTarget != null &&
               CanAttackTarget(attackTarget) &&
               GetDistanceToTarget(attackTarget) <= data.attackRange)
        {
            if (GetTargetEffectiveHP(attackTarget) <= 0)
            {
                break;
            }

            FireBomb(attackTarget);

            yield return new WaitForSeconds(data.attackCooldown);

            if (attackTarget == null ||
                GetTargetEffectiveHP(attackTarget) <= 0 ||
                !CanAttackTarget(attackTarget) ||
                GetDistanceToTarget(attackTarget) > data.attackRange)
            {
                attackTarget = FindClosestAttackableTarget();
            }
        }

        agent.isStopped = false;
        currentState = UnitState.Moving;
    }

    private Transform FindClosestAttackableTarget()
    {
        Transform closestEnemy = null;
        float closestDistance = float.MaxValue;

        foreach (Transform enemy in enemies)
        {
            if (enemy == null ||
                !enemy.gameObject.activeInHierarchy ||
                GetTargetEffectiveHP(enemy) <= 0 ||
                !CanAttackTarget(enemy))
            {
                continue;
            }

            float distance = GetDistanceToTarget(enemy);
            if (distance <= data.attackRange && distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }

        return closestEnemy;
    }

    // متد اصلاح شده در کلاس BombTower
    private void FireBomb(Transform target)
    {
        BombTowerData bombTowerData = data as BombTowerData;
        if (bombTowerData == null)
        {
            DealAreaDamage(target.position, 0f, data.attackDamage);
            return;
        }

        Vector3 impactPosition = target.position;
        List<Transform> pendingTargets = GetTargetsInExplosion(impactPosition, bombTowerData.explosionRadius);

        foreach (Transform pendingTarget in pendingTargets)
        {
            AddTargetPendingDamage(pendingTarget, data.attackDamage);
        }

        if (bombTowerData.bombPrefab == null)
        {
            DealAreaDamage(impactPosition, bombTowerData.explosionRadius, data.attackDamage);
            return;
        }

        // ساخت پرتابه بمب
        GameObject bomb = Instantiate(
            bombTowerData.bombPrefab,
            transform.position,
            Quaternion.identity,
            transform
        );

        // ********** تغییر اصلی: تنظیم اسکیل پرتابه بمب تا بزرگ نشود **********
        bomb.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

        if (bomb.TryGetComponent<Image>(out Image image))
        {
            image.color = teamId == 1 ? Color.green : Color.red;
        }

        StartCoroutine(MoveBomb(bomb, target, impactPosition, bombTowerData, pendingTargets));
    }
    private IEnumerator MoveBomb(
        GameObject bomb,
        Transform target,
        Vector3 fallbackImpactPosition,
        BombTowerData bombTowerData,
        List<Transform> pendingTargets)
    {
        activeProjectiles[bomb] = pendingTargets;
        Vector3 impactPosition = fallbackImpactPosition;

        while (bomb != null)
        {
            if (target != null)
            {
                impactPosition = target.position;
            }

            if (Vector3.Distance(bomb.transform.position, impactPosition) <= 0.08f)
            {
                break;
            }

            bomb.transform.position = Vector3.MoveTowards(
                bomb.transform.position,
                impactPosition,
                data.attackSpeed * Time.deltaTime
            );

            yield return null;
        }

        activeProjectiles.Remove(bomb);

        if (bomb != null)
        {
            Destroy(bomb);
        }

        RemovePendingDamage(pendingTargets, data.attackDamage);
        DealAreaDamage(impactPosition, bombTowerData.explosionRadius, data.attackDamage);
    }

    private List<Transform> GetTargetsInExplosion(Vector3 center, float radius)
    {
        List<Transform> targets = new List<Transform>();
        Collider[] colliders = Physics.OverlapSphere(center, radius);

        foreach (Collider col in colliders)
        {
            Transform target = col.transform;

            if (target.TryGetComponent<Unit>(out Unit unit))
            {
                if (unit.GetTeamId() != teamId &&
                    unit.GetEffectiveHP() > 0 &&
                    CanAttackTarget(target))
                {
                    targets.Add(target);
                }
            }
            else if (target.TryGetComponent<Castle>(out Castle castle))
            {
                if (castle.GetTeamId() != teamId &&
                    castle.GetEffectiveHP() > 0 &&
                    CanAttackTarget(target))
                {
                    targets.Add(target);
                }
            }
        }

        return targets;
    }

    private void DealAreaDamage(Vector3 center, float radius, float damage)
    {
        foreach (Transform target in GetTargetsInExplosion(center, radius))
        {
            if (target.TryGetComponent<Unit>(out Unit unit))
            {
                unit.TakeDamage(damage);
            }
            else if (target.TryGetComponent<Castle>(out Castle castle))
            {
                castle.TakeDamage(damage);
            }
        }
    }

    private void RemovePendingDamage(List<Transform> targets, float damage)
    {
        foreach (Transform target in targets)
        {
            if (target != null)
            {
                AddTargetPendingDamage(target, -damage);
            }
        }
    }

    protected override void Die()
    {
        BombTowerData bombTowerData = data as BombTowerData;
        if (bombTowerData != null &&
            bombTowerData.deathExplosionDamage > 0f &&
            bombTowerData.deathExplosionRadius > 0f)
        {
            DealAreaDamage(transform.position, bombTowerData.deathExplosionRadius, bombTowerData.deathExplosionDamage);
        }

        base.Die();
    }

    private void OnDestroy()
    {
        if (data == null) return;

        foreach (var kvp in activeProjectiles)
        {
            RemovePendingDamage(kvp.Value, data.attackDamage);

            if (kvp.Key != null)
            {
                Destroy(kvp.Key);
            }
        }

        activeProjectiles.Clear();
    }
}