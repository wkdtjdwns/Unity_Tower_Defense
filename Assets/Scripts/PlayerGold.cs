using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGold : MonoBehaviour
{
    [SerializeField]
    private int current_gold = 100;

    // 외부 클래스에서도 확인 할 수 있도록 프로퍼티 (set, get 가능) 생성
    public int Current_gold
    {
        // Mathf.Max(0, value) -> 0 ~ 해당 변수의 타입에 따른 최대 값까지의 값 가장 큰 값
        // -> 0 ~ (int = 2,147,483,647) 까지로 값을 제어함
        set => current_gold = Mathf.Max(0, value);
        get => current_gold;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1)) { current_gold += 1000; }
    }
}
