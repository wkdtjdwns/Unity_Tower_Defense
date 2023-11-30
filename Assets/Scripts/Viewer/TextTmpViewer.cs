using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextTmpViewer : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI text_player_hp; // Text - TextMeshPro UI [플레이어의 체력]
    [SerializeField]
    private TextMeshProUGUI text_player_gold; // Text - TextMeshPro UI [플레이어의 골드]
    [SerializeField]
    private TextMeshProUGUI text_wave; // Text - TextMeshPro UI [현재 웨이브 / 총 웨이비]
    [SerializeField]
    private TextMeshProUGUI text_enemy_count; // Text - TextMeshPro UI [현재 생존 적 숫자 / 총 적 숫자]
    [SerializeField]
    private PlayerHp player_hp;
    [SerializeField]
    private PlayerGold player_gold;
    [SerializeField]
    private WaveSystem wave_system;
    [SerializeField]
    private EnemySpawner enemy_spawner;

    private void Update()
    {
        text_player_hp.text = player_hp.Current_hp + "/" + player_hp.Max_hp;
        text_player_gold.text = player_gold.Current_gold.ToString();
        text_wave.text = wave_system.Current_wave + "/" + wave_system.Max_wave;
        text_enemy_count.text = enemy_spawner.Current_enemy_count + "/" + enemy_spawner.Max_enemy_count;
    }
}
