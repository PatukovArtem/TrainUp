using UnityEngine;
using UnityEngine.UI;

public class ButtonSpriteToggle : MonoBehaviour
{
    public Sprite sprite1;
    public Sprite sprite2;

    private Image buttonImage;
    private bool usingFirstSprite = true;

    void Start()
    {
        buttonImage = GetComponent<Image>();
    }

    public void ToggleSprite()
    {
        if (buttonImage == null) return;

        buttonImage.sprite = usingFirstSprite ? sprite2 : sprite1;
        usingFirstSprite = !usingFirstSprite;
    }
}