using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHp : MonoBehaviour
{
    [SerializeField]
    private float max_hp;
    private float current_hp;
    private bool is_die;
    private Enemy enemy;
    private SpriteRenderer sprite_renderer;

    // 외부 클래스에서도 확인 할 수 있도록 프로퍼티 생성
    public float Max_hp => max_hp;
    public float Current_hp => current_hp;

    private void Awake()
    {
        current_hp = max_hp;

        enemy = GetComponent<Enemy>();
        sprite_renderer = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(float damage)
    {
        // 함수가 enemy.OnDie() 함수가 여러번 실행되는 것을 방지함
        if (is_die) return;

        current_hp -= damage;

        StopCoroutine("HitAlphaAnimation");
        StartCoroutine("HitAlphaAnimation");

        if (current_hp <= 0)
        {
            is_die = true;
            enemy.OnDie(EnemyDestroyType.Kill); // 적이 파괴되었을 떄
        }
    }

    private IEnumerator HitAlphaAnimation()
    {
        Color color = sprite_renderer.color;

        // 투명도를 설정하고 적용함
        color.a = 0.4f;
        sprite_renderer.color = color;

        yield return new WaitForSeconds(0.05f);

        color.a = 1.0f;
        sprite_renderer.color = color;
    }
}
