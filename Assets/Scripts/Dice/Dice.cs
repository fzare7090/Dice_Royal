using System.Collections;
using RTLTMPro;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using DG.Tweening;

public class Dice : MonoBehaviour
{
    [SerializeField] private RTLTextMeshPro diceText;
    [SerializeField] private RTLTextMeshPro diceTimer;
    [SerializeField] private RTLTextMeshPro tossDiceNotice;
    [SerializeField] private Button diceButton;





    [Header("Animation Settings")]
    [SerializeField] private float rollDuration = 1f;
    [SerializeField] private float rollSpeed = 720f;
    [SerializeField] private AnimationCurve rollCurve;

    [Header("Cooldown Settings")]
    [SerializeField] private float cooldownDuration = 30f;

    [Header("Elexir Settings")]

    [SerializeField] private Elexir elexir;
    [SerializeField] private int elexirMultiplier = 10;

    [Header("UI Settings")]
    [SerializeField] private TMP_Text timerText;


    // private int diceNumber;
    private bool isDiceLocked = false;

    public System.Action<int> OnDiceRolled;

    public bool IsRolling { get; private set; }
    public bool IsLocked { get; private set; }
    public int LastResult { get; private set; }

    private Quaternion startRotation;
    private Vector3 startPosition;
    private Collider diceCollider;
    private Camera mainCamera;
    private Coroutine activeTimerCoroutine;
    private Sequence readyNoticeSequence;


