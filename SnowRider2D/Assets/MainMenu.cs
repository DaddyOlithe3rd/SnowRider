using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void JouerJeu()
    {
        SceneManager.LoadScene(1);
    }

    public void Quit()
    {
        Debug.Log("ça marche");
        Application.Quit();
    }
}
