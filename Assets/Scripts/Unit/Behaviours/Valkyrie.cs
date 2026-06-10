using UnityEngine;
using System.Collections;

public class Valkyrie : Unit
{
    protected override IEnumerator AttackRoutine()
    {
        ValkyrieData valData = data as ValkyrieData;
        agent.isStopped = true;

        // تغییر: چک کردن امکان حمله به هدف اصلی
        while (attackTarget != null && CanAttackTarget(attackTarget) && GetDistanceToTarget(attackTarget) <= data.attackRange)
        {
            DealAreaDamage(valData);

            if (attackTarget == null)
                break;

            yield return new WaitForSeconds(data.attackCooldown);

            if (attackTarget == null || GetDistanceToTarget(attackTarget) > data.attackRange)
            {
                break;
            }
        }

        agent.isStopped = false;

        if (attackTarget == null || GetTargetEffectiveHP(attackTarget) <= 0)
        {
            ReturnToOriginalPath();
        }
        else
        {
            currentState = UnitState.MovingToEnemy;
            ChaseEnemy();
        }
    }

    private void DealAreaDamage(ValkyrieData valData)
    {
        float radius = (valData != null) ? valData.areaOfEffectRadius : 1.5f;
        if (enemies == null) return;

        foreach (Transform enemy in enemies)
        {
            if (enemy == null || !enemy.gameObject.activeInHierarchy) continue;

            float distance = Vector3.Distance(transform.position, enemy.position);

            if (distance <= radius)
            {
                // تغییر: بررسی اینکه آیا والکری اجازه دمیج زدن به این هدف (مثلا هوایی نبودن) را دارد یا خیر
                if (!CanAttackTarget(enemy)) continue;

                if (enemy.TryGetComponent<Unit>(out Unit unit))
                {
                    unit.TakeDamage(data.attackDamage);
                }
                else if (enemy.TryGetComponent<Castle>(out Castle castle))
                {
                    castle.TakeDamage(data.attackDamage);
                }
            }
        }
    }
}