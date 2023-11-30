using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    // 타일에 타워가 건설되어 있는지 확인하는 변수
    public bool is_build_tower { set; get; }

    private void Awake()
    {
        is_build_tower = false;
    }
}
