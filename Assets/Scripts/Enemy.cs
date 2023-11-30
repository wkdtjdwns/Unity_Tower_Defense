using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyDestroyType { Kill = 0, Arrive }

public class Enemy : MonoBehaviour
{
    private int way_point_cnt; // 이동 경로 개수
    private Transform[] way_points; // 이동 경로
    private int cur_index; // 현재 목표지점 인덱스
    private Move move; // 오브젝트 이동 제어 (Move 클래스)
    private EnemySpawner enemy_spawner;
    [SerializeField]
    private int gold = 10;

    public void Setup(EnemySpawner enemy_spawner, Transform[] way_points)
    {
        move = GetComponent<Move>();
        this.enemy_spawner = enemy_spawner;

        // 적 이동 경로 설정
        way_point_cnt = way_points.Length;
        this.way_points = new Transform[way_point_cnt];
        this.way_points = way_points;

        // 적의 위치를 처음으로 설정
        transform.position = way_points[cur_index].position;

        // 적 이동 및 목표지정 설정 코루틴 함수 시작
        StartCoroutine("OnMove");
    }

    private IEnumerator OnMove()
    {
        // 다음 이동 방향 설정하기
        NextMoveTo();

        while (true)
        {
            // 적 오브젝트 회전 (없어도 됨)
            transform.Rotate(Vector3.forward * 10);

            // 적의 현재 위치와 목표 거리의 위치가 0.02 * 속도 보다 작을 때 -> 속도를 곱해주지 않으면 조건문에 걸리지 않고 뚫는 오브젝트가 생길 수 있음
            if (Vector3.Distance(transform.position, way_points[cur_index].position) < 0.02f * move.speed)
            {
                // 다음 이동 방향 설정
                NextMoveTo();
            }

            yield return null;
        }
    }

    private void NextMoveTo()
    {
        // 이동해야할 거리가 남아있을 때
        if (cur_index < way_point_cnt - 1)
        {
            // 적의 위치를 목표 위치로 이동
            transform.position = way_points[cur_index].position;

            // 이동 방향 설정 = 다음 목표 지점
            cur_index++;
            Vector3 dir = (way_points[cur_index].position - transform.position).normalized;
            move.MoveTo(dir);
        }

        else
        {
            gold = 0;
            OnDie(EnemyDestroyType.Arrive);
        }
    }

    public void OnDie(EnemyDestroyType type)
    {
        enemy_spawner.DestroyEnemy(type, this, gold);
    }
}
