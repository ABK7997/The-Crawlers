using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public abstract class StageManager : MonoBehaviour {

    //Stage-Specific objects
    public Camera mainCamera;
    public GameObject exit;
    public GameObject[] floorTiles;
    public GameObject[] obstacles;
    public GameObject[] enemies;
    public GameObject[] outerWalls;
    public GameObject[] coins;
    public GameObject boss;
    public string bossName;
    public string stageName;
    public string unlocked;

    public int bossX, bossY;

    [HideInInspector] public bool cleared = false;

    public int overX, overY;

    public int columns, rows; //Board size
    public int numStages;

    //Coin placement: {index1, index2, countMin, countMax}
    public int[][] coinArrangements;

    //Enemy Count: {index1, index2, countMin, countMax}
    public int[][] enemyArrangements;

    //Floor
    public int[][] floorArrangements;

    //Obstacles, {index1, index2, countMin, countMax}
    public int[][] obstacleArrangements;

    public void returnToOverworld()
    {
        SceneManager.LoadScene(1);
    }
}
