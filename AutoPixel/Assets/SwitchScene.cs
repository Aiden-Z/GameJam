using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchScene : MonoBehaviour
{
    public string SceneName;

    public void Switch()
    {
        SceneManager.LoadScene(SceneName);
    }
}