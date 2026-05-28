using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening; // اضافه کردن فضای نام DOTween

public class BattleManager : MonoBehaviour
{
    [SerializeField] private Button changeSceneButton;

    void Start()
    {
        changeSceneButton.onClick.AddListener(OnLoadSceneButtonClicked);
    }

    private void OnLoadSceneButtonClicked()
    {
        SceneManager.LoadScene("BeforeBattelTransition");
    }
}
