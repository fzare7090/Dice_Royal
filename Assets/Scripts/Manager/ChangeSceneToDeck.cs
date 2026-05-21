using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class ChangeSceneToDeck : MonoBehaviour
{
    [SerializeField] private Button changeSceneToDeck;
    void Start()
    {
        changeSceneToDeck.onClick.AddListener(ChangingSceneToDeck);
    }

    private void ChangingSceneToDeck()
    {
        SceneManager.LoadScene("DeckBuilder");
    }

}
