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

        // Ÿ�� �Ǽ� ���� ���� Ȯ��
        // ���� Ÿ���� ��ġ�� Ÿ���� �Ǽ��Ǿ� �ִٸ�
        if (tile.isBuildTower)
        {
            return;
        }

        // ���� Ÿ���� ��ġ�� Ÿ���� �Ǽ��Ǿ� ���� �ȴٸ�
        tile.isBuildTower = true;
        // ������ Ÿ���� ��ġ�� Ÿ�� ��ȯ
        GameObject clone = Instantiate(towerPrefab, tileTransform.position, Quaternion.identity);
        clone.GetComponent<TowerWeapon>().Setup(enemySpawner);
    }
}
