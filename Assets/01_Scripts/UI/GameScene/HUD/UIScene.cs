namespace Scripts.UI
{
    public class UIScene : UIBase
    {
        public override void Initialize()
        {
            UIManager.Instance.SetCanvas(gameObject, false);
        }
    }
}