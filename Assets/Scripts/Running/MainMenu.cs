using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.IO;

public class MainMenu : MonoBehaviour {

	public void startButton(int index)
    {
        File.Delete(Application.persistentDataPath + "/PartyData.dat");
        File.Delete(Application.persistentDataPath + "/InventoryData.dat");
        File.Delete(Application.persistentDataPath + "/StageData.dat");
        SceneManager.LoadScene(index);
    }

    public void continueButton()
    {
        if (File.Exists(Application.persistentDataPath + "/StageData.dat"))
        {
            StoryManager.load();
            SceneManager.LoadScene(1);
        }
    }

    public void quitButton()
    {
        Application.Quit();
    }

}
