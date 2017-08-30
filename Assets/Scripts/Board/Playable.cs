using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Random = UnityEngine.Random;

public class Playable : Mob {

    Ray ray;
    RaycastHit hit;
    private Party party;
    private float restartLevelDelay = 1.5f;
    private int level = 1, specialTurn = 0;
    public Canvas spellHolder;
    public Inventory inventory;
    public Canvas itemHolder;
    public int specialTurns;

    public enum STATE {
        NORMAL, ATTACK, MAGIC, ITEM, SPECIAL, DEFENDING
    };

    public STATE state = STATE.NORMAL;

    [HideInInspector] public bool usingItem = false;
    [HideInInspector] public Item itemChosen;

    void Awake()
    {
        direction = 0;

        party = FindObjectOfType<Party>();
        index = party.partyMembers.IndexOf(this);

        hp = maxHP;
        mp = maxMP;
        pwr = basePwr;
        mag = baseMag;
        def = baseDef;
        mDef = baseMDef;
        spd = baseSpd;
        moves = spd;

        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        display = GetComponentInChildren<Text>();
    }

    // Update is called once per frame
    void Update () {
        //DISPLAY
        //Hovering text
        if (turn) display.fontStyle = FontStyle.BoldAndItalic;
        else display.fontStyle = FontStyle.Bold;
        if (hovering)
        {
            display.text =
                "HP: " + hp + "/" + maxHP;
            if (turn) display.text += 
                    "\nMP: " + mp + "/" + maxMP +
                    "\n" + "AP: " + moves + " / " + spd;

            //Potential Heal
            try
            {
                if (BoardManager.activePlayer.getState() == 2 && BoardManager.activePlayer.spellChosen.GetComponent<Spell>().type == "Cure")
                {
                    display.text += "\n+" + (int)(BoardManager.activePlayer.spellChosen.GetComponent<Spell>().damage * BoardManager.activePlayer.getMag());
                }
                if (BoardManager.activePlayer.getState() == 3)
                {
                    if (!turn) display.text += "\nMP: " + mp + "/" + maxMP;
                    display.text += "\n+" + BoardManager.activePlayer.itemChosen.value;
                }
            }
            catch { }
        }
        else display.text = "";

        base.Update();

        //MOVEMENT (with collision detection)
        if (!turn || moving || asleep || BoardManager.doingSetup) return;

        int horizontal = 0;
        int vertical = 0;
        horizontal = (int)(Input.GetAxisRaw("Horizontal"));
        vertical = (int)(Input.GetAxisRaw("Vertical"));

        if (Input.GetKeyDown(KeyCode.W)) vertical = 1;
        else if (Input.GetKeyDown(KeyCode.S)) vertical = -1;
        if (Input.GetKeyDown(KeyCode.A)) horizontal = -1;
        else if (Input.GetKeyDown(KeyCode.D)) horizontal = 1;

        Vector3 newPosition = new Vector3(0.0f, 0.0f, 0.0f);

        if (horizontal != 0 || vertical != 0)
        {
            if (horizontal == 1)
            {
                direction = 0;
                animator.SetInteger("Direction", 0);
                newPosition.x++;
            }
            else if (horizontal == -1)
            {
                direction = 1;
                animator.SetInteger("Direction", 1);
                newPosition.x--;
            }
            else if (vertical == 1) newPosition.y++;
            else if (vertical == -1) newPosition.y--;

            if (!collisionDetection(newPosition, horizontal, vertical) && moves > 0)
            {
                transform.Translate(newPosition);
                StartCoroutine(move(1, .15f));
            }
        }

        //COMMANDS LIST
        checkNumCommands();
    }

    protected override void nextCharacter()
    {
        resetMenu();
        for (int i = index+1; i < party.partyMembers.Count; i++)
        {
            if (party.partyMembers[i].getDead() || party.partyMembers[i].checkSleep() || party.partyMembers[i].getSpecial())
            {
                party.partyMembers[i].setTurn(false);
                party.partyMembers[i].setGone(true);
            }
            else {
                party.partyMembers[i].setTurn(true);
                return;
            }
        }
    }

    public override void setTurn(bool b)
    {
        turn = b;
        if (turn)
        {
            def = baseDef;
            animator.SetBool("Defending", false);
        }
    }

    private bool collisionDetection(Vector3 newPosition, int xDir, int yDir)
    {
        if (moves == 0) return true;
        else if (transform.position.x + newPosition.x < 0) return true;
        else if (transform.position.x + newPosition.x > BoardManager.columns-1) return true;
        else if (transform.position.y + newPosition.y < 0) return true;
        else if (transform.position.y + newPosition.y > BoardManager.rows-1) return true;

        RaycastHit2D hit;
        if (!Move(xDir, yDir, out hit)) return true;

        return false;
    }

