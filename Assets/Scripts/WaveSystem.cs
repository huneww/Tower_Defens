using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSystem : MonoBehaviour
{
    // 현재 스테이지의 모든 웨이브 정보
    [SerializeField]
    private Wave[] waves;
    [SerializeField]
    private EnemySpawner enemySpawner;
    // 현재 웨이브 인덱스
    private int currentWaveIndex = -1;

    // 웨이브 정보 출력을 위한 Get 프로퍼티 (현재 웨이브, 총 웨이브)
    public int CurrentWave => currentWaveIndex + 1;
    public int MaxWave => waves.Length;

    public void WaveStart()
    {
        // 현재 맵에 적이 없고, 웨이브가 남아있다면
        if (enemySpawner.EnemyList.Count == 0 && currentWaveIndex < waves.Length)
        {
            // 인덱스의 시작이 -1이기 때문에 웨이브 인덱스 증가를 제일 먼저 함
            currentWaveIndex++;
            // EnemySpawn의 StartWave() 메서드 호출, 현재 웨이브 정보 제공
            enemySpawner.StartWave(waves[currentWaveIndex]);
        }
    }
}

[System.Serializable]
public struct Wave
{
    // 현재 웨이브 적 생성 주기
    public float spawnTime;
    // 현재 웨이브 적 등장 수
    public int maxEnemyCount;
    // 현재 웨이브 적 등장 종류
    public GameObject[] enemyPrefabs;
}