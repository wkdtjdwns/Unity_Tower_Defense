using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// System�� UnityEngine�� ��� Random Ŭ������ �ֱ� ������ 
// �� �� �ϳ��� ������
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject enemy_hp_slider_prefab;
    [SerializeField]
    private Transform canvas_transform;
    [SerializeField]
    private Transform[] way_points;
    [SerializeField]
    private PlayerHp player_hp;
    [SerializeField]
    private PlayerGold player_gold;
    private Wave current_wave;
    private int current_enemy_count;
    private List<Enemy> enemy_list; // ���� �ʿ� �����ϴ� ��� ���� ����

    // �ܺ� Ŭ�������� ���� Ȯ�� �� �� �ֵ��� ������Ƽ ����
    // ���� ������ ������ EnemySpawner���� �����ϱ� ������ set�� �ʿ� X
    public List<Enemy> Enemy_list => enemy_list;
    public int Current_enemy_count => current_enemy_count;
    public int Max_enemy_count => current_wave.max_enemy_count;

    private void Awake()
    {
        // �� ����Ʈ �޸� �Ҵ�
        enemy_list = new List<Enemy>();
    }

    public void StartWave(Wave wave)
    {
        // �Ű������� �޾ƿ� ���̺� ���� ����
        current_wave = wave;

        // ���� ���̺��� �ִ� �� ���� ����
        current_enemy_count = current_wave.max_enemy_count;

        // ���̺� ����
        StartCoroutine("SpawnEnemy");
    }

    private IEnumerator SpawnEnemy()
    {
        int spawn_enemy_count = 0;

        /*while (true)*/
        // ���� ���̺꿡�� �����Ǿ�� �ϴ� ���� ���ڸ�ŭ ���� �����ϰ� �ڷ�ƾ �Լ� ����
        while (spawn_enemy_count < current_wave.max_enemy_count)
        {
            /*GameObject clone = Instantiate(enemy_prefab);*/

            // ���̺꿡 �����ϴ� ���� ������ ���� ������ �� ������ ���� �����ϰ� ��
            int enemy_index = Random.Range(0, current_wave.enemy_prefabs.Length);
            GameObject clone = Instantiate(current_wave.enemy_prefabs[enemy_index]);
            Enemy enemy = clone.GetComponent<Enemy>();

            enemy.Setup(this, way_points);
            enemy_list.Add(enemy); // ����Ʈ�� ������ ���� ���� ����

            SpawnEnemyHpSlider(clone);

            spawn_enemy_count++;

            yield return new WaitForSeconds(current_wave.spawn_time);
        }
    }

    public void DestroyEnemy(EnemyDestroyType type, Enemy enemy, int gold)
    {
        // ���� ��ǥ�������� �������� ��
        if (type == EnemyDestroyType.Arrive)
        {
            player_hp.TakeDamage(1);
        }

        else if (type == EnemyDestroyType.Kill)
        {
            player_gold.Current_gold += gold;
        }

        // ���� ����� ������ ���� �� ���� ���� (UI ǥ��)
        current_enemy_count--;

        // ����Ʈ���� ����� �� ���� ����
        enemy_list.Remove(enemy);

        // �� ������Ʈ ����
        Destroy(enemy.gameObject);
    }

    private void SpawnEnemyHpSlider(GameObject enemy)
    {
        GameObject slider_clone = Instantiate(enemy_hp_slider_prefab);

        // �����̴��� parent("Canvas")�� �ڽ����� ���� ("Canvas"�� �ڽ� ������Ʈ�� ����)
        slider_clone.transform.SetParent(canvas_transform);

        // ���� �������� �ٲ� ũ�⸦ �ٽ� ������
        slider_clone.transform.localScale = Vector3.one;

        // �����̴��� �Ѿƴٴ� ����� ������
        slider_clone.GetComponent<SliderPositionAutoSetter>().SetUp(enemy.transform);

        // �����̴��� ü�� ������ ǥ���ϵ��� ������
        slider_clone.GetComponent<EnemyHpViewer>().SetUp(enemy.GetComponent<EnemyHp>());
    }
}
