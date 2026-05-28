using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;


public class BeforBattelTransition : MonoBehaviour
{
    [SerializeField] private Image beforBattelImage;
    void Start()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(0.1f);
        sequence.Append(beforBattelImage.DOFade(1f, 0.5f).SetEase(Ease.InOutQuad));
        sequence.InsertCallback(0.1f, () => PlayStartBattleMusic());
        sequence.InsertCallback(3f, () => ChangingSceneToDeck());
    }

    private void ChangingSceneToDeck()
    {
        SceneManager.LoadScene("GameBoard");
    }
    private void PlayStartBattleMusic()
    {
        OrginalAudioManager.Instance.PlayStartBattleMusic();
    }

}


