using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class OldTown : StageManager
{

    private Party party;
    public GameObject partyMember;

    void Awake()
    {
        party = FindObjectOfType<Party>();

        //Coin Count: {index1, index2, countMin, countMax}
        coinArrangements = new int[][]
        {
            new int[] { 0, 1, 0, 0 }, //1
            new int[] { 0, 1, 3, 4 }, //2
            new int[] { 1, 2, 1, 1 }, //3
            new int[] { 0, 1, 0, 0 }, //4
            new int[] { 1, 3, 2, 3 }, //5
            new int[] { 0, 1, 0, 0 }, //6
            new int[] { 2, 2, 2, 5 }, //7
            new int[] { 0, 0, 0, 0 }, //8
        };

        //Enemy Count: {min1, max1, min2, max2, min3, max3}
        enemyArrangements = new int[][]
        {
            new int[] { 2, 2, 0, 0, 0, 0, 0, 0 }, //1
            new int[] { 1, 1, 2, 2, 0, 0, 0, 0 }, //2
            new int[] { 1, 1, 1, 1, 1, 1, 1, 1 }, //3
            new int[] { 0, 0, 0, 0, 4, 5, 0, 0 }, //4
            new int[] { 3, 3, 2, 2, 0, 0, 1, 1 }, //5
            new int[] { 0, 1, 3, 4, 0, 0, 1, 3 }, //6
            new int[] { 2, 4, 1, 3, 1, 3, 0, 0 }, //7
            new int[] { 0, 0, 0, 0, 0, 0, 0, 0 }, //8
        };

        //Floor
        floorArrangements = new int[][]
        {
            new int[] {0, 1}, //1
            new int[] {0, 4}, //2 
            new int[] {0, 4}, //3
            new int[] {4, 8}, //4
            new int[] {4, 8}, //5
            new int[] {0, 8}, //6
            new int[] {0, 8}, //7
            new int[] {0, 1} //8
        };

        //Obstacles, {index1, index2, countMin, countMax}
        obstacleArrangements = new int[][]
        {
            new int[] {0, 0, 0, 0}, //1
            new int[] {0, 0, 1, 3}, //2
            new int[] {0, 1, 2, 4}, //3
            new int[] {0, 1, 3, 6}, //4
            new int[] {1, 2, 3, 4}, //5
            new int[] {1, 2, 4, 6}, //6
            new int[] {0, 2, 1, 2}, //7
            new int[] {0, 0, 0, 0}, //8
        };
    }

    public void foundLobish()
    {
        //adds Dimmler to the player's party
        party.enableCharacter(2);

        StoryManager.areasAvailable = 5; //unlocks Tunnels

        if (!StoryManager.stagesCleared[2])
        {
            SceneManager.LoadScene("Cutscene_FindingLobish");
            StoryManager.stagesCleared[2] = true;
        }
        else SceneManager.LoadScene(1); //Return to Overworld if the area has been cleared before.
    }
}
