using System.Collections;
using Framework.Audio;
using Framework.Characters;
using UnityEngine;

public class Trampoline : MonoBehaviour
{
    [Header("Trampoline Settings")]
    public float jumpForce = 50f; // 기본 점프 힘
    public float bounceAnimationDuration = 0.2f;
    private Vector3 _originalScale;
    private Coroutine _bounceCoroutine;
    
    [Header("Sound")] 
     public AudioClip jumpSound;

    private void Start()
    {
        _originalScale = transform.localScale;
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
                Debug.Log("위에서 트리거에 닿음");
                // 충돌한 객체가 플레이어 태그인지 확인
                if (!other.gameObject.CompareTag("Player")) return;

                // Rigidbody 가져오기
                if (!other.transform.TryGetComponent<MovementController>(out var movement)) return;

                movement.ApplytForceY(-jumpForce);


                // Bounce 애니메이션 실행
                if(_bounceCoroutine != null)
                    StopCoroutine(_bounceCoroutine);
        
                //애니메이션 코루틴 시작
                _bounceCoroutine = StartCoroutine(BounceAnimation());

                // 사운드 효과 재생
                SoundManager.PlaySFX(jumpSound);
            }
        }
        else
        {
            Debug.Log("아래에서 트리거에 닿음");
        }
    }

    // 점프대가 눌리는 애니메이션 효과
    private IEnumerator BounceAnimation()
    {
        var compressedScale = _originalScale * 0.8f; // 살짝 눌리는 크기

        // 크기를 줄임
        transform.localScale = compressedScale;
        yield return new WaitForSeconds(bounceAnimationDuration);

        // 원래 크기로 복구
        transform.localScale = _originalScale;
    }
}