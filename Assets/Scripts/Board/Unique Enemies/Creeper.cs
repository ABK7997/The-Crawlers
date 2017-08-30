using UnityEngine;
using System.Collections;

public class Creeper : Enemy
{

    public GameObject spawn1;
    public GameObject spawn2;
    public GameObject spawn3;

    void Start()
    {
        GameObject instance = Instantiate(spawn1, new Vector3(1f, 2f, 0f), Quaternion.identity) as GameObject;
        BoardManager.addEnemy(instance.GetComponent<Enemy>());

        GameObject instance2 = Instantiate(spawn2, new Vector3(5f, 0f, 0f), Quaternion.identity) as GameObject;
        BoardManager.addEnemy(instance2.GetComponent<Enemy>());

        GameObject instance3 = Instantiate(spawn3, new Vector3(0f, 6f, 0f), Quaternion.identity) as GameObject;
        BoardManager.addEnemy(instance3.GetComponent<Enemy>());
    }

    public override void behavior()
    {
        Vector3 position = transform.position;
        Vector3 newPosition = new Vector3(0f, 0f, 0f);
        Playable player = findClosestPlayer();
        Vector3 target = player.transform.position;

        if (BoardManager.turn % 3 == 0 && magic(player)) ; //Cast magic from a distance

        else if (!attack(player)) moveSpace(player, newPosition); //If player is out of range, move closer

        else poisonPlayer(player, 25); //Player can be poisoned if hit by a physical attack
    }

    public override bool magic(Playable player)
    {
        if (mp <= 0) return false;
        animator.SetTrigger("Magic");
        spellChosen = spells[Random.Range(0, spells.Length)];

        Vector3 target = player.transform.position;
        Vector3 position = transform.position;

        if (Mathf.Abs(target.x - position.x) <= 5 && Mathf.Abs(target.y - position.y) <= 5)
        {
            spellChosen.SetActive(true);
            spellChosen.GetComponent<Spell>().startAnimation(this, player);
            return true;
        }
        else return false;
    }

}
