using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Dice : MonoBehaviour
{
    [SerializeField] private TMP_Text diceText;
    [SerializeField] private TMP_Text diceTimer;
    [SerializeField] private TMP_Text tossDiceNotice;
    [SerializeField] private Button diceButton;
    [SerializeField] private Elexir elexir;
    [SerializeField] private int diceCooldown = 30;

    private int diceNumber;
    private bool isDiceLocked = false; 
    private int elexirMultiplier = 10;

    private void Start()
    {
        isDiceLocked = false;

        diceButton.onClick.AddListener(OnDiceClicked);
        //OnDiceClicked();
        UpdateDiceButtonState();
        tossDiceNotice.gameObject.SetActive(false);
    }

    private Coroutine activeTimerCoroutine;

    private void OnDiceClicked()
    {
        if (isDiceLocked) return;

        TossDice();
        isDiceLocked = true;
        UpdateDiceButtonState();

        if (activeTimerCoroutine != null)
            StopCoroutine(activeTimerCoroutine);
        activeTimerCoroutine = StartCoroutine(DiceTimer());
    }

    private void OnDisable()
    {
        if (activeTimerCoroutine != null)
            StopCoroutine(activeTimerCoroutine);
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
        for (int i = diceCooldown; i > 0; i--)
        {
            ShowTimer(i);
            yield return new WaitForSeconds(1f);
        }

        ShowTimer(0);
        StartCoroutine(NoticeForTossDice());
        isDiceLocked = false;
        UpdateDiceButtonState();
    }

    private void UpdateDiceButtonState()
    {
        diceButton.interactable = !isDiceLocked;
    }

    private void ShowTimer(int curTime)
    {
        diceTimer.SetText(curTime.ToString());
    }

    private IEnumerator NoticeForTossDice()
    {
        tossDiceNotice.gameObject.SetActive(true);
        tossDiceNotice.SetText("Dice is ready to roll!");
        yield return new WaitForSeconds(1.5f);
        tossDiceNotice.gameObject.SetActive(false);
    }
    public void SetElexirMultiplier(int newMultiplier)
    {
        elexirMultiplier = newMultiplier;
    }
}
