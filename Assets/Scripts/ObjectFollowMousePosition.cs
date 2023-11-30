using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFollowMousePosition : MonoBehaviour
{
    private Camera main_camera;

    private void Awake() { main_camera = Camera.main; }

    private void Update()
    {
        // ȭ���� ���콺 ��ǥ�� �������� ���� ���� ��ǥ�� ����
        Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y);
        transform.position = main_camera.ScreenToWorldPoint(position);

        // z ��ġ�� 0���� ������
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }
}
