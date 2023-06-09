using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHp : MonoBehaviour
{
    // 전체 화면을 덮을 빨간색 이미지
    [SerializeField]
    private Image screenImage;
    // 최대 체력
    [SerializeField]
    private float maxHp = 20;
    // 현재 체력
    private float currentHp;

    public float MaxHp => maxHp;
    public float CurrentHp => currentHp;

    private void Awake()
    {
        // 현재 체력을 최대 체력으로 설정
        currentHp = maxHp;
    }

    public void TakeDamage(float damage)
    {
        // 현재 체력을 damage만큼 감소
        currentHp -= damage;

        StopCoroutine(HitAlphaAnimation());
        StartCoroutine(HitAlphaAnimation());
        
        // 체력이 0이 되면 게임 오버
        if (currentHp <= 0)
        {

        }
    }

    private IEnumerator HitAlphaAnimation()
    {
        // 전체 화면 크기로 배치된 screenImage의 색상을 color 변수에 저장
        // screenImage의 투명도를 40%로 설정
        Color color = screenImage.color;
        color.a = 0.4f;
        screenImage.color = color;

        // 투명도가 0%가 될때까지 감소
        while (color.a >= 0.0f)
        {
            color.a -= Time.deltaTime;
            screenImage.color = color;

            yield return null;
        }
    }
}
