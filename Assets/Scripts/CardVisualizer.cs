using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UIElements;

public class CardVisualizer : MonoBehaviour
{
    [SerializeField] private Sprite[] PossibleCards;
    [SerializeField] private GameObject ButtonPrefab;

    private Dictionary<(Suit, Rank), Sprite> CardIdentifier;

    private void Start()
    {
        CardIdentifier = new Dictionary<(Suit, Rank), Sprite>();

        for (int i = 0; i < PossibleCards.Length; i++)
        {
            Suit cardSuit = PossibleCards[i].name[0] switch
            {
                'H' => Suit.Hearts,
                'D' => Suit.Diamonds,
                'S' => Suit.Spades,
                'C' => Suit.Clubs,
                'J' => Suit.Joker,
                _ => (Suit)0
            };

            Rank cardRank;
            if (int.TryParse(PossibleCards[i].name[1].ToString(), out int n))
            {
                cardRank = (Rank)n;
            }
            else
            {
                cardRank = PossibleCards[i].name[1] switch
                {
                    'J' => Rank.Jack,
                    'Q' => Rank.Queen,
                    'K' => Rank.King,
                    'A' => Rank.Ace,
                    _ => (Rank)0
                };
            }

            CardIdentifier.Add((cardSuit, cardRank), PossibleCards[i]);
        }

        VisualizeCards(new() { new(Suit.Spades, Rank.Seven, 0) });
    }

    public void VisualizeCards(List<Card> cards)
    {
        if (cards == null || cards.Count == 0) 
            throw new ArgumentNullException("cards is null", nameof(cards));

        foreach (Card card in cards)
        {
            var sprite = CardIdentifier[(card.suit, card.rank)];
            (Suit suit, Rank rank) = (card.suit, card.rank);
            var gameObject = Instantiate(ButtonPrefab, Vector3.zero, Quaternion.identity, transform);
            gameObject.GetComponent<Image>().sprite = sprite;
        }
    }
}
