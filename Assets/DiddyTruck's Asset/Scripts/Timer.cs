using System;
using UnityEngine;
using TMPro; // Standard for text rendering

public class Timer : MonoBehaviour
{
    public enum TimerType { CountDown, CountUp }

    [Header("Timer Setup")]
    [SerializeField] private TimerType timerType = TimerType.CountDown;
    [SerializeField] private float timeLimit = 60f; // Seconds for CountDown
    [SerializeField] private bool autoStart = true;

    [Header("UI Reference")]
    [SerializeField] private TextMeshProUGUI timerText; // Drag TMP text element here

    public float ElapsedTime { get; private set; }
    public float RemainingTime { get; private set; }
    public bool IsRunning { get; private set; }

    // Events for game managers to listen to
    public event Action OnChallengeSuccess;
    public event Action OnChallengeFail;
    public float CurrentTime => RemainingTime;

    private void Start()
    {
        ResetTimer();
        if (autoStart) StartTimer();
    }

    private void Update()
    {
        if (!IsRunning) return;

        if (timerType == TimerType.CountDown)
        {
            RemainingTime -= Time.deltaTime;
            UpdateUI(RemainingTime);

            if (RemainingTime <= 0f)
            {
                RemainingTime = 0f;
                UpdateUI(0f);
                FailChallenge();
            }
        }
        else // CountUp
        {
            ElapsedTime += Time.deltaTime;
            UpdateUI(ElapsedTime);

            if (timeLimit > 0 && ElapsedTime >= timeLimit)
            {
                ElapsedTime = timeLimit;
                UpdateUI(timeLimit);
                FailChallenge(); // Reached maximum allowed time
            }
        }
    }

    public void StartTimer() => IsRunning = true;
    public void PauseTimer() => IsRunning = false;

    public void ResetTimer()
    {
        IsRunning = false;
        ElapsedTime = 0f;
        RemainingTime = timeLimit;
        UpdateUI(timerType == TimerType.CountDown ? RemainingTime : ElapsedTime);
    }

    public void CompleteChallenge()
    {
        if (!IsRunning) return;
        IsRunning = false;
        OnChallengeSuccess?.Invoke();
        Debug.Log($"Challenge Passed! Final Time: {GetFormattedTime(ElapsedTime)}");
    }

    private void FailChallenge()
    {
        IsRunning = false;
        OnChallengeFail?.Invoke();
        Debug.Log("Challenge Failed! Time Ran Out.");
    }

    private void UpdateUI(float timeToDisplay)
    {
        if (timerText != null)
        {
            timerText.text = GetFormattedTime(timeToDisplay);
        }
    }

    public string GetFormattedTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60F);
        int seconds = Mathf.FloorToInt(time % 60F);
        int milliseconds = Mathf.FloorToInt((time * 100F) % 100F);

        return string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, milliseconds);
    }
}