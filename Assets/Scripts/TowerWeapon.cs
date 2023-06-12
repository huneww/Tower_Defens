using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;

public enum WeaponType { Cannon = 0, Laser, Slow, Buff}
public enum WeaponState { SerachTarget = 0, TryAttackCannon, TryAttackLaser}

public class TowerWeapon : MonoBehaviour
{
    [Header("Commons")]
    // Ÿ������
    [SerializeField]
    private TowerTemplate towerTemplate;
    // �߻�ü ���� ��ġ
    [SerializeField]
    private Transform spawnPoint;
    [SerializeField]
    private WeaponType weaponType;

    [Header("Cannon")]
    // �߻�ü ������
    [SerializeField]
    private GameObject projectilePrefab;

    [Header("Laser")]
    // �������� ���� ��
    [SerializeField]
    private LineRenderer lineRenderer;
    // Ÿ�� ȿ��
    [SerializeField]
    private Transform hitEffect;
    // ������ �ε����� ���̾� ����
    [SerializeField]
    private LayerMask targetLayer;

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

    private TowerSpawner towerSpawner;
    // ������ ���� �߰��� ������
    private float addedDamage;
    // ������ �޴��� ���� ���� (0 : ����X, 1 ~ 3 : �޴� ���� ����)
    private int buffLevel;

    public float Damage => towerTemplate.weapon[level].damage;
    public float Rate => towerTemplate.weapon[level].rate;
    public float Range => towerTemplate.weapon[level].range;
    public int Level => level + 1;
    public int MaxLevel => towerTemplate.weapon.Length;
    public float Slow => towerTemplate.weapon[level].slow;
    public int UpgradeCost => Level < MaxLevel ? towerTemplate.weapon[level + 1].cost : 0;
    public int SellCost => towerTemplate.weapon[level].sell;
    public SpriteRenderer TowerSprite => spriteRenderer;
    public WeaponType WeaponType => weaponType;
    public float Buff => towerTemplate.weapon[level].buff;
    public float AddedDamage
    {
        set => addedDamage = Mathf.Max(0, value);
        get => addedDamage;
    }
    public int BuffLevel
    {
        set => buffLevel = Mathf.Max(0, value);
        get => buffLevel;
    }

    public void Setup(TowerSpawner towerSpawner, EnemySpawner enemySpawner, PlayerGold playerGold, Tile tile)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        this.towerSpawner = towerSpawner;
        this.playerGold = playerGold;
        this.enemySpawner = enemySpawner;
        this.ownerTile = tile;
        // ���� �Ӽ��� ĳ��, �������� ��
        if (weaponType == WeaponType.Cannon || weaponType == WeaponType.Laser)
        {
            // ���� ���¸� WeaponState.SerachTarget���� ����
            ChangeState(WeaponState.SerachTarget);
        }
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
            attackTarget = FindClosesAttackTarget();

            if (attackTarget != null)
            {
                if (weaponType == WeaponType.Cannon)
                    ChangeState(WeaponState.TryAttackCannon);
                else if (weaponType == WeaponType.Laser)
                    ChangeState(WeaponState.TryAttackLaser);
            }

