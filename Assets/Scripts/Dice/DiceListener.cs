using UnityEngine;

public class DiceListener : MonoBehaviour
{
    [SerializeField] private Dice dice;
    [SerializeField] private Player targetPlayer;

    private void Start()
    {
        if (dice == null)
        {
   
            if (dice == null)
            {
                Debug.LogError("[DiceListener] Dice not found!");
                return;
            }
        }

        dice.OnDiceRolled += OnDiceRolledHandler;
    }

    private void OnDiceRolledHandler(int diceNumber)
    {
        Debug.Log($"[DiceListener] Dice number: {diceNumber}");

        if (targetPlayer != null)
        {
            Debug.Log($"[DiceListener] Notified player {targetPlayer.name}");
        }
    }

    private void OnDestroy()
    {
        if (dice != null)
            dice.OnDiceRolled -= OnDiceRolledHandler;
    }
}