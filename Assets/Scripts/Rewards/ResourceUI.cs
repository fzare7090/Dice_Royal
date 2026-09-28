
using TMPro;
using UnityEngine;

public class ResourceUI : MonoBehaviour
{
    [SerializeField] private PlayerResourceData playerResource;

    [Header("UI")]
    [SerializeField] private TMP_Text coinText;
    [SerializeField] private TMP_Text gemText;

    private void OnEnable()
    {
        playerResource.OnCoinChanged += UpdateCoin;
        playerResource.OnGemChanged += UpdateGem;

        Refresh();
    }

    private void OnDisable()
    {
        playerResource.OnCoinChanged -= UpdateCoin;
        playerResource.OnGemChanged -= UpdateGem;
    }

    private void Refresh()
    {
        UpdateCoin(playerResource.Coin);
        UpdateGem(playerResource.Gem);
    }

    private void UpdateCoin(int value)
    {
        coinText.text = value.ToString();
    }

    private void UpdateGem(int value)
    {
        gemText.text = value.ToString();
    }
}