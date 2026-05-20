using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Wizard : Unit
{
    protected override IEnumerator AttackRoutine()
    {
        WizardData wizardData = (WizardData)data;
        currentState = UnitState.Attacking;

        while (attackTarget != null &&
               Vector3.Distance(transform.localPosition, attackTarget.localPosition) <= wizardData.attackRange)
        {

            GameObject fire = Instantiate(
                wizardData.firePrefab,
                transform.position,
                Quaternion.identity,
                transform
            );
            AudioManager.Instance.PlayFireEffect();

            Image img = fire.GetComponent<Image>();
            img.color = teamId == 1 ? Color.green : Color.red;


            while (attackTarget != null &&
                   Vector3.Distance(fire.transform.position, attackTarget.position) > 2f)
            {
                fire.transform.position = Vector3.MoveTowards(
                    fire.transform.position,
                    attackTarget.position,
                    wizardData.attackSpeed * Time.deltaTime
                );

                yield return null;
            }

            Destroy(fire);


            DealDamage();


            if (attackTarget == null ||
                Vector3.Distance(transform.localPosition, attackTarget.localPosition) > wizardData.attackRange)
            {
                break;
            }


            yield return new WaitForSeconds(wizardData.attackCooldown);
        }


        if (attackTarget != null &&
            Vector3.Distance(transform.localPosition, attackTarget.localPosition) <= data.enemyFindingRange)
        {

            currentState = UnitState.MovingToEnemy;
            SetEnemyPath();
        }
        else
        {

            ReturnToOriginalPath();
        }
    }
}
