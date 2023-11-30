using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ��Ӱ� �������� �̿��ؼ� ĳ��Ÿ��, ������Ÿ�� ��� ���� Ŭ������ ������ �����ϸ� �� ������
// ������ �ش� Ŭ�������� ��� �����ϱ�� ��
public enum WeaponType { Cannon = 0, Laser, Slow, Buff, }

// enum -> ������ ����
public enum WeaponState { SearchTarget = 0, TryAttackCannon, TryAttackLaser, }

public class TowerWeapon : MonoBehaviour
{
    // [Header("string")]
    // -> �ν����� â���� ǥ�õǴ� �������� �뵵���� �����ϱ� ���� ����ϴ� �Ӽ�
    // -> string������ �ۼ��� ������ ���� ǥ������
    [Header("Commons")]
    [SerializeField]
    private TowerTemplate tower_template;
    [SerializeField]
    private Transform spawn_point;
    [SerializeField]
    private WeaponType weapon_type;

    [Header("Cannon")]
    [SerializeField]
    private GameObject projectile_prefab;

    [Header("Laser")]
    [SerializeField]
    private LineRenderer line_renderer;
    [SerializeField]
    private Transform hit_effect;
    [SerializeField]
    private LayerMask target_layer;

    private int level = 0;
    private WeaponState weapon_state = WeaponState.SearchTarget; // Ÿ�� ������ ����
    private Transform attack_target = null;
    private SpriteRenderer sprite_renderer;
    private TowerSpawner tower_spawner;
    private EnemySpawner enemy_spawner;
    private PlayerGold player_gold;
    private Tile owner_tile;

    private float added_damage;
    private int buff_level;

    // �ٸ� Ŭ���������� ������ �� �ֵ��� ������Ƽ ����
    public Sprite Tower_sprite => tower_template.weapon[level].sprite;
    public float Damage => tower_template.weapon[level].damage;
    public float Rate => tower_template.weapon[level].rate;
    public float Range => tower_template.weapon[level].range;
    public int Upgrade_cost => Level < Max_level ? tower_template.weapon[level+1].cost : 0;
    public int Sell_cost => tower_template.weapon[level].sell;
    public int Level => level + 1; // level�� ���� ���� 0�̱� ����
    public int Max_level => tower_template.weapon.Length;
    public float Slow => tower_template.weapon[level].slow;
    public float Buff => tower_template.weapon[level].buff;
    public WeaponType Weapon_type => weapon_type;
    public float Added_damage
    {
        set => added_damage = Mathf.Max(0, value);
        get => added_damage;
    }
    public int Buff_level
    {
        set => buff_level = Mathf.Max(0, value);
        get => buff_level;
    }
    public bool Is_sell {private set; get; } = false;

    public void Setup(TowerSpawner tower_spawner, EnemySpawner enemy_spawner, PlayerGold player_gold, Tile owner_tile)
    {
        sprite_renderer = GetComponent<SpriteRenderer>();
        this.tower_spawner = tower_spawner;
        this.enemy_spawner = enemy_spawner;
        this.player_gold = player_gold;
        this.owner_tile = owner_tile;

        // ���� �Ӽ��� ĳ��, ������ �ϋ�
        if (weapon_type == WeaponType.Cannon || weapon_type == WeaponType.Laser)
        {
            // ���� ���¸� WeaponState.SearchTarget���� ����
            ChangeState(WeaponState.SearchTarget);
        }
    }

    public void ChangeState(WeaponState new_state)
    {
        // ������ ������̴� ���� ����
        StopCoroutine(weapon_state.ToString());

        // ���� ����
        weapon_state = new_state;

        // ���ο� ���� ����
        StartCoroutine(weapon_state.ToString());
    }

    private void Update()
    {
        if (attack_target != null) { RotateToTarget(); }
    }

