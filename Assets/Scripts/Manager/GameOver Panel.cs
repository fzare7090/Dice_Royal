using TMPro;
using UnityEngine;
using RTLTMPro;
using DG.Tweening; // اضافه کردن کتابخانه دو تویین

[RequireComponent(typeof(CanvasGroup))]
public class GameOverPanel : MonoBehaviour
{
    [SerializeField] private Chest chest;
    [SerializeField] private RTLTextMeshPro gameOverText;


    [SerializeField] private RectTransform panelContainer; // کانتینر اصلی حاوی المان‌های پنل
    [SerializeField] private CanvasGroup canvasGroup;

    

    public Chest GetChest()
    {
        return chest;
    }

    public void SetGameOverText(int winnerIndex)
    {
        if (winnerIndex == -1)
        {
            gameOverText.text = " بازی مساوی شد! ";
        }
        else
        {
            gameOverText.text = $" برنده بازی\nبازیکن {winnerIndex + 1}";
        }
    }

    public void GivingChest(int winnerIndex)
    {
        if (winnerIndex == -1) return;

        chest.GiveMatchRewards(winnerIndex);
        chest.SetRewardsText(winnerIndex);
    }

    // متد جدید برای نمایش جذاب پنل با دو تویین
    public void ShowAnimated()
    {
        gameObject.SetActive(true);

        // 1. آماده‌سازی قبل از شروع انیمیشن (کوچک کردن و نامرئی کردن)
        canvasGroup.alpha = 0f;
        panelContainer.localScale = Vector3.zero;
        gameOverText.transform.localScale = Vector3.zero;

        if (chest != null)
            chest.transform.localScale = Vector3.zero;

        // 2. ساخت یک توالی (Sequence)
        Sequence seq = DOTween.Sequence();

        // این دستور بسیار مهم است! باعث می‌شود انیمیشن در TimeScale = 0 هم اجرا شود
        seq.SetUpdate(true);

        // 3. فید شدن پس‌زمینه تاریک (در 0.3 ثانیه)
        seq.Append(canvasGroup.DOFade(1f, 0.3f));

        // 4. پاپ شدن پنل اصلی با افکت فنری (OutBack)
        seq.Append(panelContainer.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack));

        // 5. پرش متن پیروزی به داخل با افکت پرشی (OutBounce)
        // از Insert استفاده می‌کنیم تا انیمیشن متن کمی قبل از اتمام انیمیشن پنل شروع شود (ثانیه 0.5)
        seq.Insert(0.5f, gameOverText.transform.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBounce));

        // 6. نمایش صندوقچه و جوایز با افکت کشسان (OutElastic) برای جذابیت بیشتر
        if (chest != null)
        {
            seq.Insert(0.7f, chest.transform.DOScale(Vector3.one, 0.6f).SetEase(Ease.OutElastic));
        }
    }
}