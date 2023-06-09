using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyDestroyType { Kill = 0, Arrive}

public class Enemy : MonoBehaviour
{
    // �̵� ��� ����
    private int wayPointCount;
    // �̵� ��� ����
    private Transform[] wayPoints;
    // ���� ��ǥ���� �ε���
    private int currentIndex;
    // ������Ʈ �̵� ����
    private MoveMent moveMent;
    // ���� ������ ������ ���� �ʰ� EnemySpawner�� �˷��� ����
    private EnemySpawner enemySpawner;
    // �� ����� ȹ�� ���
    [SerializeField]
    private int gold = 10;

    public void Setup(EnemySpawner enemySpawner, Transform[] wayPoints)
    {
        moveMent = GetComponent<MoveMent>();
        this.enemySpawner = enemySpawner;

        // �� �̵� ��� WayPoints ���� ����
        wayPointCount = wayPoints.Length;
        this.wayPoints = new Transform[wayPointCount];
        this.wayPoints = wayPoints;

        // ���� ��ġ�� ù ��° wayPoints ��ġ�� ����
        transform.position = wayPoints[0].position;

        // �� �̵�/��ǥ���� ���� �ڷ�ƾ �޼��� ����
        StartCoroutine(OnMove());
    }

    IEnumerator OnMove()
    {
        // ���� �̵� ���� ����
        NextMoveTo();

        while (true)
        {
            // �� ������Ʈ ȸ��
            transform.Rotate(transform.forward * 10);

            // ���� ������ġ�� ��ǥ��ġ�� �Ÿ��� 0.02 * moveMent.MoveSpeed���� ���� �� ���ǹ� ����
            // moveMent.MoveSpeed�� �����ִ� ������ �ӵ��� ������ �� �����ӿ� 0.02���� ũ�� �����̱� ������
            // ���ǹ��� �ɸ��� �ʰ� ��θ� Ż���ϴ� ������Ʈ�� �߻��� �� �ִ�.
            if (Vector3.Distance(transform.position, wayPoints[currentIndex].position) < 0.02f * moveMent.MoveSpeed)
            {
                NextMoveTo();
            }

            yield return null;
        }
    }

    private void NextMoveTo()
    {
        // ���� �̵��� wayPoints�� ���Ҵٸ�
        if (currentIndex < wayPointCount - 1)
        {
            // ���� ��ġ�� ��Ȯ�ϰ� ��ǥ��ġ�� ����
            transform.position = wayPoints[currentIndex].position;
            // ���� ��ǥ ���� ����
            currentIndex++;
            Vector3 direction = (wayPoints[currentIndex].position - transform.position).normalized;
            moveMent.MoveTo(direction);
        }
        // ���� ��ġ��  ������ wayPoints���
        else
        {
            // ��ǥ������ �����ؼ� ����� ���� ���� ���� �ʵ���
            gold = 0;
            // �� ������Ʈ ����
            OnDie(EnemyDestroyType.Arrive);
        }
    }

    public void OnDie(EnemyDestroyType type)
    {
        // EnemySpawner���� ����Ʈ�� �� ������ �����ϱ� ������ Destroy()�� �������� �ʰ�
        // EnemySpawner���� ������ ������ �� �ʿ��� ó���� �ϵ��� DestroyEnemy() �޼��� ȣ��
        enemySpawner.DestroyEnemy(type, this, gold);
    }
}
