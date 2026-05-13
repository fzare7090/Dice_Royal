using TMPro;
using UnityEngine;

public class CastleHp : MonoBehaviour
{


    int maxHP = 100;
    int currentHP;
    public bool exist = true;


    private void Start()
    {
        currentHP = maxHP;
    }

    public int GetHPCastle()
    {
        return currentHP;
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;

        if (currentHP <= 0)
        {
            Collapse();
        }
    }

    void Collapse(){

        Destroy (gameObject); 
        exist = false;

    }

  
}
