using GamConstant;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class BetPhaseManager : MonoBehaviour//NetworkBehaviour
{
    public float bettingTime = 30f; // Timer duration
    private float timer = 0f; // Countdown timer
    private HUDController hudController; // Reference to the HUD controller

    private Dictionary<Players, PlayerZone> playerReadyStates = new(); // Track player states

    //[SyncVar]
    private bool bettingPhaseActive = false; // Synchronize phase state
    private bool bettingPhaseStarted = false; // Synchronize phase state

    public bool IsBettingPhaseActive() => bettingPhaseActive;
    public bool IsBettingPhaseStarted() => bettingPhaseStarted;

    // Start the betting phase
    //[Server]
    public void StartBettingPhase(List<PlayerZone> players)
    {
        bettingPhaseActive = true;
        timer = bettingTime;

        // Initialize player ready states
        playerReadyStates.Clear();
        hudController = HUDController.Instance;

        foreach (var player in players)
        {
            playerReadyStates[player.Owner] = player;
            player.StartPlacingBet();
        }

        hudController.ShowBetTimer(bettingTime);

        StartCoroutine(BettingTimer());
    }

    // Coroutine for the betting timer
    //[Server]
    private IEnumerator BettingTimer()
    {
        yield return new WaitForSeconds(1f);

        bettingPhaseStarted = true;

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            hudController.UpdateBetTimer(timer);

            // Check if all players are ready
            if (AreAllPlayersReady())
            {
                EndBettingPhase();
                yield break;
            }

            yield return null;
        }

        // Timer expired, move to the next phase
        EndBettingPhase();
    }

    // Check if all players have finished betting
    private bool AreAllPlayersReady()
    {
        foreach (var player in playerReadyStates.Values)
        {
            if (!player.IsBetPlaced()) return false;
        }
        return true;
    }

    // Player signals they are ready
    //[Server]
    public void PlayerFinishedBetting(Players player)
    {
        if (playerReadyStates.ContainsKey(player))
        {
            playerReadyStates[player].ConfirmBet();
        }
    }

    // End the betting phase
    //[Server]
    private void EndBettingPhase()
    {
        bettingPhaseActive = false;
        bettingPhaseStarted = false;
        // Notify all clients and proceed to the next phase
        RpcEndBettingPhase();
    }

    // Client-side notification
    //[ClientRpc]
    private void RpcEndBettingPhase()
    {
        Debug.Log("Betting phase ended. Moving to the next phase.");
        // Trigger transition to the next phase
        hudController.HideBetTimer();
        timer = 0;

        GameController.Instance.BetPhaseEnded();
    }
}
