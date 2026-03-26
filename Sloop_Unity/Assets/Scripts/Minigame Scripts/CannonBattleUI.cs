using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBattleUI : MonoBehaviour
{
    public void Retry()
    {
        GameManager.Instance.UpdateGameState(GameState.CannonBattle);
    }

    public void Return()
    {
        GameManager.Instance.UpdateGameState(GameState.Sailing);
    }



}
