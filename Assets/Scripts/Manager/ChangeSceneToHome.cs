using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChangeSceneToHome : MonoBehaviour
{
    [SerializeField] private Button changeSceneToHome;
    void Start()
    {
        changeSceneToHome.onClick.AddListener(ChangingSceneToHome);
    }

    private void ChangingSceneToHome()
    {
        SceneManager.LoadScene("GameHub");
    }

}
