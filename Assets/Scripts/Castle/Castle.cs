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
    [SerializeField] private GameObject area;
    [SerializeField] private Collider castleCollider;

    private List<Transform> enemies;
    private Transform attackTarget;
    private float currentHP;
    public bool exist = true;


    private float attackTimer = 0f;

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

            if (attackTimer > 0)
            {
                attackTimer -= Time.deltaTime;
            }

            CheckForEnemyInRange();
            yield return null;
        }
    }

    public Collider GetCastleCollider()
    {
        return castleCollider;
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
        AudioManager.Instance.PlayDistroyCastleEffect();
        Destroy(gameObject);
        exist = false;
    }

    public Castle GetCastleHP()
    {
        if (!exist)
        {
            return null;
        }
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


        if (attackTarget != null)
        {
            if (!attackTarget.gameObject.activeInHierarchy ||
                Vector3.Distance(transform.localPosition, attackTarget.localPosition) > data.attackRange)
            {
                attackTarget = null;
            }
        }


        if (attackTarget == null)
        {
            foreach (Transform enemy in enemies)
            {
                if (enemy == null) continue;

                float distance = Vector3.Distance(transform.localPosition, enemy.localPosition);

                if (distance <= data.attackRange)
                {
                    attackTarget = enemy;
                    break;
                }
            }
        }


        if (attackTarget != null && attackTimer <= 0f)
        {

            FireProjectile(attackTarget);

            attackTimer = data.attackCooldown;
        }
    }

    private void FireProjectile(Transform target)
    {
        GameObject spawnedProjectile = Instantiate(
            data.projectilePrefab,
            transform.position,
            Quaternion.identity,
            transform
        );
        AudioManager.Instance.PlayProjectileEffect();

        if (spawnedProjectile.TryGetComponent<Image>(out var image))
        {
            image.color = (teamId == 1) ? Color.green : Color.red;
        }


        StartCoroutine(MoveProjectile(spawnedProjectile, target));
    }


    private IEnumerator MoveProjectile(GameObject projectile, Transform target)
    {
        while (projectile != null && target != null &&
               Vector3.Distance(projectile.transform.position, target.position) > 0.08f)
        {
            projectile.transform.position = Vector3.MoveTowards(
                projectile.transform.position,
                target.position,
                data.attackSpeed * Time.deltaTime
            );
            yield return null;
        }

        if (projectile != null)
        {
            Destroy(projectile);


            if (target != null)
            {
                if (target.TryGetComponent<Unit>(out Unit targetUnit))
                {
                    targetUnit.TakeDamage(data.attackDamage);
                }
                else if (target.TryGetComponent<Castle>(out Castle targetCastle))
                {
                    targetCastle.TakeDamage(data.attackDamage);
                }
            }
        }
    }

    public void SetCastleArea(PrinceCastleConfig princeCastle)
    {

    }
}
