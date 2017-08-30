using UnityEngine;
using UnityEngine.UI;

public class Spell : MonoBehaviour
{

    public string Name, type, direction;
    public int cost, apCost, speed;
    public float damage;
    public int range;

    public Sprite sprite1;
    public Sprite sprite2;
    protected int anim = 0;
    protected SpriteRenderer render;

    [HideInInspector]
    public bool animating = false;
    [HideInInspector]
    public Mob user, target;
    [HideInInspector]

    void Awake()
    {
        render = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        if (animating) animate(target);
    }

    public virtual void animate(Mob target)
    {
        anim++;
        if (anim < 8) render.sprite = sprite1;
        else render.sprite = sprite2;

        if (anim > 16) anim = 0;

        Vector3 p = target.transform.position;
        Vector3 e = transform.position;

        transform.position += new Vector3(p.x - e.x, p.y - e.y, 0f) * Time.deltaTime * speed;

        if (Mathf.Abs(e.x - p.x) <= 1f && Mathf.Abs(e.y - p.y) <= 1f)
        {
            user.startMove(0, .75f);
            cast(target);
            animating = false;
            gameObject.SetActive(false);
        }
    }

    public virtual void cast(Mob target)
    {
        switch(type)
        {
            case "Off": target.setHP((int)(((-damage * user.getMag())/target.getMagDef()) * target.getMagDefMod())); break;
            case "Cure": target.setHP((int)(damage * user.getMag())); break;
            case "Def": target.setDefMod(damage); break;
            case "MagDef": target.setMagDefMod(damage); break;
            case "Omnis": target.setDefMod(damage); target.setMagDefMod(damage); break;
            case "Dispel": target.setDefMod(1); target.setMagDefMod(1); break;
            case "Rejuvenate": target.rejuvenate(); break;
            case "Revive": target.revive(); break;
            default: break;
        }

        transform.rotation = Quaternion.identity;
    }

    public virtual void startAnimation(Mob user, Mob target)
    {
        gameObject.SetActive(true);

        this.user = user;
        user.setMP(-cost);

        this.target = target;

        transform.position = new Vector3(user.transform.position.x, user.transform.position.y, 0f);

        Vector3 p = user.transform.position;
        Vector3 e = target.transform.position;

        if (p.x < e.x) user.GetComponent<Animator>().SetInteger("Direction", 0);
        else if (p.x > e.x) user.GetComponent<Animator>().SetInteger("Direction", 1);

        if (p.x < e.x && p.y < e.y) transform.Rotate(new Vector3(0f, 0f, 90f));
        else if (p.x > e.x && p.y < e.y) transform.Rotate(new Vector3(0f, 0f, 180f));
        else if (p.x > e.x && p.y > e.y) transform.Rotate(new Vector3(0f, 0f, -90f));

        user.GetComponent<Animator>().SetTrigger("Magic");
        enabled = true;
        animating = true; anim = 0;

        user.alterMoveCount(-apCost);
    }

    public bool checkSpellRange(Mob user, Mob target)
    {
        Vector3 p = user.transform.position;
        Vector3 e = target.transform.position;

        if (direction == "fullRange")
        {
            if (Mathf.Abs(p.x - e.x) <= range && Mathf.Abs(p.y - e.y) <= range) return true;
        }

        else if (direction == "straight") //Cross
        {
            if (Mathf.Abs(p.x - e.x) <= range && p.y == e.y) return true;
            else if (p.x == e.x && Mathf.Abs(p.x - p.y) <= range) return true;
        }

        return false;
    }
}
