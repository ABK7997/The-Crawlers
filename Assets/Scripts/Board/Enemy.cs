using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class Enemy : Mob {

    protected Canvas canvas;
    protected List<Playable> party;
    protected List<Enemy> enemyParty;
    public GameObject[] spells;

    //Enemy-specific states
    public int range, turns; //physical attack range
    public string behaviorType;
    private int baseTurns;

    //BOSS
    protected bool isBoss = false;

    //Status immunities
    public bool poisonImmune, illnessImmune, paralysisImmune, sleepImmune;

    protected enum STATE
    {
        NORMAL, AILMENT, DEAD
    }

	// Use this for initialization
	void Awake () {
        direction = 1;
        enemy = true;

        hp = maxHP;
        mp = maxMP;
        pwr = basePwr;
        mag = baseMag;
        def = baseDef;
        mDef = baseMDef;
        spd = baseSpd;
        moves = spd;
        baseTurns = turns;

        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        display = GetComponentInChildren<Text>();

        party = FindObjectOfType<Party>().partyMembers;
        enemyParty = BoardManager.activeEnemies;
	}
	
	// Update is called once per frame
	void Update () {
        if (turn) display.fontStyle = FontStyle.BoldAndItalic;
        else display.fontStyle = FontStyle.Bold;

        if (hovering)
        {
            display.text = "HP: " + hp + "/" + maxHP;
            if (turn) display.text += "\n" + "AP: " + moves + " / " + spd;

            //Potential Damage
            try
            {
                if (BoardManager.activePlayer.getState() == 2
                && BoardManager.activePlayer.getSpell().GetComponent<Spell>().type == "Off")
                {
                    display.text += "\nDMG: -" + (int)((BoardManager.activePlayer.getMagicDamage() / mDef) * magDefMod);
                }
                else if (BoardManager.activePlayer.getState() == 1) display.text += "\nDMG: -" + (int)(BoardManager.activePlayer.getPwr() / def);
            }
            catch { }
        }
        else display.text = "";

        base.Update();
    }

    public Text getDisplay()
    {
        return display;
    }

    public override void kill()
    {
        gameObject.SetActive(false);
        BoardManager.enemyCount--;
        BoardManager.activeEnemies.Remove(this);
    }

    protected override void nextCharacter()
    {
    }

    public override List<Mob> getMembers()
    {
        List<Mob> mobs = new List<Mob>();

        foreach (Enemy enemy in enemyParty)
        {
            mobs.Add(enemy);
        }

        return mobs;
    }

    //MOVES
    public virtual void behavior()
    {
        Vector3 position = transform.position;
        Vector3 newPosition = new Vector3(0f, 0f, 0f);
        Playable player;

        switch(behaviorType)
        {
            case "Follow":
                player = findClosestPlayer();
                if (!attack(player)) moveSpace(player, newPosition);
                break;

            case "FollowHPHigh":
                player = findPlayerWithHighestHP();
                if (!attack(player)) moveSpace(player, newPosition);
                break;

            case "FollowHPLow":
                player = findPlayerWithHighestHP();
                if (!attack(player)) moveSpace(player, newPosition);
                break;

            case "FollowDefLow":
                player = findPlayerWithLowestHP();
                if (!attack(player)) moveSpace(player, newPosition);
                break;

            case "Immobile_Magic":
                player = findClosestPlayer();
                magic(player);
                break;

            default: break;
        }
    }

    //BEHAVIORS
    public Playable findClosestPlayer()
    {
        Playable p = party[0];
        Vector3 enemy = transform.position;
        Vector3 target = party[0].transform.position;

        foreach (Playable player in party)
        {
            Vector3 newTarget = player.transform.position;

            if (Mathf.Abs(enemy.x - newTarget.x) <= Mathf.Abs(enemy.x - target.x) ||
                Mathf.Abs(enemy.y - newTarget.y) < Mathf.Abs(enemy.y - target.y))
            {
                if (!player.getDead())
                {
                    p = player;
                    target = newTarget;
                }
            }
        }
        turnTowardsPlayer(p);

        return p;
    }

    public Playable findPlayerWithHighestHP()
    {
        Playable p = party[0];
        Vector3 enemy = transform.position;
        Vector3 target = party[0].transform.position;

        foreach (Playable player in party)
        {
            Vector3 newTarget = player.transform.position;

            if (player.getHP() > p.getHP())
            {
                p = player;
                target = newTarget;
            }
        }
        turnTowardsPlayer(p);

        return p;
    }

    public Playable findPlayerWithLowestHP()
    {
        Playable p = party[0];
        Vector3 enemy = transform.position;
        Vector3 target = party[0].transform.position;

        foreach (Playable player in party)
        {
            Vector3 newTarget = player.transform.position;

            if (player.getHP() <= p.getHP())
            {
                p = player;
                target = newTarget;
            }
        }
        turnTowardsPlayer(p);

        return p;
    }

    public Playable findPlayerWithLowestDef()
    {
        Playable p = party[0];
        Vector3 enemy = transform.position;
        Vector3 target = party[0].transform.position;

        foreach (Playable player in party)
        {
            Vector3 newTarget = player.transform.position;

            if ((player.getDef() * player.getDefMod()) < (p.getDef() * p.getDefMod()))
            {
                p = player;
                target = newTarget;
            }
        }
        turnTowardsPlayer(p);

        return p;
    }

    public void turnTowardsPlayer(Playable player)
    {
        Vector3 position = transform.position;
        Vector3 target = player.transform.position;

        if (position.x < target.x) animator.SetInteger("Direction", 0);
        else if (position.x > target.x) animator.SetInteger("Direction", 1);
    }

    //ATTACK
    public virtual bool attack(Playable player)
    {
        Vector3 target = player.transform.position;
        Vector3 position = transform.position;

        if (Mathf.Abs(target.x - position.x) <= range && Mathf.Abs(target.y - position.y) <= range)
        {
            player.setAsleep(false);
            animator.SetTrigger("Attacking");
            player.setHP((int)((-pwr / player.getDef()) * player.getDefMod()));
            return true;
        }
        else return false;
    }

    //MOVE
    public void moveSpace(Playable player, Vector3 newPosition)
    {
        Vector3 target = player.transform.position;
        Vector3 position = transform.position;

        if (Mathf.Abs(target.x - position.x) > Mathf.Abs(target.y - position.y))
        {
            if (position.x < target.x) newPosition.x++;
            else if (position.x > target.x) newPosition.x--;
        }
        else
        {
            if (position.y < target.y) newPosition.y++;
            else if (position.y > target.y) newPosition.y--;
        }

        RaycastHit2D hit;

        for (int i = 0; i < spd; i++)
        {
            if (Move((int)newPosition.x, (int)newPosition.y, out hit))
            {
                transform.Translate(newPosition);
            }
            else animator.SetTrigger("Immobile");
        }
    }

    //MAGIC
    public virtual bool magic(Playable player)
    {
        if (mp <= 0) return false;
        animator.SetTrigger("Magic");
        spellChosen = spells[Random.Range(0, spells.Length)];

        Vector3 target = player.transform.position;
        Vector3 position = transform.position;

        if (Mathf.Abs(target.x - position.x) <= range && Mathf.Abs(target.y - position.y) <= range)
        {
            spellChosen.SetActive(true);
            spellChosen.GetComponent<Spell>().startAnimation(this, player);
            return true;
        }
        else return false;
    }

    //STATUS ATTACKS
    public virtual void poisonPlayer(Playable player, int hit)
    {
        int chance = Random.Range(0, 100);
        if (chance <= hit) player.setPoisoned(true);
    }
    public virtual void sickenPlayer(Playable player, int hit)
    {
        int chance = Random.Range(0, 100);
        if (chance <= hit) player.setIll(true);
    }
    public virtual void paralyzePlayer(Playable player, int hit)
    {
        int chance = Random.Range(0, 100);
        if (chance <= hit) player.setParalyzed(true);
    }
    public virtual void drowzePlayer(Playable player, int hit)
    {
        int chance = Random.Range(0, 100);
        if (chance <= hit) player.setAsleep(true);
    }
    public virtual void cursePlayer(Playable player, int hit)
    {
        int chance = Random.Range(0, 100);
        if (chance <= hit) player.setCursed(true);
    }

    //STATUS AILMENTS
    public override void checkAilments()
    {
        if (ill)
        {
            setMP(-maxMP / 10);
        }
        if (poisoned)
        {
            setHP(-maxHP / 10);
        }
        if (paralyzed)
        {
            turns = baseTurns/2;
            if (turns == 0) turns = 1;
        }
        if (asleep)
        {
            int chance = Random.Range(1, 10);
            if (chance < 5)
            {
                endTurn();
            }
            else asleep = false;
        }
    }

    public override void setPoisoned(bool boolean)
    {
        if (!poisonImmune) base.setPoisoned(boolean);
    }
    public override void setIll(bool boolean)
    {
        if (!illnessImmune) base.setIll(boolean);
    }
    public override void setParalyzed(bool boolean)
    {
        if (!paralysisImmune) base.setParalyzed(boolean);
    }
    public override void setAsleep(bool boolean)
    {
        if (!sleepImmune) base.setAsleep(boolean);
    }

}
