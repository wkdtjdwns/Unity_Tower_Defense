using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHp : MonoBehaviour
{
    [SerializeField]
    private Image image_screen;
    [SerializeField]
    private float max_hp = 20;
    private float current_hp;

    // �ܺ� Ŭ���������� Ȯ�� �� �� �ֵ��� ������Ƽ ����
    public float Max_hp => max_hp;
    public float Current_hp => current_hp;

    private void Awake()
    {
        current_hp = max_hp;
    }

    public void TakeDamage(float damage)
    {
        current_hp -= damage;

        StopCoroutine("HitAlphaAnimation");
        StartCoroutine("HitAlphaAnimation");

        if (current_hp <= 0)
        {
            print("�׾���?");
        }
    }

    private IEnumerator HitAlphaAnimation()
    {
        // ������ ������
        Color color = image_screen.color;
        color.a = 0.4f;
        image_screen.color = color;

        // ������ �ٽ� 0%�� �ɶ����� ���ҽ�Ŵ
        while (color.a >= 0.0f)
        {
            color.a -= Time.deltaTime;
            image_screen.color = color;

            // �ڷ�ƾ �Լ� ����
            yield return null;
        }
    }
}
