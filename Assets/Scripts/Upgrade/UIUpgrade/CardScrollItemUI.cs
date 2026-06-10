using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardScrollItemUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text cardNameText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private Image cardIcon;
    [SerializeField] private Button button;

    private UnitData unitData;

    public void Initialize(
        UnitData unit,
        CardData cardData,
        CardDetailsPanel detailsPanel)
    {
        unitData = unit;

        cardNameText.text = unit.name;
        levelText.text = $"Lv.{cardData.level}";

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            Debug.Log("Card Clicked");
            button.onClick.AddListener(() =>
            {
                detailsPanel.gameObject.SetActive(true);
                detailsPanel.ShowCard(unit, cardData);
            });
            detailsPanel.ShowCard(unit, cardData);
        });
    }

    public void Refresh(CardData cardData)
    {
        levelText.text = $"Lv.{cardData.level}";
    }
}