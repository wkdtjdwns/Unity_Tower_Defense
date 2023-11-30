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
    private Transform hit_transform = null; // 마우스 클릭으로 선택한 오브젝트 임시 저장

    private void Awake()
    {
        // 메인 카메라 설정
        main = Camera.main;
    }

    private void Update()
    {
        // 마우스가 UI에 머물러 있으면 실행 X
        // IsPointerOverGameObject() -> 마우스가 UI에 머물러 있는지 판단
        if (EventSystem.current.IsPointerOverGameObject()) { return; }

        if (Input.GetMouseButtonDown(0))
        {
            // 카메라 위치에서 화면의 마우스 위치를 관통하는 광선 생성
            // ray.origin : 광선의 시작위치(= 카메라 위치)
            // ray.direction : 광선의 방향
            ray = main.ScreenPointToRay(Input.mousePosition);

            // 2D 모니터를 통해 3D 월드의 오브젝트를 마우스로 선택하는 법
            // 광선에 부딪히는 오브젝트를 검출해서 hit에 저장
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                hit_transform = hit.transform;

                // 광선에 부딪힌 오브젝트의 태그 확인
                if (hit.transform.CompareTag("Tile")) // 태그가 "Tile"임
                {
                    // 타워 생성
                    tower_spawner.SpawnTower(hit.transform);
                }

                else if (hit.transform.CompareTag("Tower")) // 태그가 "Tower"임
                {
                    // 타워의 정보를 출력함
                    tower_data_viewer.OnPanel(hit.transform);
                }
            }
        }

        else if (Input.GetMouseButtonUp(0))
        {
            // 마우스를 때었을 때 선택한 오브젝트가 없거나 타워가 아니면 실행
            if (hit_transform == null || !hit_transform.CompareTag("Tower")) { tower_data_viewer.OffPanel(); }

            hit_transform = null;
        }
    }
}
