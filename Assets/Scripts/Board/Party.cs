using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class Party : MonoBehaviour {

    public Playable[] allPlayers;
    public List<Playable> partyMembers;
    public static Party instance;
    public Canvas commands;

    private bool[] activeMembers = {true, false, false, false, false, false};

    void Awake()
    {
        if (instance == null)
        {
            load();
            instance = this;
        }
        else if (instance != this) Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    void OnLevelWasLoaded()
    {
        if (SceneManager.GetActiveScene().buildIndex > 3 && SceneManager.GetActiveScene().buildIndex < 16)
        {
            instantiateMembers();
            resetTurns();
            resetPositions();
        }
        else if (SceneManager.GetActiveScene().name == "DeathScreen" || SceneManager.GetActiveScene().buildIndex == 0) Destroy(gameObject);
        else {
            foreach (Playable player in partyMembers)
            {
                player.gameObject.SetActive(false);
            }
        }
    }

    public bool hasGone()
    {
        foreach (Playable player in partyMembers)
        {
            if (player.turnFinished()) continue;
            else return false;
        }

        commands.gameObject.SetActive(false);
        return true;
    }

    public void resetStats()
    {
        foreach (Playable player in partyMembers)
        {
            player.resetStats();
        }
    }

    public void resetTurns()
    {
        commands.gameObject.SetActive(true);

        foreach (Playable player in partyMembers)
        {
            player.setTurn(false);
            player.setGone(false);
            player.dmg = 0;
            player.checkAilments();

            //check specials
            player.incrementSpecial();
            if (player.getSpecialTurn() == player.specialTurns)
            {
                player.specialFinish();
            }
        }

        foreach (Playable player in partyMembers)
        {
            if (player.getDead() || player.checkSleep() || player.getSpecial())
            {
                player.setTurn(false);
                player.setGone(true);
            }
            else {
                player.setTurn(true);
                return;
            }
        }
    }

    public void resetPositions()
    {
        foreach (Playable player in partyMembers)
        {
            if (player.getHP() == 0)
            {
                partyMembers.Remove(player);
                player.gameObject.SetActive(false);
            }
            player.gameObject.transform.position.Set(partyMembers.IndexOf(player), 0.0f, 0.0f);
            player.rejuvenate();
        }
    }

    public void addMember(Playable player)
    {
        if (partyMembers.Count < 3)
        {
            partyMembers.Add(player);
            player.setIndex(partyMembers.IndexOf(player));
        }
    }

    public void instantiateMembers()
    {
        float i = 0f;

        foreach (Playable player in partyMembers)
        {
            Vector3 startingPosition = new Vector3(i, 0f, 0f);

            player.gameObject.SetActive(true);
            player.gameObject.transform.position = startingPosition;
            player.setGone(false);

            i++;
        }
    }

    public void clearParty()
    {
        partyMembers.Clear();
    }

    public bool partyIsDead()
    {
        foreach (Playable p in partyMembers)
        {
            if (p.isActiveAndEnabled) return false;
        }
        return true;
    }

    public void fillCharacters(bool[] partyAvailable)
    {
        for (int i = 0; i < allPlayers.Length; i++)
        {
            if (partyAvailable[i]) allPlayers[i].enabled = true;
        }
    }

    public void enableCharacter(int index)
    {
        allPlayers[index].enabled = true;
        activeMembers[index] = true;
    }

    //SAVE AND LOAD DATA
    public void save()
    {
        fillCharacters(activeMembers);
        foreach (Playable player in allPlayers)
        {
            player.save();
        }

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/PartyData.dat");

        PartyData data = new PartyData();
        data.partyAvailable = activeMembers;

        bf.Serialize(file, data);
        file.Close();
    }

    public void load()
    {
        foreach (Playable player in partyMembers)
        {
            player.load();
        }

        if (File.Exists(Application.persistentDataPath + "/PartyData.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/PartyData.dat", FileMode.Open);

            PartyData data = bf.Deserialize(file) as PartyData;
            file.Close();

            activeMembers = data.partyAvailable;

            fillCharacters(activeMembers);
        }
    }
}

[Serializable]
class PartyData
{
    public bool[] partyAvailable;
}
