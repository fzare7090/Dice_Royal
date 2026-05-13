using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Dice : MonoBehaviour
{
    [SerializeField] private TMP_Text diceText;
    [SerializeField] private Button diceButton;

    private int diceNumber;
    private bool diceLock = true;

    private void Start()
    {
        diceButton.onClick.AddListener(OnDiceClicked);
        StartCoroutine(DiceTimer());
        UpdateDiceButtonState();
    }

    public void TossDice()
    {
        diceNumber = UnityEngine.Random.Range(1, 7);
        diceText.SetText(diceNumber.ToString());
    }

    public int GetDiceFace()
    {
        return diceNumber;
    }

    private void OnDiceClicked()
    {
        if (diceLock)
            return;

        TossDice();
        diceLock = true;
        UpdateDiceButtonState();
        StartCoroutine(DiceTimer());
    }

    public IEnumerator DiceTimer()
    {
        yield return new WaitForSeconds(30f);
        diceLock = false;
        UpdateDiceButtonState();
    }

    private void UpdateDiceButtonState()
    {
        diceButton.interactable = !diceLock;
    }
}



