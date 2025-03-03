using System.IO;
using Framework.Utilities;
using UnityEngine;

namespace Framework.SaveSystem
{
    public class SaveManager : Singleton<SaveManager>
    {
        private const string SAVE_FILE_NAME = "save_data.json"; // 저장 파일 이름
        private string saveFilePath;

        public SaveData CurrentSaveData { get; private set; }
        

        protected override void InitializeManager()
        {
            saveFilePath = Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);

            Load(); // 게임 시작 시 저장 파일 로드
        }

        /// <summary>
        /// 데이터 저장
        /// </summary>
        public void Save()
        {
            string json = JsonUtility.ToJson(CurrentSaveData, true); // JSON 변환
            File.WriteAllText(saveFilePath, json); // 파일로 저장
            Debug.Log($"Data saved at: {saveFilePath}");
        }

        /// <summary>
        /// 데이터 불러오기
        /// </summary>
        public void Load()
        {
            if (File.Exists(saveFilePath))
            {
                string json = File.ReadAllText(saveFilePath); // 파일에서 읽기
                CurrentSaveData = JsonUtility.FromJson<SaveData>(json); // JSON 파싱
                Debug.Log("Save data loaded!");
            }
            else
            {
                Debug.Log("No save file found, creating new data.");
                CurrentSaveData = new SaveData(); // 새 데이터 생성
            }
        }

        /// <summary>
        /// 데이터 리셋
        /// </summary>
        public void ResetData()
        {
            CurrentSaveData = new SaveData(); // 기본값으로 초기화
            Save(); // 초기화된 데이터 저장
            Debug.Log("Save data reset to default settings.");
        }
    }
}