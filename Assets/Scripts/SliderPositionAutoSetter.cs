using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderPositionAutoSetter : MonoBehaviour
{
    [SerializeField]
    private Vector3 distance = Vector3.down * 20.0f;
    private Transform target_transform;
    private RectTransform rect_transform;

    public void SetUp(Transform target)
    {
        // 슬라이더가 쫓아다닐 target 설정
        target_transform = target;

        rect_transform = GetComponent<RectTransform>();
    }

    // 오브젝트의 위치가 갱신된 이후에 슬라이더도 함께 위치를 설정하기 위해
    // LateUpdate()에서 호출함

    // LateUpdate() -> 모든 Update 종류의 함수들이 모두 실행된 다음에 실행됨
    private void LateUpdate()
    {
        // 적이 파괴되면 슬라이더 삭제
        if (target_transform == null)
        {
            Destroy(gameObject);
            return;
        }

        // 오브젝트의 월드 좌표를 기준으로 화면에서의 좌표 값을 구함
        Vector3 screen_position = Camera.main.WorldToScreenPoint(target_transform.position);

        // 슬라이더의 위치를 화면내에서의 좌표 + distance만큼 떨어진 위치로 설정
        rect_transform.position = screen_position + distance;
    }
}
