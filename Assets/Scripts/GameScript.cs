using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.Netcode;

public class GameScript : NetworkBehaviour
{
    //[SerializeField] TextMeshProUGUI roundText;
    //[SerializeField] TextMeshProUGUI stackText;

    public static GameScript Instance;

    private Deck deck;
    private int round;
    private int stack;

    private bool dealtCardsForRound;

    private Dictionary<int, List<Card>> playerHands;
    private int currentPlayerIndex;

    private int numberOfPlayers;

    [SerializeField] private Button StartGameButton;

    //public bool GameStarted
    //{
    //    get;
    //    private set;
    //}

    public NetworkVariable<bool> GameStarted = new NetworkVariable<bool>(false);


    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        NetworkManager.Singleton.ConnectionApprovalCallback += (request, response) =>
        {
            response.Approved = true;

            if (GameStarted.Value) 
            {
                response.Approved = false;
                response.Reason = "Game has started";
            }
        };


        //if (!IsServer) return;

        NetworkManager.Singleton.OnClientConnectedCallback += (id) =>
        {
            numberOfPlayers++;
        };

        NetworkManager.Singleton.OnClientDisconnectCallback += (id) =>
        {
            numberOfPlayers--;
        };

        //NetworkManager.Singleton.ConnectedClientsIds

        deck = new Deck();
        
        round = 1;
        stack = 1;

        dealtCardsForRound = false;

        playerHands = new Dictionary<int, List<Card>>(Globals.amountOfPlayers);
        for (int i = 0; i < Globals.amountOfPlayers; i++)
        {
            playerHands.Add(i, new List<Card>());
        }

        currentPlayerIndex = 0;
    }

    // TODO:
    // Multiplayer
    // Deal cards
    // Get player input on bets
    // Show other players bets
    // Show stack and hold which player that starts
    // Ask for player input (card to stack)
    // Show who won round, show player points
    // Repeat round until 20 rounds
    // Show who won all and start over if they want

    [ServerRpc]
    public void PlayCardServerRpc()
    {

    }

    void Update()
    {
        return;

        //roundText.text = $"Round: {round}";
        //stackText.text = $"Stack: {stack}";

        if (dealtCardsForRound == false)
        {
            int amountOfCards = NumberOfStacks();

            for (int i = 0; i < amountOfCards; i++)
            {
                for (int j = 0; j < Globals.amountOfPlayers; j++)
                {
                    playerHands[j].Add(deck.TopCard());
                }
            }

            dealtCardsForRound = true;
        }
    }
    
    private int NumberOfStacks()
    {
        int numOfStacks;
        if (round > 10)
            numOfStacks = 21 - round;
        else
            numOfStacks = round;

        

        if (numOfStacks * Globals.amountOfPlayers > deck.cardsAmount)
        {
            for (int i = numOfStacks - 1; i > 0; i--)
            {
                if (i * Globals.amountOfPlayers <= deck.cardsAmount)
                {
                    numOfStacks = i;
                }
            }
        }

        return numOfStacks;
    }

    public static bool IsCardEligible(Card card, Suit currentSuit, Suit trumfSuit, List<Card> hand)
    {
        // First, check if the player had nextStackCard in their hand
        // Second, check if nextStackCard's suit is of currentSuit, and if not, check if it had any other card of currentSuit on Player hand
        // Third, check if nextStackCard's suit is of trumfSuit, and if not, check if it had any other card of trumfSuit on Player hand

        // First check: Verify if the player has the nextStackCard in their hand
        if (!hand.Contains(card))
        {
            return false;
        }

        // Second check: If nextStackCard's suit is not the currentSuit,
        // verify if there are any cards of currentSuit in the player's hand
        else if (card.suit != currentSuit && currentSuit != Suit.Joker)
        {
            if (hand.Any(handCard => handCard.suit == currentSuit))
            {
                return false;
            }
            else
            {
                // Third check: If nextStackCard's suit is not the trumfSuit,
                // verify if there are any cards of trumfSuit in the player's hand
                if (card.suit != trumfSuit && trumfSuit != Suit.Joker && hand.Any(handCard => handCard.suit == trumfSuit))
                {
                    return false;
                }
            }
        }

        // Card is eligible
        return true;
    }
}

public static class Globals
{
    public static int amountOfPlayers;
}