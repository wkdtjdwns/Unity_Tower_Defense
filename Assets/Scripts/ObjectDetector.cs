using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectDetector : MonoBehaviour
{
    [SerializeField]
    private TowerSpawner tower_spawner;
    [SerializeField]
    private TowerDataViewer tower_data_viewer;

    private Camera main;
    private Ray ray;
    private RaycastHit hit;
    private Transform hit_transform = null; // ���콺 Ŭ������ ������ ������Ʈ �ӽ� ����

    private void Awake()
    {
        // ���� ī�޶� ����
        main = Camera.main;
    }

    private void Update()
    {
        // ���콺�� UI�� �ӹ��� ������ ���� X
        // IsPointerOverGameObject() -> ���콺�� UI�� �ӹ��� �ִ��� �Ǵ�
        if (EventSystem.current.IsPointerOverGameObject()) { return; }

        if (Input.GetMouseButtonDown(0))
        {
            // ī�޶� ��ġ���� ȭ���� ���콺 ��ġ�� �����ϴ� ���� ����
            // ray.origin : ������ ������ġ(= ī�޶� ��ġ)
            // ray.direction : ������ ����
            ray = main.ScreenPointToRay(Input.mousePosition);

            // 2D ����͸� ���� 3D ������ ������Ʈ�� ���콺�� �����ϴ� ��
            // ������ �ε����� ������Ʈ�� �����ؼ� hit�� ����
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                hit_transform = hit.transform;

                // ������ �ε��� ������Ʈ�� �±� Ȯ��
                if (hit.transform.CompareTag("Tile")) // �±װ� "Tile"��
                {
                    // Ÿ�� ����
                    tower_spawner.SpawnTower(hit.transform);
                }

                else if (hit.transform.CompareTag("Tower")) // �±װ� "Tower"��
                {
                    // Ÿ���� ������ �����
                    tower_data_viewer.OnPanel(hit.transform);
                }
            }
        }

        else if (Input.GetMouseButtonUp(0))
        {
            // ���콺�� ������ �� ������ ������Ʈ�� ���ų� Ÿ���� �ƴϸ� ����
            if (hit_transform == null || !hit_transform.CompareTag("Tower")) { tower_data_viewer.OffPanel(); }

            hit_transform = null;
        }
    }
}
