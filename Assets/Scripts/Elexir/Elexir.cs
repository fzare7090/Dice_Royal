using TMPro;
using UnityEngine;

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
        RefreshUI();
    }


    void Update()
    {
        RefreshUI();
    }

    public int GetElexir()
    {
        return currentElexir;
    }

    public int GetMaxElexir()
    {
        return maxElexir;
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
        {
            currentElexir -= AmountRequired;
            RefreshUI();
        }
    }

    public void GettingElexir(int addingValue)
    {
        currentElexir += addingValue;
        if (currentElexir > maxElexir)
        {
            currentElexir = maxElexir;
        }

        RefreshUI();
    }

    public void RunningOutElexirNotice()
    {

        

    }

    private void RefreshUI()
    {
        if (elexirText != null)
            elexirText.text = currentElexir.ToString();
    }


}
