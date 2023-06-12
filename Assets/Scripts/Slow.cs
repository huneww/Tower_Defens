using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slow : MonoBehaviour
{
    private TowerWeapon towerWeapon;

    private void Awake()
    {
        towerWeapon = GetComponent<TowerWeapon>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy")) return;

        MoveMent moveMent = collision.GetComponent<MoveMent>();
        // 이동속도 = 이동속도 - 이동속도 * 감속률
        // 4 = 5 - 5 * 0.2
        moveMent.MoveSpeed -= moveMent.MoveSpeed * towerWeapon.Slow;

    }

    private void OnTriggerExitg2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy")) return;

        collision.GetComponent<MoveMent>().ResetMoveSpeed();
    }
}
