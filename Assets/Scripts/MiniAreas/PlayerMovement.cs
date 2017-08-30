using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour {

    public Shop shop;
    private bool nearVendor = false;

    private enum STATE {
        NORMAL, SHOP, DIALOGUE
    };

    private STATE state = STATE.NORMAL;

	//Movement
	private Rigidbody2D rb;
    public int speed = 4;
	private int dir = 0;

	//Player componenents
	private Animator animator;

	//Player actions
	private bool walking = false; //determines moving or idle animation

	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator>();
		rb = GetComponent<Rigidbody2D>(); 
		rb.freezeRotation = true;
	}
	
	// Update is called once per frame
	void Update () {
        
        switch(state)
        {
            case STATE.NORMAL:
                //Movement
                if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
                {
                    rb.transform.position += Vector3.left * speed * Time.deltaTime;
                    dir = 1; walking = true;
                }
                else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
                {
                    rb.transform.position += Vector3.right * speed * Time.deltaTime;
                    dir = 0; walking = true;
                }
                if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
                {
                    rb.transform.position += Vector3.up * speed * Time.deltaTime;
                    walking = true;
                }
                else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
                {
                    rb.transform.position += Vector3.down * speed * Time.deltaTime;
                    walking = true;
                }

                //Enter Shop
                if (Input.GetKeyDown(KeyCode.E) && nearVendor)
                {
                    state = STATE.SHOP;
                    shop.gameObject.SetActive(true);
                }
            break;

            default: break;
        }

        animator.SetBool("Walking", walking);
        animator.SetInteger("Direction", dir);

        walking = false;
	}

    void OnTriggerEnter2D (Collider2D other)
    {
        //Leave to Overworld
        if (other.tag == "Exit") SceneManager.LoadScene(1);

        if (other.tag == "Vendor") nearVendor = true;

        if (other.tag == "NPC")
        {
            other.GetComponent<NPC>().startDialogue();
            state = STATE.DIALOGUE;
        }
    }

    void OnTriggerExit2D (Collider2D other)
    {
        if (other.tag == "Vendor") nearVendor = false;
    }

    public void setState(int num)
    {
        switch(num)
        {
            case 0: state = STATE.NORMAL; break;
            case 1: state = STATE.SHOP; break;
            case 2: state = STATE.DIALOGUE; break;
        }
    }
}
