using UnityEngine;
using UnityEngine.UI;
using RTLTMPro;
using System;

public class CardUI : MonoBehaviour
{
    [SerializeField] private RTLTextMeshPro cardName;
    [SerializeField] private Button cardButton;

    private UnitData unitData;

    public Action<CardUI> OnCardClicked;

    void Start()
    {
        cardButton.onClick.AddListener(Click);
        Initialize();
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
    }

    private void Initialize()
    {
        cardName.text = unitData.name;
    }

    public UnitData GetData()
    {
        return unitData;
    }
    
}
