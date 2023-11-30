using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TowerDataViewer : MonoBehaviour
{
    [SerializeField]
    private Image image_tower;
    [SerializeField]
    private TextMeshProUGUI text_damage;
    [SerializeField]
    private TextMeshProUGUI text_rate;
    [SerializeField]
    private TextMeshProUGUI text_range;
    [SerializeField]
    private TextMeshProUGUI text_level;
    [SerializeField]
    private TextMeshProUGUI tower_upgrade_cost;
    [SerializeField]
    private TextMeshProUGUI tower_sell_cost;
    [SerializeField]
    private TowerAttackRange tower_attack_range;
    [SerializeField]
    private Button button_upgrade;
    [SerializeField]
    private SystemTextViewer system_text_viewer;

    private TowerWeapon current_tower;

    private void Awake()
    {
        OffPanel();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) { OffPanel(); }
    }

    public void OnPanel(Transform tower_weapon)
    {
        // ����ؾ��ϴ� Ÿ���� ������ �޾ƿ� ������
        current_tower = tower_weapon.GetComponent<TowerWeapon>();

        gameObject.SetActive(true);

        // Ÿ�� ���� ����
        UpdateTowerData();

        // Ÿ�� ������Ʈ �ֺ��� Ÿ�� ���ݹ��� ǥ��
        tower_attack_range.OnAttackRange(current_tower.transform.position, current_tower.Range);
    }

    public void OffPanel()
    {
        gameObject.SetActive(false);

        tower_attack_range.OffAttackRange();
    }

    private void UpdateTowerData()
    {
        if (current_tower.Weapon_type == WeaponType.Cannon || current_tower.Weapon_type == WeaponType.Laser)
        {
            image_tower.rectTransform.sizeDelta = new Vector2(88, 59);
            // "<color=red>" ??? "</color>" -> �±׵� ���̿� �ִ� �ؽ�Ʈ�� ������ ���������� �ٲ�
            text_damage.text = "Damage : " + current_tower.Damage + "+" + "<color=red>" + current_tower.Added_damage.ToString("F1") + "</color>";
        }

        else
        {
            image_tower.rectTransform.sizeDelta = new Vector2(59, 59);

            if (current_tower.Weapon_type == WeaponType.Slow) { text_damage.text = "Slow : " + current_tower.Slow * 100 + "%"; }

            else if (current_tower.Weapon_type == WeaponType.Buff) { text_damage.text = "Buff : " + current_tower.Buff * 100 + "%"; }
        }

        image_tower.sprite = current_tower.Tower_sprite;
        text_rate.text = "Rate : " + current_tower.Rate;
        text_range.text = "Range : " + current_tower.Range;
        text_level.text = "Level : " + current_tower.Level;
        tower_upgrade_cost.text = current_tower.Upgrade_cost.ToString();
        tower_sell_cost.text = current_tower.Sell_cost.ToString();

        // ���׷��̵尡 �Ұ��������� ��ư ��Ȱ��ȭ
        // ��ư.interactable -> ��ư�� Ȱ��ȭ / ��Ȱ��ȭ�� ������
        button_upgrade.interactable = current_tower.Level < current_tower.Max_level ? true : false;
    }

    public void OnClickEventTowerUpgrade()
    {
        // Ÿ�� ���׷��̵� �õ�
        bool is_success = current_tower.Upgrade();

        if (is_success)
        {
            UpdateTowerData();

            tower_attack_range.OnAttackRange(current_tower.transform.position, current_tower.Range);
        }

        else { system_text_viewer.PrintText(SystemType.Money); }
    }

    public void OnClickEventTowerSell()
    {
        current_tower.Sell();
        OffPanel();
    }
}
