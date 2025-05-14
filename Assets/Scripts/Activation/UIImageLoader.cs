using UnityEngine;
using UnityEngine.UI;

public class UIImageLoader : MonoBehaviour
{
    [Tooltip("Image, в который будем ставить спрайт")]
    [SerializeField] private Image targetImage;

    [Tooltip("Тот же Pref Key, что и в кнопке")]
    [SerializeField] private string prefKey = "ChosenUI_1";

    private void Awake()
    {
        if (targetImage == null) return;

        string chosen = PlayerPrefs.GetString(prefKey, "");
        if (string.IsNullOrEmpty(chosen)) return;

        var sprite = Resources.Load<Sprite>($"Sprites/{chosen}");
        if (sprite != null)
            targetImage.sprite = sprite;
        else
            Debug.LogError($"[UIImageLoader] Sprite '{chosen}' not found!");
    }
}
