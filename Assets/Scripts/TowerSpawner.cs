using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject towerPrefab;
    [SerializeField]
    private EnemySpawner enemySpawner;
    // 타워 건설 비용
    [SerializeField]
    private int towerBuildGold = 50;
    // 타워 건설 시 골드 감소
    [SerializeField]
    private PlayerGold playerGold;

    public void SpawnTower(Transform tileTransform)
    {
        // 타워 건설 가능 여부 확인
        // 타워 건설할 만큼 돈이 없으면 타워 건설 X
        if (towerBuildGold > playerGold.CurrentGold)
        {
            return;
        }

        Tile tile = tileTransform.GetComponent<Tile>();

        // 타워 건설 가능 여부 확인
        // 현재 타일의 위치에 타워가 건설되어 있다면
        if (tile.isBuildTower)
        {
            return;
        }

        // 현재 타일의 위치에 타워가 건설되어 있지 안다면
        tile.isBuildTower = true;
        // 타워 건설에 필요한 골드만큼 감소
        playerGold.CurrentGold -= towerBuildGold;
        // 선택한 타일의 위치에 타워 소환 (타일보다 z축 -1의 위치에 배치
        Vector3 position = tileTransform.position + Vector3.back;
        GameObject clone = Instantiate(towerPrefab, position, Quaternion.identity);
        clone.GetComponent<TowerWeapon>().Setup(enemySpawner);
    }
}
