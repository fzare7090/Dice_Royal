
using TMPro;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private TMP_Text coinText;
    [SerializeField] private PlayerResourceData playerResource;

    [SerializeField] private int maxCoin = 999999;
    [SerializeField] private int currentCoin = 0;

    private void Start()
    {
        if (playerResource != null)
            currentCoin = playerResource.GetCoin();

        UpdateCoinUI();

    }


    private void Update()
    {
        UpdateCoinUI();
    }
    public int GetCoin()
    {
        return playerResource != null ? playerResource.GetCoin() : currentCoin;
    }

    public void SetCoin(int newValue)
    {
        currentCoin = Mathf.Clamp(newValue, 0, maxCoin);
        if (playerResource != null)
            playerResource.SetCoin(currentCoin);

        UpdateCoinUI();
    }

    public void AddCoin(int amount)
    {
        if (amount <= 0) return;

        int currentValue = GetCoin();
        currentCoin = Mathf.Min(currentValue + amount, maxCoin);
        if (playerResource != null)
            playerResource.SetCoin(currentCoin);

        UpdateCoinUI();
    }

    public bool SpendCoin(int amount)
    {
        int currentValue = GetCoin();

        if (currentValue < amount)
            return false;

        currentCoin = currentValue - amount;
        if (playerResource != null)
            playerResource.SetCoin(currentCoin);

        UpdateCoinUI();
        return true;
    }

    private void UpdateCoinUI()
    {
        if (coinText != null)
            coinText.text = GetCoin().ToString();
    }

    public void SetCoinText()
    {
        coinText.SetText($"coins:{coinText} \n");
    }
}


