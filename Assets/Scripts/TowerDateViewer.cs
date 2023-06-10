using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TowerDateViewer : MonoBehaviour
{
    [SerializeField]
    private Image imageTower;
    [SerializeField]
    private TextMeshProUGUI textDamage;
    [SerializeField]
    private TextMeshProUGUI textRate;
    [SerializeField]
    private TextMeshProUGUI textRange;
    [SerializeField]
    private TextMeshProUGUI textLevel;
    [SerializeField]
    private TowerAttackRange towerAttackRange;
    [SerializeField]
    private Button buttonUpgrade;
    [SerializeField]
    private SystemTextViewer systemTextViewer;

    private TowerWeapon currentTower;


    private void Awake()
    {
        OffPanel();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OffPanel();
        }
    }

    public void OnPanel(Transform towerWeapon)
    {
        // ����ؾ��ϴ� Ÿ�� ������ �޾ƿͼ� ����
        currentTower = towerWeapon.GetComponent<TowerWeapon>();
        // Ÿ�� ���� Panel On
        gameObject.SetActive(true);
        // Ÿ�� ���� ����
        UpdateTowerDate();
        // Ÿ�� ������Ʈ �ֺ��� ǥ�õǴ� Ÿ�� ���ݹ��� Sprite On
        towerAttackRange.OnAttackRange(currentTower.transform.position, currentTower.Range);
    }

    public void OffPanel()
    {
        // Ÿ�� ���� Panel Off
        gameObject.SetActive(false);
        // Ÿ�� ���ݹ��� Sprite Off
        towerAttackRange.OffAttackRange();
    }

    private void UpdateTowerDate()
    {
        imageTower.sprite = currentTower.TowerSprite.sprite;
        textDamage.text = "Damage : " + currentTower.Damage;
        textRate.text = "Rate : " + currentTower.Rate;
        textRange.text = "Range : " + currentTower.Range;
        textLevel.text = "Level : " + currentTower.Level;

        // ���׷��̵� �Ұ��������� ��ư ��Ȱ��ȭ
        buttonUpgrade.interactable = currentTower.Level < currentTower.MaxLevel - 1 ? true : false;
    }

    public void OnClickEventTowerUpgrade()
    {
        // Ÿ�� ���׷��̵� �õ�
        bool isSuccess = currentTower.Upgrade();


        if (isSuccess)
        {
            // Ÿ�� ���׷��̵� �Ǿ��� ������ Ÿ�� ���� ����
            UpdateTowerDate();
            // Ÿ�� �ֺ��� ���̴� ���ݹ��� ����
            towerAttackRange.OnAttackRange(currentTower.transform.position, currentTower.Range);
        }
        else
        {
            // ���׷��̵忡 �ʿ��� ����� �����ϴٰ� ���
            systemTextViewer.PrintText(SystemType.Money);
        }
    }

    public void OnClickEventTowerSell()
    {
        // Ÿ�� �Ǹ�
        currentTower.Sell();
        // ������ Ÿ���� ������� Panel, ���ݹ��� off
        OffPanel();
    }
}
