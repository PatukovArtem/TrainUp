using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Timer_2 : MonoBehaviour
{
    public UnityEvent onTimerEnd;

    [Range(0, 23)] public int hours;
    [Range(0, 59)] public int minutes;
    [Range(0, 59)] public int seconds;

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

    private bool timerRunning = false;
    private bool timerPaused = false;
    public double timeRemaining;
    private float initialTime;

    private int clickCounter = 0;

    private const string SavedTimeKeyBase = "SavedTime";
    private const string ClickCountKeyBase = "ClickCount";
    private string prefsPrefix;

    private void Awake()
    {
        prefsPrefix = SceneManager.GetActiveScene().name + "_";
        clickCounter = PlayerPrefs.GetInt(prefsPrefix + ClickCountKeyBase, 0);

        if (!standardText && GetComponent<Text>())
            standardText = GetComponent<Text>();
        if (!textMeshProText && GetComponent<TextMeshProUGUI>())
            textMeshProText = GetComponent<TextMeshProUGUI>();
        if (!standardSlider && GetComponent<Slider>())
            standardSlider = GetComponent<Slider>();
        if (!dialSlider && GetComponent<Image>())
            dialSlider = GetComponent<Image>();
    }

    private void Start()
    {
        LoadSavedTime();
        DisplayInTextObject();
        if (startAtRuntime && timeRemaining > 0)
            timerRunning = true;
    }

    private void Update()
    {
        if (!timerRunning) return;

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
        if (timeRemaining < initialTime)
        {
            timeRemaining += Time.deltaTime;
            DisplayInTextObject();
        }
        else
        {
            onTimerEnd.Invoke();
            timeRemaining = initialTime;
            timerRunning = false;
            DisplayInTextObject();
        }
    }

    private void StandardSliderDown() => standardSlider.value = (float)timeRemaining;
    private void StandardSliderUp() => standardSlider.value = (float)timeRemaining;

    private void DialSliderDown()
    {
        float t = Mathf.InverseLerp(initialTime, 0, (float)timeRemaining);
        dialSlider.fillAmount = Mathf.Lerp(1, 0, t);
    }

    private void DialSliderUp()
    {
        float t = Mathf.InverseLerp(0, initialTime, (float)timeRemaining);
        dialSlider.fillAmount = Mathf.Lerp(0, 1, t);
    }

    private void DisplayInTextObject()
    {
        string formatted = DisplayFormattedTime(timeRemaining);
        if (standardText) standardText.text = formatted;
        if (textMeshProText) textMeshProText.text = formatted;
    }

    public double GetRemainingSeconds() => timeRemaining;

    public void StartTimer()
    {
        if (!timerRunning && !timerPaused)
            timerRunning = true;
    }

    public void StopTimer()
    {
        timerRunning = false;
        ResetTimerValues();
    }

   
    public void TogglePause()
    {
        if (timerRunning)
        {
       
            timerRunning = false;
            timerPaused = true;
            Debug.Log($"[{prefsPrefix}] Timer paused at {DisplayFormattedTime(timeRemaining)}");
        }
        else if (timerPaused)
        {
           
            timerPaused = false;
            timerRunning = true;
            Debug.Log($"[{prefsPrefix}] Timer resumed");
        }
    }

    private void ResetTimerValues()
    {
        timerPaused = false;
        timeRemaining = initialTime;

        if (standardSlider)
        {
            standardSlider.maxValue = initialTime;
            standardSlider.value = (float)timeRemaining;
        }
        if (dialSlider)
            dialSlider.fillAmount = (float)timeRemaining / initialTime;

        DisplayInTextObject();
    }

    public float ReturnTotalSeconds() => hours * 3600 + minutes * 60 + seconds;

    public double ConvertToTotalSeconds(float h, float m, float s)
    {
        timeRemaining = h * 3600 + m * 60 + s;
        return timeRemaining;
    }

    public string DisplayFormattedTime(double remainingSeconds)
    {
        RemainingSecondsToHHMMSSMMM(remainingSeconds, out float h, out float m, out float s);
        string hStr = hoursDisplay ? $"{h:00}:" : "";
        string mStr = minutesDisplay ? $"{m:00}:" : "";
        string sStr = secondsDisplay ? $"{s:00}" : "";
        return hStr + mStr + sStr;
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

    public void Add10Seconds()
    {
        clickCounter++;
        PlayerPrefs.SetInt(prefsPrefix + ClickCountKeyBase, clickCounter);
        PlayerPrefs.Save();

        if (clickCounter >= 5)
        {
            float current = PlayerPrefs.GetFloat(prefsPrefix + SavedTimeKeyBase, ReturnTotalSeconds());
            current += 10f;
            PlayerPrefs.SetFloat(prefsPrefix + SavedTimeKeyBase, current);

            clickCounter = 0;
            PlayerPrefs.SetInt(prefsPrefix + ClickCountKeyBase, 0);
            PlayerPrefs.Save();

            Debug.Log($"[{prefsPrefix}] Added +10s; will apply on next load. Total saved: {current} sec");
        }
    }

    public void ResetToDefaultTime()
    {
        float defaultSec = ReturnTotalSeconds();
        PlayerPrefs.SetFloat(prefsPrefix + SavedTimeKeyBase, defaultSec);
        PlayerPrefs.SetInt(prefsPrefix + ClickCountKeyBase, 0);
        PlayerPrefs.Save();

        initialTime = defaultSec;
        timeRemaining = defaultSec;
        timerRunning = false;
        timerPaused = false;

        if (standardSlider)
        {
            standardSlider.maxValue = initialTime;
            standardSlider.value = (float)timeRemaining;
        }
        if (dialSlider)
            dialSlider.fillAmount = (float)timeRemaining / initialTime;

        DisplayInTextObject();
        Debug.Log($"[{prefsPrefix}] Timer reset to default: {defaultSec} sec");
    }

    private void LoadSavedTime()
    {
        float savedTime = PlayerPrefs.GetFloat(prefsPrefix + SavedTimeKeyBase, ReturnTotalSeconds());
        initialTime = savedTime;
        timeRemaining = savedTime;

        if (standardSlider)
        {
            standardSlider.maxValue = initialTime;
            standardSlider.value = (float)timeRemaining;
        }
        if (dialSlider)
            dialSlider.fillAmount = (float)timeRemaining / initialTime;
    }
}
