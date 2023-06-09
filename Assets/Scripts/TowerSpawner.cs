using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject towerPrefab;
    [SerializeField]
    private EnemySpawner enemySpawner;

    public void SpawnTower(Transform tileTransform)
    {
        Tile tile = tileTransform.GetComponent<Tile>();

        // 타워 건설 가능 여부 확인
        // 현재 타일의 위치에 타워가 건설되어 있다면
        if (tile.isBuildTower)
        {
            return;
        }

        // 현재 타일의 위치에 타워가 건설되어 있지 안다면
        tile.isBuildTower = true;
        // 선택한 타일의 위치에 타워 소환
        GameObject clone = Instantiate(towerPrefab, tileTransform.position, Quaternion.identity);
        clone.GetComponent<TowerWeapon>().Setup(enemySpawner);
    }
}
