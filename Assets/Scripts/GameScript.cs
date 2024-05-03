using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using System.Text;

public class GameScript : NetworkBehaviour
{
    //[SerializeField] TextMeshProUGUI roundText;
    //[SerializeField] TextMeshProUGUI stackText;

    public static GameScript Instance;

    private Deck Deck;
    private int Round;
    private int Stack;

    private bool dealtCardsForRound;

    private Dictionary<ulong, List<Card>> PlayerHands;
    private int currentPlayerIndex;

    private int NumberOfPlayers;

    [SerializeField] private Button StartGameButton;

    public NetworkVariable<bool> GameStarted = new NetworkVariable<bool>(false);

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        } 
        else 
        {
            Destroy(gameObject);
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

        NetworkManager.Singleton.OnClientConnectedCallback += (id) =>
        {
            Debug.Log($"Player {id} connected!");
            NumberOfPlayers++;
        };

        NetworkManager.Singleton.OnClientDisconnectCallback += (id) =>
        {
            Debug.Log($"Player {id} disconnected!");
            NumberOfPlayers--;
        };

        StartGameButton.onClick.AddListener(() =>
        {
            StartGame();
        });
    }

    public void StartGame()
    {
        Deck = new Deck();
        
        Round = 1;
        Stack = 1;

        dealtCardsForRound = false;

        PlayerHands = new Dictionary<ulong, List<Card>>();
        for (int i = 0; i < NumberOfPlayers; i++)
        {
            PlayerHands.Add(NetworkManager.Singleton.ConnectedClientsIds[i], new List<Card>());
        }

        

        currentPlayerIndex = 0;

        DealCards();

        GameStarted.Value = true;
    }

    private void DealCards()
    {
        int amountOfCards = NumberOfStacks();

        for (int i = 0; i < amountOfCards; i++)
        {
            foreach (var playerHand in PlayerHands)
            {
                playerHand.Value.Add(Deck.TopCard());
            }
        }

        dealtCardsForRound = true;

        foreach (var hand in PlayerHands)
        {
            ClientRpcParams clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[]
                    {
                        hand.Key
                    }
                }
            };

            string serializedHand = SerializeHand(hand.Value);

            UpdateHandClientRpc(serializedHand, clientRpcParams);
        }
    }

    private string SerializeHand(List<Card> hand) 
    {
        StringBuilder stringBuilder = new StringBuilder();

        foreach (var card in hand)
        {
            stringBuilder.Append(card.Index);
            stringBuilder.Append(" ");
        }

        return stringBuilder.ToString();
    }

    
    [ClientRpc]
    public void UpdateHandClientRpc(string serializedHand, ClientRpcParams clientRpcParams)
    {
        Debug.Log("Got hand: " + serializedHand);
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
                for (int j = 0; j < 1/*Globals.amountOfPlayers*/; j++)
                {
                    //PlayerHands[j].Add(Deck.TopCard());
                }
            }

            dealtCardsForRound = true;
        }
    }
    
    /// <returns>The number of stacks for this round</returns>
    private int NumberOfStacks()
    {
        int numOfStacks;

        if (Round > 10)
            numOfStacks = 21 - Round;
        else
            numOfStacks = Round;
        

        if (numOfStacks * NumberOfPlayers > Deck.CardsAmount)
        {
            for (int i = numOfStacks - 1; i > 0; i--)
            {
                if (i * NumberOfPlayers <= Deck.CardsAmount)
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
        else if (card.Suit != currentSuit && currentSuit != Suit.Joker)
        {
            if (hand.Any(handCard => handCard.Suit == currentSuit))
            {
                return false;
            }
            else
            {
                // Third check: If nextStackCard's suit is not the trumfSuit,
                // verify if there are any cards of trumfSuit in the player's hand
                if (card.Suit != trumfSuit && trumfSuit != Suit.Joker && hand.Any(handCard => handCard.Suit == trumfSuit))
                {
                    return false;
                }
            }
        }

        // Card is eligible
        return true;
    }
}
