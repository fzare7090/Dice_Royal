using System.Collections;
using TMPro;
using UnityEngine;

public class GameTimeManager : MonoBehaviour
{

    [SerializeField] private TMP_Text timeBox;


    private int currentSecond = 0;

    private bool isTimerRunning = false;

    private void Start()
    {
        ToStandardForm(GetCurrnetSecond());
    }

    private int GetCurrnetSecond()
    {
        return currentSecond;
    }

    public bool GetRunningTimeBool()
    {
        return isTimerRunning;
    }


    public IEnumerator RunTimer()
    {
        while (isTimerRunning == true)
        {

            yield return new WaitForSeconds(1f);
            currentSecond++;
            ToStandardForm(currentSecond);
        }
    }

    public void StartTimer()
    {
        if (!isTimerRunning)
        {
            isTimerRunning = true;
            StartCoroutine(RunTimer());
        }
    }

    public void StopTimer()
    {
        isTimerRunning = false;
        Debug.Log("isTimerRunning is false");
    }


    public void ToStandardForm(int TotalSec)
    {
        int minutes = 0;
        int seconds = 0;

        minutes = TotalSec / 60;
        seconds = TotalSec % 60;

        timeBox.SetText($"{minutes} : {seconds}");

    }


}
