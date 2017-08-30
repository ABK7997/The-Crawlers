using UnityEngine;
using System.Collections;

public class Switcher : Enemy {

    private int targetIndex;

    public override void behavior()
    {
        Vector3 position = transform.position;
        Vector3 newPosition = new Vector3(0f, 0f, 0f);
        Playable player;

        player = switchTarget();
        if (!attack(player)) moveSpace(player, newPosition);
    }

    public Playable switchTarget()
    {
        if (targetIndex == party.Count) targetIndex = 0;

        if (party[targetIndex].getDead()) targetIndex++;
        if (targetIndex == party.Count) targetIndex = 0;

        if (party[targetIndex].getDead()) targetIndex++;
        if (targetIndex == party.Count) targetIndex = 0;

        Playable p;

        p = party[targetIndex];

        targetIndex++;
        return p;
    }

}
