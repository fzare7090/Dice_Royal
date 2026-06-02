using UnityEngine;

public class Giant : Unit
{
    // بازنویسی متد پیدا کردن دشمن تا فقط قلعه‌ها را پیدا کند
    protected override void CheckForEnemyInRange()
    {
        GiantData giantData = (GiantData)data;
        if (enemies == null) return;

        // پاکسازی تارگت در صورتی که قلعه نابود شده باشد
        if (attackTarget != null && (!attackTarget.gameObject.activeInHierarchy || GetTargetEffectiveHP(attackTarget) <= 0))
        {
            attackTarget = null;
        }

        // اگر در حال اتک است و هدف هنوز وجود دارد، اجازه بده کارش را بکند
        if (currentState == UnitState.Attacking && attackTarget != null)
        {
            float dist = GetDistanceToTarget(attackTarget);
            if (dist > giantData.attackRange)
            {
                currentState = UnitState.MovingToEnemy;
                ChaseEnemyModified(); // استفاده از تعقیب اصلاح شده
            }
            return;
        }

        Transform closestCastle = null;
        float closestDistance = float.MaxValue;

        foreach (Transform enemy in enemies)
        {
            if (enemy == null || !enemy.gameObject.activeInHierarchy || GetTargetEffectiveHP(enemy) <= 0) continue;

            if (!enemy.TryGetComponent<Castle>(out _))
                continue;

            float distance = GetDistanceToTarget(enemy);
            if (distance <= giantData.enemyFindingRange && distance < closestDistance)
            {
                closestDistance = distance;
                closestCastle = enemy;
            }
        }

        if (closestCastle != null)
        {
            // آپدیت کردن تارگت در صورت پیدا کردن قلعه جدید
            if (attackTarget != closestCastle)
            {
                attackTarget = closestCastle;
            }

            // چک کردن رنج اتک (مستقل از اینکه تارگت جدید است یا قدیمی)
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
                    // آپدیت مداوم مقصد مستقیماً به سمت مرکز قلعه (حل مشکل گوشه رفتن)
                    agent.SetDestination(attackTarget.position);
                    Debug.Log("Updating destination to castle center: " + attackTarget.position);
                }
            }
        }
        else
        {
            // اگر قلعه‌ای نبود، برگردد به مسیر قبلی
            attackTarget = null;
            if (currentState != UnitState.Moving)
            {
                ReturnToOriginalPath();
            }
        }
    }

    // یک متد اختصاصی برای جاینت تا مستقیم به مرکز هدف برود
    private void ChaseEnemyModified()
    {
        if (attackTarget == null) return;
        agent.isStopped = false;
        
        // فاصله توقف روی صفر تنظیم می‌شود چون توقف با تابع GetDistanceToTarget کنترل می‌شود
        agent.stoppingDistance = 0f; 

        // حرکت مستقیم به سمت مرکز هدف
        agent.SetDestination(attackTarget.position);
    }
}