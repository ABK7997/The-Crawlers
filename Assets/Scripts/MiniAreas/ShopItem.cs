using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour {

    public Shop shop;
    private Inventory inventory;
    private Item item;
    private Text text;
    private Slider slider;

    private int stock;

    private int maxCapacity;
    private float lastValue = 0;

    void Awake()
    {
        inventory = FindObjectOfType<Inventory>();

        foreach (Item i in inventory.items)
        {
            if (name == i.name) item = i;
        }

        slider = GetComponentInChildren<Slider>();
        text = GetComponentInChildren<Text>();

        try
        {
            maxCapacity = inventory.money / item.price;
            text.text = item.Name + "\n" +
            "Stock: " + slider.value + "\n" +
            "Price: " + item.price;
        } catch { maxCapacity = 0; }

        slider.maxValue = maxCapacity;
        lastValue = slider.value;
    }

    void Update()
    {
        if (slider.value == lastValue) return;

        if ((int)((slider.value - lastValue) * item.price) + shop.cost > inventory.money)
        {
            slider.value = lastValue;
            return;
        }

        stock = (int)slider.value;

        shop.cost += ((int)(slider.value - lastValue) * item.price);

        text.text = item.Name + "\n" +
            "Stock: " + slider.value + "\n" +
            "Price: " + item.price;

        lastValue = slider.value;
    }

    public void purchase()
    {
        try
        {
            item.maxCapacity += stock;
            stock = 0;
            slider.value = 0;
        }
        catch { }
    }
}
