using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    // 적 프리팹
    [SerializeField]
    private GameObject enemyPrefab;
    // 적 체력을 나타내는 Slider UI 프리팹
    [SerializeField]
    private GameObject enemyHpSliderPrefab;
    // UI를 표현할 Canvas 오브젝트의 Transform
    [SerializeField]
    private Transform canvasTransform;
    // 소환 주기
    [SerializeField]
    private float spawnTime;
    // 현재 스테이지 이동 경로
    [SerializeField]
    private Transform[] wayPoints;
    // 플레이어 체력 컴포넌트
    [SerializeField]
    private PlayerHp playerHp;
    // 플레이어 골드 컴포넌트
    [SerializeField]
    private PlayerGold playerGold;
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
            enemy.Setup(this, wayPoints);
            // 리스트에 생성된 적 정보 저장
            enemyList.Add(enemy);
            // 적 체력을 나타내는 Slider UI 생성 및 설정
            SpawnEnemyHpSlider(clone);

            // spawnTime 시간 동안 대기
            yield return new WaitForSeconds(spawnTime);
        }
    }

    public void DestroyEnemy(EnemyDestroyType type, Enemy enemy, int gold)
    {
        // 적이 목표지점까지 도착했을 때
        if (type == EnemyDestroyType.Arrive)
        {
            // 플레이어의 체력 -1
            playerHp.TakeDamage(1);
        }
        // 적이 플레이어의 발사체에게 사망했을 때
        else if (type == EnemyDestroyType.Kill)
        {
            // 적의 종류에 따라 사망 시 골드 획득
            playerGold.CurrentGold += gold;
        }

        // 리스트에서 사망하는 적 정보 삭제
        enemyList.Remove(enemy);
        // 적 오브젝트 삭제
        Destroy(enemy.gameObject);
    }

    private void SpawnEnemyHpSlider(GameObject enemy)
    {
        // 적 체력을 나타내는 Slider UI 생성
        GameObject sliderClone = Instantiate(enemyHpSliderPrefab);
        // Slider UI 오브젝트를 parent("Canavs" 오브젝트)의 자식으로 설정
        // UI는 캔버스의 자식오브젝트로 설정되어 있어야 화면에 보인다.
        sliderClone.transform.SetParent(canvasTransform);
        // 계층 설정으로 바뀐 크기를 다시 (1, 1, 1)로 설정
        sliderClone.transform.localScale = Vector3.one;

        // Slider UI가 쫒아다닐 대상을 보인으로 설정
        sliderClone.GetComponent<SliderPositionAutoSetter>().Setup(enemy.transform);
        // Slider UI에 자신의 체력 정보를 표시하도록 설정
        sliderClone.GetComponent<EnemyHpViewer>().Setup(enemy.GetComponent<EnemyHp>());
    }
}
