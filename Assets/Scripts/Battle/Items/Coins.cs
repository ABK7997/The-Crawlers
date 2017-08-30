using UnityEngine;
using System.Collections;

public class Coins : MonoBehaviour {

    public int lowerBound;
    public int upperBound;
    private int value;

    void Awake()
    {
        value = Random.Range(lowerBound, upperBound);
    }

    public int getValue()
    {
        return value;
    }

}
