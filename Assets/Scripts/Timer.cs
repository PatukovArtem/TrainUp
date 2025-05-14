using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class Timer : MonoBehaviour
{
    public UnityEvent onTimerEnd;

    [Range(0, 23)]
    public int hours;
    [Range(0, 59)]
    public int minutes;
    [Range(0, 59)]
    public int seconds;

    public enum CountMethod { CountDown, CountUp }
    public enum OutputType { None, StandardText, TMPro, HorizontalSlider, Dial }

    public bool startAtRuntime = true;
    public bool hoursDisplay = false;
    public bool minutesDisplay = true;
    public bool secondsDisplay = true;

    public CountMethod countMethod;
    public OutputType outputType;
    public Text standardText;
    public TextMeshProUGUI textMeshProText;
    public Slider standardSlider;
    public Image dialSlider;

    bool timerRunning = false;
    bool timerPaused = false;
    public double timeRemaining;

    // Слайдеры для установки времени с UI
    private Slider hoursSlider;
    private Slider minutesSlider;
    private Slider secondsSlider;

    private void Awake()
    {
        // Автоматически ищем слайдеры в сцене
        hoursSlider = GameObject.Find("HoursSlider")?.GetComponent<Slider>();
        minutesSlider = GameObject.Find("MinutesSlider")?.GetComponent<Slider>();
        secondsSlider = GameObject.Find("SecondsSlider")?.GetComponent<Slider>();

        if (!standardText && GetComponent<Text>())
            standardText = GetComponent<Text>();

        if (!textMeshProText && GetComponent<TextMeshProUGUI>())
            textMeshProText = GetComponent<TextMeshProUGUI>();

        if (!standardSlider && GetComponent<Slider>())
            standardSlider = GetComponent<Slider>();

        if (!dialSlider && GetComponent<Image>())
            dialSlider = GetComponent<Image>();

        if (standardSlider)
        {
            standardSlider.maxValue = ReturnTotalSeconds();
            standardSlider.value = countMethod == CountMethod.CountDown ? standardSlider.maxValue : standardSlider.minValue;
        }

        if (dialSlider)
            dialSlider.fillAmount = countMethod == CountMethod.CountDown ? 1f : 0f;
    }

    void Start()
    {
        if (startAtRuntime)
            StartTimer();
        else
        {
            double initialTime = countMethod == CountMethod.CountDown ? ReturnTotalSeconds() : 0;
            if (standardText) standardText.text = DisplayFormattedTime(initialTime);
            if (textMeshProText) textMeshProText.text = DisplayFormattedTime(initialTime);
        }
    }

    void Update()
    {
        if (timerRunning)
        {
            if (countMethod == CountMethod.CountDown)
            {
                CountDown();
                if (standardSlider) StandardSliderDown();
                if (dialSlider) DialSliderDown();
            }
            else
            {
                CountUp();
                if (standardSlider) StandardSliderUp();
                if (dialSlider) DialSliderUp();
            }
        }
    }

    private void CountDown()
    {
        if (timeRemaining > 0.02)
        {
            timeRemaining -= Time.deltaTime;
            DisplayInTextObject();
        }
        else
        {
            timeRemaining = 0;
            timerRunning = false;
            onTimerEnd.Invoke();
            DisplayInTextObject();
        }
    }

    private void CountUp()
    {
        if (timeRemaining < ReturnTotalSeconds())
        {
            timeRemaining += Time.deltaTime;
            DisplayInTextObject();
        }
        else
        {
            timeRemaining = ReturnTotalSeconds();
            timerRunning = false;
            onTimerEnd.Invoke();
            DisplayInTextObject();
        }
    }

    private void StandardSliderDown()
    {
        if (standardSlider.value > standardSlider.minValue)
            standardSlider.value -= Time.deltaTime;
    }

    private void StandardSliderUp()
    {
        if (standardSlider.value < standardSlider.maxValue)
            standardSlider.value += Time.deltaTime;
    }

    private void DialSliderDown()
    {
        float clamped = Mathf.InverseLerp(ReturnTotalSeconds(), 0, (float)timeRemaining);
        dialSlider.fillAmount = Mathf.Lerp(1, 0, clamped);
    }

    private void DialSliderUp()
    {
        float clamped = Mathf.InverseLerp(0, ReturnTotalSeconds(), (float)timeRemaining);
        dialSlider.fillAmount = Mathf.Lerp(0, 1, clamped);
    }

    private void DisplayInTextObject()
    {
        if (standardText)
            standardText.text = DisplayFormattedTime(timeRemaining);
        if (textMeshProText)
            textMeshProText.text = DisplayFormattedTime(timeRemaining);
    }

    public double GetRemainingSeconds() => timeRemaining;

    public void StartTimer()
    {
        if (!timerRunning && !timerPaused)
        {
            ResetTimer();
            timerRunning = true;

            if (countMethod == CountMethod.CountDown)
                ConvertToTotalSeconds(hours, minutes, seconds);
            else
                StartTimerCustom(0);
        }
    }

    private void StartTimerCustom(double timeToSet)
    {
        if (!timerRunning && !timerPaused)
        {
            timeRemaining = timeToSet;
            timerRunning = true;
        }
    }

    public void StopTimer()
    {
        timerRunning = false;
        ResetTimer();
    }

    public void PauseOrResumeTimer()
    {
        if (timerRunning)
        {
            timerRunning = false;
            timerPaused = true;
            Debug.Log("Таймер на паузе");
        }
        else if (timerPaused)
        {
            timerRunning = true;
            timerPaused = false;
            Debug.Log("Таймер возобновлён");
        }
    }

    private void ResetTimer()
    {
        timerPaused = false;

        if (countMethod == CountMethod.CountDown)
        {
            timeRemaining = ReturnTotalSeconds();
            DisplayInTextObject();
            if (standardSlider)
            {
                standardSlider.maxValue = ReturnTotalSeconds();
                standardSlider.value = standardSlider.maxValue;
            }
            if (dialSlider) dialSlider.fillAmount = 1f;
        }
        else
        {
            timeRemaining = 0;
            DisplayInTextObject();
            if (standardSlider)
            {
                standardSlider.maxValue = ReturnTotalSeconds();
                standardSlider.value = standardSlider.minValue;
            }
            if (dialSlider) dialSlider.fillAmount = 0f;
        }
    }

    public float ReturnTotalSeconds()
    {
        return hours * 3600 + minutes * 60 + seconds;
    }

    public double ConvertToTotalSeconds(float hours, float minutes, float seconds)
    {
        timeRemaining = hours * 3600 + minutes * 60 + seconds;
        DisplayFormattedTime(timeRemaining);
        return timeRemaining;
    }

    public string DisplayFormattedTime(double remainingSeconds)
    {
        float h, m, s;
        RemainingSecondsToHHMMSSMMM(remainingSeconds, out h, out m, out s);

        string HoursFormat() => hoursDisplay ? $"{h:00}{(minutesDisplay || secondsDisplay ? ":" : "")}" : "";
        string MinutesFormat() => minutesDisplay ? $"{m:00}{(secondsDisplay ? ":" : "")}" : "";
        string SecondsFormat() => secondsDisplay ? $"{s:00}" : "";

        return HoursFormat() + MinutesFormat() + SecondsFormat();
    }

    private static void RemainingSecondsToHHMMSSMMM(double totalSeconds, out float hours, out float minutes, out float seconds)
    {
        hours = Mathf.FloorToInt((float)totalSeconds / 3600);
        minutes = Mathf.FloorToInt(((float)totalSeconds % 3600) / 60);
        seconds = Mathf.FloorToInt((float)totalSeconds % 60);
    }

    private void OnValidate()
    {
        timeRemaining = ConvertToTotalSeconds(hours, minutes, seconds);
    }

    // Вызывается кнопкой для запуска с UI-слайдеров
    public void StartTimerFromUI()
    {
        if (hoursSlider != null) hours = (int)hoursSlider.value;
        if (minutesSlider != null) minutes = (int)minutesSlider.value;
        if (secondsSlider != null) seconds = (int)secondsSlider.value;

        Debug.Log($"Timer Started: {hours:00}:{minutes:00}:{seconds:00}");
        StartTimer();
    }
}
