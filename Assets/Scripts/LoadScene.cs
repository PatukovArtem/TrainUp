using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


public class LoadScene : MonoBehaviour
{
  public void ChangeScene(int scene)
    {
    SceneManager.LoadScene(scene);
    }
}
