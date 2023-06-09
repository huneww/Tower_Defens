using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextTMPViewer : MonoBehaviour
{
    // �÷��̾� ü��
    [SerializeField]
    private TextMeshProUGUI textPlayerHp;
    // �÷��̾� ���
    [SerializeField]
    private TextMeshProUGUI textPlayerGold;
    // �÷��̾� ü�� ����
    [SerializeField]
    private PlayerHp playerHp;
    // �÷��̾� ��� ����
    [SerializeField]
    private PlayerGold playerGold;

    private void Update()
    {
        textPlayerHp.text = playerHp.CurrentHp + "/" + playerHp.MaxHp;
        textPlayerGold.text = playerGold.CurrentGold.ToString();
    }

}
