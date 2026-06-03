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
        }
    }

    public void StartSuddenDeath(float extraTime = 60f)
    {
        matchDuration += extraTime; 
        isGameOver = false;
        isTimerRunning = true;
    }

    public void StopTimer()
    {
        isTimerRunning = false;
        isGameOver = true;

    }

    public void ToStandardForm(int totalSec)
    {
        int minutes = totalSec / 60;
        int seconds = totalSec % 60;

        if (minutes == 0)
        {
            timeBox.SetText($"{seconds}");
        }
        else
        {
            timeBox.SetText($"{minutes}:{seconds:00}");
        }
    }

    public float GetCurrentTime() => currentTime;
    public bool GetRunningTimeBool() => isTimerRunning;
}