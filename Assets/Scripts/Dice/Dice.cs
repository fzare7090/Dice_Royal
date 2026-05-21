using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Dice : MonoBehaviour
{
    [SerializeField] private TMP_Text diceText;
    [SerializeField] private Button diceButton;
    [SerializeField] private Elexir elexir;
    [SerializeField] private int diceCooldown = 30;

    private int diceNumber;
    private bool isDiceLocked = false; // حالا مشخص است: اگر true باشد یعنی قفل است
    private int elexirMultiplier = 10;

    private void Start()
    {
        // در شروع بازی دکمه آزاد است
        isDiceLocked = false;

        diceButton.onClick.AddListener(OnDiceClicked);
        UpdateDiceButtonState();
    }

    private void OnDiceClicked()
    {
        // اگر قفل است، اجازه کلیک نده
        if (isDiceLocked)
        {
            Debug.Log("Dice is locked. Please wait.");
            return;
        }

        TossDice();

        // بعد از پرتاب، دکمه را قفل کن و تایمر را شروع کن
        isDiceLocked = true;
        UpdateDiceButtonState();
        StartCoroutine(DiceTimer());
    }

    public void TossDice()
    {
        diceNumber = Random.Range(1, 7);
        diceText.SetText(diceNumber.ToString());

        int gainedElexir = diceNumber * elexirMultiplier;
        elexir.GettingElexir(gainedElexir);
        Debug.Log("Dice rolled: Elexir added.");
    }

    public IEnumerator DiceTimer()
    {
        Debug.Log("Timer started...");
        yield return new WaitForSeconds(diceCooldown);

        // زمان تمام شد، قفل را باز کن
        isDiceLocked = false;
        UpdateDiceButtonState();
        Debug.Log("Timer finished. Dice unlocked.");
    }

    private void UpdateDiceButtonState()
    {
        // دکمه وقتی interactable است که قفل نباشد
        diceButton.interactable = !isDiceLocked;
    }
}
