using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerCommands : MonoBehaviour {

    public Text text;

    void OnLevelWasLoaded()
    {
        if (SceneManager.GetActiveScene().buildIndex < 4 || SceneManager.GetActiveScene().buildIndex > 16) gameObject.SetActive(false);
    }

    public void Update()
    {
        text.text = " " + BoardManager.activePlayer.getName();
    }

    //1. ATTACK
    public void button_attack()
    {
        BoardManager.activePlayer.attack_button();
    }

    //2. MAGIC
    public void button_magic()
    {
        BoardManager.activePlayer.magic_button();
    }

    //3. INVENTORY
    public void button_inventory()
    {
        BoardManager.activePlayer.item_button();
    }

    //4. DEFEND
    public void button_defend()
    {
        BoardManager.activePlayer.defend_button();
    }

    //5. SPECIAL
    public void button_special()
    {
        BoardManager.activePlayer.special_button();
    }

    //6. WAIT
    public void button_wait()
    {
        BoardManager.activePlayer.endTurn();
    }
}
