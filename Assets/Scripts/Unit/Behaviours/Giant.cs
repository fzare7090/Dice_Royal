using UnityEngine;

public class Giant : Unit
{
    protected override void CheckForEnemyInRange()
    {
        GiantData giantData = (GiantData)data;
        if (enemies == null) return;

        if (attackTarget != null && (!attackTarget.gameObject.activeInHierarchy || GetTargetEffectiveHP(attackTarget) <= 0))
        {
            attackTarget = null;
        }

        if (currentState == UnitState.Attacking && attackTarget != null)
        {
            float dist = GetDistanceToTarget(attackTarget);
            if (dist > giantData.attackRange)
            {
                currentState = UnitState.MovingToEnemy;
                ChaseEnemyModified();
            }
            return;
        }

        Transform closestTarget = null;
        float closestDistance = float.MaxValue;

        foreach (Transform enemy in enemies)
        {
            if (enemy == null || !enemy.gameObject.activeInHierarchy || GetTargetEffectiveHP(enemy) <= 0) continue;

            // تغییر کلیدی: استفاده از منطق داینامیک به جای هاردکد کردن Castle
            if (!CanAttackTarget(enemy))
                continue;

            float distance = GetDistanceToTarget(enemy);
            if (distance <= giantData.enemyFindingRange && distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = enemy;
            }
        }

        if (closestTarget != null)
        {
            if (attackTarget != closestTarget)
            {
                attackTarget = closestTarget;
            }

            if (closestDistance <= giantData.attackRange)
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
                    ChaseEnemyModified();
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

    private void ChaseEnemyModified()
    {
        if (attackTarget == null) return;
        agent.isStopped = false;
        agent.stoppingDistance = 0f;
        agent.SetDestination(attackTarget.position);
    }
}