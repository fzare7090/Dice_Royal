using UnityEngine;

public class Giant : Unit
{
    // بازنویسی متد پیدا کردن دشمن تا فقط قلعه‌ها را پیدا کند
    protected override void CheckForEnemyInRange()
    {
        GiantData giantData = (GiantData)data;
        if (enemies == null) return;
        Transform closestCastle = null;
        float closestDistance = float.MaxValue;

        foreach (Transform enemy in enemies)
        {
            if (enemy == null) continue;

            // -------- تفاوت اصلی اینجاست --------
            // اگر هدف قلعه (Castle) نبود، آن را نادیده بگیر و به سراغ بعدی برو
            if (!enemy.TryGetComponent<Castle>(out _))
                continue;
            // ------------------------------------

            float distance = GetDistanceToTarget(enemy);
            if (distance <= giantData.enemyFindingRange && distance < closestDistance)
            {
                closestDistance = distance;
                closestCastle = enemy;
            }
        }

        // ادامه منطق مانند کلاس اصلی (Unit) اما این بار فقط برای قلعه‌ها
        if (closestCastle != null)
        {
            if (attackTarget != closestCastle)
            {
                attackTarget = closestCastle;
                if (closestDistance <= giantData.attackRange)
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
}
