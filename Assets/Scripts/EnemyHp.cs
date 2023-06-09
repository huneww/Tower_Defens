using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHp : MonoBehaviour
{
    // 최대 체력
    [SerializeField]
    private float maxHp;
    // 현재 체력
    private float currentHp;
    // 적이 사망상태이면 isDie를 true로 변경
    private bool isDie = false;
    private Enemy enemy;
    private SpriteRenderer spriteRenderer;

    public float MaxHp => maxHp;
    public float CurrentHp => currentHp;

    private void Awake()
    {
        // 현재 체력을 최대 체력으로 설정
        currentHp = maxHp;
        enemy = GetComponent<Enemy>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(float damage)
    {
        // 적의 체력이 damage 만큼 감소해서 죽을 상황일 때 여러 타워의 공격을 동시에 받으면
        // enemy.OnDie() 메서드가 여러 번 실행될 수 있다.

        // 현재 적의 상태가 사망 상태이면 아래 코드를 실행하지 않는다.
        if (isDie)
        {
            return;
        }

        // 현재 체력을 damage 만큼 감소
        currentHp -= damage;

        StopCoroutine(HitAlphaAnimator());
        StartCoroutine(HitAlphaAnimator());

        if (currentHp < 0)
        {
            isDie = true;
            // 적 사망
            enemy.OnDie(EnemyDestroyType.Kill);
        }
    }

    private IEnumerator HitAlphaAnimator()
    {
        // 현재 적의 색상을 color 변수에 저장
        Color color = spriteRenderer.color;

        // 투명도를  40%로 설정
        color.a = 0.4f;
        spriteRenderer.color = color;

        // 0.05초 동아 대기
        yield return new WaitForSeconds(0.05f);

        // 투명도를 100%로 설정
        color.a = 1.0f;
        spriteRenderer.color = color;
    }
}
