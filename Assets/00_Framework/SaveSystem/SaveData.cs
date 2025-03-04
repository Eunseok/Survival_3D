using System;

namespace Framework.SaveSystem
{
    [Serializable]
    public class SaveData
    {
        public float playerScore; // 플레이어 점수 저장
        public int level = 1; // 현재 레벨
        public float volume = 1f; // 오디오 볼륨
        public bool isMuted; // 음소거 여부

    }
}