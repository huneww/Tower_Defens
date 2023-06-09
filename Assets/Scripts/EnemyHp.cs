using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHp : MonoBehaviour
{
    // �ִ� ü��
    [SerializeField]
    private float maxHp;
    // ���� ü��
    private float currentHp;
    // ���� ��������̸� isDie�� true�� ����
    private bool isDie = false;
    private Enemy enemy;
    private SpriteRenderer spriteRenderer;

    public float MaxHp => maxHp;
    public float CurrentHp => currentHp;

    private void Awake()
    {
        // ���� ü���� �ִ� ü������ ����
        currentHp = maxHp;
        enemy = GetComponent<Enemy>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(float damage)
    {
        // ���� ü���� damage ��ŭ �����ؼ� ���� ��Ȳ�� �� ���� Ÿ���� ������ ���ÿ� ������
        // enemy.OnDie() �޼��尡 ���� �� ����� �� �ִ�.

        // ���� ���� ���°� ��� �����̸� �Ʒ� �ڵ带 �������� �ʴ´�.
        if (isDie)
        {
            return;
        }

        // ���� ü���� damage ��ŭ ����
        currentHp -= damage;

        StopCoroutine(HitAlphaAnimator());
        StartCoroutine(HitAlphaAnimator());

        if (currentHp < 0)
        {
            isDie = true;
            // �� ���
            enemy.OnDie(EnemyDestroyType.Kill);
        }
    }

    private IEnumerator HitAlphaAnimator()
    {
        // ���� ���� ������ color ������ ����
        Color color = spriteRenderer.color;

        // ������  40%�� ����
        color.a = 0.4f;
        spriteRenderer.color = color;

        // 0.05�� ���� ���
        yield return new WaitForSeconds(0.05f);

        // ������ 100%�� ����
        color.a = 1.0f;
        spriteRenderer.color = color;
    }
}
