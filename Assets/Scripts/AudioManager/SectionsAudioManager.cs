using UnityEngine;

public class SectionsAudioManager : MonoBehaviour
{
    void Start()
    {
        OrginalAudioManager.Instance.PlayBGMusic();
    }
    void OnDestroy()
    {
        if (OrginalAudioManager.Instance != null)
        {
            OrginalAudioManager.Instance.StopMusic();
        }
    }
}
