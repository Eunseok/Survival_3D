using Scripts.Characters;
using UnityEngine;

namespace Scripts.Items
{
    // 인터랙션 가능한 객체에 상속할 인터페이스
    public interface IInteractable
    {
        public string GetInteractPrompt(); // UI에 표시할 정보
        public void OnInteract(); // 인터랙션 호출
    }

    public class ItemObject : MonoBehaviour, IInteractable
    {
        public ItemData data;

        // 읽기 전용 속성 도입
        private string FormattedPrompt => data != null ? FormatInteractPrompt(data) : "Unknown Item";

        // UI에 표시할 정보 준비 로직 분리 (재사용성 증가)
        private string FormatInteractPrompt(ItemData itemData)
        {
            return $"{itemData.ItemName}\n{itemData.ItemDescription}";
        }

        public string GetInteractPrompt()
        {
            return FormattedPrompt;
        }

        public void OnInteract()
        {
            if (data == null) return;
            
            var player = CharacterManager.Instance.Player;
            if (player != null)
            {
                player.AddItem?.Invoke(data);
            }

            Destroy(gameObject);
        }
    }
}