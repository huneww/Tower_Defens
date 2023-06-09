using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHp : MonoBehaviour
{
    // ��ü ȭ���� ���� ������ �̹���
    [SerializeField]
    private Image screenImage;
    // �ִ� ü��
    [SerializeField]
    private float maxHp = 20;
    // ���� ü��
    private float currentHp;

    public float MaxHp => maxHp;
    public float CurrentHp => currentHp;

    private void Awake()
    {
        // ���� ü���� �ִ� ü������ ����
        currentHp = maxHp;
    }

    public void TakeDamage(float damage)
    {
        // ���� ü���� damage��ŭ ����
        currentHp -= damage;

        StopCoroutine(HitAlphaAnimation());
        StartCoroutine(HitAlphaAnimation());
        
        // ü���� 0�� �Ǹ� ���� ����
        if (currentHp <= 0)
        {

        }
    }

    private IEnumerator HitAlphaAnimation()
    {
        // ��ü ȭ�� ũ��� ��ġ�� screenImage�� ������ color ������ ����
        // screenImage�� ������ 40%�� ����
        Color color = screenImage.color;
        color.a = 0.4f;
        screenImage.color = color;

        // ������ 0%�� �ɶ����� ����
        while (color.a >= 0.0f)
        {
            color.a -= Time.deltaTime;
            screenImage.color = color;

            yield return null;
        }
    }
}
