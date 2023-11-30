using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGold : MonoBehaviour
{
    [SerializeField]
    private int current_gold = 100;

    // �ܺ� Ŭ���������� Ȯ�� �� �� �ֵ��� ������Ƽ (set, get ����) ����
    public int Current_gold
    {
        // Mathf.Max(0, value) -> 0 ~ �ش� ������ Ÿ�Կ� ���� �ִ� �������� �� ���� ū ��
        // -> 0 ~ (int = 2,147,483,647) ������ ���� ������
        set => current_gold = Mathf.Max(0, value);
        get => current_gold;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1)) { current_gold += 1000; }
    }
}
