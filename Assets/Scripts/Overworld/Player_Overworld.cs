using UnityEngine;
using System.Collections;

public class Player_Overworld : MonoBehaviour {

	void Awake () {
        transform.position = new Vector3(BoardManager.overX, BoardManager.overY, 0f);
	}
}
