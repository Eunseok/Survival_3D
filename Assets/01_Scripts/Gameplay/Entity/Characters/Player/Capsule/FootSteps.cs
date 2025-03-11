using Framework.Audio;
using UnityEngine;

namespace Capsule
{
    public class FootSteps : MonoBehaviour
    {
        public AudioClip[] footstepClips;
        private Rigidbody _rigidbody;
        private CharacterBody _characterBody;
        public float footstepThreshold;
        public float footstepRate;
        private float _footStepTime;

        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _characterBody = GetComponent<CharacterBody>();
        }

        private void Update()
        {
            if (_characterBody.IsOnGrounded&&_characterBody.IsMoving())
            {
                // 속도에 따라 Footstep Rate 변경 (속도가 빠를수록 더 자주 들리게 설정)
                float currentFootstepRate = footstepRate / Mathf.Clamp(_rigidbody.velocity.magnitude, 1, 5);

                if (Time.time - _footStepTime >= currentFootstepRate)
                {
                    _footStepTime = Time.time;

                    // 랜덤 발소리 재생
                    SoundManager.PlaySFX(footstepClips[Random.Range(0, footstepClips.Length)]);
                }
            }
        }
    }
}