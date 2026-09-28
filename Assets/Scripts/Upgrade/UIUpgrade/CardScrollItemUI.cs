using TMPro;
using UnityEngine;
using UnityEngine.UI;
using RTLTMPro;
public class CardScrollItemUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private RTLTextMeshPro cardNameText;
    [SerializeField] private RTLTextMeshPro levelText;
    [SerializeField] private Image cardIcon;
    [SerializeField] private Button button;

    private UnitData unitData;

    public void Initialize(
        UnitData unit,
        CardData cardData,
        CardDetailsPanel detailsPanel)
    {
        //unitData = unit;

        //cardNameText.text = unit.name;
        levelText.text = $"Lv.{cardData.level}";

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            //Debug.Log("Card Clicked");
            detailsPanel.ShowCard(unit, cardData);
        });
    }

    public void SetData(UnitData data)
    {
        unitData = data;

        //Debug.Log("unitData = " + unitData);

        if (cardNameText == null)
            //Debug.LogError("cardName NULL");

        if (cardIcon == null)
           // Debug.LogError("cardImage NULL");

        //if (unitData == null)
            Debug.LogError("unitData NULL");

        cardNameText.text = unitData.name;
        if (cardIcon == null)
        {
            cardIcon = GetComponentInChildren<Image>(true);
        }

        //Debug.Log("Found Image = " + cardIcon);

        cardIcon.sprite = unitData.cardIcon;
    }

    public void Refresh(CardData cardData)
    {
        levelText.text = $"Lv.{cardData.level}";
    }
}
