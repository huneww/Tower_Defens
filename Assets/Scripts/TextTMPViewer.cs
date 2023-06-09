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
    // 플레이어 체력 정보
    [SerializeField]
    private PlayerHp playerHp;
    // 플레이어 골드 정보
    [SerializeField]
    private PlayerGold playerGold;

    private void Update()
    {
        textPlayerHp.text = playerHp.CurrentHp + "/" + playerHp.MaxHp;
        textPlayerGold.text = playerGold.CurrentGold.ToString();
    }

}