            yield return null; 
        }
    }

    private IEnumerator TryAttackCannon()
    {
        while (true)
        {
            if (IsPossibleToAttackTarget() == false)
            {
                ChangeState(WeaponState.SerachTarget);
                break;
            }

            // attackRate ��ŭ ���
            yield return new WaitForSeconds(towerTemplate.weapon[level].rate);

            // ����
            SpawnProjectile();
        }
    }

    private IEnumerator TryAttackLaser()
    {
        // ������, ������ Ÿ�� ȿ�� Ȱ��ȭ
        EnableLaser();

        while (true)
        {
            // target�� �����ϴ°� �������� �˻�
            if (IsPossibleToAttackTarget() == false)
            {
                // ������, ������ Ÿ�� ȿ�� ��Ȱ��ȭ
                DisableLaser();
                ChangeState(WeaponState.SerachTarget);
                break;
            }

            // ������ ����
            SpawnLaser();

            yield return null;
        }
    }

    public void OnBuffAroundTower()
    {
        // ���� �ʿ� ��ġ�� "Tower" �±׸� ���� ��� ������Ʈ Ž��
        GameObject[] tower = GameObject.FindGameObjectsWithTag("Tower");

        for (int i = 0; i < tower.Length; i++)
        {
            TowerWeapon weapon = tower[i].GetComponent<TowerWeapon>();

            // �̹� ������ �ް� �ְ�, ���� ���� Ÿ���� �������� ���� �����̸� �н�
            if (weapon.BuffLevel > level) continue;

            // ���� ���� Ÿ���� �Ÿ��� �˻��ؼ� ���� �ȿ� Ÿ���� ������
            if (Vector3.Distance(weapon.transform.position, transform.position) <= towerTemplate.weapon[level].range)
            {
                // ������ ������ ĳ��, ������ Ÿ���̸�
                if (weapon.WeaponType == WeaponType.Cannon || weapon.WeaponType == WeaponType.Laser)
                {
                    // ������ ���� ���ݷ� ����
                    weapon.AddedDamage = weapon.Damage * (towerTemplate.weapon[level].buff);
                    // Ÿ���� �ް� �ִ� ���� ���� ����
                    weapon.BuffLevel = level;
                }
            }
        }
    }

    private Transform FindClosesAttackTarget()
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

        return attackTarget;
    }

    private bool IsPossibleToAttackTarget()
    {
        // target�� �ִ��� �˻� (�ٸ� �߻�ü�� ���� ����, Goal �������� �̵��� ���� ��)
        if ( attackTarget == null)
        {
            return false;
        }
        // target�� ���� ���� �ȿ� �ִ��� �˻� (���� ������ ����� ���ο� �� Ž��)
        float distance = Vector3.Distance(attackTarget.position, transform.position);
        if (distance > towerTemplate.weapon[level].range)
        {
            attackTarget = null;
            return false;
        }

        return true;
    }

    private void SpawnProjectile()
    {
        GameObject clone = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.identity);
        // ���ݷ� = Ÿ�� �⺻ ���ݷ� + ������ ���� �߰��� ���ݷ�
        float damage = towerTemplate.weapon[level].damage + AddedDamage;
        clone.GetComponent<Projectile>().Setup(attackTarget, damage);
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

        // ���� �Ӽ��� �������̸�
        if (weaponType == WeaponType.Laser)
        {
            // ������ ���� �������� �B�⼳��
            lineRenderer.startWidth = 0.05f + level * 0.05f;
            lineRenderer.endWidth = 0.05f;
        }

        // Ÿ���� ���׷��̵� �� �� ��� ���� Ÿ���� ���� ȿ������
        // ���� Ÿ���� ���� Ÿ���� ���, ���� Ÿ���� ���� Ÿ���� ���
        towerSpawner.OnBuffAllBuffTowers();

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

    private void EnableLaser()
    {
        lineRenderer.gameObject.SetActive(true);
        hitEffect.gameObject.SetActive(true);
    }

    private void DisableLaser()
    {
        lineRenderer.gameObject.SetActive(false);
        hitEffect.gameObject.SetActive(false);
    }

    private void SpawnLaser()
    {
        Vector3 direction = attackTarget.position - transform.position;
        RaycastHit2D[] hit = Physics2D.RaycastAll(spawnPoint.position, direction, towerTemplate.weapon[level].range, targetLayer);

        // ���� �������� ���� ���� ������ ���� �� �� ���� attackTarget�� ������ ������Ʈ�� ����
        for (int i = 0; i < hit.Length; i++)
        {
            if (hit[i].transform == attackTarget)
            {
                // ���� ������
                lineRenderer.SetPosition(0, spawnPoint.position);
                // ���� ��ǥ����
                lineRenderer.SetPosition(1, new Vector3(hit[i].point.x, hit[i].point.y, 0) + Vector3.back);
                // Ÿ�� ȿ�� ��ġ ����
                hitEffect.position = hit[i].point;
                // �� ü�� ���� (1�ʿ� damage��ŭ ����)
                // ���ݷ� = Ÿ�� �⺻ ���ݷ� + ������ ���� �߰��� ���ݷ�
                float damage = towerTemplate.weapon[level].damage + AddedDamage;
                attackTarget.GetComponent<EnemyHp>().TakeDamage(damage * Time.deltaTime);
            }
        }
    }
}
