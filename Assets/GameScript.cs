using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class GameScript : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI roundText;
    [SerializeField] TextMeshProUGUI stackText;

    private Deck deck;
    private int round;
    private int stack;

    private bool dealtCardsForRound;

    private Dictionary<int, List<Card>> playerHands;
    private int currentPlayerIndex;

    void Start()
    {
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
    // Deal cards: Done
    // Get player input on bets
    // Show other players bets
    // Show stack and hold which player that starts
    // Ask for player input (card to stack)
    // Show who won round, show player points
    // Repeat round until 20 rounds
    // Show who won all and start over if they want

    void Update()
    {
        roundText.text = $"Round: {round}";
        stackText.text = $"Stack: {stack}";

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

class Deck
{
    private List<Card> cards;
    public int cardsAmount
    {
        get => cards.Count;
    }

    public Deck()
    {
        Init();
    }

    public void Init()
    {
        // 52 kort och tre jokrar
        cards = new List<Card>(55);

        int index = 0;

        // Går igenom alla färger och nummer för att skapa en av varje
        foreach (Suit suit in Enum.GetValues(typeof(Suit)))
        {
            if (suit != Suit.Joker)
            {
                foreach (Rank rank in Enum.GetValues(typeof(Rank)))
                {
                    cards.Add(new Card(suit, rank, index));
                    index++;
                }
            }
        }

        // Tre Jokrar
        for (int i = 0; i < 3; i++)
        {
            cards.Add(new Card(Suit.Joker, Rank.Ace, index));
            index++;
        }
    }

    public void Shuffle()
    {
        // Fisher-Yates metoden
        for (int n = cards.Count; 1 < n; n--)
        {
            int k = Random.Range(0, n + 1);
            (cards[n], cards[k]) = (cards[k], cards[n]);
        }
    }

    public Card TopCard()
    {
        // Tar översta kortet
        Card topCard = cards[0];
        cards.Remove(topCard);
        return topCard;
    }
}

public struct Card
{
    public readonly Suit suit;
    public readonly Rank rank;
    public readonly int index;

    public Card(Suit suit, Rank rank, int index)
    {
        this.suit = suit;
        this.rank = rank;
        this.index = index;
    }

    public override bool Equals(object obj)
    {
        if (!(obj is Card))
            return false;

        return Equals((Card)obj);
    }

    private bool Equals(Card card)
    {
        return suit == card.suit && rank == card.rank;
    }

    public override int GetHashCode()
    {
        return index;
    }
}

public enum Suit
{
    Hearts, Diamonds, Clubs, Spades,
    Joker,
}

public enum Rank
{
    Two = 2, Three, Four, Five, Six, Seven, Eight, Nine, Ten,
    Jack, Queen, King, Ace,
}
