using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    // �� ������
    [SerializeField]
    private GameObject enemyPrefab;
    // �� ü���� ��Ÿ���� Slider UI ������
    [SerializeField]
    private GameObject enemyHpSliderPrefab;
    // UI�� ǥ���� Canvas ������Ʈ�� Transform
    [SerializeField]
    private Transform canvasTransform;
    // ��ȯ �ֱ�
    [SerializeField]
    private float spawnTime;
    // ���� �������� �̵� ���
    [SerializeField]
    private Transform[] wayPoints;
    // �÷��̾� ü�� ������Ʈ
    [SerializeField]
    private PlayerHp playerHp;
    // �÷��̾� ��� ������Ʈ
    [SerializeField]
    private PlayerGold playerGold;
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
            enemy.Setup(this, wayPoints);
            // ����Ʈ�� ������ �� ���� ����
            enemyList.Add(enemy);
            // �� ü���� ��Ÿ���� Slider UI ���� �� ����
            SpawnEnemyHpSlider(clone);

            // spawnTime �ð� ���� ���
            yield return new WaitForSeconds(spawnTime);
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
