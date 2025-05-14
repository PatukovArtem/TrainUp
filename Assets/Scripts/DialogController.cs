using UnityEngine;
using UnityEngine.UI;

public class DialogController : MonoBehaviour
{
    public GameObject dialogPanel; 
    public Button openDialogButton;
    public Button closeDialogButton; 

    void Start()
    {
        
        dialogPanel.SetActive(false);

        openDialogButton.onClick.AddListener(OpenDialog);
        closeDialogButton.onClick.AddListener(CloseDialog);
    }

    void OpenDialog()
    {
        dialogPanel.SetActive(true);
    }

    void CloseDialog()
    {
        dialogPanel.SetActive(false);
    }
}
