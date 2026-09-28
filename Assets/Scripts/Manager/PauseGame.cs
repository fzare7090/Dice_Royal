using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseGame : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;

    [Header("Buttons")]
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button exitButton;

    private bool isPaused;

    private void Start()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (pauseButton != null)
            pauseButton.onClick.AddListener(Pause);

        if (resumeButton != null)
            resumeButton.onClick.AddListener(ResumeGame);

        if (exitButton != null)
            exitButton.onClick.AddListener(ChangeSceneToHome);
    }

    public void Pause()
    {
        if (isPaused) return;

        isPaused = true;
        Time.timeScale = 0f;

        if (pausePanel != null)
            pausePanel.SetActive(true);
    }

    public void ResumeGame()
    {
        if (!isPaused) return;

        isPaused = false;
        Time.timeScale = 1f;

        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    private void ChangeSceneToHome()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene("GameHub");
    }

    private void OnDestroy()
    {
        if (pauseButton != null)
            pauseButton.onClick.RemoveListener(Pause);

        if (resumeButton != null)
            resumeButton.onClick.RemoveListener(ResumeGame);

        if (exitButton != null)
            exitButton.onClick.RemoveListener(ChangeSceneToHome);
    }
}