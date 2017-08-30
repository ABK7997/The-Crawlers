using UnityEngine;
using System.Collections;

public class Destructible : MonoBehaviour {

    private SpriteRenderer render;
    public Sprite damaged;
    public int hp;
    private int maxHP;

    [HideInInspector] public bool hovering = false;

    void Awake()
    {
        maxHP = hp;
        render = GetComponent<SpriteRenderer>();
    }

    public void hit()
    {
        hp--;
        if (hp < maxHP)
        {
            render.sprite = damaged;
        }
        if (hp <= 0)
        {
            BoardManager.destructibles.Remove(this);
            gameObject.SetActive(false);
        }
    }

    void OnMouseOver()
    {
        hovering = true;
    }

    void OnMouseExit()
    {
        hovering = false;
    }
}
