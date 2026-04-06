using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sloop.UI
{
    public class CannonBattleHUDPanel : UIPanel
    {
        [SerializeField] private Slider playerHealthSlider;
        [SerializeField] private Slider enemyHealthSlider;
        
        public void HealthInit(int playerMax, int enemyMax)
        {
            playerHealthSlider.maxValue = playerMax;
            enemyHealthSlider.maxValue = enemyMax;
        }
        public void SetHealth(int player, int enemy)
        {
            playerHealthSlider.value = player;
            enemyHealthSlider.value= enemy;
        }
    }
}