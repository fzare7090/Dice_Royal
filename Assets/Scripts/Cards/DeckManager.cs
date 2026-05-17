using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class DeckManager : MonoBehaviour
{
    [SerializeField] private CardDataBase cardDataBase;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform content;
    [SerializeField] private List<Transform> slots;
    [SerializeField] private List<UnitData> finalDeck = new List<UnitData>();
    [SerializeField] private Button changeSceneButton;

    private List<CardUI> cardUIs = new List<CardUI>();
    private List<UnitData> allCards;

    void Awake()
    {
        allCards = cardDataBase.allCards;
    }

    void Start()
    {
        SetUpCards();
        changeSceneButton.onClick.AddListener(LoadScene);
    }
    public void LoadScene()
    {
        GameDeckManager.Instance.SetPlayerDeck(finalDeck);
        SceneManager.LoadScene("GameBoard");
    }

    private void SetUpCards()
    {
        for (int i = 0; i < allCards.Count; i++)
        {
            GameObject spawnedCard = Instantiate(cardPrefab, content);

            CardUI cardUI = spawnedCard.GetComponent<CardUI>();
            cardUI.SetData(allCards[i]);

            cardUI.OnCardClicked += OnCardClicked;

            cardUIs.Add(cardUI);
        }
    }

    void OnCardClicked(CardUI card)
    {
        if (card.transform.IsChildOf(content))
        {
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i].GetComponentInChildren<CardUI>() == null)
                {
                    card.transform.SetParent(slots[i].transform , false);
                    card.transform.localPosition = Vector3.zero;
                    if (!finalDeck.Contains(card.GetData()))
                    {
                        finalDeck.Add(card.GetData());
                    }
                    break;
                }
            }
        }
        else
        {
            card.transform.SetParent(content , false);
            finalDeck.Remove(card.GetData());
        }
    }
}
