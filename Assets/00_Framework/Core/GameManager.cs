using Framework.Utilities;
using UnityEngine;

namespace _00_Framework.Core
{
    public class GameManager : Singleton<GameManager>
    {
        protected override void InitializeManager()
        {
            Debug.Log("GameManager Initialized");
        }
    }
}