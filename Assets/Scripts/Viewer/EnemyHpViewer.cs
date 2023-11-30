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
        // �����̴��� maxValue�� 1�̱� ������ ������
        hp_slider.value = enemy_hp.Current_hp / enemy_hp.Max_hp;
    }
}
