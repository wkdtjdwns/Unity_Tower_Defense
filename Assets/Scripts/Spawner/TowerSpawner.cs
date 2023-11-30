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

        // 버튼 중복클릭 방지
        if (is_on_tower_button) { return; }

        // 타워 건설 가능 여부 확인
        // 타워를 건설할 만큼의 돈이 없으면 타워 건설 X
        if (tower_template[tower_type].weapon[0].cost > player_gold.Current_gold)
        {
            system_text_viewer.PrintText(SystemType.Money);
            return;
        }

        is_on_tower_button = true;

        // 마우스를 따라다니는 임시 타워 생성
        follow_tower_clone = Instantiate(tower_template[tower_type].follow_tower_prefab);

        // 타워 건설을 취소할 수 있는 코루틴 함수 시작
        StartCoroutine("OnTowerCancelSystem");
    }

    public void SpawnTower(Transform tile_pos)
    {
        // 타워 건설 버튼을 눌렀을 때만 타워 건설 가능
        if (!is_on_tower_button) { return; }

        Tile tile = tile_pos.GetComponent<Tile>();

        // 현재 타일에 이미 타워가 있으면 설치 불가
        if (tile.is_build_tower)
        {
            system_text_viewer.PrintText(SystemType.Build);
            return;
        }

        is_on_tower_button = false;

        // 타워가 건설 되었을 때 실행
        tile.is_build_tower = true;

        // 타워 건설에 필요한 골드만큼 보유 골드 감소
        player_gold.Current_gold -= tower_template[tower_type].weapon[0].cost;

        // 선택한 위치에 타워 건설 (타일보다 z축 - 1의 위치에 배치함 -> 타워를 우선 클릭할 수 있게 함)
        Vector3 position = tile_pos.position + Vector3.back;
        GameObject clone = Instantiate(tower_template[tower_type].tower_prefab, position, Quaternion.identity);
        clone.GetComponent<TowerWeapon>().Setup(this, enemy_spawner, player_gold, tile);

        // 버프 타워의 버프 효과 갱신 (설치하자마자 버프를 받을 수 있게 함)
        OnBuffAllBuffTowers();

        // 임시 타워 삭제
        Destroy(follow_tower_clone);

        // 타워 건설을 취소할 수 있는 코루틴 함수 중지
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
