using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public abstract class Mob : MonoBehaviour {

    [HideInInspector] public bool enemy = false;
    public string Name;
    public int maxHP, maxMP, basePwr, baseMag, baseSpd;
    public double baseDef, baseMDef;
    protected int hp, mp, pwr, mag, spd, moves;
    protected double def, mDef;
    protected GameObject spellChosen;

    protected bool hasGone = false, dead = false, turn = false, moving = false, special = false;
    protected int index; //number in the party
    protected Text display; //textbox attached to all individual mobs
    public Text damageCounter; //displays damage or healing done to character
    public Image healthbar;
    public Image manabar;
    private float barWidth = 60f;
    [HideInInspector] public int dmg = 0;
    [HideInInspector] public bool hovering = false;

    //Status Effect
    protected bool poisoned = false, ill = false, cursed = false, paralyzed = false, asleep = false;
    public Image poison, illness, paralysis, curse, shell, field;
    public Text sleep;
    protected double defMod = 1, magDefMod = 1;

    //Commands
    [HideInInspector] public bool targetSelection = false;
    [HideInInspector] public int damage = 0;

    //Board Movement Variables
    public LayerMask blockingLayer;
    protected int direction;

    protected Animator animator;
    protected BoxCollider2D boxCollider;
    protected Rigidbody2D rb;
    protected float inverseMoveTime;

    protected virtual void Update()
    {
        if (dmg != 0) damageCounter.text = "" + dmg;
        else damageCounter.text = "";

        //Healthbar
        float health = hp * 1f;
        float maxHealth = maxHP * 1f;
        float ratio = health / maxHealth;
        healthbar.rectTransform.sizeDelta = new Vector2(ratio * barWidth, healthbar.rectTransform.sizeDelta.y);

        float mana = mp * 1f;
        float maxMana = maxMP * 1f;
        float mRatio = mana / maxMana;
        manabar.rectTransform.sizeDelta = new Vector2(mRatio * barWidth, manabar.rectTransform.sizeDelta.y);
    }

    //STAT GETTERS/SETTERS/////////////////////////////////

    //NAME
    public string getName()
    {
        return Name;
    }

    //HIT POINTS (HP)
    public int getHP()
    {
        return hp;
    }
    public int getMaxHP()
    {
        return maxHP;
    }
    public void setMaxHP(double num)
    {
        maxHP = (int) (maxHP * num);
    }
    public void setHP(int num)
    {
        hp += num; dmg = num;
        if (hp > maxHP) hp = maxHP;
        else if (hp <= 0)
        {
            dead = true;
            kill();
            hp = 0;
        }
        if (num < 0)
        {
            animator.SetTrigger("Hit");
            damageCounter.color = Color.red;
        }
        else damageCounter.color = Color.green;
    }

    //MAGIC POINTS (MP)
    public int getMP()
    {
        return mp;
    }
    public int getMaxMP()
    {
        return maxMP;
    }
    public void setMP(int num)
    {
        if (num < 0) damageCounter.color = new Color(255f, 0f, 255f);
        else damageCounter.color = Color.cyan;
        mp += num; dmg = num;
        if (mp > maxMP) mp = maxMP;
        else if (mp < 0) mp = 0;
    }
    public void setMaxMP(double num)
    {
        maxMP = (int)(maxMP * num);
    }

    //POWER (PWR)
    public int getPwr()
    {
        return pwr;
    }
    public int getBasePwr()
    {
        return basePwr;
    }
    public void setPwr(double num)
    {
        pwr = (int)(pwr * num);
    }
    public void setBasePwr(double num)
    {
        basePwr = (int)(basePwr * num);
    }

    //MAGIC (MAG)
    public int getMag()
    {
        return mag;
    }
    public int getBaseMag()
    {
        return baseMag;
    }
    public void setMag(double num)
    {
        mag = (int)(mag * num);
    }
    public void setBaseMag(double num)
    {
        baseMag = (int)(baseMag * num);
    }

    //SPEED
    public int getSpeed()
    {
        return spd;
    }
    public int getBaseSpeed()
    {
        return baseSpd;
    }
    public int getMovements()
    {
        return moves;
    }
    public void setMoves(int num)
    {
        moves = num;
    }
    public void alterMoveCount(int num)
    {
        moves += num;
    }

    public void setBaseSpeed(double num)
    {
        baseSpd = (int)(maxHP * num);
    }
    public void setSpeed(int num)
    {
        spd *= num;
    }

    //DEFENSE (DEF)
    public double getDef()
    {
        return def;
    }
    public double getBaseDef()
    {
        return baseDef;
    }
    public void setDef(double num)
    {
        def *= num;
    }
    public void setBaseDef(double num)
    {
        baseDef *= num;
    }

    //MAGIC DEFENSE (mDEF)
    public double getMagDef()
    {
        return mDef;
    }
    public double getBaseMagDef()
    {
        return baseMDef;
    }
    public void setMagDef(double num)
    {
        mDef *= num;
    }
    public void setBaseMagDef(double num)
    {
        baseMDef *= num;
    }

    //DEFENSE MODIFIERS for SHELL and FIELD
    public double getDefMod()
    {
        return defMod;
    }

    public double getMagDefMod()
    {
        return magDefMod;
    }

    public void setDefMod(double num)
    {
        defMod = num;

        if (num < 1) shell.gameObject.SetActive(true);
        else shell.gameObject.SetActive(false);
    }

    public void setMagDefMod(double num)
    {
        magDefMod = num;

        if (num < 1) field.gameObject.SetActive(true);
        else field.gameObject.SetActive(false);
    }

    //TURNS
    public bool turnFinished()
    {
        return hasGone;
    }
    public void setGone(bool b)
    {
        hasGone = b;
    }
    public void checkTurns()
    {
        if (moves <= 0) nextCharacter();
    }
    public bool getTurn()
    {
        return turn;
    }
    public virtual void setTurn(bool b)
    {
        turn = b;
    }
    public void endTurn()
    {
        moves = 0;
        StartCoroutine(move(0, 1f));
    }

    //DEATH
    public abstract void kill();

    public virtual List<Mob> getMembers()
    {
        return null;
    }

    //BOARD MOVEMENT
    public IEnumerator move(int moveCount, float waitTime)
    {
        moving = true;
        moves -= moveCount;
        yield return new WaitForSeconds(waitTime);
        if (moves <= 0)
        {
            moving = false;
            turn = false;
            hasGone = true;
            nextCharacter();
        }
        resetMenu();
        moving = false;
    }

    public void startMove(int moveCount, float waitTime)
    {
        StartCoroutine(move(moveCount, waitTime));
    }

    protected bool Move(int xDir, int yDir, out RaycastHit2D hit)
    {
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(xDir, yDir);

        boxCollider.enabled = false;
        hit = Physics2D.Linecast(start, end, blockingLayer);
        boxCollider.enabled = true;

        if (hit.transform == null) return true;
        else return false;
    }

    protected abstract void nextCharacter();

    void OnMouseOver()
    {
        hovering = true;
    }

    void OnMouseExit()
    {
        hovering = false;
    }

    public int getIndex()
    {
        return index;
    }
    public void setIndex(int num)
    {
        index = num;
    }

    public virtual void generateSpellList() { }

    public virtual void resetMenu() { }

    public virtual void changeState() { }

    //Status Effects
    public bool isAiled()
    {
        if (poisoned) return true;
        if (ill) return true;
        if (cursed) return true;
        if (paralyzed) return true;
        if (asleep) return true;
        return false;
    }
    public void rejuvenate()
    {
        setPoisoned(false);
        setIll(false);
        setParalyzed(false);
        setAsleep(false);
    }
    public virtual void checkAilments() { }

    public virtual void setPoisoned(bool boolean)
    {
        poisoned = boolean;
        if (poisoned) poison.gameObject.SetActive(true);
        else poison.gameObject.SetActive(false);

        if (isAiled()) animator.SetBool("Ailment", true);
        else animator.SetBool("Ailment", false);
    }
    public virtual void setIll(bool boolean)
    {
        ill = boolean;
        if (ill) illness.gameObject.SetActive(true);
        else illness.gameObject.SetActive(false);

        if (isAiled()) animator.SetBool("Ailment", true);
        else animator.SetBool("Ailment", false);
    }
    public void setCursed(bool boolean)
    {
        cursed = boolean;

        if (cursed) curse.gameObject.SetActive(true);
        else curse.gameObject.SetActive(false);

        if (isAiled()) animator.SetBool("Ailment", true);
        else animator.SetBool("Ailment", false);
    }
    public virtual void setParalyzed(bool boolean)
    {
        paralyzed = boolean;
        if (paralyzed) paralysis.gameObject.SetActive(true);

        if (isAiled()) animator.SetBool("Ailment", true);
        else animator.SetBool("Ailment", false);

        if (!paralyzed)
        {
            paralysis.gameObject.SetActive(false);
            spd = baseSpd;
            if (!hasGone) moves = spd;
        }
    }
    public virtual void setAsleep(bool boolean)
    {
        asleep = boolean;

        if (asleep) sleep.gameObject.SetActive(true);
        else sleep.gameObject.SetActive(false);

        if (isAiled()) animator.SetBool("Ailment", true);
        else animator.SetBool("Ailment", false);
    }

    public bool getDead()
    { 
        return dead;
    }

    //REVIVE
    public void revive()
    {
        dead = false;
        setHP(maxHP / 5);
        animator.SetBool("Dead", false);
    }
}
