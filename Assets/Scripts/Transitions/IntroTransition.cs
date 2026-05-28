using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;


public class IntroTransition : MonoBehaviour
{
    [SerializeField] private Image introImage;
    void Start()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(1f);
        sequence.Append(introImage.DOFade(1f, 1f).SetEase(Ease.InOutQuad));
        sequence.InsertCallback(1f, () => PlayIntroMusic());
        sequence.InsertCallback(6f, () => ChangingSceneToDeck());
    }

    private void ChangingSceneToDeck()
    {
        SceneManager.LoadScene("GameHub");
    }
    private void PlayIntroMusic()
    {
        OrginalAudioManager.Instance.PlayIntroMusic();
    }

}


