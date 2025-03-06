using Framework.Audio;
using Framework.Characters;
using UnityEngine;

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
        if(_characterBody.IsGrounded())
        {
            if(_characterBody.IsMoving())
            {
                if(Time.time - _footStepTime > footstepRate)
                {
                    _footStepTime = Time.time;
                    SoundManager.PlaySFX(footstepClips[Random.Range(0, footstepClips.Length)]);
                }
            }
        }
    }
}