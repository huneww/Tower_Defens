using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;

public enum WeaponType { Cannon = 0, Laser, Slow, Buff}
public enum WeaponState { SerachTarget = 0, TryAttackCannon, TryAttackLaser}

public class TowerWeapon : MonoBehaviour
{
    [Header("Commons")]
    // 타워정보
    [SerializeField]
    private TowerTemplate towerTemplate;
    // 발사체 생성 위치
    [SerializeField]
    private Transform spawnPoint;
    [SerializeField]
    private WeaponType weaponType;

    [Header("Cannon")]
    // 발사체 프리펩
    [SerializeField]
    private GameObject projectilePrefab;

    [Header("Laser")]
    // 레이저로 사용될 선
    [SerializeField]
    private LineRenderer lineRenderer;
    // 타격 효과
    [SerializeField]
    private Transform hitEffect;
    // 광선에 부딪히는 레이어 설정
    [SerializeField]
    private LayerMask targetLayer;

    // 타워 레벨
    private int level = 0;
    // 타워 무기 상태
    private WeaponState weaponState = WeaponState.SerachTarget;
    // 공격 대상
    private Transform attackTarget = null;
    // 게임에 존재하는 적 정보 획득
    private EnemySpawner enemySpawner;
    // 타워 오브젝트 이미지 변경
    private SpriteRenderer spriteRenderer;
    // 플레이어 골드 정보 획득 및 설정
    private PlayerGold playerGold;
    // 현재 타워가 배치되어 있는 타일
    private Tile ownerTile;

