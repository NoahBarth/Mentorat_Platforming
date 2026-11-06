using TMPro;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;

    private float elapsedTime;
    private bool isRunning = true;

    void Update()
    {
        if (!isRunning)
        {
            return;
        }

        elapsedTime += Time.deltaTime;
        UpdateTimerText();
    }

    private void UpdateTimerText()
    {
        if (timerText == null)
        {
            return;
        }

        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);
        int milliseconds = Mathf.FloorToInt((elapsedTime * 100f) % 100f);

        timerText.text = $"{minutes:00}:{seconds:00}:{milliseconds:00}";
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public void ResumeTimer()
    {
        isRunning = true;
    }

    public void ResetTimer()
    {
        elapsedTime = 0f;
        isRunning = true;
        UpdateTimerText();
    }

    public float GetElapsedTime()
    {
        return elapsedTime;
    }
}