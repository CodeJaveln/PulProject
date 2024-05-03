using System.Collections.Generic;
using System;
using Unity.Collections.LowLevel.Unsafe;

public readonly struct Card
{
    public static Card[] AllCards = GenerateCards();

    private static Card[] GenerateCards()
    {
        // 52 cards and three jokers
        Card[] cards = new Card[55];

        
        int i = 0;

        foreach (Suit suit in Enum.GetValues(typeof(Suit)))
        {
            if (suit != Suit.Joker)
            {
                foreach (Rank rank in Enum.GetValues(typeof(Rank)))
                {
                    cards[i] = new Card(suit, rank, i);
                    i++;
                }
            }
        }

        // Jokers
        for (int j = 0; j < 3; j++)
        {
            cards[i] = new Card(Suit.Joker, Rank.Ace, i);
            i++;
        }

        return cards;
    }

    public readonly Suit Suit;
    public readonly Rank Rank;
    public readonly int Index;

    public Card(Suit suit, Rank rank, int index)
    {
        this.Suit = suit;
        this.Rank = rank;
        this.Index = index;
    }

    public override bool Equals(object obj)
    {
        if (!(obj is Card))
            return false;

        return Equals((Card)obj);
    }

    private bool Equals(Card card)
    {
        return Suit == card.Suit && Rank == card.Rank;
    }

    // GetHashCode gets the cards hashcode by getting the hashcode from the card
    public override int GetHashCode()
    {
        return Index;
    }
}
