using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class Poisonous_Troll : Enemy {

    public override bool attack(Playable player)
    {
        Vector3 target = player.transform.position;
        Vector3 position = transform.position;

        if (Mathf.Abs(target.x - position.x) == range && Mathf.Abs(target.y - position.y) <= range)
        {
            player.setAsleep(false);
            animator.SetTrigger("Attacking");
            player.setHP((int)(-pwr / player.getDef()));

            poisonPlayer(player, 100);
            return true;
        }
        else return false;
    }

}
