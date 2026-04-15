using System.Collections;
using System.Collections.Generic;
using Sloop.UI;
using TMPro;
using UnityEngine;

namespace Sloop.UI
{
    public class CannonBallResultPanel : UIPanel
    {
        [SerializeField] private TMP_Text resultText;
        public void SetResult(string msg)
        {
            resultText.text = msg;
        }

        public void Retry()
        {
            GameManager.Instance.UpdateGameState(GameState.CannonBattle);
        }

        public void Return()
        {
            GameManager.Instance.UpdateGameState(GameState.Sailing);
        }
    }

    
}