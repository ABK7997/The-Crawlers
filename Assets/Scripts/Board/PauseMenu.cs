using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {

    public Image menu;

	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            menu.gameObject.SetActive(!menu.isActiveAndEnabled);
        }
	}

    public void returnToOverworld()
    {
        SceneManager.LoadScene(1);
    }

    public void returnButton()
    {
        menu.gameObject.SetActive(false);
    }
}
