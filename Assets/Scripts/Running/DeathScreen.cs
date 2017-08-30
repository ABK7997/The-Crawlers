using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class DeathScreen : MonoBehaviour {

	public void button_mainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void button_continue()
    {
        StoryManager.load();
        SceneManager.LoadScene(1);
    }

    public void quit_button()
    {
        Application.Quit();
    }
}
