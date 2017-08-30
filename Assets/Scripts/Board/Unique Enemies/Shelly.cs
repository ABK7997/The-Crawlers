using UnityEngine;
using System.Collections;

public class Shelly : Enemy
{
    private Vector3 spawner = new Vector3(2f, 2f, 0f);

    public override void behavior()
    {
        Vector3 position = transform.position;
        Vector3 newPosition = new Vector3(0f, 0f, 0f);
        Playable player = findClosestPlayer();
        Vector3 target = player.transform.position;

        if (BoardManager.turn % 2 == 1) //Heal Self
        {
            if (mp <= 0) return;
            spellChosen = spells[1];

            spellChosen.SetActive(true);
            spellChosen.GetComponent<Spell>().startAnimation(this, this);
        }

        else //Cast Offensive Magic
        {
            if (mp <= 0) return;
            spellChosen = spells[0];

            if (Mathf.Abs(target.x - position.x) <= range && Mathf.Abs(target.y - position.y) <= range)
            {
                spellChosen.SetActive(true);
                spellChosen.GetComponent<Spell>().startAnimation(this, player);
            }
        }
    }

}
