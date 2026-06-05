using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Wizard : Unit
{
    private Dictionary<GameObject, List<Transform>> activeProjectiles = new Dictionary<GameObject, List<Transform>>();

    protected override IEnumerator AttackRoutine()
    {
        WizardData wizardData = (WizardData)data;
        agent.isStopped = true;

        while (attackTarget != null &&
               CanAttackTarget(attackTarget) &&
               GetDistanceToTarget(attackTarget) <= data.attackRange)
        {
            if (GetTargetEffectiveHP(attackTarget) <= 0)
            {
                break;
            }

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

        Vector3 predictedImpactPos = target.position;
        Collider[] collidersInRange = Physics.OverlapSphere(predictedImpactPos, wizardData.fireRange);

        List<Transform> pendingTargets = new List<Transform>();

        foreach (Collider col in collidersInRange)
        {
            if (col.TryGetComponent<Unit>(out Unit hitUnit) && hitUnit.GetTeamId() != this.teamId)
            {
                if (hitUnit.GetEffectiveHP() > 0 && CanAttackTarget(hitUnit.transform))
                {
                    hitUnit.AddPendingDamage(data.attackDamage);
                    pendingTargets.Add(hitUnit.transform);
                }
            }
            else if (col.TryGetComponent<Castle>(out Castle hitCastle) && hitCastle.GetTeamId() != this.teamId)
            {
                if (hitCastle.GetEffectiveHP() > 0 && CanAttackTarget(hitCastle.transform))
                {
                    hitCastle.AddPendingDamage(data.attackDamage);
                    pendingTargets.Add(hitCastle.transform);
                }
            }
        }

        StartCoroutine(MoveProjectile(fire, target, wizardData, pendingTargets));
    }

    private IEnumerator MoveProjectile(GameObject projectile, Transform target, WizardData wizardData, List<Transform> pendingTargets)
    {
        GameObject projRef = projectile;
        activeProjectiles[projRef] = pendingTargets;

        Vector3 targetPos = target != null ? target.position : Vector3.zero;


        while (projectile != null)
        {
            if (target != null)
            {
                targetPos = target.position;
            }


            if (Vector3.Distance(projectile.transform.position, targetPos) <= 0.08f)
                break;

            projectile.transform.position = Vector3.MoveTowards(
                projectile.transform.position,
                targetPos,
                wizardData.attackSpeed * Time.deltaTime
            );

            yield return null;
        }

        activeProjectiles.Remove(projRef);

        Vector3 impactPos = projectile != null ? projectile.transform.position : targetPos;

        if (projectile != null)
        {
            Destroy(projectile);
        }


        foreach (Transform t in pendingTargets)
        {
            if (t != null)
            {
                if (t.TryGetComponent<Unit>(out Unit u)) u.AddPendingDamage(-data.attackDamage);
                else if (t.TryGetComponent<Castle>(out Castle c)) c.AddPendingDamage(-data.attackDamage);
            }
        }


        Collider[] collidersInRange = Physics.OverlapSphere(impactPos, wizardData.fireRange);

        foreach (Collider col in collidersInRange)
        {
            if (col.TryGetComponent<Unit>(out Unit hitUnit))
            {
                if (hitUnit.GetTeamId() != this.teamId && CanAttackTarget(hitUnit.transform))
                {
                    hitUnit.TakeDamage(data.attackDamage);
                }
            }
            else if (col.TryGetComponent<Castle>(out Castle hitCastle))
            {
                if (hitCastle.GetTeamId() != this.teamId && CanAttackTarget(hitCastle.transform))
                {
                    hitCastle.TakeDamage(data.attackDamage);
                }
            }
        }
    }

    private void OnDestroy()
    {
        if (data == null) return;
        foreach (var kvp in activeProjectiles)
        {
            foreach (Transform t in kvp.Value)
            {
                if (t != null)
                {
                    if (t.TryGetComponent<Unit>(out Unit u)) u.AddPendingDamage(-data.attackDamage);
                    else if (t.TryGetComponent<Castle>(out Castle c)) c.AddPendingDamage(-data.attackDamage);
                }
            }
        }
        activeProjectiles.Clear();
    }
}
