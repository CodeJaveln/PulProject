using System.Collections.Generic;
using UnityEngine;

public class CardVisualizer : MonoBehaviour
{
    [SerializeField] Sprite[] PossibleCards;

    private Dictionary<(Suit, Rank), Sprite> CardIdentifier;

    private void Start()
    {
        CardIdentifier = new Dictionary<(Suit, Rank), Sprite>();

        for (int i = 0; i < PossibleCards.Length; i++)
        {
            Suit cardSuit;
            switch (PossibleCards[i].name[0])
            {
                case 'H':
                    cardSuit = Suit.Hearts;
                    break;
                case 'D':
                    cardSuit = Suit.Diamonds;
                    break;
                case 'S':
                    cardSuit = Suit.Spades;
                    break;
                case 'C':
                    cardSuit = Suit.Clubs;
                    break;
                case 'J':
                    cardSuit = Suit.Joker;
                    break;
                default:
                    cardSuit = (Suit)0;
                    break;
            }

            Rank cardRank;
            if (int.TryParse(PossibleCards[i].name[1].ToString(), out int n))
            {
                cardRank = (Rank)n;
            }
            else
            {
                switch (PossibleCards[i].name[1])
                {
                    case 'J':
                        cardRank = Rank.Jack;
                        break;
                    case 'Q':
                        cardRank = Rank.Queen;
                        break;
                    case 'K':
                        cardRank = Rank.King;
                        break;
                    case 'A':
                        cardRank = Rank.Ace;
                        break;
                    default:
                        cardRank = (Rank)0;
                        break;
                }
            }

            CardIdentifier.Add((cardSuit, cardRank), PossibleCards[i]);
        }
    }

    public void VizualieCards(List<Card> cards)
    {
        foreach (Card card in cards)
        {
            // Search by suit
            // Search by rank

            //CardIdentifier[(card.suit, card.rank)];
        }
    }
}
