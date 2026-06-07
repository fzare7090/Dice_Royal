using UnityEngine;

public class UIPanelOrder : MonoBehaviour
{
    private void OnEnable()
    {
        transform.SetParent(GetComponentInParent<Canvas>().transform, true);
        transform.SetAsLastSibling();
    }
}
