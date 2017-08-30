using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class StoryManager : MonoBehaviour {

    public static int areasAvailable = 2;
    public static bool[] stagesCleared = new bool[12];

    public static void save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/StageData.dat");

        StageData data = new StageData();
        data.areasAvailable = areasAvailable;
        data.stagesCleared = stagesCleared;

        bf.Serialize(file, data);
        file.Close();
    }

    public static void load()
    {
        if (File.Exists(Application.persistentDataPath + "/StageData.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/StageData.dat", FileMode.Open);

            StageData data = bf.Deserialize(file) as StageData;
            file.Close();

            areasAvailable = data.areasAvailable;
            stagesCleared = data.stagesCleared;
        }
    }

}

[Serializable]
class StageData {

    public int areasAvailable;
    public bool[] stagesCleared;

}