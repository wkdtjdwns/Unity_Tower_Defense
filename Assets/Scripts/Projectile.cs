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
            // 발사체를 target의 위치로 이동
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
        if (!collision.CompareTag("Enemy")) { return; } // 적이 아닌 대상과 부딪히면
        if (collision.transform != target) { return; } // 현재 targer이 아닌 적과 부딪히면

        /*collision.GetComponent<Enemy>().OnDie();*/
        collision.GetComponent<EnemyHp>().TakeDamage(damage);
        Destroy(gameObject);
    }
}
