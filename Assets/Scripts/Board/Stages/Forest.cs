using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Forest : StageManager {

    private Party party;
    public GameObject partyMember;

    void Awake()
    {
        party = FindObjectOfType<Party>();

        //Coin Count: {index1, index2, countMin, countMax}
        coinArrangements = new int[][]
        {
            new int[] { 0, 1, 0, 0 }, //1
            new int[] { 0, 1, 1, 1 }, //2
            new int[] { 0, 1, 0, 0 }, //3
            new int[] { 0, 1, 1, 3 }, //4
            new int[] { 0, 1, 0, 0 }, //5
            new int[] { 1, 2, 1, 1 }, //6
            new int[] { 0, 0, 1, 1 }, //7
        };

        //Enemy Count: {min1, max1, min2, max2, min3, max3}
        enemyArrangements = new int[][]
        {
            new int[] { 1, 1, 0, 0, 0, 0 }, //1
            new int[] { 0, 0, 2, 2, 0, 0 }, //2
            new int[] { 2, 3, 0, 2, 0, 0 }, //3
            new int[] { 0, 0, 0, 0, 2, 2 }, //4
            new int[] { 1, 3, 1, 3, 1, 3 }, //5
            new int[] { 2, 2, 2, 2, 2, 2 }, //6
            new int[] { 0, 0, 2, 2, 0, 0 }, //7
        };

        //Floor
        floorArrangements = new int[][]
        {
            new int[] {0, 1}, //1
            new int[] {0, 2}, //2 
            new int[] {0, 2}, //3
            new int[] {0, 2}, //4
            new int[] {2, 4}, //5
            new int[] {2, 4}, //6
            new int[] {0, 1}, //7
        };

        //Obstacles, {index1, index2, countMin, countMax}
        obstacleArrangements = new int[][]
        {
            new int[] {0, 0, 0, 0}, //1
            new int[] {0, 0, 1, 3}, //2
            new int[] {0, 0, 2, 4}, //3
            new int[] {0, 0, 3, 6}, //4
            new int[] {1, 2, 3, 4}, //5
            new int[] {1, 2, 4, 7}, //6
            new int[] {0, 0, 0, 0}, //7
        };
    }

    public void foundDimmler()
    {
        //adds Dimmler to the player's party
        party.enableCharacter(1);

        StoryManager.areasAvailable = 4; //unlocks Beach and OldTown

        if (!StoryManager.stagesCleared[0])
        {
            SceneManager.LoadScene("Cutscene_FindingDimmler");
            StoryManager.stagesCleared[0] = true;
        }
        else SceneManager.LoadScene(1); //Return to Overworld if the area has been cleared before.
    }
}
