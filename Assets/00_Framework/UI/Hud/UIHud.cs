namespace Scripts.UI
{
    public class UIHud : UIBase
    {
        protected virtual void Awake()
        {
            UIManager.Instance.SetCanvas(gameObject, false);
        }
    }
}