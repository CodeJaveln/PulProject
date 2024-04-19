using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

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
