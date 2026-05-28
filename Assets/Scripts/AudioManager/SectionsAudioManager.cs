using UnityEngine;

public class SectionsAudioManager : MonoBehaviour
{
    void Start()
    {
        OrginalAudioManager.Instance.PlayBGMusic();
    }
    void OnDestroy()
    {
        OrginalAudioManager.Instance.StopMusic();
    }
}
