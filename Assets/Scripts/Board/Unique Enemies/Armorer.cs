using UnityEngine;
using System.Collections;

public class Armorer : Enemy
{

    public override void behavior()
    {
        if (mp <= 0) return;

        //Heal
        if (BoardManager.turn % 3 == 1)
        {
            spellChosen = spells[Random.Range(2, 4)];
            Enemy e = enemyParty[Random.Range(0, enemyParty.Count)];

            spellChosen.SetActive(true);
            spellChosen.GetComponent<Spell>().startAnimation(this, e);
        }

        //Defense
        else
        {
            spellChosen = spells[Random.Range(0, 2)];
            Enemy e = enemyParty[Random.Range(0, enemyParty.Count)];

            spellChosen.SetActive(true);
            spellChosen.GetComponent<Spell>().startAnimation(this, e);
        }

        animator.SetTrigger("Magic");
    }

}