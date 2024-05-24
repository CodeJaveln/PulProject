using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using System.Text;
using System;
using UnityEngine.XR;

public class GameScript : NetworkBehaviour
{
    /// <summary>
    /// The gamescript singelton. Holds all game info available. The server should have authority
    /// </summary>
    public static GameScript Instance;


    // Game variables
    private Deck Deck;
    private int Round;
    private int Stack;

    private bool DealtCardsForRound;

    private int NumberOfPlayers;

    public List<PlayerInfo> Players;
    private Dictionary<ulong, List<Card>> PlayerHands;

    public Card TrumpCard;
    //public NetworkVariable<int> TrumpCard = new NetworkVariable<int>(GameState.Betting);


    public NetworkVariable<bool> GameStarted = new NetworkVariable<bool>(false);
    private int CurrentPlayerIndex;

    private Dictionary<ulong, byte> PlayerBets;


    // UI stuff
    [SerializeField] private Button StartGameButton;
    [SerializeField] private Button BetButton;
    [SerializeField] private TMP_InputField BetInputField;

    public NetworkVariable<GameState> State = new NetworkVariable<GameState>(global::GameState.Betting);



    private List<Card> CurrentStack = new List<Card>();

    // Hög -> trump -> whatevs
    // joker wins allways
    // one round, several piles
    // only get point if guess = wins. 10 + guess if win


    void Start()
    {
        // Make sure only one game instance can exist
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
            if (IsHost) InitializeGame();
        });

        BetButton.onClick.AddListener(() =>
        {
            if (byte.TryParse(BetInputField.text, out byte bet))
            {
                SetBetServerRpc(bet, new ServerRpcParams());
            }
        });
    }

    public void InitializeGame()
    {
        Deck = new Deck();
        
        Round = 1;
        Stack = 1;

        DealtCardsForRound = false;

        Players = new List<PlayerInfo>();

        for (int i = 0; i < NumberOfPlayers; i++)
        {
            Players.Add(new PlayerInfo(NetworkManager.Singleton.ConnectedClientsIds[i]));
        }



        PlayerBets = new Dictionary<ulong, byte>();

        //currentPlayerIndex = 0;

        DealCards();

        GameStarted.Value = true;
    }


    private void StartRound()
    {
        Deck.Reset();
        TrumpCard = Deck.DrawTopCard();
        DealCards();
    }

    private void DealCards()
    {
        int amountOfCards = NumberOfStacks();

        PlayerHands = new Dictionary<ulong, List<Card>>();

        for (int i = 0; i < NumberOfPlayers; i++)
        {
            PlayerHands.Add(Players[i].NetworkId, new List<Card>());
        }

        for (int i = 0; i < amountOfCards; i++)
        {
            foreach (var playerHand in PlayerHands)
            {
                playerHand.Value.Add(Deck.DrawTopCard());
            }
        }

        DealtCardsForRound = true;

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

            UpdateClientHandClientRpc(serializedHand, clientRpcParams);
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

    private List<Card> DeserializeHand(string serializedHand)
    {
        List<Card> cards = new List<Card>();

        string[] strings = serializedHand.Trim().Split(' ');

        foreach (var item in strings)
        {
            cards.Add(Card.AllCards[Convert.ToInt32(item)]);
        }

        return cards;
    }
    
    [ClientRpc]
    public void UpdateClientHandClientRpc(string serializedHand, ClientRpcParams clientRpcParams)
    {
        Debug.Log("Got hand: " + serializedHand);

        CardVisualizer.Instance.VisualizeCards(DeserializeHand(serializedHand));
    }

    [ClientRpc]
    public void AskForBetClientRpc()
    {
        // Notify that we can bet
        Debug.Log("Yo, go bet!");
    }

    [ClientRpc]
    public void AskForCardClientRpc(ClientRpcParams clientRpcParams)
    {
        Debug.Log("Yo, go bet!");
        // Notify that we can bet'
    }

    /// <summary>
    /// Called when client wants to bet
    /// </summary>
    /// <param name="bet"></param>
    /// <param name="serverRpcParams"></param>
    [ServerRpc(RequireOwnership = false)]
    public void SetBetServerRpc(byte bet, ServerRpcParams serverRpcParams)
    {
        if (State.Value == global::GameState.Betting)
        {
            PlayerBets.TryAdd(serverRpcParams.Receive.SenderClientId, bet);
            Debug.Log($"Received bet {bet} from {serverRpcParams.Receive.SenderClientId}");
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void PlayCardServerRpc(int card, ServerRpcParams serverRpcParams)
    {
        if (State.Value != GameState.PlayingStack) return;
        
        // Player id to index
        ulong senderId = serverRpcParams.Receive.SenderClientId;


        // Check that the correct player is sending the package
        if (Players.IndexOf(Players.Find(x => x.NetworkId == senderId)) == CurrentPlayerIndex)
        {
            CurrentStack.Add(Card.AllCards[card]);
            PlayerHands[senderId].Remove(Card.AllCards[card]);

            ClientRpcParams clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[]
                    {
                        senderId
                    }
                }
            };

            UpdateClientHandClientRpc(SerializeHand(PlayerHands[senderId]), clientRpcParams);

            CurrentPlayerIndex++;
            if (CurrentPlayerIndex >= NumberOfPlayers) State.Value = GameState.EndOfRound;
        }
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

    void Update()
    {
        // Safety checks
        if (!IsHost) return;
        if (!GameStarted.Value) return;

        switch (State.Value)
        {
            case GameState.StartOfRound:

                StartRound();

                AskForBetClientRpc();
                State.Value = GameState.Betting;

                break;
            case GameState.Betting:
                if (PlayerBetsReady())
                {
                    State.Value = GameState.PlayingStack;
                }
                break;
            case GameState.PlayingStack:

                

                break;
            case GameState.EndOfRound:
                break;
            default:
                break;
        }


        //roundText.text = $"Round: {round}";
        //stackText.text = $"Stack: {stack}";


    }


    private bool PlayerBetsReady()
    {
        return PlayerBets.Count == NumberOfPlayers;
    }
    
    /// <returns>
    /// The number of stacks for this round
    /// </returns>
    private int NumberOfStacks()
    {
        return 3;

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

    public static bool IsCardEligible(Card card, Suit currentSuit, Suit trumpSuit, List<Card> hand)
    {
        // First, check if the player had nextStackCard in their hand
        // Second, check if nextStackCard's suit is of currentSuit, and if not, check if it had any other card of currentSuit on Player hand
        // Third, check if nextStackCard's suit is of trumpSuit, and if not, check if it had any other card of trumpSuit on Player hand

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
                // Third check: If nextStackCard's suit is not the trumpSuit,
                // verify if there are any cards of trumpSuit in the player's hand
                if (card.Suit != trumpSuit && trumpSuit != Suit.Joker && hand.Any(handCard => handCard.Suit == trumpSuit))
                {
                    return false;
                }
            }
        }

        // Card is eligible
        return true;
    }
}
