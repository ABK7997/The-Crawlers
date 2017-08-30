using UnityEngine;
using System.Collections;

public class Ailment : Spell {

    public string ailment;
    public int chance;

    public override void cast(Mob target)
    {
        target.setHP((int)-damage * user.getMag());

        switch (ailment)
        {
            case "poison": poison(target, chance); break;
            case "virus": virus(target, chance); break;
            case "paralysis": paralysis(target, chance); break;
            case "drowze": drowze(target, chance); break;
            case "curse": curse(target, chance); break;
            default: break;
        }

        transform.rotation = Quaternion.identity;
    }

    public void poison(Mob target, int hit)
    {
        int chance = Random.Range(0, 100);
        if (chance <= hit)
        {
            target.setPoisoned(true);
        }
    }

    public void virus(Mob target, int hit)
    {
        int chance = Random.Range(0, 100);
        if (chance <= hit)
        {
            target.setIll(true);
        }
    }

    public void paralysis(Mob target, int hit)
    {
        int chance = Random.Range(0, 100);
        if (chance <= hit)
        {
            target.setParalyzed(true);
        }
    }

    public void drowze(Mob target, int hit)
    {
        int chance = Random.Range(0, 100);
        if (chance <= hit)
        {
            target.setAsleep(true);
        }
    }

    public void curse(Mob target, int hit)
    {
        int chance = Random.Range(0, 100);
        if (chance <= hit)
        {
            target.setCursed(true);
        }
    }

}
