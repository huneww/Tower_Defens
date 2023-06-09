using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSystem : MonoBehaviour
{
    // ���� ���������� ��� ���̺� ����
    [SerializeField]
    private Wave[] waves;
    [SerializeField]
    private EnemySpawner enemySpawner;
    // ���� ���̺� �ε���
    private int currentWaveIndex = -1;

    // ���̺� ���� ����� ���� Get ������Ƽ (���� ���̺�, �� ���̺�)
    public int CurrentWave => currentWaveIndex + 1;
    public int MaxWave => waves.Length;

    public void WaveStart()
    {
        // ���� �ʿ� ���� ����, ���̺갡 �����ִٸ�
        if (enemySpawner.EnemyList.Count == 0 && currentWaveIndex < waves.Length)
        {
            // �ε����� ������ -1�̱� ������ ���̺� �ε��� ������ ���� ���� ��
            currentWaveIndex++;
            // EnemySpawn�� StartWave() �޼��� ȣ��, ���� ���̺� ���� ����
            enemySpawner.StartWave(waves[currentWaveIndex]);
        }
    }
}

[System.Serializable]
public struct Wave
{
    // ���� ���̺� �� ���� �ֱ�
    public float spawnTime;
    // ���� ���̺� �� ���� ��
    public int maxEnemyCount;
    // ���� ���̺� �� ���� ����
    public GameObject[] enemyPrefabs;
}