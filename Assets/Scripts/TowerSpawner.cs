using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject towerPrefab;
    [SerializeField]
    private EnemySpawner enemySpawner;
    // Ÿ�� �Ǽ� ���
    [SerializeField]
    private int towerBuildGold = 50;
    // Ÿ�� �Ǽ� �� ��� ����
    [SerializeField]
    private PlayerGold playerGold;

    public void SpawnTower(Transform tileTransform)
    {
        // Ÿ�� �Ǽ� ���� ���� Ȯ��
        // Ÿ�� �Ǽ��� ��ŭ ���� ������ Ÿ�� �Ǽ� X
        if (towerBuildGold > playerGold.CurrentGold)
        {
            return;
        }

        Tile tile = tileTransform.GetComponent<Tile>();

        // Ÿ�� �Ǽ� ���� ���� Ȯ��
        // ���� Ÿ���� ��ġ�� Ÿ���� �Ǽ��Ǿ� �ִٸ�
        if (tile.isBuildTower)
        {
            return;
        }

        // ���� Ÿ���� ��ġ�� Ÿ���� �Ǽ��Ǿ� ���� �ȴٸ�
        tile.isBuildTower = true;
        // Ÿ�� �Ǽ��� �ʿ��� ��常ŭ ����
        playerGold.CurrentGold -= towerBuildGold;
        // ������ Ÿ���� ��ġ�� Ÿ�� ��ȯ (Ÿ�Ϻ��� z�� -1�� ��ġ�� ��ġ
        Vector3 position = tileTransform.position + Vector3.back;
        GameObject clone = Instantiate(towerPrefab, position, Quaternion.identity);
        clone.GetComponent<TowerWeapon>().Setup(enemySpawner);
    }
}
