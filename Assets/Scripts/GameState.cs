using UnityEngine;

namespace GameFSM
{
    public enum STATE
    {
        GAME_PLAY,
        PAUSE
    }

    public abstract class GameState : MonoBehaviour
    {
        // https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/lambda-expressions 
        // https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/reference-types#the-delegate-type
        public System.Func<GameState> ConstructorFunc;

        public abstract void Initialise();
        public abstract void OnEnter();
        public abstract void OnExit();
        public abstract void Tick();
    }
}
