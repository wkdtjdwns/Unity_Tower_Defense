using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    // Ÿ�Ͽ� Ÿ���� �Ǽ��Ǿ� �ִ��� Ȯ���ϴ� ����
    public bool is_build_tower { set; get; }

    private void Awake()
    {
        is_build_tower = false;
    }
}
