using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponState { SerachTarget = 0, AttackTarget}

public class TowerWeapon : MonoBehaviour
{
    // Ÿ������
    [SerializeField]
    private TowerTemplate towerTemplate;
    // �߻�ü ������
    [SerializeField]
    private GameObject projectilePrefab;
    // �߻�ü ���� ��ġ
    [SerializeField]
    private Transform spawnPoint;
    // Ÿ�� ����
    private int level = 0;
    // Ÿ�� ���� ����
    private WeaponState weaponState = WeaponState.SerachTarget;
    // ���� ���
    private Transform attackTarget = null;
    // ���ӿ� �����ϴ� �� ���� ȹ��
    private EnemySpawner enemySpawner;
    // Ÿ�� ������Ʈ �̹��� ����
    private SpriteRenderer spriteRenderer;
    // �÷��̾� ��� ���� ȹ�� �� ����
    private PlayerGold playerGold;
    // ���� Ÿ���� ��ġ�Ǿ� �ִ� Ÿ��
    private Tile ownerTile;

    public float Damage => towerTemplate.weapon[level].damage;
    public float Rate => towerTemplate.weapon[level].rate;
    public float Range => towerTemplate.weapon[level].range;
    public int Level => level;
    public int MaxLevel => towerTemplate.weapon.Length;
    public SpriteRenderer TowerSprite => spriteRenderer;

    public void Setup(EnemySpawner enemySpawner, PlayerGold playerGold, Tile tile)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        this.playerGold = playerGold;
        this.enemySpawner = enemySpawner;
        this.ownerTile = tile;
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
                if (distance <= towerTemplate.weapon[level].range && distance <= closesDisSqr)
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
            if (distance > towerTemplate.weapon[level].range)
            {
                attackTarget = null;
                ChangeState(WeaponState.SerachTarget);
                break;
            }

            // attackRate ��ŭ ���
            yield return new WaitForSeconds(towerTemplate.weapon[level].rate);

            // ����
            SpawnProjectile();
        }
    }

    private void SpawnProjectile()
    {
        GameObject clone = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.identity);
        clone.GetComponent<Projectile>().Setup(attackTarget, towerTemplate.weapon[level].damage);
    }

    public bool Upgrade()
    {
        // Ÿ�� ���׷��̵忡 �ʿ��� ��尡 ������� �˻�
        if (playerGold.CurrentGold < towerTemplate.weapon[level + 1].cost)
        {
            return false;
        }

        // Ÿ�� ���� ����
        level++;
        // Ÿ�� ���� ����
        spriteRenderer.sprite = towerTemplate.weapon[level].sprite;
        // ��� ����
        playerGold.CurrentGold -= towerTemplate.weapon[level].cost;

        return true;
    }

    public void Sell()
    {
        // ��� ����
        playerGold.CurrentGold += towerTemplate.weapon[level].sell;
        // ���� Ÿ�Ͽ� �ٽ� Ÿ�� �Ǽ��� �����ϵ��� ����
        ownerTile.isBuildTower = false;
        // Ÿ�� �ı�
        Destroy(gameObject);
    }
}
