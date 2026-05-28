using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Wizard : Unit
{
    protected override IEnumerator AttackRoutine()
    {
        WizardData wizardData = (WizardData)data;
        agent.isStopped = true;

        while (attackTarget != null &&
               GetDistanceToTarget(attackTarget) <= data.attackRange)
        {

            FireProjectile(wizardData, attackTarget);

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


        if (attackTarget == null)
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

    private void FireProjectile(WizardData wizardData, Transform target)
    {
        GameObject fire = Instantiate(
            wizardData.firePrefab,
            transform.position,
            Quaternion.identity,
            transform
        );
        fire.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

        AudioManager.Instance.PlayFireEffect();

        Image img = fire.GetComponent<Image>();
        if (img != null)
        {
            img.color = teamId == 1 ? Color.green : Color.red;
        }


        StartCoroutine(MoveProjectile(fire, target, wizardData));
    }

    private IEnumerator MoveProjectile(GameObject projectile, Transform target, WizardData wizardData)
    {
        Vector3 targetPos = target != null ? GetClosestPointOnTarget(target) : Vector3.zero;

        while (projectile != null && target != null)
        {
            targetPos = GetClosestPointOnTarget(target);

            if (Vector3.Distance(projectile.transform.position, targetPos) <= 0.07f)
                break;

            projectile.transform.position = Vector3.MoveTowards(
                projectile.transform.position,
                targetPos,
                wizardData.attackSpeed * Time.deltaTime
            );

            yield return null;
        }


        Vector3 impactPos = projectile != null ? projectile.transform.position : targetPos;

        if (projectile != null)
        {
            Destroy(projectile);
        }


        Collider[] collidersInRange = Physics.OverlapSphere(impactPos, wizardData.fireRange);

        foreach (Collider col in collidersInRange)
        {

            if (col.TryGetComponent<Unit>(out Unit hitUnit))
            {
                if (hitUnit.GetTeamId() != this.teamId)
                {
                    hitUnit.TakeDamage(data.attackDamage);
                }
            }

            else if (col.TryGetComponent<Castle>(out Castle hitCastle))
            {
                if (hitCastle.GetTeamId() != this.teamId)
                {
                    hitCastle.TakeDamage(data.attackDamage);
                }
            }
        }
    }
}
