using System.Collections.Generic;
using Framework.Utilities;
using UnityEngine;

namespace Framework.Audio
{
    /// <summary>
    /// 게임 전역에서 오디오(BGM/SFX)를 관리하는 매니저
    /// </summary>
    public class AudioManager : Singleton<AudioManager>
    {
        [Header("BGM Settings")] [SerializeField]
        private AudioSource bgmSource; // 배경 음악용 오디오 소스

        [SerializeField] private List<AudioClip> bgmClips; // 배경 음악 클립 리스트

        [Header("SFX Settings")] [SerializeField]
        private AudioSource sfxSource; // 효과음용 오디오 소스

        [SerializeField] private float sfxVolume = 1f; // 효과음 볼륨

        [Header("Volume Settings")] [Range(0f, 1f)]
        public float bgmVolume = 1f; // 배경음악(BGM)의 볼륨

        [SerializeField] private bool isMuted; // 음소거 설정 여부

        protected override void InitializeManager()
        {
            ApplyVolumeSettings(); // 초기 볼륨 설정
        }

        /// <summary>
        /// BGM 재생
        /// </summary>
        /// <param name="clipName">재생할 BGM 클립 이름</param>
        public void PlayBGM(string clipName)
        {
            if (isMuted) return;

            // 이름으로 BGM 찾기
            AudioClip clip = bgmClips.Find(c => c.name == clipName);

            if (clip != null && bgmSource.clip != clip)
            {
                bgmSource.clip = clip;
                bgmSource.loop = true;
                bgmSource.volume = bgmVolume;
                bgmSource.Play();
            }
        }

        /// <summary>
        /// BGM 중지
        /// </summary>
        public void StopBGM()
        {
            bgmSource.Stop();
        }

        /// <summary>
        /// SFX 재생
        /// </summary>
        /// <param name="clip">재생할 효과음 클립</param>
        public void PlaySFX(AudioClip clip)
        {
            if (isMuted || clip == null) return;

            sfxSource.PlayOneShot(clip, sfxVolume);
        }

        /// <summary>
        /// 볼륨 설정 적용
        /// </summary>
        private void ApplyVolumeSettings()
        {
            bgmSource.volume = isMuted ? 0f : bgmVolume;
            sfxSource.volume = isMuted ? 0f : sfxVolume;
        }

        /// <summary>
        /// 오디오 음소거 설정
        /// </summary>
        /// <param name="mute">음소거 여부</param>
        public void SetMute(bool mute)
        {
            isMuted = mute;
            ApplyVolumeSettings();
        }
        
        
        /// <summary>
        /// 배경음악 볼륨 설정
        /// </summary>
        /// <param name="volume">볼륨 크기</param>
        public void SetBGMVolume(float volume)
        {
            bgmVolume = Mathf.Clamp(volume, 0f, 1f);
            ApplyVolumeSettings();
        }
        
        
        /// <summary>
        /// 효과음 볼륨 설정
        /// </summary>
        /// <param name="volume">볼륨 크기</param>
        public void SetSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp(volume, 0f, 1f);
            ApplyVolumeSettings();
        }
        
    }
}