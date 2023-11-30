using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 상속과 다형성을 이용해서 캐논타워, 레이저타워 등과 같이 클래스를 나눠서 구현하면 더 좋지만
// 지금은 해당 클래스에서 모두 구현하기로 함
public enum WeaponType { Cannon = 0, Laser, Slow, Buff, }

// enum -> 열거형 변수
public enum WeaponState { SearchTarget = 0, TryAttackCannon, TryAttackLaser, }

public class TowerWeapon : MonoBehaviour
{
    // [Header("string")]
    // -> 인스펙터 창에서 표시되는 변수들을 용도별로 구분하기 위해 사용하는 속성
    // -> string형으로 작성된 내용을 굵게 표시해줌
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
    private WeaponState weapon_state = WeaponState.SearchTarget; // 타워 무기의 상태
    private Transform attack_target = null;
    private SpriteRenderer sprite_renderer;
    private TowerSpawner tower_spawner;
    private EnemySpawner enemy_spawner;
    private PlayerGold player_gold;
    private Tile owner_tile;

    private float added_damage;
    private int buff_level;

    // 다른 클래스에서도 접근할 수 있도록 프로퍼티 생성
    public Sprite Tower_sprite => tower_template.weapon[level].sprite;
    public float Damage => tower_template.weapon[level].damage;
    public float Rate => tower_template.weapon[level].rate;
    public float Range => tower_template.weapon[level].range;
    public int Upgrade_cost => Level < Max_level ? tower_template.weapon[level+1].cost : 0;
    public int Sell_cost => tower_template.weapon[level].sell;
    public int Level => level + 1; // level의 시작 값이 0이기 때문
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

        // 무기 속성이 캐논, 레이저 일떄
        if (weapon_type == WeaponType.Cannon || weapon_type == WeaponType.Laser)
        {
            // 최초 상태를 WeaponState.SearchTarget으로 설정
            ChangeState(WeaponState.SearchTarget);
        }
    }

    public void ChangeState(WeaponState new_state)
    {
        // 이전에 재생중이던 상태 종료
        StopCoroutine(weapon_state.ToString());

        // 상태 변경
        weapon_state = new_state;

        // 새로운 상태 지정
        StartCoroutine(weapon_state.ToString());
    }

    private void Update()
    {
        if (attack_target != null) { RotateToTarget(); }
    }

    private void RotateToTarget()
    {
        // 원점으로부터의 거리와 수평축으로부터의 각도를 이용해 위치를 구하는 극 좌표계 이용
        // 각도 = arctan(y/x)
        
        // x, y 변위값 구하기
        float dx = attack_target.position.x - transform.position.x;
        float dy = attack_target.position.y - transform.position.y;

        // x, y 변위값을 바탕으로 각도 구하기
        // 각도가 radian 단위이기 때문에 Mathf.Rad2Deg을 곱해 도 단위를 구함
        float degree = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, degree);
    }

    private IEnumerator SearchTarget()
    {
        while (true)
        {
            // 가장 가까이 있는 적 탐색
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
            // 공격을 할 수 있는지 검사
            if (!IsPossibleToAttackTarget())
            {
                ChangeState(WeaponState.SearchTarget);
                break;
            }

            // 검사가 끝난 후 공격
            yield return new WaitForSeconds(tower_template.weapon[level].rate);

            // 4. 공격 (발사체 생성)
            SpawnProjectile();
        }
    }

    private IEnumerator TryAttackLaser()
    {
        // 레이저, 레이저 타격 효과 활성화
        EnableLaser();

        while (true)
        {
            // 타겟을 공격할 수 있는지 검사
            if (!IsPossibleToAttackTarget())
            {
                // 레이저, 레이저 타격 효과 비활성화
                DisableLaser();
                ChangeState(WeaponState.SearchTarget);
                break;
            }

            // 레이저 공격
            SpawnLaser();

            yield return null;
        }
    }

    public void OnBuffAroundTower()
    {
        // 현재 맵에 있는 "Tower" 태그 오브젝트를 탐색함
        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");

        for (int i = 0; i < towers.Length; ++i)
        {
            TowerWeapon weapon = towers[i].GetComponent<TowerWeapon>();

            // 이미 버프를 받고 있고, 현재 버프 타워의 레벨보다 높은 버프이면 패스함
            if (weapon.Buff_level > Level) { continue; }

            // 현재 버프 타워와 다른 타워의 거리를 검사해서 범위 안에 타워가 있으면 실행함
            if (Vector3.Distance(transform.position, weapon.transform.position) <= tower_template.weapon[level].range)
            {
                // 공격이 가능한 캐논, 레이저 타워면 실행함
                if (weapon.Weapon_type == WeaponType.Cannon || weapon.Weapon_type == WeaponType.Laser)
                {
                    // 공격력 증가
                    weapon.Added_damage = weapon.Damage * (tower_template.weapon[level].buff);

                    // 버프 레벨 설정
                    weapon.Buff_level = level;
                }
            }
        }
    }

    private Transform FindClosesAttackTarget()
    {
        // 제일 가까이 있는 적을 찾기 위해 최초 거리를 최대한 크게 설정
        float closest_dist_sqr = Mathf.Infinity;

        // EnemySpawner의 Enemy_list에 있는 현재 맵에 있는 모든 적 감시
        for (int i = 0; i < enemy_spawner.Enemy_list.Count; i++)
        {
            float distance = Vector3.Distance(enemy_spawner.Enemy_list[i].transform.position, transform.position);

            // 현재 검사중인 적과의 거리가 공격범위 내에 있고, 현재까지 검사한 적보다 거리가 가까우면
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
        // 타겟이 있는지 검사 (발사체에 의해 제거, Goal 지점 도착 등)
        if (attack_target == null) { return false; }

        // 타겟이 공격 범위 안에 있는지 검사 (공격 범위를 벗어나면 새로운 적 탐색
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

        // 공격력 = 기본 공격력 + 추가 공격력
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

        // 같은 방향으로 여러 개의 광선을 쏴서 그 중 현재 attack_target과 동일한 오브젝트를 찾음
        for (int i = 0; i < hit.Length; i++)
        {
            if (hit[i].transform == attack_target)
            {
                // 선의 시작지점
                line_renderer.SetPosition(0, spawn_point.position);

                // 선의 목표지점
                line_renderer.SetPosition(1, new Vector3(hit[i].point.x, hit[i].point.y, 0) + Vector3.back);

                // 타격 효과 위치 설정
                hit_effect.position = hit[i].point;

                // 공격력 = 기본 공격력 + 추가 공격력
                float damage = tower_template.weapon[level].damage + Added_damage;

                // 적 체력 감소 (1초마다 damage만큼 감소)
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
            // 레벨에 따라 레이저의 굵기 설정
            line_renderer.startWidth = 0.05f + level * 0.05f;
            line_renderer.endWidth = 0.05f;
        }

        // 타워를 업그레이드 할 때 버프 타워의 효과 갱신 (업그레이드 된 버프를 받을 수 있게 함)
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
