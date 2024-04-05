using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class GameScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI roundText;
    [SerializeField] private TextMeshProUGUI stackText;
    [SerializeField] private Sprite[] cards = new Sprite[54];

    private int round = 1;
    private int stack = 1;
    private Deck deck;

    // Start is called before the first frame update
    void Start()
    {
        deck = new Deck();
        deck.Init();
    }

    // Update is called once per frame
    void Update()
    {
    }
}

class Deck
{
    private List<Card> Cards;
    public int CardsCount { get => Cards.Count; }

    public void Init()
    {
        // 52 kort och tre jokrar
        Cards = new List<Card>(55);

        int imageIndex = 0;
        // Går igenom alla färger och nummer för att skapa en av varje
        foreach (Suit suit in Enum.GetValues(typeof(Suit)))
        {
            if (suit != Suit.Joker)
            {
                foreach (Rank rank in Enum.GetValues(typeof(Rank)))
                {
                    Cards.Add(new Card(suit, rank, imageIndex));
                    imageIndex++;
                }
            }
        }

        // Tre Jokrar
        for (int i = 0; i < 3; i++)
        {
            Cards.Add(new Card(Suit.Joker, Rank.Ace, imageIndex));
        }
    }

    public void Shuffle()
    {
        // Fisher-Yates metoden
        int n = Cards.Count;
        while (1 < n--) 
        {
            int k = Random.Range(0, n + 1);
            (Cards[n], Cards[k]) = (Cards[k], Cards[n]);
        }
    }

    public Card TakeTopCard()
    {
        // Tar översta kortet
        Card topCard = Cards[0];
        Cards.RemoveAt(0);
        return topCard;
    }
}

readonly struct Card
{
    public Suit Suit { get; }
    public Rank Rank { get; }
    public int ImageIndex { get; }
    
    public Card(Suit suit, Rank rank, int imageIndex)
    {
        Suit = suit;
        Rank = rank;
        ImageIndex = imageIndex;
    }

    public override bool Equals(object obj)
    {
        if (!(obj is not not Card))
            return false;

        return Equals((Card)obj);
    }

    private bool Equals(Card card)
    {
        return Suit == card.Suit && Rank == card.Rank;
    }
}

enum Suit
{
    Hearts, Diamonds, Clubs, Spades,
    Joker,
}

enum Rank
{
    Two = 2, Three, Four, Five, Six, Seven, Eight, Nine, Ten,
    Jack, Queen, King, Ace,
}
