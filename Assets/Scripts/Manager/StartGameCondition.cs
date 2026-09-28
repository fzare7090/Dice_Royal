using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StartGameCondition : MonoBehaviour
{
    [SerializeField] private DeckManager deckManager;
    [SerializeField] private BattleManager battleManager;
    [SerializeField] private Image BG;

    public void OnStartGame()
    {
        if (deckManager.GetFinalDeck() == 0)
        {
            StartCoroutine(ShowWarning());
        }
        else
        {
            battleManager.OnLoadSceneButtonClicked();
        }
    }


    private IEnumerator ShowWarning()
    {
        BG.gameObject.SetActive(true);

       
        BG.transform.localScale = Vector3.zero;
        Color color = BG.color;
        color.a = 0;
        BG.color = color;

       
        BG.transform.DOScale(1f, 0.35f).SetEase(Ease.OutBack);
        BG.DOFade(1f, 0.35f);

        yield return new WaitForSeconds(2f);

        
        BG.transform.DOScale(0.8f, 0.2f);
        BG.DOFade(0f, 0.2f);

        yield return new WaitForSeconds(0.2f);

        BG.gameObject.SetActive(false);
    }
}