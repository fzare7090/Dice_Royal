using UnityEngine;
using UnityEngine.UI;
using RTLTMPro;
using System;

public class CardUI : MonoBehaviour
{
    [SerializeField] private RTLTextMeshPro cardName;
    [SerializeField] private Button cardButton;
    [SerializeField] private Image cardImage;

    private UnitData unitData;

    public Action<CardUI> OnCardClicked;

    void Start()
    {
        cardButton.onClick.AddListener(Click);
    }

    private void Awake()
    {
       // Debug.Log("CardUI Object = " + gameObject.name);

        //Debug.Log("cardName = " + cardName);

        //Debug.Log("cardImage = " + cardImage);

        //Debug.Log("All Images Count = " + GetComponentsInChildren<Image>(true).Length);
    }
    void Click()
    {
        if (OnCardClicked != null)
        {
            OnCardClicked.Invoke(this);
        }
    }

    public void SetData(UnitData data)
    {
        unitData = data;

        //Debug.Log("unitData = " + unitData);

        if (cardName == null)
            Debug.LogError("cardName NULL");

        if (cardImage == null)
            //Debug.LogError("cardImage NULL");

        if (unitData == null)
            Debug.LogError("unitData NULL");

        cardName.text = unitData.name;
        if (cardImage == null)
        {
            cardImage = GetComponentInChildren<Image>(true);
        }

        Debug.Log("Found Image = " + cardImage);

        cardImage.sprite = unitData.cardIcon;
    }

    public UnitData GetData()
    {
        return unitData;
    }
    
}
