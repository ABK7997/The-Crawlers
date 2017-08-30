using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ItemSelectionMenu : MonoBehaviour {

    private Inventory inventory;
    public Text display;

    void Awake()
    {
        inventory = FindObjectOfType<Inventory>();
    }
	
	// Update is called once per frame
	void Update () {
        display.text = "Capacity: " + inventory.capacity + "/" + inventory.maxCapacity;
	}
}
