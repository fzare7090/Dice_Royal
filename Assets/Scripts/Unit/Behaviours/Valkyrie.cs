using UnityEngine;
using System.Collections;

public class Valkyrie : Unit
{
    protected override IEnumerator AttackRoutine()
    {
        // تبدیل دیتای کلی به دیتای والکری برای دسترسی به areaOfEffectRadius
        ValkyrieData valData = data as ValkyrieData;

        agent.isStopped = true;

        while (attackTarget != null && GetDistanceToTarget(attackTarget) <= data.attackRange)
        {
            // انجام عملیات آسیب رسانی ناحیه‌ای
            DealAreaDamage(valData);

            // اینجا می‌توانید افکت چرخش یا انیمیشن را فعال کنید
            // Debug.Log("Valkyrie is spinning and dealing AoE damage!");

            if (attackTarget == null)
                break;

            yield return new WaitForSeconds(data.attackCooldown);

            // بررسی مجدد: آیا هدف هنوز زنده است و در محدوده است؟
            if (attackTarget == null || GetDistanceToTarget(attackTarget) > data.attackRange)
            {
                break;
            }
        }

        agent.isStopped = false;

        // برگشت به مسیر اصلی اگر دشمنی نمانده
        if (attackTarget == null || GetTargetEffectiveHP(attackTarget) <= 0)
        {
            ReturnToOriginalPath();
        }
        else
        {
            // اگر هنوز هدفی وجود دارد اما در رنج نیست، تعقیب کن
            currentState = UnitState.MovingToEnemy;
            ChaseEnemy();
        }
    }

    private void DealAreaDamage(ValkyrieData valData)
    {
        float radius = (valData != null) ? valData.areaOfEffectRadius : 1.5f;

        // لیست دشمنان در کلاس Unit وجود دارد (protected List<Transform> enemies)
        // ما از همین لیست استفاده می‌کنیم تا بهینه باشد
        if (enemies == null) return;

        foreach (Transform enemy in enemies)
        {
            if (enemy == null || !enemy.gameObject.activeInHierarchy) continue;

            // محاسبه فاصله تا این دشمن خاص
            float distance = Vector3.Distance(transform.position, enemy.position);

            // اگر دشمن در شعاع دایره چرخش والکری است
            if (distance <= radius)
            {
                // آسیب زدن
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