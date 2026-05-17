using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardButtonUI : MonoBehaviour
{
    [Header("UI Elements")]
    // [SerializeField] private Image cardIcon;        
    [SerializeField] private TMP_Text ElexirText; 
    [SerializeField] private TMP_Text cardName; 
    [SerializeField] private Button button;         

    
    public void SetCard(UnitData data)
    {
        
        if (ElexirText != null)
        {
            ElexirText.text = data.elexir.ToString();
        }
        if (cardName != null)
        {
            cardName.text = data.name;
        }

    }

    public Button GetButton()
    {
        return button;
    }
}
