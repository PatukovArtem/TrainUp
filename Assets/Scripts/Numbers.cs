using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderValueDisplay : MonoBehaviour
{
    public Slider slider;
    public TextMeshProUGUI valueText;

    void Start()
    {
        if (slider != null && valueText != null)
        {
            UpdateText(slider.value);
            slider.onValueChanged.AddListener(UpdateText);
        }
    }

    void UpdateText(float value)
    {
        valueText.text = Mathf.RoundToInt(value).ToString("00");
    }
}
