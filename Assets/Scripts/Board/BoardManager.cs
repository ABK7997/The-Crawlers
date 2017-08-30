using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BoardManager : MonoBehaviour {

    //Board Components - Stage specific
    [HideInInspector] public static int columns, rows;
    private GameObject exit;
    private GameObject[] floorTiles;
    private GameObject[] obstacles;
    private GameObject[] enemies;
    private GameObject boss;
    private GameObject[] outerWalls;
    private GameObject[] coins;
    private String stageName;
    private int[][] enemyArrangements;
    private int[][] floorArrangements;
    private int[][] obstacleArrangements;
    private int[][] coinArrangements;
    private int numStages;
    private string bossName;

    //Misc
    public static Coins[] coinsList;
    public static List<Enemy> activeEnemies = new List<Enemy>();
    public static List<Destructible> destructibles = new List<Destructible>();

    //Board - General
    private int level = 0;
    private Transform boardHolder;
    private List<Vector3> gridPositions = new List<Vector3>();
    public Party party;
    public LayerMask mask;
    private bool enemiesMoving = false;

    //GameManager
    public float levelStartDelay = 1.5f;
    private Text levelText;
    private GameObject levelImage;
    public static bool doingSetup = true;
    private bool inGame = false;

    public static BoardManager instance;
    private static StageManager assets;
    [HideInInspector] public static Playable activePlayer;
    [HideInInspector] public static Enemy targetEnemy;
    [HideInInspector] public static Playable targetPlayer;
    [HideInInspector] public static int enemyCount, turn = 0, coinsCollected = 0, overX = -3, overY = -21;

    //End of Area
    private GameObject endDisplay;

    public void setAssets()
    {
        assets = FindObjectOfType<StageManager>();

        exit = assets.exit;
        floorTiles = assets.floorTiles;
        obstacles = assets.obstacles;
        enemies = assets.enemies;
        outerWalls = assets.outerWalls;
        coins = assets.coins;
        boss = assets.boss;
        bossName = assets.bossName;
        stageName = assets.stageName;

        enemyArrangements = assets.enemyArrangements;
        floorArrangements = assets.floorArrangements;
        obstacleArrangements = assets.obstacleArrangements;
        coinArrangements = assets.coinArrangements;
        numStages = assets.numStages;

        columns = assets.columns;
        rows = assets.rows;

        assets.mainCamera.orthographicSize = 5 - ((8 - columns)/2f);
        assets.mainCamera.transform.Translate(0f, -(8 - columns)/2f, 0f);
        overX = assets.overX;
        overY = assets.overY;
    }

    void OnLevelWasLoaded()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
        DontDestroyOnLoad(gameObject);

        party = FindObjectOfType<Party>();

        if (SceneManager.GetActiveScene().buildIndex > 3 && SceneManager.GetActiveScene().buildIndex < 16)
        {
            setAssets();
            level++;
            endDisplay = GameObject.Find("EndDisplay");
            endDisplay.SetActive(false);
            inGame = true;

            if (level > numStages)
            {
                endLevel();
                return;
            }

            turn = 0;
            InitGame();
        }
        else if (SceneManager.GetActiveScene().name == "DeathScreen" || SceneManager.GetActiveScene().buildIndex == 0) Destroy(gameObject);
        else {
            inGame = false;
            level = 0;
        }
    }

    void Update()
    {
        if (!inGame) return;

        //EXIT
        if (GameObject.FindGameObjectWithTag("Exit") == null && enemyCount == 0 && inGame)
        {
            for (int i = 0; i < coinsList.Length; i++)
            {
                coinsList[i].gameObject.SetActive(false);
            }
            Instantiate(exit, new Vector3(columns - 1, rows - 1, 0f), Quaternion.identity);
        }

        targetPlayer = null; targetEnemy = null;

        //PLAYER MOVEMENTS/////////////////////////////////////
        if (!party.hasGone() || enemiesMoving || doingSetup)
        {
            try { //choose current player and player or enemy the mouse is hovering over
                foreach (Playable player in party.partyMembers)
                {
                    if (player.getTurn()) activePlayer = player;
                    if (player.hovering) targetPlayer = player;
                }
                foreach (Enemy e in activeEnemies)
                {
                    if (e.hovering) targetEnemy = e;
                }
            }
            catch {  return;   }

            if (Input.GetMouseButtonDown(1)) //Cancel moves
            {
                activePlayer.resetMenu();
            }

            else if (activePlayer.targetSelection && Input.GetMouseButtonDown(0) && activePlayer.getState() != 0)
            {
                try
                {
                    switch (activePlayer.getState())
                    {
                        //Attack
                        case 1:
                            foreach (Destructible destructible in destructibles)
                            {
                                if (destructible.hovering)
                                {
                                    activePlayer.attackObstacle(destructible);
                                    return;
                                }
                            }

                            activePlayer.attackTarget(targetEnemy); break;

                        //Magic
                        case 2:
                            if (activePlayer.getSpell() == null) break;
                            if (activePlayer.getSpell().GetComponent<Spell>().type == "Off")
                            {
                                if (!activePlayer.getSpell().GetComponent<Spell>().checkSpellRange(activePlayer, targetEnemy)) return;
                                activePlayer.getSpell().GetComponent<Spell>().startAnimation(activePlayer, targetEnemy);
                                activePlayer.move(activePlayer.getSpell().GetComponent<Spell>().apCost, 1f);
                            }
                            else
                            {
                                if (!activePlayer.getSpell().GetComponent<Spell>().checkSpellRange(activePlayer, targetPlayer)) return;
                                activePlayer.getSpell().GetComponent<Spell>().startAnimation(activePlayer, targetPlayer);
                                activePlayer.move(activePlayer.getSpell().GetComponent<Spell>().apCost, 1f);
                            }
                            break;

                        //Item
                        case 3:
                            if (activePlayer.itemChosen == null) break;
                            activePlayer.itemChosen.use(targetPlayer);
                            StartCoroutine(activePlayer.move(1, .1f));
                            break;
                    }
                } catch { }
            }
        }
        /////////////////////////////////////////////////////////
        else StartCoroutine(MoveEnemies());
    }

    void InitializeList()
    {
        gridPositions.Clear();

        for (int x = 1; x < columns-1; x++)
        {
            for (int y = 1; y < rows-1; y++) gridPositions.Add(new Vector3(x, y, 0f));
        }
    }

    void BoardSetup()
    {
        boardHolder = new GameObject("Board").transform;
        for (int x = -1; x < columns + 1; x++)
        {
            for (int y = -1; y < rows + 1; y++)
            {
                GameObject toInstantiate = floorTiles[Random.Range(floorArrangements[level-1][0], floorArrangements[level-1][1])];

                if (level == numStages) toInstantiate = floorTiles[Random.Range(floorArrangements[0][0], floorArrangements[0][1]+1)];
                //Outer Wall
                if (x == -1 || x == columns || y == -1 || y == rows) toInstantiate = outerWalls[Random.Range(0, outerWalls.Length)];

                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
                instance.transform.SetParent(boardHolder);
            }
        }
    }

    Vector3 RandomPosition()
    {
        int randomIndex = Random.Range(0, gridPositions.Count);
        Vector3 randomPosition = gridPositions[randomIndex];
        gridPositions.RemoveAt(randomIndex);

        return randomPosition;
    }

    void LayoutObjectAtRandom(GameObject[] tiles, int min, int max, int firstIndex, int lastIndex)
    {
        int objectCount = Random.Range(min, max + 1);

        for (int i = 0; i < objectCount; i++)
        {
            Vector3 randomPosition = RandomPosition();
            GameObject tileChoice = tiles[Random.Range(firstIndex, lastIndex)];
            Instantiate(tileChoice, randomPosition, Quaternion.identity);
        }
    }

    //Create single board
    public void SetupScene(int level)
    {
        BoardSetup();
        InitializeList();

        if (level == numStages)
        {
            Vector3 bossPosition = new Vector3(assets.bossX, assets.bossY, 0f);
            Instantiate(boss, bossPosition, Quaternion.identity);
        }
        else {
            LayoutObjectAtRandom(obstacles, obstacleArrangements[level - 1][2], obstacleArrangements[level - 1][3], obstacleArrangements[level - 1][0], obstacleArrangements[level - 1][1]);
            Destructible[] list = FindObjectsOfType(typeof(Destructible)) as Destructible[];

            for (int i = 0; i < list.Length; i++)
            {
                destructibles.Add(list[i]);
            }

            for (int i = 0; i < enemies.Length; i++)
            {
                LayoutObjectAtRandom(enemies, enemyArrangements[level - 1][i * 2], enemyArrangements[level - 1][(i*2) + 1], i, i+1);
            }

            LayoutObjectAtRandom(coins, coinArrangements[level - 1][2], coinArrangements[level - 1][3], coinArrangements[level - 1][0], coinArrangements[level - 1][1]);
        }
        coinsList = FindObjectsOfType(typeof(Coins)) as Coins[];
        Enemy[] enemyList = FindObjectsOfType(typeof(Enemy)) as Enemy[];

        for (int i = 0; i < enemyList.Length; i++)
        {
            activeEnemies.Add(enemyList[i]);
        }
        enemyCount = activeEnemies.Count;
    }

    //GAME MANAGEMENT/////////////
    void InitGame()
    {
        doingSetup = true;

        levelImage = GameObject.Find("LevelImage");
        levelText = GameObject.Find("LevelText").GetComponent<Text>();

        if (level == numStages) levelText.text = stageName + " - " + bossName;
        else levelText.text = stageName + " - " + level;

        levelImage.SetActive(true);
        Invoke("HideLevelImage", levelStartDelay);

        SetupScene(level);
    }

    private void HideLevelImage()
    {
        levelImage.SetActive(false);
        levelText.enabled = false;
        doingSetup = false;
    }

    public void GameOver()
    {
        levelText.text = "Your crawlers have fallen";
        levelImage.SetActive(true);

        enabled = false;
    }

    public static void addEnemy(Enemy enemy)
    {
        activeEnemies.Add(enemy);
        enemyCount++;
    }

    IEnumerator MoveEnemies()
    {
        float turnDelay = 1f;
        enemiesMoving = true;
        yield return new WaitForSeconds(.5f);

        for (int i = 0; i < activeEnemies.Count; i++)
        {
            activeEnemies[i].behavior();
            yield return new WaitForSeconds(turnDelay);

            if (activeEnemies[i].turns == 2)
            {
                activeEnemies[i].behavior();
                yield return new WaitForSeconds(turnDelay);
            }

            if (activeEnemies[i].turns == 3)
            {
                activeEnemies[i].behavior();
                yield return new WaitForSeconds(turnDelay);
            }

            if (activeEnemies[i].turns == 4)
            {
                activeEnemies[i].behavior();
                yield return new WaitForSeconds(turnDelay);
            }

            if (party.partyIsDead())
            {
                yield return new WaitForSeconds(turnDelay);
                SceneManager.LoadScene("DeathScreen"); 
            }
        }

        enemiesMoving = false;
        turn++;
        party.resetTurns();

        foreach (Enemy enemy in activeEnemies)
        {
            enemy.dmg = 0;
            enemy.checkAilments();
        }

        yield return new WaitForSeconds(turnDelay);
    }

    //END DISPLAY
    public void endLevel()
    {
        inGame = false;
        foreach (Playable player in party.partyMembers)
        {
            player.gameObject.SetActive(false);
        }

        endDisplay.SetActive(true);
        endDisplay.GetComponentInChildren<Text>().text =

            "Cleared " + stageName + "\n" +
            "Coins Collected: " + coinsCollected + "\n" +
            "Areas Unlocked: " + assets.unlocked;

        coinsCollected = 0;
        level = 7;
    }

}
