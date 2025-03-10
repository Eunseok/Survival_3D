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

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("OnCollisionEnter");

            // 충돌된 모든 접점(Contact Point) 검사
            foreach (ContactPoint contact in collision.contacts)
            {
                // 충돌체에 아래가 아니면 continue
                if (!(Vector3.Dot(contact.normal, Vector3.down) > 0.9f)) continue;
                
                // 플랫폼의 자식으로 설정
                collision.transform.SetParent(transform);
                
                break; 
            }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null); // 플랫폼에서 내려올 때 자식 관계 해제
        }
    }
}