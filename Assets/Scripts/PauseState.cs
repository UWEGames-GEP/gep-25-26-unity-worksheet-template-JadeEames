using UnityEngine;

namespace GameFSM
{
    public class PauseState : GameState
    {
        public new System.Func<GameState> ConstructorFunc = () => new PauseState();

        public override void Initialise()
        {

        }

        public override void OnEnter()
        {
            Time.timeScale = 0;
        }

        public override void OnExit()
        {
            Time.timeScale = 1;
        }

        public override void Tick()
        {
            
        }
    }
}