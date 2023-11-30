using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slow : MonoBehaviour
{
    private TowerWeapon tower_weapon;

    private void Awake()
    {
        tower_weapon = GetComponent<TowerWeapon>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy")) { return; }

        Move move = collision.GetComponent<Move>();

        move.Move_speed -= move.Move_speed * tower_weapon.Slow;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy")) { return; }

        collision.GetComponent<Move>().ResetMoveSpeed();
    }
}
