using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class TowerTemplate : ScriptableObject
{
    // 타워를 생성할 프리펩
    public GameObject towerPrefab;
    // 임시 타워 프리팹
    public GameObject followTowerPrefab;
    // 레벨별 타워 정보
    public Weapon[] weapon;

    [System.Serializable]
    public struct Weapon
    {
        // 보여지는 타워 이미지 (UI)
        public Sprite sprite;
        // 공격력
        public float damage;
        // 발사 속도
        public float rate;
        // 발사 범위
        public float range;
        // 건설 비용
        public int cost;
        // 판매 비용
        public int sell;
    }
}
