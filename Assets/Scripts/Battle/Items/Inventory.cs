using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;

public class Inventory : MonoBehaviour {

    [HideInInspector] public int capacity = 0;
    public int money, maxCapacity;
    public Item[] items;
    public static Inventory instance;
    public Canvas itemHolder;

	void Awake()
    {
        if (instance == null)
        {
            instance = this;
            load();
        }
        else if (instance != this) Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        capacity = maxCapacity - maxCapacity;
    }

    void OnLevelWasLoaded()
    {
        if (SceneManager.GetActiveScene().name == "DeathScreen" || SceneManager.GetActiveScene().buildIndex == 0) Destroy(gameObject);
    }

    public void save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/InventoryData.dat");

        InventoryData data = new InventoryData();
        //Save Stock
        data.itemStocks = new int[items.Length];

        int i = 0;
        foreach (Item item in items)
        {
            data.itemStocks[i] = item.maxCapacity;
            i++;
        }

        data.capacity = maxCapacity;
        data.money = money;

        bf.Serialize(file, data);
        file.Close();
    }

    public void load()
    {
        if (File.Exists(Application.persistentDataPath + "/InventoryData.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/InventoryData.dat", FileMode.Open);

            InventoryData data = bf.Deserialize(file) as InventoryData;
            file.Close();

            //Restore Stock
            int i = 0;
            foreach (Item item in items)
            {
                item.maxCapacity = data.itemStocks[i];
                i++;
            }

            maxCapacity = data.capacity;
            money = data.money;
        }
    }

}


[Serializable]
class InventoryData
{
    public int[] itemStocks;
    public int capacity;
    public int money;
}