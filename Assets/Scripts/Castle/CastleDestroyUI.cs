using UnityEngine;
using DG.Tweening;

public class CastleDestroyUI : MonoBehaviour
{
    public static CastleDestroyUI Instance;

    [SerializeField] private UIPanelAnimator animator;

    [SerializeField] private float showTime = 1.5f;

    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }


    private Tween hideTween;


    public void ShowCastleDestroyed()
    {
        hideTween?.Kill();

        gameObject.SetActive(true);

        animator.Show();

        hideTween = DOVirtual.DelayedCall(showTime, () =>
        {
            animator.Hide();
        });
    }
}