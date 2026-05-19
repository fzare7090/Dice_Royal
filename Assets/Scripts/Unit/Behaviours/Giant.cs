using UnityEngine;

public class Giant : Unit
{
    protected override void CheckForEnemyInRange()
    {
        GiantData giantData = (GiantData)data;
        if (enemies == null) return;

        Transform closestCastle = null;
        float closestDistance = float.MaxValue;

        foreach (Transform enemy in enemies)
        {
            if (enemy == null) continue;

            
            if (!enemy.TryGetComponent<Castle>(out Castle castle)) continue;

            float distance = Vector3.Distance(
                transform.localPosition,
                enemy.localPosition
            );

            if (distance <= giantData.enemyFindingRange && distance < closestDistance)
            {
                closestDistance = distance;
                closestCastle = enemy;
            }
        }

        if (closestCastle != null)
        {
            
            if (attackTarget != closestCastle)
            {
                attackTarget = closestCastle;

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
}
