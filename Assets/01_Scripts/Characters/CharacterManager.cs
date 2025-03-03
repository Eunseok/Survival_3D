using Framework.Utilities;

public class CharacterManager : Singleton<CharacterManager>
{
    protected override void InitializeManager()
    {
        SetDontDestroyOnLoad(false);
    }

    public Player Player { get;  set; }
}