using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextTMPViewer : MonoBehaviour
{
    // 플레이어 체력
    [SerializeField]
    private TextMeshProUGUI textPlayerHp;
    // 플레이어 골드
    [SerializeField]
    private TextMeshProUGUI textPlayerGold;
    // 현재 웨이브 / 총 웨이브
    [SerializeField]
    private TextMeshProUGUI textWave;
    // 현재 적 숫자 / 최대 적 숫자
    [SerializeField]
    private TextMeshProUGUI textEnemyCount;
    // 플레이어 체력 정보
    [SerializeField]
    private PlayerHp playerHp;
    // 플레이어 골드 정보
    [SerializeField]
    private PlayerGold playerGold;
    // 웨이브 정보
    [SerializeField]
    private WaveSystem waveSystem;
    // 적 숫자 정보
    [SerializeField]
    private EnemySpawner enemySpawner;

    private void Update()
    {
        textPlayerHp.text = playerHp.CurrentHp + "/" + playerHp.MaxHp;
        textPlayerGold.text = playerGold.CurrentGold.ToString();
        textWave.text = waveSystem.CurrentWave + "/" + waveSystem.MaxWave;
        textEnemyCount.text = enemySpawner.CurrentEnemyCount + "/" + enemySpawner.MaxEnemyCount;
    }

}
