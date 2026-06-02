using System.Collections;
using RTLTMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Dice : MonoBehaviour
{
    [SerializeField] private RTLTextMeshPro diceText;
    [SerializeField] private RTLTextMeshPro diceTimer;
    [SerializeField] private RTLTextMeshPro tossDiceNotice;
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
        tossDiceNotice.text = "تاس آماده‌ی پرتاب است!";

        tossDiceNotice.transform.localScale = Vector3.zero;

        Sequence seq = DOTween.Sequence();
        seq.Append(tossDiceNotice.transform.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack));
        seq.AppendInterval(1.5f);
        seq.Append(tossDiceNotice.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack));

        yield return new WaitForSeconds(2.2f);
        tossDiceNotice.gameObject.SetActive(false);
    }
    public void SetElexirMultiplier(int newMultiplier)
    {
        elexirMultiplier = newMultiplier;
    }
}
