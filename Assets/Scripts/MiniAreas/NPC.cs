using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NPC : MonoBehaviour {

    public PlayerMovement player;
    public string[] dialogue;
    public Canvas canvas;
    public Text text;
    private int i; //dialogue progression

    public void startDialogue()
    {
        canvas.gameObject.SetActive(true);
        text.text = dialogue[0];
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.E))
        {
            i++;
            if (i == dialogue.Length)
            {
                player.setState(0);
                canvas.gameObject.SetActive(false);
                return;
            }
            else text.text = dialogue[i];
        }
	}
}
