using UnityEngine;
using System.Collections;

public class ElderTree : Enemy {

    public GameObject spawn;
    private Vector3 spawner = new Vector3(2f, 2f, 0f);

    void Start()
    {
        GameObject instance = Instantiate(spawn, new Vector3(1f, 2f, 0f), Quaternion.identity) as GameObject;
        BoardManager.addEnemy(instance.GetComponent<Enemy>());

        GameObject instance2 = Instantiate(spawn, new Vector3(5f, 0f, 0f), Quaternion.identity) as GameObject;
        BoardManager.addEnemy(instance2.GetComponent<Enemy>());
    }

    public override void behavior()
    {
        Vector3 position = transform.position;
        Vector3 newPosition = new Vector3(0f, 0f, 0f);
        Playable player = findClosestPlayer();
        Vector3 target = player.transform.position;

        if (BoardManager.turn % 2 == 0) //Spawn Sapling
        {
            RaycastHit2D hit;
            if (Move((int)spawner.x, (int)spawner.y, out hit))
            {
                GameObject instance = Instantiate(spawn, spawner, Quaternion.identity) as GameObject;
                BoardManager.addEnemy(instance.GetComponent<Enemy>());
                spawner.x++;
                if (spawner.x >= 7) spawner.x = 0;
            }
            else
            {
                spawner.x++;
                if (spawner.x >= 7) spawner.x = 0;
                GameObject instance = Instantiate(spawn, spawner, Quaternion.identity) as GameObject;
                BoardManager.addEnemy(instance.GetComponent<Enemy>());
            }
            animator.SetTrigger("Magic");
        }

        else if (!attack(player)) { //Attack if player is in range. Cast magic otherwise
            if (mp <= 0) return;
            spellChosen = spells[Random.Range(0, spells.Length)];

            if (Mathf.Abs(target.x - position.x) <= 5 && Mathf.Abs(target.y - position.y) <= 5)
            {
                spellChosen.SetActive(true);
                spellChosen.GetComponent<Spell>().startAnimation(this, player);
            }
        }
    }

}
