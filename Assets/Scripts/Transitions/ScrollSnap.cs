using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using System.Collections.Generic;
using System.Collections;

public class ScrollSnap : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private int pageCount = 5;
    [SerializeField] private float swipeThreshold = 50f; // حداقل مقدار کشیدن (با پیکسل) برای رفتن به صفحه بعد/قبل

    private int currentPage = 2;
    private List<float> pagePositions = new List<float>();

    void Start()
    {
        // در پایان بازی Time.timeScale صفر می‌شود، بنابراین وقتی به هاب برمی‌گردیم 
        // باید آن را به 1 برگردانیم تا انیمیشن‌های DOTween فریز نشوند روی صفحه!
        Time.timeScale = 1f;

        for (int i = 0; i < pageCount; i++)
        {
            float pos = (float)i / (pageCount - 1);
            pagePositions.Add(pos);
        }

        // استفاده از کوروتین برای اعمال پوزیشن بعد از ساخته شدت کامل لایوت‌ها (جلوگیری از باگ هنگام بازگشت از بازی)
        StartCoroutine(SetInitialPosition());
    }

    private IEnumerator SetInitialPosition()
    {
        yield return null; // یه فریم صبر می‌کنیم
        scrollRect.horizontalNormalizedPosition = pagePositions[currentPage];
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // توقف حرکت‌های انیمیشنی قبلی موقعی که کاربر دستش رو میذاره روی صفحه
        DOTween.Kill(scrollRect);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        float dragDistance = eventData.position.x - eventData.pressPosition.x;

        if (Mathf.Abs(dragDistance) > swipeThreshold)
        {
            // اگر کاربر اندازه کافی کشیده باشد، یک صفحه کامل تغییر می‌دهد (نه بیشتر!)
            if (dragDistance > 0 && currentPage > 0)
            {
                GoToPage(currentPage - 1); // کشیدن به راست
            }
            else if (dragDistance < 0 && currentPage < pageCount - 1)
            {
                GoToPage(currentPage + 1); // کشیدن به چپ
            }
            else
            {
                GoToPage(currentPage); // اگر به انتها/ابتدا رسیده بود، برگرده سرجاش
            }
        }
        else
        {
            // اگه خیلی کم کشیده بود، میچسبه به همون صفحه نزدیکتر
            SnapToClosestPage();
        }
    }

    private void SnapToClosestPage()
    {
        float currentPos = scrollRect.horizontalNormalizedPosition;
        int closestPage = 0;
        float minDistance = float.MaxValue;
        for (int i = 0; i < pagePositions.Count; i++)
        {
            float distance = Mathf.Abs(currentPos - pagePositions[i]);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestPage = i;
            }
        }

        GoToPage(closestPage);
    }

    public void GoToPage(int index)
    {
        currentPage = index;
        scrollRect.velocity = Vector2.zero; // توقف اینرسی (Inertia) خود اسکرول که با Tween تداخل می‌کنه

        DOTween.To(() => scrollRect.horizontalNormalizedPosition,
                   x => scrollRect.horizontalNormalizedPosition = x,
                   pagePositions[index], 0.3f)
               .SetEase(Ease.OutQuad)
               .SetUpdate(true) // این دستور باعث می‌شود Tween کاملاً مستقل از Time.timeScale کار کند
               .SetId(scrollRect); // شناسه میدیم تا بتونیم بعدا کیلش کنیم
    }
}
