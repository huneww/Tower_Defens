using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private MoveMent moveMent;
    private Transform target;
    private int damage;

    public void Setup(Transform target, int damage)
    {
        moveMent = GetComponent<MoveMent>();
        this.target = target;
        this.damage = damage;
    }

    private void Update()
    {
        if (target != null)
        {
            // 발사체를 target의 위치로 이동
            Vector3 distance = (target.position - transform.position).normalized;
            moveMent.MoveTo(distance);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 적이 아닌 대상과 부딪히면
        if (!collision.CompareTag("Enemy")) return;
        // 현재 target인 적이 아닐 떄
        if (collision.transform != target) return;

        // 적 체력을 damage만큼 감소
        collision.GetComponent<EnemyHp>().TakeDamage(damage);
        // 발사체 오브젝트 제거
        Destroy(gameObject);
    }
}
