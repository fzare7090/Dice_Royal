using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class UIPanelAnimator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform panel;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Animation")]
    [SerializeField] private float showDuration = 0.35f;
    [SerializeField] private float hideDuration = 0.25f;

    [SerializeField] private Ease showEase = Ease.OutBack;
    [SerializeField] private Ease hideEase = Ease.InBack;

    private void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        if (panel == null)
            panel = GetComponent<RectTransform>();
    }

    public void Show()
    {
        Debug.Log("Animator Show");

        Debug.Log("Before Show alpha = " + canvasGroup.alpha);
        gameObject.SetActive(true);

        panel.DOKill();
        canvasGroup.DOKill();

        panel.localScale = Vector3.zero;
        canvasGroup.alpha = 0f;

        Sequence sequence = DOTween.Sequence();
        sequence.SetUpdate(true);

        sequence.Append(
            panel.DOScale(1f, showDuration)
                 .SetEase(showEase));

        sequence.Join(
            canvasGroup.DOFade(1f, showDuration * 0.8f));

        sequence.OnKill(() =>
        {
            Debug.Log("Sequence Killed");
        });

        sequence.OnComplete(() =>
        {
            Debug.Log("Sequence Complete");
        });

    }

    public void Hide()
    {
        panel.DOKill();
        canvasGroup.DOKill();

        Sequence sequence = DOTween.Sequence();
        sequence.SetUpdate(true);
        sequence.Append(
            panel.DOScale(0f, hideDuration)
                 .SetEase(hideEase));

        sequence.Join(
            canvasGroup.DOFade(0f, hideDuration * 0.8f));

        sequence.OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }
}