    //Next Stage
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Coins")
        {
            inventory.GetComponent<Inventory>().money += other.GetComponent<Coins>().getValue();
            BoardManager.coinsCollected += other.GetComponent<Coins>().getValue();
            other.gameObject.SetActive(false);
        }
        else if (other.tag == "Exit")
        {
            Invoke("Restart", restartLevelDelay);
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    //Character KO
    public override void kill()
    {
        dead = true;
        animator.SetBool("Dead", true);
        rejuvenate();
    }

    public int getLevel()
    {
        return level;
    }

    public Party getParty()
    {
        return party;
    }

    public override List<Mob> getMembers()
    {
        List<Mob> mobs = new List<Mob>();

        foreach (Playable player in party.partyMembers)
        {
            mobs.Add(player);
        }

        return mobs;
    }

    //COMMANDS
    public void checkNumCommands()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) attack_button();
        else if (Input.GetKeyDown(KeyCode.Alpha2)) magic_button();
        else if (Input.GetKeyDown(KeyCode.Alpha3)) item_button();
        else if (Input.GetKeyDown(KeyCode.Alpha4)) defend_button();
        else if (Input.GetKeyDown(KeyCode.Alpha5)) special_button();
        else if (Input.GetKeyDown(KeyCode.Alpha6)) endTurn();
        else if (Input.GetKeyDown(KeyCode.Alpha7)) resetMenu();
    }

    //1. ATTACK
    public void attack_button()
    {
        if (moves >= 2)
        {
            resetMenu();
            state = STATE.ATTACK;
            targetSelection = true;
        }
    }

    public void attackTarget(Enemy target)
    {
        Vector3 e = target.transform.position;
        Vector3 p = transform.position;
        bool canHit = false;
        int dir;

        if (name == "Steiner") //1x1 area
        {
            if (Mathf.Abs(p.x - e.x) <= 1 && Mathf.Abs(p.y - e.y) <= 1) canHit = true;
        }
        else if (name == "Lobish") //2x2 area
        {
            if (Mathf.Abs(p.x - e.x) <= 3 && Mathf.Abs(p.y - e.y) <= 3) canHit = true;
        }
        else if (name == "Ricard") //Left and Right only
        {
            if (Math.Abs(p.x - e.x) <= 1 && p.y == e.y) canHit = true;
        }

        //1x1 cross - Oscar, Heide, Dimmler
        else if (Math.Abs(p.x - e.x) <= 1 && p.y == e.y) canHit = true;
        else if (p.x == e.x && Math.Abs(p.y - e.y) <= 1) canHit = true;

        if (p.x <= e.x) dir = 0;
        else dir = 1;

        animator.SetInteger("Direction", dir);
        if (!canHit) return;
        animator.SetTrigger("Attacking");

        //Physical Damage
        int attack = (int)((pwr / target.getDef()) * target.getDefMod());
        targetSelection = false;

        target.setHP(-attack);

        StartCoroutine(move(2, .9f));
    }

    public void attackObstacle(Destructible target)
    {
        Vector3 e = target.transform.position;
        Vector3 p = transform.position;
        bool canHit = false;
        int dir;

        if (name == "Oscar") //1x1 cross
        {
            if (Math.Abs(p.x - e.x) <= 1 && p.y == e.y) canHit = true;
            else if (p.x == e.x && Math.Abs(p.y - e.y) <= 1) canHit = true;
        }
        else if (name == "Lobish") //2x2 area
        {
            if (Mathf.Abs(p.x - e.x) <= 3 && Mathf.Abs(p.y - e.y) <= 3) canHit = true;
        }

        //1x1 area
        else if (Mathf.Abs(p.x - e.x) <= 1 && Mathf.Abs(p.y - e.y) <= 1) canHit = true;

        if (p.x <= e.x) dir = 0;
        else dir = 1;

        animator.SetInteger("Direction", dir);
        if (!canHit) return;
        animator.SetTrigger("Attacking");

        target.hit();

        StartCoroutine(move(2, 1f));
    }

    //2. MAGIC
    public void magic_button()
    {
        if (state == STATE.MAGIC) resetMenu();
        else if (moves >= 1)
        {
            resetMenu();
            spellHolder.gameObject.SetActive(true);
            state = STATE.MAGIC;
        }
        else resetMenu();
    }

    public void chooseSpell(GameObject spell)
    {
        spellChosen = spell;
    }

    public void handleList(bool boolean)
    {
        spellHolder.gameObject.SetActive(boolean);
    }

    public GameObject getSpell()
    {
        return spellChosen;
    }

    public int getMagicDamage()
    {
        return (int)(spellChosen.GetComponent<Spell>().damage * mag);
    }

    //3. ITEM
    public void item_button()
    {
        if (state == STATE.ITEM)
        {
            state = STATE.NORMAL;
            itemHolder.gameObject.SetActive(false);
        }

        else {
            resetMenu();
            state = STATE.ITEM;
            itemHolder.gameObject.SetActive(true);

            foreach (Item item in inventory.items)
            {
                item.reset();
            }
        }
    }

    public void handleInventory(bool boolean)
    {
        itemHolder.gameObject.SetActive(boolean);
    }

    public void chooseItem(Item item)
    {
        itemChosen = item;
    }

    //4. DEFEND
    public void defend_button()
    {
        if (moves < 2) return;

        animator.SetBool("Defending", true);
        def *= 2;
        endTurn();
    }

    //5. SPECIAL
    public void special_button()
    {
        if (moves != spd) return;

        special = true;
        animator.SetBool("Special", true);
        specialStart();
        endTurn();
    }

    public void specialStart()
    {
        specialTurn = 0;

        switch(Name)
        {
            case "Oscar": mDef = 5; break;
            case "Dimmler": break;
            case "Lobish": break;
            case "Steiner": break;
            case "Heide": break;
            case "Ricard": break;
            default: break;
        }
    }

    public void specialFinish()
    {
        switch (Name)
        {
            case "Oscar": mDef = baseMDef; break;
            case "Dimmler": setMP(maxMP/2);  break;
            case "Lobish": break;
            case "Steiner": break;
            case "Heide": break;
            case "Ricard": break;
            default: break;
        }

        special = false;
        animator.SetBool("Special", false);
    }

    public void setSpecial(bool boolean)
    {
        special = boolean;
        if (!special) specialTurn = 0;
    }

    public int getSpecialTurn()
    {
        return specialTurn;
    }

    public void incrementSpecial()
    {
        if (special) specialTurn++;
    }

    public bool getSpecial()
    {
        return special;
    }

    //7. RESET
    public override void resetMenu()
    {
        state = STATE.NORMAL;

        targetSelection = false;

        handleInventory(false);
        handleList(false);
        spellChosen = null;
        itemChosen = null;   
    }

    public int getState()
    {
        switch(state)
        {
            case STATE.ATTACK: return 1;
            case STATE.MAGIC: return 2;
            case STATE.ITEM: return 3;
            default: return 0;
        }
    }

    public void normal()
    {
        state = STATE.NORMAL;
    }

    public override void checkAilments()
    {
        if (ill) setMP(-maxMP / 20);
        if (poisoned) setHP(-maxHP / 20);
        if (paralyzed) spd = baseSpd / 2;
        else spd = baseSpd;

        moves = spd;
    }

    public bool checkSleep()
    {
        if (asleep)
        {
            int chance = Random.Range(1, 10);
            if (chance < 5) return true;
            else {
                asleep = false;
                return false;
            }
        }
        else return false;
    }

    //SAVE AND LOAD DATA
    public void save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/" + Name + ".dat");

        PlayerData data = new PlayerData();
        data.level = level;
        data.hp = maxHP;
        data.mp = maxMP;
        data.pwr = basePwr;
        data.mag = baseMag;
        data.speed = baseSpd;

        bf.Serialize(file, data);
        file.Close();
    }

    public void load()
    {
        if (File.Exists(Application.persistentDataPath + "/" + Name + ".dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/" + Name + ".dat", FileMode.Open);

            PlayerData data = bf.Deserialize(file) as PlayerData;
            file.Close();

            level = data.level;
            maxHP = data.hp;
            maxMP = data.mp;
            basePwr = data.pwr;
            baseMag = data.mag;
            baseSpd = data.speed;

            resetStats();
        }
    }

    public void resetStats()
    {
        hp = maxHP;
        mp = maxMP;
        pwr = basePwr;
        mag = baseMag;
        def = baseDef;
        mDef = baseMDef;
        spd = baseSpd;
        moves = spd;

        //rejuvenate();
        setDefMod(1);
        setMagDefMod(1);
    }
}

[Serializable]
class PlayerData
{
    public int level, hp, mp, pwr, mag, speed;
}