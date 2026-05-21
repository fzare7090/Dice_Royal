
using TMPro;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private TMP_Text coinText;

    [SerializeField] private int maxCoin = 999999;
    [SerializeField] private int currentCoin = 0;

    private void Start()
    {
        UpdateCoinUI();
    }

    public int GetCoin()
    {
        return currentCoin;
    }

    public void SetCoin(int newValue)
    {
        currentCoin = Mathf.Clamp(newValue, 0, maxCoin);
        UpdateCoinUI();
    }

    public void AddCoin(int amount)
    {
        if (amount <= 0) return;

        currentCoin = Mathf.Min(currentCoin + amount, maxCoin);
        UpdateCoinUI();
    }

    public bool SpendCoin(int amount)
    {
        if (amount <= 0) return false;

        if (currentCoin < amount)
            return false;

        currentCoin -= amount;
        UpdateCoinUI();
        return true;
    }

    private void UpdateCoinUI()
    {
        if (coinText != null)
            coinText.text = currentCoin.ToString();
    }

    public void SetCoinText()
    {
        coinText.SetText($"coins:{coinText} \n");
    }
}


