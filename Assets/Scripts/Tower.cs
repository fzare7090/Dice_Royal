using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class Tower : MonoBehaviour
{

    private List<Transform> enemies;
    [SerializeField] private int teamId;
    [SerializeField] private TowerData data;
    [SerializeField] private TMP_Text hpText;

    private Transform attackTarget;
    private float currentHP;

    private void Start()
    {
        if (data != null)
        {
            InitializeUnit();
            StartCoroutine(TowerLoop());
        }
    }

    private IEnumerator TowerLoop()
    {
        while (true)
        {
            CheckForEnemyInRange();
            yield return null;
        }
    }



    public void SetData(TowerData newData)
    {
        data = newData;
    }

    private void InitializeUnit()
    {
        currentHP = data.hp;
        UpdateHPText();
    }
    void UpdateHPText()
    {
        hpText.text = currentHP.ToString();
    }

    public void SetTeamId(int Id)
    {
        teamId = Id;
    }
    public void SetEnemies(List<Transform> newEnemies)
    {
        enemies = newEnemies;
    }
    private void CheckForEnemyInRange()
    {
        if (enemies == null) return;

        foreach (Transform enemy in enemies)
        {
            if (enemy == null) continue;

            float distance = Vector3.Distance(
                transform.localPosition,
                enemy.localPosition
            );

            if (distance <= data.attackRange && attackTarget == null)
            {
                attackTarget = enemy;
                StartCoroutine(AttackUnit());
                break;
            }
        }
    }

    private IEnumerator AttackUnit()
    {
        while (attackTarget != null &&
               Vector3.Distance(transform.localPosition, attackTarget.localPosition) <= data.attackRange)
        {
            GameObject spawnedProjectile = Instantiate(
            data.projectilePrefab,
            transform.position,
            Quaternion.identity,
            transform
            );
            Image image = spawnedProjectile.transform.GetComponent<Image>();
            if (teamId == 1)
            {
                image.color = Color.green;
            }
            else
            {
                image.color = Color.red;
            }
            while (attackTarget != null &&
                    Vector3.Distance(spawnedProjectile.transform.localPosition, attackTarget.localPosition) > 0.1f)
            {
                spawnedProjectile.transform.position = Vector3.MoveTowards(
                    spawnedProjectile.transform.position,
                    attackTarget.position,
                    data.attackSpeed * Time.deltaTime
                );
                yield return null;
            }
            Destroy(spawnedProjectile);
            if (attackTarget != null)
            {
                if (attackTarget.TryGetComponent<Unit>(out Unit targetUnit))
                {
                    targetUnit.TakeDamage(data.attackDamage);
                }
                else if (attackTarget.TryGetComponent<Tower>(out Tower targetTower))
                {
                    targetTower.TakeDamage(data.attackDamage);
                }
            }
            yield return new WaitForSeconds(data.attackCooldown);
        }
        attackTarget = null;
    }

    public void TakeDamage(float damage)
    {
        currentHP -= damage;
        UpdateHPText();
        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }

}

