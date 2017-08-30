using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Item : MonoBehaviour {

    public Party party;
    public string Name, type;
    public int value, value2, apCost, price, stock, weight, maxCapacity;

    private Text text;

    void Awake()
    {
        text = GetComponentInChildren<Text>();

        text.text = Name + "\n" + 
            "Stock: " + stock;
    }

    public void reset()
    {
        text.text = Name + "\n" +
            "Stock: " + stock;
    }

    public void button()
    {
        Playable p = BoardManager.activePlayer;

        if (stock == 0 || apCost > p.getMovements()) return;
        p.chooseItem(this);
        p.handleInventory(false);
        p.targetSelection = true;
    }

    public void use(Playable player)
    {
        switch(type)
        {
            case "potion": player.setHP(value); break;
            case "spice": player.setMP(value); break;
            case "elixir": player.setHP(value); player.setMP(value2); player.rejuvenate(); break;
            case "omnilixir":
                foreach (Playable p in player.getParty().partyMembers)
                {
                    p.setHP(value);
                    p.setMP(value2);
                    p.rejuvenate();
                }
                break;
            case "antidote": player.setPoisoned(false); break;
            case "vaccine": player.setIll(false); break;
            case "surrogate": player.setCursed(false); break;
            case "steroid": player.setParalyzed(false); break;
            case "bell": player.setAsleep(false); player.setSpeed(player.getBaseSpeed()); break;
            case "remedy": player.rejuvenate(); break;
            case "life": player.revive(); break;
            default: break;
        }

        stock--;
        maxCapacity--;
        text.text = Name + "\n" + "Stock: " + stock;
    }

}
