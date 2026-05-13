using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class Unit : MonoBehaviour
{
    [SerializeField] private int teamId;
    [SerializeField] private UnitData data;
    [SerializeField] private TMP_Text hpText;
    [SerializeField] private GameObject towerManager;


    private List<Transform> enemies;
    private Transform attackTarget;

    private List<Vector3> pathPoints = new List<Vector3>();
    int currentPoint = 0;
    private Vector3 minBoard = new Vector3(0, -1544, 0);
    private Vector3 maxBoard = new Vector3(0, 1544, 0);
    private Vector3 minRightBridge = new Vector3(400, -200, 0);
    private Vector3 maxRightBridge = new Vector3(400, 200, 0);
    private Vector3 minLeftBridge = new Vector3(-400, -200, 0);
    private Vector3 maxLeftBridge = new Vector3(-400, 200, 0);
    private float midelBoardX = 0;

    private float currentHP;

    private enum UnitState
    {
        Moving,
        Attacking
    }

    private UnitState currentState;

    void Start()
    {
        if (data != null)
        {
            InitializeUnit();
        }
    }

    public void SetData(UnitData newData)
    {
        data = newData;

    }

    private void InitializeUnit()
    {
        currentHP = data.hp;
        UpdateHPText();
        SetPath();
        currentState = UnitState.Moving;
        StartCoroutine(MoveUnit());

    }
    public int GetElexir()
    {
        return data.elexir;
    }
    void UpdateHPText()
    {
        hpText.text = currentHP.ToString();
    }

    public int GetTeamId()
    {
        return teamId;
    }
    public void SetTeamId(int id)
    {
        teamId = id;
    }
    public Vector3 GetMinBoard()
    {
        return minBoard;
    }
    public Vector3 GetMaxBoard()
    {
        return maxBoard;
    }
    public void SetEnemies(List<Transform> newEnemies)
    {
        enemies = newEnemies;
    }
    public float GetCurrentHP()
    {
        return currentHP;
    }

    private IEnumerator MoveUnit()
    {
        while (true)
        {
            if (currentState == UnitState.Moving)
            {
                CheckForEnemyInRange();
                if (currentState == UnitState.Moving)
                    MoveForward();
            }
            yield return null;
        }
    }

    private void MoveForward()
    {
        if (currentPoint >= pathPoints.Count)
        {
            return;
        }
        transform.localPosition = Vector3.MoveTowards(
            transform.localPosition,
            pathPoints[currentPoint],
            data.moveSpeed * Time.deltaTime
        );
        if (Vector3.Distance(transform.localPosition, pathPoints[currentPoint]) < 0.1f)
        {
            currentPoint++;
        }
    }
    private void SetPath()
    {
        pathPoints.Clear();
        currentPoint = 0;
        TowersPosition towerPosition = towerManager.GetComponent<TowersPosition>();
        Vector3 targetPos1;
        Vector3 targetPos2;
        Vector3 targetPos3;
        Vector3 targetPos4;
        if (teamId == 1)
        {
            if (transform.localPosition.x > midelBoardX)
            {
                targetPos1 = minRightBridge;
                targetPos2 = maxRightBridge;
                targetPos3 = towerPosition.GetRightPrincessTowerP2();
                targetPos4 = towerPosition.GetKingTowerP2();
            }
            else
            {
                targetPos1 = minLeftBridge;
                targetPos2 = maxLeftBridge;
                targetPos3 = towerPosition.GetLeftPrincessTowerP2();
                targetPos4 = towerPosition.GetKingTowerP2();
            }
        }
        else
        {
            if (transform.localPosition.x > midelBoardX)
            {
                targetPos1 = maxRightBridge;
                targetPos2 = minRightBridge;
                targetPos3 = towerPosition.GetRightPrincessTowerP1();
                targetPos4 = towerPosition.GetKingTowerP1();
            }
            else
            {
                targetPos1 = maxLeftBridge;
                targetPos2 = minLeftBridge;
                targetPos3 = towerPosition.GetLeftPrincessTowerP1();
                targetPos4 = towerPosition.GetKingTowerP1();
            }
        }
        pathPoints.Add(targetPos1);
        pathPoints.Add(targetPos2);
        pathPoints.Add(targetPos3);
        pathPoints.Add(targetPos4);
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
        currentState = UnitState.Attacking;

        while (attackTarget != null &&
               Vector3.Distance(transform.localPosition, attackTarget.localPosition) <= data.attackRange)
        {
            GameObject spawnedFire = Instantiate(
            data.firePrefab,
            transform.position,
            Quaternion.identity,
            transform
            );
            Image image = spawnedFire.transform.GetComponent<Image>();
            if (teamId == 1)
            {
                image.color = Color.green;
            }
            else
            {
                image.color = Color.red;
            }
            while (attackTarget != null &&
                    Vector3.Distance(spawnedFire.transform.position, attackTarget.position) > 0.1f)
            {
                spawnedFire.transform.position = Vector3.MoveTowards(
                    spawnedFire.transform.position,
                    attackTarget.position,
                    data.attackSpeed * Time.deltaTime
                );
                yield return null;
            }
            Destroy(spawnedFire);
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
        currentState = UnitState.Moving;
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
