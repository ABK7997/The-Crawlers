using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SpellSelector : MonoBehaviour {

    private Text text;
    public Spell spell;

    void Awake()
    {
        text = GetComponentInChildren<Text>();
    }

    void Start()
    {
        text.text = spell.Name + "\n" +
            "MP:" + spell.cost + "  AP:" + spell.apCost;
    }

    public void button()
    {
        Playable p = BoardManager.activePlayer;

        if (spell.cost > p.getMP() || spell.apCost > p.getMovements()) return;
        p.chooseSpell(spell.gameObject);
        p.handleList(false);
        p.targetSelection = true;
    }

}
