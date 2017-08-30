using UnityEngine;
using System.Collections;

public class AoE : Spell {

    public override void animate(Mob target)
    {
        anim++;
        if (anim < 20) render.sprite = sprite1;
        else render.sprite = sprite2;

        if (anim == 50)
        {
            user.startMove(0, .75f);
            cast(target);
            animating = false;
            gameObject.SetActive(false);
        }
    }

    public override void cast(Mob target)
    {
        Vector3 pos = transform.position;

        switch (type)
        {
            case "Off":
                foreach (Mob mob in target.getMembers())
                {
                    Vector3 mPos = mob.transform.position;
                    if (Mathf.Abs(pos.x - mPos.x) <= range && Mathf.Abs(pos.y - mPos.y) <= range)
                    mob.setHP((int)((-damage * user.getMag()) / mob.getMagDef()));
                }
                break;
            case "Cure":
                foreach (Mob mob in user.getMembers())
                {
                    Vector3 mPos = mob.transform.position;
                    if (Mathf.Abs(pos.x - mPos.x) <= range && Mathf.Abs(pos.y - mPos.y) <= range)
                        mob.setHP((int)damage * user.getMag());
                }
                break;
        }

        transform.rotation = Quaternion.identity;
    }

    public override void startAnimation(Mob user, Mob target)
    {
        transform.position = user.transform.position;

        user.GetComponent<Animator>().SetTrigger("Magic");
        enabled = true;
        animating = true; anim = 0;
    }

}
