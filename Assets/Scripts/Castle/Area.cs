using UnityEngine;

public class Area : MonoBehaviour
{
    [SerializeField] private int teamId; 
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

    }

    public int GetTeamId()
    {
        return teamId;
    }

    public void SetTeamId(int newTeamId)
    {
        teamId = newTeamId;
        

    }

    public RectTransform GetRectTransform()
    {

        return rectTransform;

    }

}