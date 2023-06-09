using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    // �� ������
    //[SerializeField]
    //private GameObject enemyPrefab;
    // �� ü���� ��Ÿ���� Slider UI ������
    [SerializeField]
    private GameObject enemyHpSliderPrefab;
    // UI�� ǥ���� Canvas ������Ʈ�� Transform
    [SerializeField]
    private Transform canvasTransform;
    // ��ȯ �ֱ�
    //[SerializeField]
    //private float spawnTime;
    // ���� �������� �̵� ���
    [SerializeField]
    private Transform[] wayPoints;
    // �÷��̾� ü�� ������Ʈ
    [SerializeField]
    private PlayerHp playerHp;
    // �÷��̾� ��� ������Ʈ
    [SerializeField]
    private PlayerGold playerGold;
    // ���� ���̺� ����
    private Wave currentWave;
    // ���� ���̺꿡 �����ִ� �� ����
    private int currentEnemyCount;
    // ���� �ʿ� �����ϴ� ��� ���� ����
    private List<Enemy> enemyList;
    // ���� ������ ������ EnemySpawner���� �ϱ� ������ set�� �ʿ� ����
    public List<Enemy> EnemyList => enemyList;
    // ���� ���̺��� �����ִ� ��, �ִ� �� ����
    public int CurrentEnemyCount => currentEnemyCount;
    public int MaxEnemyCount => currentWave.maxEnemyCount;

    private void Awake()
    {
        // �� ����Ʈ �޸� �Ҵ�
        enemyList = new List<Enemy>();
        // �� ���� �ڷ�ƾ �޼��� ����
        //StartCoroutine(SpawnEnemy());
    }

    public void StartWave(Wave wave)
    {
        // �Ű������� �޾ƿ� ���̺� ���� ����
        currentWave = wave;
        // ���� ���̺��� �ִ� �� ���ڸ� ����
        currentEnemyCount = currentWave.maxEnemyCount;
        // ���� ���̺� ����
        StartCoroutine(SpawnEnemy());
    }

    IEnumerator SpawnEnemy()
    {
        // ���� ���̺꿡�� ������ �� ����
        int spawnEnemyCount = 0;

        // ���� ���̺꿡�� �����Ǿ�� �ϴ� ���� ���ڸ�ŭ ���� �����ϰ� �ڷ�ƾ ����
        while (spawnEnemyCount < currentWave.maxEnemyCount)
        {
            // ���̺꿡 �����ϴ� ���� ������ ���� ������ �� ������ ���� �����ϵ��� �����ϰ�, �� ������Ʈ ����
            int enemyIndex = Random.Range(0, currentWave.enemyPrefabs.Length);
            GameObject clone = Instantiate(currentWave.enemyPrefabs[enemyIndex]);
            Enemy enemy = clone.GetComponent<Enemy>();

            // wayPoints�� �Ű������� �޼��� ȣ��
            enemy.Setup(this, wayPoints);
            enemyList.Add(enemy);

            // �� ü���� ��Ÿ���� Slider UI ���� �� ����
            SpawnEnemyHpSlider(clone);

            // ���� ���̺꿡�� ������ ���� ���� ����
            spawnEnemyCount++;

            // currentWave.spawnTime ��ŭ ���
            yield return new WaitForSeconds(currentWave.spawnTime);
        }
    }

    public void DestroyEnemy(EnemyDestroyType type, Enemy enemy, int gold)
    {
        // ���� ��ǥ�������� �������� ��
        if (type == EnemyDestroyType.Arrive)
        {
            // �÷��̾��� ü�� -1
            playerHp.TakeDamage(1);
        }
        // ���� �÷��̾��� �߻�ü���� ������� ��
        else if (type == EnemyDestroyType.Kill)
        {
            // ���� ������ ���� ��� �� ��� ȹ��
            playerGold.CurrentGold += gold;
        }

        // ���� ����� ������ ���� ���̺��� ���� �� ���� ���� (UI��)
        currentEnemyCount--;
        // ����Ʈ���� ����ϴ� �� ���� ����
        enemyList.Remove(enemy);
        // �� ������Ʈ ����
        Destroy(enemy.gameObject);
    }

    private void SpawnEnemyHpSlider(GameObject enemy)
    {
        // �� ü���� ��Ÿ���� Slider UI ����
        GameObject sliderClone = Instantiate(enemyHpSliderPrefab);
        // Slider UI ������Ʈ�� parent("Canavs" ������Ʈ)�� �ڽ����� ����
        // UI�� ĵ������ �ڽĿ�����Ʈ�� �����Ǿ� �־�� ȭ�鿡 ���δ�.
        sliderClone.transform.SetParent(canvasTransform);
        // ���� �������� �ٲ� ũ�⸦ �ٽ� (1, 1, 1)�� ����
        sliderClone.transform.localScale = Vector3.one;

        // Slider UI�� �i�ƴٴ� ����� �������� ����
        sliderClone.GetComponent<SliderPositionAutoSetter>().Setup(enemy.transform);
        // Slider UI�� �ڽ��� ü�� ������ ǥ���ϵ��� ����
        sliderClone.GetComponent<EnemyHpViewer>().Setup(enemy.GetComponent<EnemyHp>());
    }
}
