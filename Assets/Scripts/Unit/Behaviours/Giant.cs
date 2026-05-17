using UnityEngine;

public class Giant : Unit
{

   
    protected override void CheckForEnemyInRange()
    {
        GiantData giantData = (GiantData)data;
        if (enemies == null) return;

        foreach (Transform enemy in enemies)
        {
            if (enemy == null) continue;
            
            
            
                float distance = Vector3.Distance(
                transform.localPosition,
                enemy.localPosition
            );

            if (distance <= giantData.attackRange && attackTarget == null)
            {
                attackTarget = enemy;
                if (attackTarget.TryGetComponent<Castle>(out Castle targetCastle))
                {
                    StartCoroutine(AttackUnit());
                }
                break;
            }
            
            
        }
    }

}