    private void RotateToTarget()
    {
        // �������κ����� �Ÿ��� ���������κ����� ������ �̿��� ��ġ�� ���ϴ� �� ��ǥ�� �̿�
        // ���� = arctan(y/x)
        
        // x, y ������ ���ϱ�
        float dx = attack_target.position.x - transform.position.x;
        float dy = attack_target.position.y - transform.position.y;

        // x, y �������� �������� ���� ���ϱ�
        // ������ radian �����̱� ������ Mathf.Rad2Deg�� ���� �� ������ ����
        float degree = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, degree);
    }

    private IEnumerator SearchTarget()
    {
        while (true)
        {
            // ���� ������ �ִ� �� Ž��
            attack_target = FindClosesAttackTarget();

            if (attack_target != null)
            {
                if (weapon_type == WeaponType.Cannon) { ChangeState(WeaponState.TryAttackCannon); }
                else if (weapon_type == WeaponType.Laser) { ChangeState(WeaponState.TryAttackLaser); }
            }

            yield return null;
        }
    }

    private IEnumerator TryAttackCannon()
    {
        while (true)
        {
            // ������ �� �� �ִ��� �˻�
            if (!IsPossibleToAttackTarget())
            {
                ChangeState(WeaponState.SearchTarget);
                break;
            }

            // �˻簡 ���� �� ����
            yield return new WaitForSeconds(tower_template.weapon[level].rate);

            // 4. ���� (�߻�ü ����)
            SpawnProjectile();
        }
    }

    private IEnumerator TryAttackLaser()
    {
        // ������, ������ Ÿ�� ȿ�� Ȱ��ȭ
        EnableLaser();

        while (true)
        {
            // Ÿ���� ������ �� �ִ��� �˻�
            if (!IsPossibleToAttackTarget())
            {
                // ������, ������ Ÿ�� ȿ�� ��Ȱ��ȭ
                DisableLaser();
                ChangeState(WeaponState.SearchTarget);
                break;
            }

            // ������ ����
            SpawnLaser();

            yield return null;
        }
    }

    public void OnBuffAroundTower()
    {
        // ���� �ʿ� �ִ� "Tower" �±� ������Ʈ�� Ž����
        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");

        for (int i = 0; i < towers.Length; ++i)
        {
            TowerWeapon weapon = towers[i].GetComponent<TowerWeapon>();

            // �̹� ������ �ް� �ְ�, ���� ���� Ÿ���� �������� ���� �����̸� �н���
            if (weapon.Buff_level > Level) { continue; }

            // ���� ���� Ÿ���� �ٸ� Ÿ���� �Ÿ��� �˻��ؼ� ���� �ȿ� Ÿ���� ������ ������
            if (Vector3.Distance(transform.position, weapon.transform.position) <= tower_template.weapon[level].range)
            {
                // ������ ������ ĳ��, ������ Ÿ���� ������
                if (weapon.Weapon_type == WeaponType.Cannon || weapon.Weapon_type == WeaponType.Laser)
                {
                    // ���ݷ� ����
                    weapon.Added_damage = weapon.Damage * (tower_template.weapon[level].buff);

                    // ���� ���� ����
                    weapon.Buff_level = level;
                }
            }
        }
    }

    private Transform FindClosesAttackTarget()
    {
        // ���� ������ �ִ� ���� ã�� ���� ���� �Ÿ��� �ִ��� ũ�� ����
        float closest_dist_sqr = Mathf.Infinity;

        // EnemySpawner�� Enemy_list�� �ִ� ���� �ʿ� �ִ� ��� �� ����
        for (int i = 0; i < enemy_spawner.Enemy_list.Count; i++)
        {
            float distance = Vector3.Distance(enemy_spawner.Enemy_list[i].transform.position, transform.position);

            // ���� �˻����� ������ �Ÿ��� ���ݹ��� ���� �ְ�, ������� �˻��� ������ �Ÿ��� ������
            if (distance <= tower_template.weapon[level].range && distance <= closest_dist_sqr)
            {
                closest_dist_sqr = distance;
                attack_target = enemy_spawner.Enemy_list[i].transform;
            }
        }

        return attack_target;
    }

    private bool IsPossibleToAttackTarget()
    {
        // Ÿ���� �ִ��� �˻� (�߻�ü�� ���� ����, Goal ���� ���� ��)
        if (attack_target == null) { return false; }

        // Ÿ���� ���� ���� �ȿ� �ִ��� �˻� (���� ������ ����� ���ο� �� Ž��
        float distance = Vector3.Distance(attack_target.position, transform.position);
        if (distance > tower_template.weapon[level].range)
        {
            attack_target = null;
            return false;
        }

        return true;
    }

    private void SpawnProjectile()
    {
        GameObject clone = Instantiate(projectile_prefab, spawn_point.position, Quaternion.identity);

        // ���ݷ� = �⺻ ���ݷ� + �߰� ���ݷ�
        float damage = tower_template.weapon[level].damage + Added_damage;
        clone.GetComponent<Projectile>().Setup(attack_target, damage);
    }

    private void EnableLaser()
    {
        line_renderer.gameObject.SetActive(true);
        hit_effect.gameObject.SetActive(true);
    }

    private void DisableLaser()
    {
        line_renderer.gameObject.SetActive(false);
        hit_effect.gameObject.SetActive(false);
    }

    private void SpawnLaser()
    {
        Vector3 direction = attack_target.position - spawn_point.position;
        RaycastHit2D[] hit = Physics2D.RaycastAll(spawn_point.position, direction, tower_template.weapon[level].range, target_layer);

        // ���� �������� ���� ���� ������ ���� �� �� ���� attack_target�� ������ ������Ʈ�� ã��
        for (int i = 0; i < hit.Length; i++)
        {
            if (hit[i].transform == attack_target)
            {
                // ���� ��������
                line_renderer.SetPosition(0, spawn_point.position);

                // ���� ��ǥ����
                line_renderer.SetPosition(1, new Vector3(hit[i].point.x, hit[i].point.y, 0) + Vector3.back);

                // Ÿ�� ȿ�� ��ġ ����
                hit_effect.position = hit[i].point;

                // ���ݷ� = �⺻ ���ݷ� + �߰� ���ݷ�
                float damage = tower_template.weapon[level].damage + Added_damage;

                // �� ü�� ���� (1�ʸ��� damage��ŭ ����)
                attack_target.GetComponent<EnemyHp>().TakeDamage(damage * Time.deltaTime);
            }
        }
    }

    public bool Upgrade()
    {
        if (player_gold.Current_gold < tower_template.weapon[level + 1].cost) { return false; }

        level++;
        sprite_renderer.sprite = tower_template.weapon[level].sprite;
        player_gold.Current_gold -= tower_template.weapon[level].cost;

        if (weapon_type == WeaponType.Laser)
        {
            // ������ ���� �������� ���� ����
            line_renderer.startWidth = 0.05f + level * 0.05f;
            line_renderer.endWidth = 0.05f;
        }

        // Ÿ���� ���׷��̵� �� �� ���� Ÿ���� ȿ�� ���� (���׷��̵� �� ������ ���� �� �ְ� ��)
        tower_spawner.OnBuffAllBuffTowers();

        return true;
    }

    public void Sell()
    {
        Is_sell = true;

        player_gold.Current_gold += tower_template.weapon[level].sell;
        owner_tile.is_build_tower = false;
        Destroy(gameObject);
    }
}
