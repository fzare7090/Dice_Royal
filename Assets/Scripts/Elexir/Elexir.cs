using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.iOS;

public class Elexir : MonoBehaviour
{

    [SerializeField] private TMP_Text elexirText;

    private int maxElexir = 100;
    private int currentElexir;
    private int extraTimeElexirAdding = 10;
    private int addingElexir = 5;

    void Start()
    {
        currentElexir = maxElexir;

    }
    void Update()
    {
        elexirText.text = currentElexir.ToString();
    }



    public int GetElexir()
    {
        return currentElexir;
    }

    public int GetAddingElexir()
    {
        return addingElexir;
    }
    public int GetExtraTimeElexirAdding()
    {
        return extraTimeElexirAdding;
    }

    public void UsingElexir(int AmountRequired)
    {
        if (currentElexir - AmountRequired >= 0)
            currentElexir -= AmountRequired;
    }

    public void GettingElexir(int addingValue)
    {

        if (currentElexir < maxElexir)
        {
            currentElexir += addingValue;

        }

    }

    public void RunningOutElexirNotice()
    {

        //notice 

    }


}
