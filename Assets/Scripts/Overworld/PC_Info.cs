using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class PC_Info : MonoBehaviour {

    private Party party;
    [HideInInspector] public Playable player;
    public string Name;
    private Text text;
    private Toggle toggle;

    void Awake()
    {
        party = FindObjectOfType<Party>();

        text = GetComponentInChildren<Text>();
        toggle = GetComponentInChildren<Toggle>();

        foreach (Playable p in party.allPlayers)
        {
            if (p.Name == Name && p.enabled) player = p;
        }

        if (player == null) gameObject.SetActive(false);
    }

    void Start()
    {
        text.text = player.getName() + "\n" +
            "HP: " + player.getMaxHP() + " \n" +
            "MP: " + player.getMaxMP() + " \n";
    }

    public void partySelection()
    {

        if (!toggle.isOn)
        {
            List<Playable> newParty = new List<Playable>();

            foreach (Playable p in party.partyMembers)
            {
                if (p.GetInstanceID() != player.GetInstanceID())
                {
                    newParty.Add(p);
                }
            }

            party.partyMembers.Clear();

            foreach (Playable p in newParty)
            {
                party.addMember(p);
            }
        }
        else {
            if (party.partyMembers.Count == 3) return;

            foreach (Playable p in party.partyMembers)
            {
                if (p.GetInstanceID() == player.GetInstanceID()) return;
            }

            party.addMember(player);
        }
    }

}
