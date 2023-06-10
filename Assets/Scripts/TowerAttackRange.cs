using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerAttackRange : MonoBehaviour
{
    public void OnAttackRange(Vector3 position, float range)
    {
        gameObject.SetActive(true);

        // ���� ���� ũ��
        float diamertor = range * 2.0f;
        transform.localScale = Vector3.one * diamertor;
        // ���� ���� ��ġ
        transform.position = position;
    }

    public void OffAttackRange()
    {
        gameObject.SetActive(false);
    }
}
