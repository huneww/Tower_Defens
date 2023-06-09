using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponState { SerachTarget = 0, AttackTarget}

public class TowerWeapon : MonoBehaviour
{
    // 발사체 프리펩
    [SerializeField]
    private GameObject projectilePrefab;
    // 발사체 생성 위치
    [SerializeField]
    private Transform spawnPoint;
    // 공격 속도
    [SerializeField]
    private float attackRate = 0.5f;
    // 공격 범위
    [SerializeField]
    private float attackRange = 2.0f;
    // 공격력
    [SerializeField]
    private int attackDamage = 1;
    // 타워 무기 상태
    private WeaponState weaponState = WeaponState.SerachTarget;
    // 공격 대상
    private Transform attackTarget = null;
    // 게임에 존재하는 적 정보 획득
    private EnemySpawner enemySpawner;

    public void Setup(EnemySpawner enemySpawner)
    {
        this.enemySpawner = enemySpawner;

        // 최초 상태를 WeaponState.SerachTarget으로 설정
        ChangeState(WeaponState.SerachTarget);
    }

    public void ChangeState(WeaponState state)
    {
        // 이전 재생중이던 상태 종료
        StopCoroutine(weaponState.ToString());
        // 상태 변경
        weaponState = state;
        // 새로운 상태 재생
        StartCoroutine(weaponState.ToString());
    }

    private void Update()
    {
        if (attackTarget != null)
        {
            RotateToTarget();
        }
    }

    private void RotateToTarget()
    {
        // 원점에서 거리와 수평축으로부터 각도를 이용해 위치를 구하는 극 좌표계 이용
        // 각도 = aractan(x/y)
        // x, y 변위값 구하기
        float dx = attackTarget.position.x - transform.position.x;
        float dy = attackTarget.position.y - transform.position.y;
        // x, y 변위값을 바탕으로 각도 구하기
        // 각도가 radian 단위기 때문에 Mathf.Rad2Deg를 곱해 도 단위를 구함
        float degree = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, degree);
    }

    IEnumerator SerachTarget()
    {
        while (true)
        {
            // 제일 가까이 있는 적을 찾기 위해 최초 거리를 최대한 크게 설정
            float closesDisSqr = Mathf.Infinity;
            // EnemySpawner의 EnemyList에 있는 현재 맵에 존재하는 모든 적 검사
            for (int i = 0; i < enemySpawner.EnemyList.Count; i++)
            {
                float distance = Vector3.Distance(enemySpawner.EnemyList[i].transform.position, transform.position);
                // 현재 검사중인 적과 거리가 공격범위 내에 있고, 현재까지 검사한 적보다 거리가 가까우면
                if (distance <= attackRange && distance <= closesDisSqr)
                {
                    closesDisSqr = distance;
                    attackTarget = enemySpawner.EnemyList[i].transform;
                }
            }

            if (attackTarget != null)
            {
                ChangeState(WeaponState.AttackTarget);
            }

            yield return null; 
        }
    }

    private IEnumerator AttackTarget()
    {
        while (true)
        {
            // target이 있는지 검사 (다른 발사체에 의해 제거, Goal 지점까지 이동해 삭제 등)
            if (attackTarget == null)
            {
                ChangeState(WeaponState.SerachTarget);
                break;
            }

            // target이 공격 범위 안에 있는지 검사 (공격 범위를 벗어나면 새로운 적 탐색)
            float distance = Vector3.Distance(attackTarget.position, transform.position);
            if (distance > attackRange)
            {
                attackTarget = null;
                ChangeState(WeaponState.SerachTarget);
                break;
            }

            // attackRate 만큼 대기
            yield return new WaitForSeconds(attackRate);

            // 공격
            SpawnProjectile();
        }
    }

    private void SpawnProjectile()
    {
        GameObject clone = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.identity);
        clone.GetComponent<Projectile>().Setup(attackTarget, attackDamage);
    }
}
