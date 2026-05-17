using UnityEngine;
using UnityEngine.EventSystems;

public class UnitCardDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector3 dragStartPos;
    private Vector3 dragEndPos;

    public Vector3 GetDragStart()
    {
        return dragStartPos;
    }

    public Vector3 GetDragEnd()
    {
        return dragEndPos;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragStartPos = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        dragEndPos = eventData.position;

        //Vector3 worldPos = Camera.main.ScreenToWorldPoint(dragEndPos);

    }
}
