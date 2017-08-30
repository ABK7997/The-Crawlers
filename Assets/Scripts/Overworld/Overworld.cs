using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Overworld : MonoBehaviour {

    private bool itemActive = false;
    private bool active = false;
    public GameObject itemDisplay;
    public GameObject partyDisplay;

    private Party party;
    private Inventory inventory;
    public Button[] areas;

    void Awake()
    {
        party = FindObjectOfType<Party>();
        inventory = FindObjectOfType<Inventory>();
        areas = GetComponentsInChildren<Button>();

        for (int i = StoryManager.areasAvailable; i < areas.Length; i++)
        {
            areas[i].gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            partyDisplay.SetActive(!active);
            active = !active;

            if (partyDisplay.activeInHierarchy)
            {
                party.partyMembers.Clear();
            }
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            itemDisplay.SetActive(!itemActive);
            itemActive = !itemActive;
        }
    }

    public void quit()
    {
        save();
        SceneManager.LoadScene(0);
    }

    public void save()
    {
        party.save();
        inventory.save();
        StoryManager.save();
        partyDisplay.SetActive(!active);
        active = !active;
    }

	public void loadLevel(int index)
    {
        if (party.partyMembers.Count == 0 && index > 3 && index < 16)
        {
            partyDisplay.SetActive(!active);
            active = !active;
        }
        else SceneManager.LoadScene(index);
    }

}
