using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TowerSpawner : MonoBehaviour
{
    [SerializeField]
    private TowerTemplate[] towerTemplate;
    [SerializeField]
    private EnemySpawner enemySpawner;
    // Ÿ�� �Ǽ� �� ��� ����
    [SerializeField]
    private PlayerGold playerGold;
    [SerializeField]
    private SystemTextViewer systemTextViewer;
    // Ÿ�� �Ǽ� ��ư�� �������� üũ
    private bool isOnTowerButton = false;
    // �ӽ� Ÿ�� ��� �Ϸ� �� ������ ���� �����ϴ� ����
    private GameObject followTowerClone = null;
    // Ÿ�� �Ӽ�
    private int towerType;

    public void ReadyToSpawnTower(int type)
    {
        towerType = type;

        if (isOnTowerButton)
        {
            return;
        }

        // Ÿ�� �Ǽ� ���� ���� Ȯ��
        // Ÿ���� �Ǽ��Ҹ�ŭ ���� ������ Ÿ�� �Ǽ� X
        if (towerTemplate[towerType].weapon[0].cost > playerGold.CurrentGold)
        {
            // ��尡 �����ؼ� Ÿ�� �Ǽ��� �Ұ����ϴٰ� ���
            systemTextViewer.PrintText(SystemType.Money);
            return;
        }

        // Ÿ�� �Ǽ� ��ư�� �����ٰ� ����
        isOnTowerButton = true;
        // ���콺�� ����ٴϴ� �ӽ� Ÿ�� ����
        followTowerClone = Instantiate(towerTemplate[towerType].followTowerPrefab);
        // Ÿ�� �Ǽ��� ����� �� �ִ� �ڷ�ƾ �޼��� ����
        StartCoroutine(OnTowerCanclesystem());
    }

    public void SpawnTower(Transform tileTransform)
    {
        // Ÿ�� �Ǽ� ��ư�� ������ ���� Ÿ�� �Ǽ�
        if (!isOnTowerButton)
        {
            return;
        }

        Tile tile = tileTransform.GetComponent<Tile>();

        // Ÿ�� �Ǽ� ���� ���� Ȯ��
        // ���� Ÿ���� ��ġ�� Ÿ���� �Ǽ��Ǿ� �ִٸ�
        if (tile.isBuildTower)
        {
            // ���� ��ġ�� Ÿ�� �Ǽ��� �Ұ����ϴٰ� ���
            systemTextViewer.PrintText(SystemType.Build);
            return;
        }
        // �ٽ� Ÿ�� �Ǽ� ��ư�� ������ Ÿ���� �Ǽ��ϵ��� ���� ����
        isOnTowerButton = false;
        // ���� Ÿ���� ��ġ�� Ÿ���� �Ǽ��Ǿ� ���� �ȴٸ�
        tile.isBuildTower = true;
        // Ÿ�� �Ǽ��� �ʿ��� ��常ŭ ����
        playerGold.CurrentGold -= towerTemplate[towerType].weapon[0].cost;
        // ������ Ÿ���� ��ġ�� Ÿ�� ��ȯ (Ÿ�Ϻ��� z�� -1�� ��ġ�� ��ġ
        Vector3 position = tileTransform.position + Vector3.back;
        GameObject clone = Instantiate(towerTemplate[towerType].towerPrefab, position, Quaternion.identity);
        clone.GetComponent<TowerWeapon>().Setup(this, enemySpawner, playerGold, tile);

        // ���� ��ġ�Ǵ� Ÿ���� ���� Ÿ�� �ֺ��� ��ġ�� ���
        // ���� ȿ���� ���� �� �ֵ��� ��� ���� Ÿ���� ���� ȿ�� ����
        OnBuffAllBuffTowers();

        // Ÿ���� ��ġ�߱� ������ ���콺�� ����ٴϴ� �ӽ� Ÿ�� ����
        Destroy(followTowerClone);
        // Ÿ�� �Ǽ��� ����� �� �ִ� �ڷ�ƾ �޼��� ����
        StopCoroutine(OnTowerCanclesystem());
    }

    private IEnumerator OnTowerCanclesystem()
    {
        while (true)
        {
            // ESCŰ �Ǵ� ���콺 ������ ��ư�� ������ �� Ÿ�� �Ǽ� ���
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
            {
                isOnTowerButton = false;
                // ���콺�� ����ٴϴ� �ӽ� Ÿ�� ����
                Destroy(followTowerClone);
                break;
            }

            yield return null;
        }
    }
    
    public void OnBuffAllBuffTowers()
    {
        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");

        for (int i = 0; i < towers.Length; i++)
        {
            TowerWeapon weapon = towers[i].GetComponent<TowerWeapon>();

            if (weapon.WeaponType == WeaponType.Buff)
            {
                weapon.OnBuffAroundTower();
            }
        }
    }
}
