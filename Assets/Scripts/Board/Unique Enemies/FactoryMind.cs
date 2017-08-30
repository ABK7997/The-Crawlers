using UnityEngine;
using System.Collections;

public class FactoryMind : Enemy
{

    public GameObject spawn1;
    public GameObject spawn2;
    public GameObject spawn3;
    private Vector3 spawner = new Vector3(2f, 2f, 0f);

    void Start()
    {
        GameObject instance = Instantiate(spawn3, new Vector3(0, 6f, 0f), Quaternion.identity) as GameObject;
        BoardManager.addEnemy(instance.GetComponent<Enemy>());

        GameObject instance2 = Instantiate(spawn3, new Vector3(6f, 6f, 0f), Quaternion.identity) as GameObject;
        BoardManager.addEnemy(instance2.GetComponent<Enemy>());
    }

    public override void behavior()
    {
        Vector3 position = transform.position;
        Vector3 newPosition = new Vector3(0f, 0f, 0f);
        Playable player = findClosestPlayer();
        Vector3 target = player.transform.position;

        if (BoardManager.turn % 4 == 0) //Spawn underlings
        {
            RaycastHit2D hit;
            if (Move((int)spawner.x, (int)spawner.y, out hit))
            {
                GameObject instance = Instantiate(spawn1, spawner, Quaternion.identity) as GameObject;
                BoardManager.addEnemy(instance.GetComponent<Enemy>());
                spawner.x++;
                if (spawner.x >= 8) spawner.x = 0;
            }
            else
            {
                spawner.x++;
                if (spawner.x >= 8) spawner.x = 0;
                GameObject instance = Instantiate(spawn1, spawner, Quaternion.identity) as GameObject;
                BoardManager.addEnemy(instance.GetComponent<Enemy>());
            }
            animator.SetTrigger("Magic");
        }

        if (BoardManager.turn % 5 == 0) //Spawn underlings
        {
            RaycastHit2D hit;
            if (Move((int)spawner.x, (int)spawner.y, out hit))
            {
                GameObject instance = Instantiate(spawn2, spawner, Quaternion.identity) as GameObject;
                BoardManager.addEnemy(instance.GetComponent<Enemy>());
                spawner.x++;
                if (spawner.x >= 8) spawner.x = 0;
            }
            else
            {
                spawner.x++;
                if (spawner.x >= 8) spawner.x = 0;
                GameObject instance = Instantiate(spawn2, spawner, Quaternion.identity) as GameObject;
                BoardManager.addEnemy(instance.GetComponent<Enemy>());
            }
            animator.SetTrigger("Magic");
        }

        else
        {

        }
    }

}
