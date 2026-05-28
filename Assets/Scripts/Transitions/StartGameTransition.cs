using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;


public class StartGameTransition : MonoBehaviour
{

    void Start()
    {
        transform.localScale = new Vector3(1.4f, 1.4f, 1.4f);
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOScale(Vector3.one, 2f).SetEase(Ease.OutExpo));
        sequence.InsertCallback(0.5f, () => PlayStartGameMusic());
    }

    private void PlayStartGameMusic()
    {
        OrginalAudioManager.Instance.PlayStartGameMusic();
    }
}
