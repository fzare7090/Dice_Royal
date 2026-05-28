using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using System.Collections.Generic;

public class ScrollSnap : MonoBehaviour, IEndDragHandler
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private int pageCount = 5;

    private int currentPage = 2;
    private List<float> pagePositions = new List<float>();

    void Start()
    {
        for (int i = 0; i < pageCount; i++)
        {
            float pos = (float)i / (pageCount - 1);
            pagePositions.Add(pos);
        }

        scrollRect.horizontalNormalizedPosition = pagePositions[currentPage];
    }

    public void OnEndDrag(PointerEventData eventData)
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
        DOTween.To(() => scrollRect.horizontalNormalizedPosition,
                   x => scrollRect.horizontalNormalizedPosition = x,
                   pagePositions[index], 0.3f)
               .SetEase(Ease.OutQuad);
    }
}
