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

    // �ܺ� Ŭ���������� Ȯ�� �� �� �ֵ��� ������Ƽ ����
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
        // �Լ��� enemy.OnDie() �Լ��� ������ ����Ǵ� ���� ������
        if (is_die) return;

        current_hp -= damage;

        StopCoroutine("HitAlphaAnimation");
        StartCoroutine("HitAlphaAnimation");

        if (current_hp <= 0)
        {
            is_die = true;
            enemy.OnDie(EnemyDestroyType.Kill); // ���� �ı��Ǿ��� ��
        }
    }

    private IEnumerator HitAlphaAnimation()
    {
        Color color = sprite_renderer.color;

        // ������ �����ϰ� ������
        color.a = 0.4f;
        sprite_renderer.color = color;

        yield return new WaitForSeconds(0.05f);

        color.a = 1.0f;
        sprite_renderer.color = color;
    }
}
