using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHpViewer : MonoBehaviour
{
    private EnemyHp enemy_hp;
    private Slider hp_slider;

    public void SetUp(EnemyHp enemy_hp)
    {
        this.enemy_hp = enemy_hp;
        hp_slider = GetComponent<Slider>();
    }

    private void Update()
    {
        // 슬라이더의 maxValue는 1이기 때문에 나눠줌
        hp_slider.value = enemy_hp.Current_hp / enemy_hp.Max_hp;
    }
}
