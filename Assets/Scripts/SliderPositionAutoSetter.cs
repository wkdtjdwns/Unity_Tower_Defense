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
        // �����̴��� �Ѿƴٴ� target ����
        target_transform = target;

        rect_transform = GetComponent<RectTransform>();
    }

    // ������Ʈ�� ��ġ�� ���ŵ� ���Ŀ� �����̴��� �Բ� ��ġ�� �����ϱ� ����
    // LateUpdate()���� ȣ����

    // LateUpdate() -> ��� Update ������ �Լ����� ��� ����� ������ �����
    private void LateUpdate()
    {
        // ���� �ı��Ǹ� �����̴� ����
        if (target_transform == null)
        {
            Destroy(gameObject);
            return;
        }

        // ������Ʈ�� ���� ��ǥ�� �������� ȭ�鿡���� ��ǥ ���� ����
        Vector3 screen_position = Camera.main.WorldToScreenPoint(target_transform.position);

        // �����̴��� ��ġ�� ȭ�鳻������ ��ǥ + distance��ŭ ������ ��ġ�� ����
        rect_transform.position = screen_position + distance;
    }
}
