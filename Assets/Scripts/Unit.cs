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


    private List<Unit> enemyUnits;
    private Unit attackTarget;

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
        // InitializeUnit();
    }

    private void InitializeUnit()
    {
        currentHP = data.hp;
        UpdateHPText();
        SetPath();
        currentState = UnitState.Moving;
        StartCoroutine(MoveUnit());

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
    public void SetEnemies(List<Unit> enemies)
    {
        enemyUnits = enemies;
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
        Vector3 targetPos1;
        Vector3 targetPos2;
        Vector3 targetPos3;
        if (teamId == 1)
        {
            if (transform.localPosition.x > midelBoardX)
            {
                targetPos1 = minRightBridge;
                targetPos2 = maxRightBridge;
                targetPos3 = maxBoard;
            }
            else
            {
                targetPos1 = minLeftBridge;
                targetPos2 = maxLeftBridge;
                targetPos3 = maxBoard;
            }

        }
        else
        {
            if (transform.localPosition.x > midelBoardX)
            {
                targetPos1 = maxRightBridge;
                targetPos2 = minRightBridge;
                targetPos3 = minBoard;
            }
            else
            {
                targetPos1 = maxLeftBridge;
                targetPos2 = minLeftBridge;
                targetPos3 = minBoard;
            }
        }
        pathPoints.Add(targetPos1);
        pathPoints.Add(targetPos2);
        pathPoints.Add(targetPos3);

    }


    private void CheckForEnemyInRange()
    {
        if (enemyUnits == null) return;

        foreach (Unit enemy in enemyUnits)
        {
            if (enemy == null) continue;

            float distance = Vector3.Distance(
                transform.localPosition,
                enemy.transform.localPosition
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
               Vector3.Distance(transform.localPosition, attackTarget.transform.localPosition) <= data.attackRange)
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
                    Vector3.Distance(spawnedFire.transform.position, attackTarget.transform.position) > 0.1f)
            {

                spawnedFire.transform.position = Vector3.MoveTowards(
                    spawnedFire.transform.position,
                    attackTarget.transform.position,
                    data.attackSpeed * Time.deltaTime
                );
                yield return null;
            }
            Destroy(spawnedFire);

            if (attackTarget != null)
            {
                attackTarget.TakeDamage(data.attackDamage);
                break;
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
