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
    [SerializeField] private AreaManager areaManager;
    [SerializeField] private Area myArea;
    [SerializeField] private GameObject visualRoot;


    private List<Transform> enemies;
    private Transform attackTarget;
    private float currentHP;
    private float pendingDamage = 0f;
    private float maxHP;
    public bool exist = true;

    private float attackTimer = 0f;

    private Dictionary<GameObject, Transform> activeProjectiles = new Dictionary<GameObject, Transform>();

    private void Start()
    {
        if (data != null)
        {
            maxHP = data.hp;
            currentHP = data.hp;
            UpdateHPText();
            StartCoroutine(CastleLoop());
        }
        else
        {
            maxHP = 100;
            currentHP = 100;
            UpdateHPText();
        }

        if (myArea == null)
        {
            myArea = GetComponent<Area>();
            if (myArea == null && area != null)
            {
                myArea = area.GetComponent<Area>();
            }
            if (myArea == null)
            {
                myArea = GetComponentInChildren<Area>();
            }
        }
    }
    public void ApplyUpgrade(float newMaxHP, float newDamage, float newAttackSpeed)
    {
        float hpPercent = currentHP / maxHP;
        maxHP = newMaxHP;
        currentHP = maxHP * hpPercent;
        data.attackDamage = newDamage;
        data.attackSpeed = newAttackSpeed;
        UpdateHPText();
    }


    public float GetCurrentHPPercent()
    {
        return currentHP / maxHP;
    }


    public CastleData GetData()
    {
        return data;
    }

    private IEnumerator CastleLoop()
    {
        while (true)
        {
            if (!exist)
                yield break;

            if (attackTimer > 0)
                attackTimer -= Time.deltaTime;

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
        TakeDamage((float)damage);
    }

    public void TakeDamage(float damage)
    {
        if (!exist) return;
        currentHP -= damage;
        pendingDamage -= damage;
        if (pendingDamage < 0) pendingDamage = 0;

        UpdateHPText();
        if (currentHP <= 0)
        {
            Collapse();
        }
    }

    public void AddPendingDamage(float damage)
    {
        pendingDamage += damage;
    }

    public float GetEffectiveHP()
    {
        if (!exist)
            return 0;

        return currentHP - pendingDamage;
    }

    void Collapse()
    {
        if (!exist)
            return;

        exist = false;
        currentHP = 0;

        StopAllCoroutines();

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayDistroyCastleEffect();


        if (CastleDestroyUI.Instance != null)
        {
            CastleDestroyUI.Instance.ShowCastleDestroyed();
        }


        if (myArea != null && areaManager != null)
        {
            int enemyTeamId = (teamId == 0) ? 1 : 0;
            areaManager.ChangeAreaTeam(myArea, enemyTeamId);
        }

        if (castleCollider != null)
            castleCollider.enabled = false;

        if (visualRoot != null)
            visualRoot.SetActive(false);
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

    public void ClearCombat()
    {
        StopAllCoroutines();
        foreach (var kvp in activeProjectiles)
        {
            if (kvp.Key != null) Destroy(kvp.Key);
        }
        activeProjectiles.Clear();
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

    private float GetTargetEffectiveHP(Transform target)
    {
        if (target == null) return 0;
        if (target.TryGetComponent<Unit>(out Unit unit)) return unit.GetEffectiveHP();
        if (target.TryGetComponent<Castle>(out Castle castle)) return castle.GetEffectiveHP();
        return 1;
    }

    private void AddTargetPendingDamage(Transform target, float damage)
    {
        if (target == null) return;
        if (target.TryGetComponent<Unit>(out Unit unit)) unit.AddPendingDamage(damage);
        else if (target.TryGetComponent<Castle>(out Castle castle)) castle.AddPendingDamage(damage);
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
                Vector3.Distance(transform.localPosition, attackTarget.localPosition) > data.attackRange ||
                GetTargetEffectiveHP(attackTarget) <= 0)
            {
                attackTarget = null;
            }
        }


        if (attackTarget == null)
        {
            Transform closestEnemy = null;
            float closestDistance = float.MaxValue;

            foreach (Transform enemy in enemies)
            {
                if (enemy == null || !enemy.gameObject.activeInHierarchy || GetTargetEffectiveHP(enemy) <= 0) continue;


                float distance = Vector3.Distance(transform.localPosition, enemy.localPosition);

                if (distance <= data.attackRange && distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = enemy;
                }
            }

            attackTarget = closestEnemy;
        }


        if (attackTarget != null && attackTimer <= 0f)
        {
            AddTargetPendingDamage(attackTarget, data.attackDamage);

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

        GameObject projRef = projectile;
        activeProjectiles[projRef] = target;

        if (target == null)
        {
            activeProjectiles.Remove(projRef);
            yield break;
        }

        Vector3 targetPosition = target.position;


        while (projectile != null && Vector3.Distance(projectile.transform.position, targetPosition) > 0.08f)
        {

            if (target != null)
            {
                targetPosition = target.position;
            }

            projectile.transform.position = Vector3.MoveTowards(
                projectile.transform.position,
                targetPosition,
                data.attackSpeed * Time.deltaTime
            );
            yield return null;
        }

        activeProjectiles.Remove(projRef);
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
                    if (targetCastle.exist)
                    {
                        targetCastle.TakeDamage(data.attackDamage);
                    }
                }
            }
        }
    }

    private void OnDestroy()
    {
        if (data == null) return;
        foreach (var kvp in activeProjectiles)
        {
            if (kvp.Value != null)
            {
                if (kvp.Value.TryGetComponent<Unit>(out Unit u)) u.AddPendingDamage(-data.attackDamage);
                else if (kvp.Value.TryGetComponent<Castle>(out Castle c)) c.AddPendingDamage(-data.attackDamage);
            }
        }
        activeProjectiles.Clear();
    }
}