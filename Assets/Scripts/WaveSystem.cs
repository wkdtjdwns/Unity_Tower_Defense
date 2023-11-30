using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSystem : MonoBehaviour
{
    [SerializeField]
    private Wave[] waves;
    [SerializeField]
    private EnemySpawner enemy_spawner;
    private int current_wave_index = -1;

    // 웨이브 정보를 얻기 위한 get 프로퍼티 생성 (현재 웨이브, 총 웨이브)
    public int Current_wave => current_wave_index + 1; // 시작이 0이기 때문에 1을 더함
    public int Max_wave => waves.Length;

    public void StartWave()
    {
        // 현재 맵에 적이 없고, Wave가 남아있을 때
        if (enemy_spawner.Enemy_list.Count == 0 && current_wave_index < waves.Length - 1)
        {
            // 인덱스의 시작이 -1이기 때문에 증가하고 시작
            current_wave_index++;
            // 현재 웨이브 시작
            enemy_spawner.StartWave(waves[current_wave_index]);
        }
    }
}

// [System.Serializable]
// -> 구조체, 클래스를 직렬화 하는 명령어 (직렬화 하면 Inspector View (인스펙터 창)에서 정보를 수정할 수 있음)
// -> 메모리 상에 존재하는 오브젝트 정보를 string 또는 byte 데이터 형태로 변형하는 것
// -> 드라이브 저장, 네트워크를 통한 데이터 전송도 가능함

// struct
// -> 구조체 변수
[System.Serializable]
public struct Wave
{
    public float spawn_time;
    public int max_enemy_count;
    public GameObject[] enemy_prefabs;
}
