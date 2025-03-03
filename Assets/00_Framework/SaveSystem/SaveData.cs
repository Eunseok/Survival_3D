using System;

namespace Framework.SaveSystem
{
    [Serializable]
    public class SaveData
    {
        public float playerScore; // 플레이어 점수 저장
        public int level; // 현재 레벨
        public float volume; // 오디오 볼륨
        public bool isMuted; // 음소거 여부

        public SaveData()
        {
            // 기본값 설정
            playerScore = 0f;
            level = 1;
            volume = 1f;
            isMuted = false;
        }
    }
}