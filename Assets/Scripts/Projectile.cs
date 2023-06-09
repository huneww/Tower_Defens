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
            // �߻�ü�� target�� ��ġ�� �̵�
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
        // ���� �ƴ� ���� �ε�����
        if (!collision.CompareTag("Enemy")) return;
        // ���� target�� ���� �ƴ� ��
        if (collision.transform != target) return;

        // �� ü���� damage��ŭ ����
        collision.GetComponent<EnemyHp>().TakeDamage(damage);
        // �߻�ü ������Ʈ ����
        Destroy(gameObject);
    }
}
