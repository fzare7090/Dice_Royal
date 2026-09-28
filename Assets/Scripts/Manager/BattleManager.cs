using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{
    public void OnLoadSceneButtonClicked()
    {
        SceneManager.LoadScene("BeforeBattelTransition");
    }
}