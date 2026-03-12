using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonPracticeUI : MonoBehaviour
{
    public void ResetTargets()
    {
        GameManager.Instance.UpdateGameState(GameState.CannonPractice);
    }

}
