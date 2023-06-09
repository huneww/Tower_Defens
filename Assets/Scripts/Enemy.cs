using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyDestroyType { Kill = 0, Arrive}

public class Enemy : MonoBehaviour
{
    // 이동 경로 개수
    private int wayPointCount;
    // 이동 경로 정보
    private Transform[] wayPoints;
    // 현재 목표지점 인덱스
    private int currentIndex;
    // 오브젝트 이동 제어
    private MoveMent moveMent;
    // 적의 삭제를 본인이 하지 않고 EnemySpawner에 알려서 삭제
    private EnemySpawner enemySpawner;
    // 적 사망시 획득 골드
    [SerializeField]
    private int gold = 10;

    public void Setup(EnemySpawner enemySpawner, Transform[] wayPoints)
    {
        moveMent = GetComponent<MoveMent>();
        this.enemySpawner = enemySpawner;

        // 적 이동 경로 WayPoints 정보 설정
        wayPointCount = wayPoints.Length;
        this.wayPoints = new Transform[wayPointCount];
        this.wayPoints = wayPoints;

        // 적의 위치를 첫 번째 wayPoints 위치로 설정
        transform.position = wayPoints[0].position;

        // 적 이동/목표지점 설정 코루틴 메서드 실행
        StartCoroutine(OnMove());
    }

    IEnumerator OnMove()
    {
        // 다음 이동 방향 설정
        NextMoveTo();

        while (true)
        {
            // 적 오브젝트 회전
            transform.Rotate(transform.forward * 10);

            // 적의 현재위치와 목표위치의 거리가 0.02 * moveMent.MoveSpeed보다 작을 때 조건문 실행
            // moveMent.MoveSpeed를 곱해주는 이유는 속도가 빠르면 한 프레임에 0.02보다 크게 움직이기 때문에
            // 조건문에 걸리지 않고 경로를 탈주하는 오브젝트가 발생할 수 있다.
            if (Vector3.Distance(transform.position, wayPoints[currentIndex].position) < 0.02f * moveMent.MoveSpeed)
            {
                NextMoveTo();
            }

            yield return null;
        }
    }

    private void NextMoveTo()
    {
        // 아직 이동할 wayPoints가 남았다면
        if (currentIndex < wayPointCount - 1)
        {
            // 적의 위치를 정확하게 목표위치로 설정
            transform.position = wayPoints[currentIndex].position;
            // 다음 목표 지점 설정
            currentIndex++;
            Vector3 direction = (wayPoints[currentIndex].position - transform.position).normalized;
            moveMent.MoveTo(direction);
        }
        // 현재 위치가  마지막 wayPoints라면
        else
        {
            // 목표지점에 도달해서 사망할 때는 돈을 주지 않도록
            gold = 0;
            // 적 오브젝트 삭제
            OnDie(EnemyDestroyType.Arrive);
        }
    }

    public void OnDie(EnemyDestroyType type)
    {
        // EnemySpawner에서 리스트로 적 정보를 관리하기 때문에 Destroy()를 직접하지 않고
        // EnemySpawner에게 본인이 삭제될 때 필요한 처리를 하도록 DestroyEnemy() 메서드 호출
        enemySpawner.DestroyEnemy(type, this, gold);
    }
}
