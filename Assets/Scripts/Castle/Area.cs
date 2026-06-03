using UnityEngine;
using UnityEngine.UI;

public class Area : MonoBehaviour
{
    [SerializeField] private int teamId;
    private RectTransform rectTransform;

    private Image img;
    private Outline outline;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        
        img = GetComponent<Image>();
        if (img == null)
        {
            img = gameObject.AddComponent<Image>();
            img.color = new Color(0, 0, 0, 0);
            img.raycastTarget = false;
        }

        outline = GetComponent<Outline>();
        if (outline == null)
        {
            outline = gameObject.AddComponent<Outline>();
            outline.effectDistance = new Vector2(4, -4);
            outline.enabled = false;
        }
    }

    public int GetTeamId()
    {
        return teamId;
    }

    public void SetTeamId(int newTeamId)
    {
        teamId = newTeamId;

        Debug.Log($"Screen size: {Screen.width} x {Screen.height}");
        Debug.Log($"Canvas scale factor: {GetComponentInParent<Canvas>().scaleFactor}");

    }

    public RectTransform GetRectTransform()
    {
        return rectTransform;
    }

    public void ShowHighlight(bool show)
    {
        if (img == null) return;

        if (show)
        {
            img.enabled = true;
            img.color = new Color(0f, 0.1f, 0.4f, 0.15f); 

            if (outline != null)
            {
                outline.enabled = true;
                outline.effectColor = new Color(0.8f, 0f, 0f, 1f); 
            }
        }
        else
        {
            img.color = new Color(0, 0, 0, 0); 
            if (outline != null)
            {
                outline.enabled = false;
            }
        }
    }
}