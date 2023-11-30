using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu] // Asset 폴더에 해당 클래스를 Create 할 수 있게 함
public class TowerTemplate : ScriptableObject
{
    public GameObject tower_prefab;
    public GameObject follow_tower_prefab;
    public Weapon[] weapon;

    // 구조체 변수를 만들고
    // 구조체, 클래스를 직렬화 함
    [System.Serializable]
    public struct Weapon
    {
        // 클래스 내부에 구조체 변수를 만들면
        // 클래스 외부에서는 구조체 변수를 선언 할 수 없음
        // 권장하는 방법은 변수를 조작할 수 없도록 private로 만든 뒤
        // 프로퍼티를 사용하는 것임
        public Sprite sprite;
        public float damage;
        public float slow;
        public float buff;
        public float rate;
        public float range;
        public int cost;
        public int sell;
    }

}
