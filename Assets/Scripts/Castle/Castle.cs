using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class Castle : MonoBehaviour
{
    [SerializeField] private TMP_Text hpText;
    [SerializeField] private int teamId;
    [SerializeField] private CastleData data;

    private List<Transform> enemies;
    private Transform attackTarget;
    private float currentHP;
    public bool exist = true;

    private void Start()
    {
        if (data != null)
        {
            currentHP = data.hp;
            UpdateHPText();
            StartCoroutine(CastleLoop());
        }
        else
        {
            currentHP = 100;
            UpdateHPText();
        }
    }

    private IEnumerator CastleLoop()
    {
        while (true)
        {
            CheckForEnemyInRange();
            yield return null;
        }
    }
    private void Awake()
    {
        hpText = GetComponentInChildren<TMP_Text>();
    }

    public int GetHPCastle()
    {
        return Mathf.RoundToInt(currentHP);
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        UpdateHPText();
        if (currentHP <= 0)
        {
            Collapse();
        }
    }


    public void TakeDamage(float damage)
    {
        currentHP -= damage;
        UpdateHPText();
        if (currentHP <= 0)
        {
            Collapse();
        }
    }

    void Collapse()
    {
        Destroy(gameObject);
        exist = false;
    }

    public Castle GetCastleHP()
    {
        return this;
    }

    public Transform GetCastleTransform()
    {
        return transform;
    }

    private void UpdateHPText()
    {
        if (hpText != null)
            hpText.SetText(Mathf.RoundToInt(currentHP).ToString());
    }


    public void SetData(CastleData newData)
    {
        data = newData;
    }

    public void SetTeamId(int Id)
    {
        teamId = Id;
    }

    public int GetTeamId()
    {
        return teamId;
    }

    public void SetEnemies(List<Transform> newEnemies)
    {
        enemies = newEnemies;
    }

    private void CheckForEnemyInRange()
    {
        if (enemies == null || data == null) return;

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

            if (spawnedProjectile.TryGetComponent<Image>(out var image))
            {
                if (teamId == 1)
                {
                    image.color = Color.green;
                }
                else
                {
                    image.color = Color.red;
                }
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
                else if (attackTarget.TryGetComponent<Castle>(out Castle targetCastle))
                {
                    targetCastle.TakeDamage(data.attackDamage);
                }
            }

            yield return new WaitForSeconds(data.attackCooldown);
        }

        attackTarget = null;
    }
}
