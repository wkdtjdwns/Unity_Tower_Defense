using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Move move;
    private Transform target;
    private float damage;

    public void Setup(Transform target, float damage)
    {
        move = GetComponent<Move>();
        this.target = target;
        this.damage = damage;
    }

    private void Update()
    {
        if (target != null)
        {
            // �߻�ü�� target�� ��ġ�� �̵�
            Vector3 dir = (target.position - transform.position).normalized;
            move.MoveTo(dir);
        }

        else
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy")) { return; } // ���� �ƴ� ���� �ε�����
        if (collision.transform != target) { return; } // ���� targer�� �ƴ� ���� �ε�����

        /*collision.GetComponent<Enemy>().OnDie();*/
        collision.GetComponent<EnemyHp>().TakeDamage(damage);
        Destroy(gameObject);
    }
}
