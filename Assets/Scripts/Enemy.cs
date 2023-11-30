using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyDestroyType { Kill = 0, Arrive }

public class Enemy : MonoBehaviour
{
    private int way_point_cnt; // �̵� ��� ����
    private Transform[] way_points; // �̵� ���
    private int cur_index; // ���� ��ǥ���� �ε���
    private Move move; // ������Ʈ �̵� ���� (Move Ŭ����)
    private EnemySpawner enemy_spawner;
    [SerializeField]
    private int gold = 10;

    public void Setup(EnemySpawner enemy_spawner, Transform[] way_points)
    {
        move = GetComponent<Move>();
        this.enemy_spawner = enemy_spawner;

        // �� �̵� ��� ����
        way_point_cnt = way_points.Length;
        this.way_points = new Transform[way_point_cnt];
        this.way_points = way_points;

        // ���� ��ġ�� ó������ ����
        transform.position = way_points[cur_index].position;

        // �� �̵� �� ��ǥ���� ���� �ڷ�ƾ �Լ� ����
        StartCoroutine("OnMove");
    }

    private IEnumerator OnMove()
    {
        // ���� �̵� ���� �����ϱ�
        NextMoveTo();

        while (true)
        {
            // �� ������Ʈ ȸ�� (��� ��)
            transform.Rotate(Vector3.forward * 10);

            // ���� ���� ��ġ�� ��ǥ �Ÿ��� ��ġ�� 0.02 * �ӵ� ���� ���� �� -> �ӵ��� �������� ������ ���ǹ��� �ɸ��� �ʰ� �մ� ������Ʈ�� ���� �� ����
            if (Vector3.Distance(transform.position, way_points[cur_index].position) < 0.02f * move.speed)
            {
                // ���� �̵� ���� ����
                NextMoveTo();
            }

            yield return null;
        }
    }

    private void NextMoveTo()
    {
        // �̵��ؾ��� �Ÿ��� �������� ��
        if (cur_index < way_point_cnt - 1)
        {
            // ���� ��ġ�� ��ǥ ��ġ�� �̵�
            transform.position = way_points[cur_index].position;

            // �̵� ���� ���� = ���� ��ǥ ����
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
