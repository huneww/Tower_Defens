using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponState { SerachTarget = 0, AttackTarget}

public class TowerWeapon : MonoBehaviour
{
    // �߻�ü ������
    [SerializeField]
    private GameObject projectilePrefab;
    // �߻�ü ���� ��ġ
    [SerializeField]
    private Transform spawnPoint;
    // ���� �ӵ�
    [SerializeField]
    private float attackRate = 0.5f;
    // ���� ����
    [SerializeField]
    private float attackRange = 2.0f;
    // ���ݷ�
    [SerializeField]
    private int attackDamage = 1;
    // Ÿ�� ���� ����
    private WeaponState weaponState = WeaponState.SerachTarget;
    // ���� ���
    private Transform attackTarget = null;
    // ���ӿ� �����ϴ� �� ���� ȹ��
    private EnemySpawner enemySpawner;

    public void Setup(EnemySpawner enemySpawner)
    {
        this.enemySpawner = enemySpawner;

        // ���� ���¸� WeaponState.SerachTarget���� ����
        ChangeState(WeaponState.SerachTarget);
    }

    public void ChangeState(WeaponState state)
    {
        // ���� ������̴� ���� ����
        StopCoroutine(weaponState.ToString());
        // ���� ����
        weaponState = state;
        // ���ο� ���� ���
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
        // �������� �Ÿ��� ���������κ��� ������ �̿��� ��ġ�� ���ϴ� �� ��ǥ�� �̿�
        // ���� = aractan(x/y)
        // x, y ������ ���ϱ�
        float dx = attackTarget.position.x - transform.position.x;
        float dy = attackTarget.position.y - transform.position.y;
        // x, y �������� �������� ���� ���ϱ�
        // ������ radian ������ ������ Mathf.Rad2Deg�� ���� �� ������ ����
        float degree = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, degree);
    }

    IEnumerator SerachTarget()
    {
        while (true)
        {
            // ���� ������ �ִ� ���� ã�� ���� ���� �Ÿ��� �ִ��� ũ�� ����
            float closesDisSqr = Mathf.Infinity;
            // EnemySpawner�� EnemyList�� �ִ� ���� �ʿ� �����ϴ� ��� �� �˻�
            for (int i = 0; i < enemySpawner.EnemyList.Count; i++)
            {
                float distance = Vector3.Distance(enemySpawner.EnemyList[i].transform.position, transform.position);
                // ���� �˻����� ���� �Ÿ��� ���ݹ��� ���� �ְ�, ������� �˻��� ������ �Ÿ��� ������
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
            // target�� �ִ��� �˻� (�ٸ� �߻�ü�� ���� ����, Goal �������� �̵��� ���� ��)
            if (attackTarget == null)
            {
                ChangeState(WeaponState.SerachTarget);
                break;
            }

            // target�� ���� ���� �ȿ� �ִ��� �˻� (���� ������ ����� ���ο� �� Ž��)
            float distance = Vector3.Distance(attackTarget.position, transform.position);
            if (distance > attackRange)
            {
                attackTarget = null;
                ChangeState(WeaponState.SerachTarget);
                break;
            }

            // attackRate ��ŭ ���
            yield return new WaitForSeconds(attackRate);

            // ����
            SpawnProjectile();
        }
    }

    private void SpawnProjectile()
    {
        GameObject clone = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.identity);
        clone.GetComponent<Projectile>().Setup(attackTarget, attackDamage);
    }
}
