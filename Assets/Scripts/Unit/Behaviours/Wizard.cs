using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Wizard : Unit
{
    
    
    protected override IEnumerator AttackUnit()
    {
        currentState = UnitState.Attacking;
        WizardData wizardData = (WizardData)data;
        while (attackTarget != null &&
               Vector3.Distance(transform.localPosition, attackTarget.localPosition) <= wizardData.attackRange)
        {
            GameObject fire = Instantiate(
                wizardData.firePrefab,
                transform.position,
                Quaternion.identity,
                transform
            );

            Image img = fire.GetComponent<Image>();
            img.color = teamId == 1 ? Color.green : Color.red;

            while (attackTarget != null &&
                   Vector3.Distance(fire.transform.position, attackTarget.position) > 0.1f)
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

            yield return new WaitForSeconds(wizardData.attackCooldown);
        }

        attackTarget = null;
        currentState = UnitState.Moving;
    }
}
