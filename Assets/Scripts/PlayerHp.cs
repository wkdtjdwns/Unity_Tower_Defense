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

    // 외부 클래스에서도 확인 할 수 있도록 프로퍼티 생성
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
            print("죽었죠?");
        }
    }

    private IEnumerator HitAlphaAnimation()
    {
        // 투명도를 설정함
        Color color = image_screen.color;
        color.a = 0.4f;
        image_screen.color = color;

        // 투명도가 다시 0%가 될때까지 감소시킴
        while (color.a >= 0.0f)
        {
            color.a -= Time.deltaTime;
            image_screen.color = color;

            // 코루틴 함수 종료
            yield return null;
        }
    }
}
