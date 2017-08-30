using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Beach : StageManager
{
    private Inventory inventory;

    void Awake()
    {
        inventory = FindObjectOfType<Inventory>();

        //Coin Count: {index1, index2, countMin, countMax}
        coinArrangements = new int[][]
        {
            new int[] { 0, 2, 1, 1 }, //1
            new int[] { 0, 1, 0, 0 }, //2
            new int[] { 0, 1, 2, 2 }, //3
            new int[] { 0, 1, 0, 0 }, //4
            new int[] { 1, 2, 1, 1 }, //5
            new int[] { 1, 2, 1, 3 }, //6
            new int[] { 0, 0, 0, 0 }, //7
        };

        //Enemy Count: {min1, max1, min2, max2, min3, max3}
        enemyArrangements = new int[][]
        {
            new int[] { 2, 2, 0, 0, 0, 0 }, //1
            new int[] { 0, 0, 2, 4, 0, 0 }, //2
            new int[] { 0, 0, 0, 0, 2, 2 }, //3
            new int[] { 1, 2, 1, 2, 1, 1 }, //4
            new int[] { 2, 3, 0, 0, 1, 2 }, //5
            new int[] { 1, 2, 1, 2, 1, 3 }, //6
            new int[] { 0, 0, 0, 0, 0, 0 }, //7
        };

        //Floor
        floorArrangements = new int[][]
        {
            new int[] {0, 1}, //1
            new int[] {0, 3}, //2 
            new int[] {0, 4}, //3
            new int[] {0, 5}, //4
            new int[] {0, 6}, //5
            new int[] {0, 6}, //6
            new int[] {0, 0}, //7
        };

        //Obstacles, {index1, index2, countMin, countMax}
        obstacleArrangements = new int[][]
        {
            new int[] {0, 0, 0, 0}, //1
            new int[] {0, 0, 0, 0}, //2
            new int[] {0, 1, 1, 2}, //3
            new int[] {0, 1, 2, 3}, //4
            new int[] {1, 2, 3, 6}, //5
            new int[] {0, 2, 2, 5}, //6
            new int[] {0, 0, 0, 0}, //7
        };
    }

    public void increaseCapacity()
    {
        if (!StoryManager.stagesCleared[1])
        {
            inventory.maxCapacity += 2;

            //SceneManager.LoadScene("");
            StoryManager.stagesCleared[1] = true;
        }
        else SceneManager.LoadScene(1);
    }
}
