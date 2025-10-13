using UnityEngine;

namespace GameFSM
{
    public class PlayState : GameState
    {
        public new System.Func<GameState> ConstructorFunc = () => new PlayState();

        public override void Initialise()
        {
            
        }

        public override void OnEnter()
        {
            
        }

        public override void OnExit()
        {
            
        }

        public override void Tick()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                 
            }

            
        }
    }
}
