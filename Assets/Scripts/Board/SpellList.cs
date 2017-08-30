using UnityEngine;
using System.Collections;

public class SpellList : MonoBehaviour {

    public static SpellList instance;

	void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }
}
