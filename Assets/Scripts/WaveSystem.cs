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

    // ���̺� ������ ��� ���� get ������Ƽ ���� (���� ���̺�, �� ���̺�)
    public int Current_wave => current_wave_index + 1; // ������ 0�̱� ������ 1�� ����
    public int Max_wave => waves.Length;

    public void StartWave()
    {
        // ���� �ʿ� ���� ����, Wave�� �������� ��
        if (enemy_spawner.Enemy_list.Count == 0 && current_wave_index < waves.Length - 1)
        {
            // �ε����� ������ -1�̱� ������ �����ϰ� ����
            current_wave_index++;
            // ���� ���̺� ����
            enemy_spawner.StartWave(waves[current_wave_index]);
        }
    }
}

// [System.Serializable]
// -> ����ü, Ŭ������ ����ȭ �ϴ� ��ɾ� (����ȭ �ϸ� Inspector View (�ν����� â)���� ������ ������ �� ����)
// -> �޸� �� �����ϴ� ������Ʈ ������ string �Ǵ� byte ������ ���·� �����ϴ� ��
// -> ����̺� ����, ��Ʈ��ũ�� ���� ������ ���۵� ������

// struct
// -> ����ü ����
[System.Serializable]
public struct Wave
{
    public float spawn_time;
    public int max_enemy_count;
    public GameObject[] enemy_prefabs;
}
