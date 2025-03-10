using System.Collections;
using Framework.Audio;
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

    private void OnCollisionEnter(Collision other)
    {
        // 충돌한 객체가 플레이어 태그인지 확인
        if (!other.gameObject.CompareTag("Player")) return;

        // Rigidbody 가져오기
        if (!other.transform.TryGetComponent<Rigidbody>(out var rb)) return;

        // 현재 y축 속도를 반전하거나 보완하기 위해 계산
        var jumpDirection = transform.up;
        float adjustedJumpForce = Mathf.Max(jumpForce, Mathf.Abs(rb.velocity.y)); // 현재 y 속도 반영
        rb.velocity = Vector3.zero; // 기존 속도 초기화 (속도의 누적 방지)

        // 점프 적용
        rb.AddForce(jumpDirection * adjustedJumpForce, ForceMode.Impulse);


        // Bounce 애니메이션 실행
        if(_bounceCoroutine != null)
            StopCoroutine(_bounceCoroutine);
        
        _bounceCoroutine = StartCoroutine(BounceAnimation());

        // 사운드 효과 재생
        SoundManager.PlaySFX(jumpSound);
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