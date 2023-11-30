using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    public float speed = 0.0f;
    [SerializeField]
    private Vector3 move_dir = Vector3.zero;
    private float base_move_speed;

    // speed 변수의 프로퍼티 (set, get 가능)
    public float Move_speed
    {
        set => speed = Mathf.Max(0, value);
        get => speed;
    }

    private void Awake()
    {
        base_move_speed = speed;
    }

    private void Update()
    {
        transform.position += move_dir * speed * Time.deltaTime;
    }

    public void MoveTo(Vector3 dir)
    {
        move_dir = dir;
    }

    public void ResetMoveSpeed()
    {
        speed = base_move_speed;
    }
}
