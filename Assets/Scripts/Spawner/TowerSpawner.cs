using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerSpawner : MonoBehaviour
{
    [SerializeField]
    private TowerTemplate[] tower_template;
    [SerializeField]
    private EnemySpawner enemy_spawner;
    [SerializeField]
    private PlayerGold player_gold;
    [SerializeField]
    private SystemTextViewer system_text_viewer;
    private bool is_on_tower_button = false;
    private GameObject follow_tower_clone = null;
    private int tower_type;

    public void ReadyToSpawnTower(int type)
    {
        tower_type = type;

        // ��ư �ߺ�Ŭ�� ����
        if (is_on_tower_button) { return; }

        // Ÿ�� �Ǽ� ���� ���� Ȯ��
        // Ÿ���� �Ǽ��� ��ŭ�� ���� ������ Ÿ�� �Ǽ� X
        if (tower_template[tower_type].weapon[0].cost > player_gold.Current_gold)
        {
            system_text_viewer.PrintText(SystemType.Money);
            return;
        }

        is_on_tower_button = true;

        // ���콺�� ����ٴϴ� �ӽ� Ÿ�� ����
        follow_tower_clone = Instantiate(tower_template[tower_type].follow_tower_prefab);

        // Ÿ�� �Ǽ��� ����� �� �ִ� �ڷ�ƾ �Լ� ����
        StartCoroutine("OnTowerCancelSystem");
    }

    public void SpawnTower(Transform tile_pos)
    {
        // Ÿ�� �Ǽ� ��ư�� ������ ���� Ÿ�� �Ǽ� ����
        if (!is_on_tower_button) { return; }

        Tile tile = tile_pos.GetComponent<Tile>();

        // ���� Ÿ�Ͽ� �̹� Ÿ���� ������ ��ġ �Ұ�
        if (tile.is_build_tower)
        {
            system_text_viewer.PrintText(SystemType.Build);
            return;
        }

        is_on_tower_button = false;

        // Ÿ���� �Ǽ� �Ǿ��� �� ����
        tile.is_build_tower = true;

        // Ÿ�� �Ǽ��� �ʿ��� ��常ŭ ���� ��� ����
        player_gold.Current_gold -= tower_template[tower_type].weapon[0].cost;

        // ������ ��ġ�� Ÿ�� �Ǽ� (Ÿ�Ϻ��� z�� - 1�� ��ġ�� ��ġ�� -> Ÿ���� �켱 Ŭ���� �� �ְ� ��)
        Vector3 position = tile_pos.position + Vector3.back;
        GameObject clone = Instantiate(tower_template[tower_type].tower_prefab, position, Quaternion.identity);
        clone.GetComponent<TowerWeapon>().Setup(this, enemy_spawner, player_gold, tile);

        // ���� Ÿ���� ���� ȿ�� ���� (��ġ���ڸ��� ������ ���� �� �ְ� ��)
        OnBuffAllBuffTowers();

        // �ӽ� Ÿ�� ����
        Destroy(follow_tower_clone);

        // Ÿ�� �Ǽ��� ����� �� �ִ� �ڷ�ƾ �Լ� ����
        StopCoroutine("OnTowerCancelSystem");
    }

    private IEnumerator OnTowerCancelSystem()
    {
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
            {
                is_on_tower_button = false;
                Destroy(follow_tower_clone);
                break;
            }

            yield return null;
        }
    }

    public void OnBuffAllBuffTowers()
    {
        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");

        for (int i = 0; i < towers.Length; ++i)
        {
            TowerWeapon weapon = towers[i].GetComponent<TowerWeapon>();

            if (weapon.Weapon_type == WeaponType.Buff && !weapon.Is_sell) { weapon.OnBuffAroundTower(); }
        }
    }
}
