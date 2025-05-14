using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonHandlerUI : MonoBehaviour
{
    [Header("Настройки смены UI Image")]
    [SerializeField] private string spriteName;
    [Tooltip("Ключ, по которому будем сохранять в PlayerPrefs")]
    [SerializeField] private string prefKey = "ChosenUI_1";
    [SerializeField] private string targetSceneName = "SceneB";
    [SerializeField] private string targetObjectName = "TargetUIImage";

    public void OnChangeSpriteButtonClicked()
    {
        Debug.Log($"[BtnHandler] Clicked: sprite={spriteName}, key={prefKey}");

        // 1) Сохраняем своё имя
        PlayerPrefs.SetString(prefKey, spriteName);
        PlayerPrefs.Save();

        // 2) Если сцена уже загружена — обновляем на лету
        var sceneB = SceneManager.GetSceneByName(targetSceneName);
        if (sceneB.isLoaded)
        {
            foreach (var root in sceneB.GetRootGameObjects())
            {
                if (root.name == targetObjectName)
                {
                    var img = root.GetComponent<Image>();
                    if (img != null)
                    {
                        var loaded = Resources.Load<Sprite>($"Sprites/{spriteName}");
                        if (loaded != null)
                        {
                            img.sprite = loaded;
                            Debug.Log("[BtnHandler] Image updated in-scene.");
                        }
                        else Debug.LogError($"[BtnHandler] Sprite '{spriteName}' not found!");
                    }
                }
            }
        }

        // Никаких переходов сцен сюда больше не кладём
    }
}
