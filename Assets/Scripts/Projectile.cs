using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private MoveMent moveMent;
    private Transform target;

    public void Setup(Transform target)
    {
        moveMent = GetComponent<MoveMent>();
        this.target = target;
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

        // �� ��� �޼��� ȣ��
        collision.GetComponent<Enemy>().OnDie();
        // �߻�ü ������Ʈ ����
        Destroy(gameObject);
    }
}
