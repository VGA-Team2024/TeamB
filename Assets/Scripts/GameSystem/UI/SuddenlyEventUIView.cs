using TeamB.Data;
using TeamB.GameSystem.Statics;
using UISystem;
using UnityEngine;

namespace TeamB.UI
{
    public class SuddenlyEventUIView : UIView
    {
        public void EndEvent()
        {
            switch (GameStatics.PrevGameState)
            {
                case GameState.Title:
                    SceneLoader.LoadScene("moch_Talk");
                    GameStatics.PrevGameState = GameState.SuddenlyEvent;
                    break;
                case GameState.Exam:
                    SceneLoader.LoadScene("moch_CharmUp");
                    GameStatics.PrevGameState = GameState.SuddenlyEvent;
                    break;
            }
        }
    }
}