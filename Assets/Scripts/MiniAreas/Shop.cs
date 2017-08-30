using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Shop : MonoBehaviour {

    [HideInInspector] public int cost = 0;
    private Inventory inventory;
    public Text costDisplay;
    public Text moneyDisplay;
    private ShopItem[] items;
    public PlayerMovement player;
	
    void Awake()
    {
        inventory = FindObjectOfType<Inventory>();
        items = GetComponentsInChildren<ShopItem>();
    }

	// Update is called once per frame
	void Update () {
        costDisplay.text = "Cost: $" + cost;
        moneyDisplay.text = "Money: $" + inventory.money;
	}

    public void makePurchase()
    {
        inventory.money -= cost;

        foreach (ShopItem item in items)
        {
            item.purchase();
        }

        player.setState(0);
        gameObject.SetActive(false);
    }
}
