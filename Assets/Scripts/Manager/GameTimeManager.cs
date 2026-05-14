using System.Collections;
using TMPro;
using UnityEngine;

public class GameTimeManager : MonoBehaviour
{
    [SerializeField] private TMP_Text timeBox;
    [SerializeField] private float matchDuration = 90f;

    private float currentTime = 0f;
    private bool isTimerRunning = false;
    private bool isGameOver = false;

    public bool IsGameOver => isGameOver;

    private void Update()
    {
        if (!isTimerRunning || isGameOver) return;

        currentTime += Time.deltaTime;
        ToStandardForm(Mathf.FloorToInt(currentTime));

        if (currentTime >= matchDuration)
        {
            StopTimer();
            Debug.Log("Time's up! Match duration reached: " + matchDuration);
        }
    }

    public void StartTimer()
    {
        if (!isTimerRunning && !isGameOver)
        {
            isTimerRunning = true;
            Debug.Log("Timer Started!");
        }
    }

    public void StopTimer()
    {
        isTimerRunning = false;
        isGameOver = true;
        Debug.Log("Timer Stopped at: " + currentTime + " seconds");
    }

    public void ToStandardForm(int totalSec)
    {
        int minutes = totalSec / 60;
        int seconds = totalSec % 60;
        timeBox.SetText($"{minutes:00}:{seconds:00}");
    }

    public float GetCurrentTime() => currentTime;
    public bool GetRunningTimeBool() => isTimerRunning;
}