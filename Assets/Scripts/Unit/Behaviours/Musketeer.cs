using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Musketeer : Unit
{
    protected override IEnumerator AttackRoutine()
    {
        MusketeerData muskyData = data as MusketeerData;

        agent.isStopped = true;

        while (attackTarget != null &&
               GetDistanceToTarget(attackTarget) <= data.attackRange)
        {
            if (GetTargetEffectiveHP(attackTarget) <= 0)
            {
                break;
            }

            if (muskyData != null && muskyData.bulletPrefab != null)
            {
                FireBullet(muskyData, attackTarget);
            }
            else
            {
                // Fallback to instant damage if no bullet prefab
                DealDamage();
            }

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

        if (attackTarget == null || GetTargetEffectiveHP(attackTarget) <= 0)
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

    private void FireBullet(MusketeerData muskyData, Transform target)
    {
        GameObject bullet = Instantiate(
            muskyData.bulletPrefab,
            transform.position,
            Quaternion.identity,
            transform
        );
        bullet.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);

        Image img = bullet.GetComponent<Image>();
        if (img != null)
        {
            img.color = teamId == 1 ? Color.cyan : Color.magenta; 
        }

        // Apply pending damage explicitly
        AddTargetPendingDamage(target, data.attackDamage);

        StartCoroutine(MoveBullet(bullet, target, muskyData));
    }

    private IEnumerator MoveBullet(GameObject bullet, Transform target, MusketeerData muskyData)
    {
        float speed = muskyData.attackSpeed;

        while (bullet != null && target != null)
        {
            // حرکت مستقیم به سمت مرکز هدف برای جلوگیری از اصابت به گوشه‌ها
            Vector3 targetPos = target.position; 
            
            bullet.transform.position = Vector3.MoveTowards(
                bullet.transform.position,
                targetPos,
                speed * Time.deltaTime
            );

            if (Vector3.Distance(bullet.transform.position, targetPos) < 0.1f)
            {
                if (GetTargetEffectiveHP(target) > -data.attackDamage) // Check if valid
                {
                    if (target.TryGetComponent<Unit>(out Unit tUnit))
                        tUnit.TakeDamage(data.attackDamage);
                    else if (target.TryGetComponent<Castle>(out Castle tCastle))
                        tCastle.TakeDamage(data.attackDamage);
                }
                Destroy(bullet);
                break;
            }
            yield return null;
        }

        if (bullet != null)
        {
            Destroy(bullet);
        }
    }
}