    private void Start()
    {
        IsRolling = false;
        IsLocked = false;

        startPosition = transform.position;
        startRotation = transform.rotation;

        diceCollider = GetComponent<Collider>();
        if (diceCollider == null)
            diceCollider = gameObject.AddComponent<BoxCollider>();

        mainCamera = Camera.main;


        NavMeshAgent navAgent = GetComponent<NavMeshAgent>();
        if (navAgent != null)
            navAgent.enabled = false;

        /*Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
        */

        if (rollCurve == null || rollCurve.keys.Length == 0)
            rollCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        HideTimer();
        HideReadyNotice();

        if (diceButton != null)
        {
            diceButton.onClick.RemoveListener(ForceRoll);
            diceButton.onClick.AddListener(ForceRoll);
        }

        Debug.Log("Dice initialized! Ready to roll.");
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && mainCamera != null)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f) && hit.collider.gameObject == gameObject)
            {
                TryToRoll();
            }
        }
    }

    private void TryToRoll()
    {
        if (IsLocked || IsRolling) return;
        StartCoroutine(RollDiceAnimation());
    }

    private IEnumerator RollDiceAnimation()
    {
        IsRolling = true;

        if (diceCollider != null)
            diceCollider.enabled = false;

        int bottomNumber = Random.Range(1, 7);

        LastResult = bottomNumber;

        Debug.Log($"Dice number: {bottomNumber}");
        if (diceText != null)
            diceText.text = bottomNumber.ToString();

        Quaternion finalRotation = GetRotationForBottomNumber(bottomNumber);

        float elapsedTime = 0;
        while (elapsedTime < rollDuration)
        {
            float t = rollCurve.Evaluate(elapsedTime / rollDuration);

            float currentRollAngle = elapsedTime * rollSpeed;
            Quaternion rollingRotation = Quaternion.Euler(
                currentRollAngle,
                currentRollAngle * 1.5f,
                currentRollAngle * 0.8f
            );
            transform.rotation = startRotation * rollingRotation;

            float verticalOffset = Mathf.Sin(t * Mathf.PI) * 0.3f;
            Vector3 tempPos = startPosition;
            tempPos.y = startPosition.y + verticalOffset;
            transform.position = tempPos;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.rotation = finalRotation;
        transform.position = startPosition;

        if (diceCollider != null)
            diceCollider.enabled = true;

        IsRolling = false;

        int gainedElexir = LastResult * elexirMultiplier;

        Debug.Log($"Dice Result: Top={LastResult}, Elexir Gained={gainedElexir}");

        if (elexir != null)
        {
            elexir.GettingElexir(gainedElexir);
        }
        else
        {
            Debug.LogError($"{nameof(Dice)} is missing an Elexir reference.", this);
        }

        OnDiceRolled?.Invoke(LastResult);

        IsLocked = true;

        if (activeTimerCoroutine != null)
            StopCoroutine(activeTimerCoroutine);
        activeTimerCoroutine = StartCoroutine(StartCooldownTimer());
    }

    private IEnumerator StartCooldownTimer()
    {
        Debug.Log($"Cooldown started: {cooldownDuration} seconds");

        ShowTimer();

        float remainingTime = cooldownDuration;

        while (remainingTime > 0)
        {
            SetTimerTextValue(Mathf.CeilToInt(remainingTime).ToString());
            remainingTime -= Time.deltaTime;
            yield return null;
        }

        HideTimer();

        IsLocked = false;
        ShowReadyNotice();
        Debug.Log("Cooldown finished! Dice is ready to roll again.");
    }
    /*
    private int GetBottomNumberFromTop(int botton)
    {
        int top = 7 - botton;
        return top;
    }
    */

    private Quaternion GetRotationForBottomNumber(int bottomNumber)
    {

        switch (bottomNumber)
        {
            case 6: return Quaternion.Euler(0, 0, 0);

            case 1: return Quaternion.Euler(180, 0, 0);

            case 2: return Quaternion.Euler(-90, 0, 0);

            case 5: return Quaternion.Euler(90, 0, 0);

            case 4: return Quaternion.Euler(0, 90, 0);

            case 3: return Quaternion.Euler(0, -90, 0);

            default: return Quaternion.identity;
        }
    }

    public void SetElexirMultiplier(int newMultiplier)
    {
        elexirMultiplier = newMultiplier;
    }

    public int GetElexirMultiplier()
    {
        return elexirMultiplier;
    }

    public void SetElexir(Elexir newElexir)
    {
        elexir = newElexir;
    }

    public void SetTimerText(TMP_Text newTimerText)
    {
        timerText = newTimerText;
        HideTimer();
    }

    public void ForceRoll()
    {
        TryToRoll();
    }

    public void ResetDice()
    {
        transform.position = startPosition;
        transform.rotation = startRotation;
        IsLocked = false;
        IsRolling = false;

        if (activeTimerCoroutine != null)
            StopCoroutine(activeTimerCoroutine);

        HideTimer();
    }

    public bool IsReady()
    {
        return !IsLocked && !IsRolling;
    }

    public int GetLastRolledNumber()
    {
        return LastResult;
    }

    private TMP_Text GetTimerLabel()
    {
        if (timerText != null)
            return timerText;

        return diceTimer;
    }

    private void ShowTimer()
    {
        TMP_Text timerLabel = GetTimerLabel();
        if (timerLabel != null)
            timerLabel.gameObject.SetActive(true);
    }

    private void HideTimer()
    {
        TMP_Text timerLabel = GetTimerLabel();
        if (timerLabel == null)
            return;

        timerLabel.text = "";
        timerLabel.gameObject.SetActive(false);
    }

    private void SetTimerTextValue(string value)
    {
        TMP_Text timerLabel = GetTimerLabel();
        if (timerLabel != null)
            timerLabel.text = value;
    }

    private void ShowReadyNotice()
    {
        if (tossDiceNotice == null)
        {
            Debug.LogWarning($"{nameof(Dice)} has no ready notice text assigned.", this);
            return;
        }

        readyNoticeSequence?.Kill();

        tossDiceNotice.gameObject.SetActive(true);
        tossDiceNotice.text = "تاس آماده‌ی پرتاب است!";
        tossDiceNotice.transform.localScale = Vector3.zero;

        readyNoticeSequence = DOTween.Sequence();
        readyNoticeSequence.Append(tossDiceNotice.transform.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack));
        readyNoticeSequence.AppendInterval(1.5f);
        readyNoticeSequence.Append(tossDiceNotice.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack));
        readyNoticeSequence.OnComplete(HideReadyNotice);
    }

    private void HideReadyNotice()
    {
        readyNoticeSequence?.Kill();
        readyNoticeSequence = null;

        if (tossDiceNotice != null)
            tossDiceNotice.gameObject.SetActive(false);
    }
}
