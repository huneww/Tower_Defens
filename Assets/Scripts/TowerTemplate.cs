using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class TowerTemplate : ScriptableObject
{
    // Ÿ���� ������ ������
    public GameObject towerPrefab;
    // �ӽ� Ÿ�� ������
    public GameObject followTowerPrefab;
    // ������ Ÿ�� ����
    public Weapon[] weapon;

    [System.Serializable]
    public struct Weapon
    {
        // �������� Ÿ�� �̹��� (UI)
        public Sprite sprite;
        // ���ݷ�
        public float damage;
        // ���� �ۼ�Ʈ (0.2 = 20%)
        public float slow;
        // ���ݷ� ������ (0.2 = 20%)
        public float buff;
        // �߻� �ӵ�
        public float rate;
        // �߻� ����
        public float range;
        // �Ǽ� ���
        public int cost;
        // �Ǹ� ���
        public int sell;
    }
}
