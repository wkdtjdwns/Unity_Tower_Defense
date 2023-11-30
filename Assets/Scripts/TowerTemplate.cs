using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu] // Asset ������ �ش� Ŭ������ Create �� �� �ְ� ��
public class TowerTemplate : ScriptableObject
{
    public GameObject tower_prefab;
    public GameObject follow_tower_prefab;
    public Weapon[] weapon;

    // ����ü ������ �����
    // ����ü, Ŭ������ ����ȭ ��
    [System.Serializable]
    public struct Weapon
    {
        // Ŭ���� ���ο� ����ü ������ �����
        // Ŭ���� �ܺο����� ����ü ������ ���� �� �� ����
        // �����ϴ� ����� ������ ������ �� ������ private�� ���� ��
        // ������Ƽ�� ����ϴ� ����
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
