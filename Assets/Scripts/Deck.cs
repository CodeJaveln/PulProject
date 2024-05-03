using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

class Deck
{
    private Stack<Card> Cards;
    //private List<Card> Cards;

    public int CardsAmount
    {
        get => Cards.Count;
    }

    public Deck()
    {
        Reset();
    }

    public void Reset()
    {
        List<Card> allCards = new List<Card>(Card.AllCards);
        Cards = new Stack<Card>(55);

        while (allCards.Count > 0)
        {
            int i = Random.Range(0, allCards.Count);
            Cards.Push(allCards[i]);
            allCards.RemoveAt(i);
        }
    }

    public void Shuffle()
    {
        //// Fisher-Yates metoden
        //for (int n = Cards.Count; 1 < n; n--)
        //{
        //    int k = Random.Range(0, n + 1);
        //    (Cards[n], Cards[k]) = (Cards[k], Cards[n]);
        //}
    }

    public Card TopCard()
    {
        return Cards.Pop();
    }
}
