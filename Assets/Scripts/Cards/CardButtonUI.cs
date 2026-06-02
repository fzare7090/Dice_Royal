using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using RTLTMPro;
using System;
using DG.Tweening;

public class CardButtonUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("UI Elements")]
    // [SerializeField] private Image cardIcon;        
    [SerializeField] private RTLTextMeshPro ElexirText;
    [SerializeField] private RTLTextMeshPro cardName;
    [SerializeField] private Button button;

    private UnitData currentData;
    private int teamId;
    private Spawner spawner;
    private Canvas canvas;
    private Vector3 originalPos;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    // Events to communicate with Spawner
    public Action<CardButtonUI, UnitData, int, Vector2> OnDragStartEvent;
    public Action<Vector2> OnDragEvent;
    public Action<CardButtonUI, UnitData, int> OnDragEndEvent;

    public void SetCard(UnitData data, int teamId, Spawner spawner)
    {
        this.currentData = data;
        this.teamId = teamId;
        this.spawner = spawner;

        UpdateUI();

        canvas = GetComponentInParent<Canvas>();

        // Initial Appearance Animation
        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack).SetDelay(transform.GetSiblingIndex() * 0.1f);
    }

    private void UpdateUI()
    {
        if (ElexirText != null)
        {
            ElexirText.text = currentData.elexir.ToString();
        }
        if (cardName != null)
        {
            cardName.text = currentData.name;
        }
    }

    public void AnimateCardCycle(UnitData newData)
    {
        // Cycle Animation: shrinks to 0, updates data, springs back to 1
        transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack).OnComplete(() =>
        {
            this.currentData = newData;
            UpdateUI();
            transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
        });
    }

    public Button GetButton()
    {
        return button;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Visual feedback when grabbed
        transform.DOScale(Vector3.one * 0.9f, 0.15f).SetEase(Ease.OutQuad);
        if (canvasGroup != null) canvasGroup.DOFade(0.6f, 0.15f);

        OnDragStartEvent?.Invoke(this, currentData, teamId, eventData.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Instead of moving the card itself (which messes with Layout Groups), 
        // we just move the indicator on the board via the event!
        OnDragEvent?.Invoke(eventData.position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Visual feedback when released
        transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBack);
        if (canvasGroup != null) canvasGroup.DOFade(1f, 0.2f);

        // Tell spawner to try and spawn the unit at the dragged position
        OnDragEndEvent?.Invoke(this, currentData, teamId);
    }
}
