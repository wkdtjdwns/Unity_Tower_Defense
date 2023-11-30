using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// System과 UnityEngine에 모두 Random 클래스가 있기 때문에 
// 둘 중 하나를 선택함
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
    private List<Enemy> enemy_list; // 현재 맵에 존재하는 모든 적의 정보

    // 외부 클래스에서 값을 확인 할 수 있도록 프로퍼티 생성
    // 적의 생성과 삭제는 EnemySpawner에서 관리하기 때문에 set은 필요 X
    public List<Enemy> Enemy_list => enemy_list;
    public int Current_enemy_count => current_enemy_count;
    public int Max_enemy_count => current_wave.max_enemy_count;

    private void Awake()
    {
        // 적 리스트 메모리 할당
        enemy_list = new List<Enemy>();
    }

    public void StartWave(Wave wave)
    {
        // 매개변수로 받아온 웨이브 정보 저장
        current_wave = wave;

        // 현재 웨이브의 최대 적 숫자 저장
        current_enemy_count = current_wave.max_enemy_count;

        // 웨이브 시작
        StartCoroutine("SpawnEnemy");
    }

    private IEnumerator SpawnEnemy()
    {
        int spawn_enemy_count = 0;

        /*while (true)*/
        // 현재 웨이브에서 생성되어야 하는 적의 숫자만큼 적을 생성하고 코루틴 함수 종료
        while (spawn_enemy_count < current_wave.max_enemy_count)
        {
            /*GameObject clone = Instantiate(enemy_prefab);*/

            // 웨이브에 등장하는 적의 종류가 여러 종류일 때 임의의 적이 등장하게 함
            int enemy_index = Random.Range(0, current_wave.enemy_prefabs.Length);
            GameObject clone = Instantiate(current_wave.enemy_prefabs[enemy_index]);
            Enemy enemy = clone.GetComponent<Enemy>();

            enemy.Setup(this, way_points);
            enemy_list.Add(enemy); // 리스트에 생성된 적의 정보 저장

            SpawnEnemyHpSlider(clone);

            spawn_enemy_count++;

            yield return new WaitForSeconds(current_wave.spawn_time);
        }
    }

    public void DestroyEnemy(EnemyDestroyType type, Enemy enemy, int gold)
    {
        // 적이 목표지점까지 도달했을 때
        if (type == EnemyDestroyType.Arrive)
        {
            player_hp.TakeDamage(1);
        }

        else if (type == EnemyDestroyType.Kill)
        {
            player_gold.Current_gold += gold;
        }

        // 적이 사망할 때마다 생존 적 숫자 감소 (UI 표시)
        current_enemy_count--;

        // 리스트에서 사망한 적 정보 삭제
        enemy_list.Remove(enemy);

        // 적 오브젝트 삭제
        Destroy(enemy.gameObject);
    }

    private void SpawnEnemyHpSlider(GameObject enemy)
    {
        GameObject slider_clone = Instantiate(enemy_hp_slider_prefab);

        // 슬라이더를 parent("Canvas")의 자식으로 설정 ("Canvas"의 자식 오브젝트로 설정)
        slider_clone.transform.SetParent(canvas_transform);

        // 계층 설정으로 바뀐 크기를 다시 설정함
        slider_clone.transform.localScale = Vector3.one;

        // 슬라이더가 쫓아다닐 대상을 설정함
        slider_clone.GetComponent<SliderPositionAutoSetter>().SetUp(enemy.transform);

        // 슬라이더가 체력 정보를 표시하도록 설정함
        slider_clone.GetComponent<EnemyHpViewer>().SetUp(enemy.GetComponent<EnemyHp>());
    }
}
