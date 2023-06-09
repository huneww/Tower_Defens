using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    // �� ������
    [SerializeField]
    private GameObject enemyPrefab;
    // ��ȯ �ֱ�
    [SerializeField]
    private float spawnTime;
    // ���� �������� �̵� ���
    [SerializeField]
    private Transform[] wayPoints;
    // ���� �ʿ� �����ϴ� ��� ���� ����
    private List<Enemy> enemyList;

    // ���� ������ ������ EnemySpawner���� �ϱ� ������ set�� �ʿ� ����
    public List<Enemy> EnemyList => enemyList;

    private void Awake()
    {
        // �� ����Ʈ �޸� �Ҵ�
        enemyList = new List<Enemy>();
        // �� ���� �ڷ�ƾ �޼��� ����
        StartCoroutine(SpawnEnemy());
    }

    IEnumerator SpawnEnemy()
    {
        while (true)
        {
            // �� ������Ʈ ����
            GameObject clone = Instantiate(enemyPrefab);
            // ������ ���� Enemy ������Ʈ
            Enemy enemy = clone.GetComponent<Enemy>();
            // wayPoints ������ �Ű������� SetUp�޼��� ȣ��
            enemy.Setup(wayPoints);
            // ����Ʈ�� ������ �� ���� ����
            enemyList.Add(enemy);
            // spawnTime �ð� ���� ���
            yield return new WaitForSeconds(spawnTime);
        }
    }

    public void DestroyEnemy(Enemy enemy)
    {
        // 
    }
}
