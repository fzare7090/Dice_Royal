using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour
{
    
    [SerializeField] private int teamId;
    [SerializeField] private float hp ;
    [SerializeField] private float moveSpeed ;
    [SerializeField] private float attackSpeed ;
    [SerializeField] private float attackRange ;
    [SerializeField] private float attackDamage ;
    [SerializeField] private float attackCooldown ;
    [SerializeField] private GameObject firePrefab ;
    private Vector3 minBoard = new Vector3(0,-1544,0);
    private Vector3 maxBoard = new Vector3(0,1544,0);
    
    
    public int GetTeamId()
    {
        return teamId ;
    }
    public void SetTeamId(int id)
    {
        teamId = id;
    }
    public Vector3 GetMinBoard()
    {
        return minBoard ;
    }
    public Vector3 GetMaxBoard()
    {
        return maxBoard ;
    }

    public IEnumerator MoveUnit()
    {
        
        if(teamId == 1)
        {
            while (Vector3.Distance(transform.position , maxBoard) > 0.05f )
            {
                transform.position = Vector3.MoveTowards(transform.position , maxBoard , moveSpeed * Time.deltaTime );
                yield return null;
            }
        }
        if(teamId == 2)
        {
            while (Vector3.Distance(transform.position , minBoard) > 0.05f )
            {
                transform.position = Vector3.MoveTowards(transform.position , minBoard , moveSpeed * Time.deltaTime );
                yield return null;
            }
        }
        yield return null;
    }
    
    private Transform attackTarget ;

    public IEnumerator AttackUnit(Transform target)
    {
        while (CheckAttackCondition(target))
        {
            GameObject spawnedFire = Instantiate(firePrefab, transform.position, Quaternion.identity, transform.parent);


            while (attackTarget != null && Vector3.Distance(spawnedFire.transform.position, attackTarget.position) <= attackRange)
            {
                spawnedFire.transform.position = Vector3.MoveTowards(spawnedFire.transform.position,
                    attackTarget.position,
                    attackSpeed * Time.deltaTime
                );
                yield return null;
            }
            Destroy(spawnedFire);
            yield return new WaitForSeconds(attackCooldown);
        }
        // canAttack = false;
    }


    public bool CheckAttackCondition(Transform target)
    {
        
            if (Vector3.Distance(transform.position, target.position) < attackRange)
            {
                attackTarget = target;
                return true;
            }
        

        attackTarget = null;
        return false;
    }



    // public bool DamageUnit(Unit fire)
    // {
    //     if (Vector3.Distance(transform.position , fire.position) < 0.1f )
    //     {
    //         hp -= 100 ;
    //     }
    // }
    


    

    
}
