using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform pointA; // 이동 시작점
    public Transform pointB; // 이동 끝점
    public float speed = 2f; // 이동 속도
    public bool isLooping = true; // 반복 여부

    private Vector3 _targetPoint;
    private bool _movingToB = true; // 현재 이동 방향

    void Start()
    {
        // 초기 타겟 포인트 설정
        _targetPoint = pointB.position;
    }

    void FixedUpdate()
    {
        // 플랫폼 이동
        MovePlatform();
    }

    private void MovePlatform()
    {
        // 현재 위치에서 목표 지점으로 이동
        transform.position = Vector3.MoveTowards(transform.position, _targetPoint, speed * Time.deltaTime);

        // 목표 지점에 도달하면 방향 변경
        if (!(Vector3.Distance(transform.position, _targetPoint) < 0.1f)) return;
        _targetPoint = _movingToB ? pointA.position : pointB.position;
        _movingToB = !_movingToB; // 방향 전환
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Ray를 캐릭터 아래로 발사
        Ray ray = new Ray(other.bounds.center - new Vector3(0, other.bounds.extents.y, 0), Vector3.down);

        // 충돌 결과
        if (Physics.Raycast(ray, out RaycastHit hit, 1f))
        {
            if (hit.collider.gameObject == this.gameObject)
            {
                other.transform.SetParent(transform);
                Debug.Log("위에서 트리거에 닿음");
            }
        }
        else
        {
            Debug.Log("아래에서 트리거에 닿음");
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.SetParent(null); // 플랫폼에서 내려올 때 자식 관계 해제
        }
    }
}