    private TowerSpawner towerSpawner;
    // 버프에 의해 추가된 데미지
    private float addedDamage;
    // 버프를 받는지 여부 설정 (0 : 버브X, 1 ~ 3 : 받는 버프 레벨)
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
        // 무기 속성이 캐논, 레이저일 때
        if (weaponType == WeaponType.Cannon || weaponType == WeaponType.Laser)
        {
            // 최초 상태를 WeaponState.SerachTarget으로 설정
            ChangeState(WeaponState.SerachTarget);
        }
    }

    public void ChangeState(WeaponState state)
    {
        // 이전 재생중이던 상태 종료
        StopCoroutine(weaponState.ToString());
        // 상태 변경
        weaponState = state;
        // 새로운 상태 재생
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
        // 원점에서 거리와 수평축으로부터 각도를 이용해 위치를 구하는 극 좌표계 이용
        // 각도 = aractan(x/y)
        // x, y 변위값 구하기
        float dx = attackTarget.position.x - transform.position.x;
        float dy = attackTarget.position.y - transform.position.y;
        // x, y 변위값을 바탕으로 각도 구하기
        // 각도가 radian 단위기 때문에 Mathf.Rad2Deg를 곱해 도 단위를 구함
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

            // attackRate 만큼 대기
            yield return new WaitForSeconds(towerTemplate.weapon[level].rate);

            // 공격
            SpawnProjectile();
        }
    }

    private IEnumerator TryAttackLaser()
    {
        // 레이저, 레이저 타격 효과 활성화
        EnableLaser();

        while (true)
        {
            // target을 공격하는게 가능한지 검사
            if (IsPossibleToAttackTarget() == false)
            {
                // 레이저, 레이저 타격 효과 비활성화
                DisableLaser();
                ChangeState(WeaponState.SerachTarget);
                break;
            }

            // 레이저 공격
            SpawnLaser();

            yield return null;
        }
    }

    public void OnBuffAroundTower()
    {
        // 현재 맵에 배치된 "Tower" 태그를 가진 모든 오브젝트 탐색
        GameObject[] tower = GameObject.FindGameObjectsWithTag("Tower");

        for (int i = 0; i < tower.Length; i++)
        {
            TowerWeapon weapon = tower[i].GetComponent<TowerWeapon>();

            // 이미 버프를 받고 있고, 현재 버프 타워의 레벨보다 높은 버프이면 패스
            if (weapon.BuffLevel > level) continue;

            // 현재 버프 타워의 거리를 검사해서 범위 안에 타워가 있으면
            if (Vector3.Distance(weapon.transform.position, transform.position) <= towerTemplate.weapon[level].range)
            {
                // 공격이 가능한 캐논, 레이저 타워이면
                if (weapon.WeaponType == WeaponType.Cannon || weapon.WeaponType == WeaponType.Laser)
                {
                    // 버프에 의해 공격력 증가
                    weapon.AddedDamage = weapon.Damage * (towerTemplate.weapon[level].buff);
                    // 타워가 받고 있는 버프 레벨 설정
                    weapon.BuffLevel = level;
                }
            }
        }
    }

    private Transform FindClosesAttackTarget()
    {
        // 제일 가까이 있는 적을 찾기 위해 최초 거리를 최대한 크게 설정
        float closesDisSqr = Mathf.Infinity;
        // EnemySpawner의 EnemyList에 있는 현재 맵에 존재하는 모든 적 검사
        for (int i = 0; i < enemySpawner.EnemyList.Count; i++)
        {
            float distance = Vector3.Distance(enemySpawner.EnemyList[i].transform.position, transform.position);
            // 현재 검사중인 적과 거리가 공격범위 내에 있고, 현재까지 검사한 적보다 거리가 가까우면
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
        // target이 있는지 검사 (다른 발사체에 의해 제거, Goal 지점까지 이동해 삭제 등)
        if ( attackTarget == null)
        {
            return false;
        }
        // target이 공격 범위 안에 있는지 검사 (공격 범위를 벗어나면 새로운 적 탐색)
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
        // 공격력 = 타워 기본 공격력 + 버프에 의해 추가된 공격력
        float damage = towerTemplate.weapon[level].damage + AddedDamage;
        clone.GetComponent<Projectile>().Setup(attackTarget, damage);
    }

    public bool Upgrade()
    {
        // 타워 업그레이드에 필요한 골드가 충분한지 검사
        if (playerGold.CurrentGold < towerTemplate.weapon[level + 1].cost)
        {
            return false;
        }

        // 타워 레벨 증가
        level++;
        // 타워 외형 변경
        spriteRenderer.sprite = towerTemplate.weapon[level].sprite;
        // 골드 차감
        playerGold.CurrentGold -= towerTemplate.weapon[level].cost;

        // 무기 속성이 레이저이면
        if (weaponType == WeaponType.Laser)
        {
            // 레벨에 따라 레이저의 꿁기설정
            lineRenderer.startWidth = 0.05f + level * 0.05f;
            lineRenderer.endWidth = 0.05f;
        }

        // 타워가 업그레이드 될 때 모든 버프 타워의 버프 효과갱신
        // 현재 타워가 버프 타워인 경우, 현재 타워가 굥격 타워인 경우
        towerSpawner.OnBuffAllBuffTowers();

        return true;
    }

    public void Sell()
    {
        // 골드 증가
        playerGold.CurrentGold += towerTemplate.weapon[level].sell;
        // 현재 타일에 다시 타워 건설이 가능하도록 설정
        ownerTile.isBuildTower = false;
        // 타워 파괴
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

        // 같은 방향으로 여러 개의 광선을 쏴서 그 중 현재 attackTarget과 동일한 오브젝트를 검출
        for (int i = 0; i < hit.Length; i++)
        {
            if (hit[i].transform == attackTarget)
            {
                // 선의 시작점
                lineRenderer.SetPosition(0, spawnPoint.position);
                // 선의 목표지점
                lineRenderer.SetPosition(1, new Vector3(hit[i].point.x, hit[i].point.y, 0) + Vector3.back);
                // 타격 효과 위치 설정
                hitEffect.position = hit[i].point;
                // 적 체력 감소 (1초에 damage만큼 감소)
                // 공격력 = 타워 기본 공격력 + 버프에 의해 추가된 공격력
                float damage = towerTemplate.weapon[level].damage + AddedDamage;
                attackTarget.GetComponent<EnemyHp>().TakeDamage(damage * Time.deltaTime);
            }
        }
    }
}
