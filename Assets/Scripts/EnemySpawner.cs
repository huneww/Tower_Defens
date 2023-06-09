using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    // 적 프리팹
    [SerializeField]
    private GameObject enemyPrefab;
    // 소환 주기
    [SerializeField]
    private float spawnTime;
    // 현재 스테이지 이동 경로
    [SerializeField]
    private Transform[] wayPoints;
    // 현재 맵에 존재하는 모든 적의 정보
    private List<Enemy> enemyList;

    // 적의 생성과 색제는 EnemySpawner에서 하기 때문에 set은 필요 없음
    public List<Enemy> EnemyList => enemyList;

    private void Awake()
    {
        // 적 리스트 메모리 할당
        enemyList = new List<Enemy>();
        // 적 생성 코루틴 메서드 실행
        StartCoroutine(SpawnEnemy());
    }

    IEnumerator SpawnEnemy()
    {
        while (true)
        {
            // 적 오브젝트 생성
            GameObject clone = Instantiate(enemyPrefab);
            // 생성된 적의 Enemy 컴포넌트
            Enemy enemy = clone.GetComponent<Enemy>();
            // wayPoints 정보를 매개변수로 SetUp메서드 호출
            enemy.Setup(wayPoints);
            // 리스트에 생성된 적 정보 저장
            enemyList.Add(enemy);
            // spawnTime 시간 동안 대기
            yield return new WaitForSeconds(spawnTime);
        }
    }

    public void DestroyEnemy(Enemy enemy)
    {
        // 
    }
}
