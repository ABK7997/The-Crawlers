using UnityEngine;
using System.Collections;

public class Loader : MonoBehaviour {

    public BoardManager boardManager;

	// Use this for initialization
	void Awake () {
        if (BoardManager.instance == null) Instantiate(boardManager);
	}
}
