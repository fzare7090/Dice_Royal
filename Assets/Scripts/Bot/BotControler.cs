using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Player botPlayer;
    [SerializeField] private Spawner spawner;
    [SerializeField] private AreaManager areaManager;
    [SerializeField] private Dice botDice;

    [Header("Settings")]
    [SerializeField] private float minDecisionTime = 2f;
    [SerializeField] private float maxDecisionTime = 5f;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1f);

        StartCoroutine(BotLoop());
    }

    private IEnumerator BotLoop()
    {
        yield return new WaitForSeconds(2f);

        while (true)
        {
            Think();

            yield return new WaitForSeconds(
                Random.Range(
                    minDecisionTime,
                    maxDecisionTime));
        }
    }
    private void Think()
    {
        IReadOnlyList<UnitData> hand =
            spawner.GetHand(1);

        if (hand == null || hand.Count == 0)
            return;

        int currentElixir =
            botPlayer
            .GetElexir()
            .GetElexir();

        int cheapestCardCost = int.MaxValue;

        List<UnitData> playableCards =
            new List<UnitData>();

        foreach (var card in hand)
        {
            if (card.elexir < cheapestCardCost)
            {
                cheapestCardCost = card.elexir;
            }

            if (card.elexir <= currentElixir)
            {
                playableCards.Add(card);
            }
        }

      
        if (playableCards.Count == 0)
        {
            if (botDice != null &&
                botDice.IsReady())
            {
                botDice.ForceRoll();

                Debug.Log("BOT ROLLED DICE");

                return;
            }

            return;
        }

        
        if (currentElixir < cheapestCardCost &&
            botDice != null &&
            botDice.IsReady())
        {
            botDice.ForceRoll();

            Debug.Log("BOT ROLLED DICE FOR ELIXIR");

            return;
        }

        UnitData selectedCard =
            playableCards[
                Random.Range(
                    0,
                    playableCards.Count)];

        List<Area> availableAreas =
            areaManager.GetTeamArea(1);

        if (availableAreas == null ||
            availableAreas.Count == 0)
            return;

        Area selectedArea =
            availableAreas[
                Random.Range(
                    0,
                    availableAreas.Count)];

        Vector3 spawnPosition =
            selectedArea
            .GetRectTransform()
            .position;

        bool played =
            spawner.PlayCard(
                selectedCard,
                1,
                spawnPosition);

        if (played)
        {
            Debug.Log(
                $"BOT PLAYED {selectedCard.name}");
        }
    }
}