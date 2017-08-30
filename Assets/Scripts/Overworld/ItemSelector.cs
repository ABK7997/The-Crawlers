using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ItemSelector : MonoBehaviour {

    private Inventory inventory;
    private Item item;
    private Text text;
    private Slider slider;
    private int maxCapacity;
    private float lastValue = 0;

    void Awake()
    {
        slider = GetComponentInChildren<Slider>();
        text = GetComponentInChildren<Text>();

        inventory = FindObjectOfType<Inventory>();

        foreach (Item i in inventory.items)
        {
            if (name == i.name) item = i;
        }

        maxCapacity = item.maxCapacity;

        slider.maxValue = maxCapacity;
        lastValue = slider.value;

        if (maxCapacity == 0) text.text = item.Name + "\n" + "Out of Stock";
        else text.text = item.Name + "\n" +
            "Stock: " + slider.value + "/" + maxCapacity;
    }

    void OnLevelWasLoaded()
    {
        maxCapacity = item.maxCapacity;

        slider.maxValue = maxCapacity;
        lastValue = slider.value;

        if (maxCapacity == 0) text.text = item.Name + "\n" + "Out of Stock";
        else text.text = item.Name + "\n" +
            "Stock: " + slider.value + "/" + maxCapacity;
    }

    void Update()
    {
        if (slider.value == lastValue) return;

        if (((int)(slider.value - lastValue)*item.weight) + inventory.capacity > inventory.maxCapacity)
        {
            slider.value = lastValue;
            return;
        }
        item.stock = (int)slider.value;
        inventory.capacity += ((int)(slider.value - lastValue) * item.weight);

        text.text = item.Name + "\n" +
            "Stock: " + slider.value + "/" + maxCapacity;

        lastValue = slider.value;
    }

}
