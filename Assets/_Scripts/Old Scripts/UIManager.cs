using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    [SerializeField] private Text playerName;
    [SerializeField] private Slider playerHealth;

    [SerializeField] private Text enemyName;
    [SerializeField] private Slider enemyHealth;

    public void StartBattle(List<S_Actor> heroes, List<S_Actor> enemies)
    {
        playerName.text = heroes[0].GetActorName();
        enemyName.text = enemies[0].GetActorName();
    }

    public void UpdateActorHealth(int current, int max, string actor)
    {
        switch(actor)
        {
            case "Player":
                playerHealth.value = current;
                playerHealth.maxValue = max;
                break;
            case "Enemy":
                enemyHealth.value = current;
                enemyHealth.maxValue = max;
                break;

        }

    }
}
