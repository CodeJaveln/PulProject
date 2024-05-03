using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class CardVisualizer : MonoBehaviour
{
    [SerializeField] private Sprite[] PossibleCards;
    [SerializeField] private GameObject ButtonPrefab;

    private Dictionary<(Suit, Rank), Sprite> CardIdentifier;

    public static CardVisualizer Instance;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        } 
        else
        {
            Destroy(gameObject);
        }

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
    }

    public void VisualizeCards(List<Card> cards)
    {
        if (cards == null || cards.Count == 0) 
            throw new ArgumentNullException("cards is null", nameof(cards));

        var buttonImage = ButtonPrefab.GetComponent<Image>();
        float x = (Screen.width - cards.Count * buttonImage.sprite.texture.width) / (float)(1 + cards.Count);
        float y = buttonImage.sprite.texture.height / (float)2;

        for (int i = 0; i < cards.Count; i++)
        {
            var sprite = CardIdentifier[(cards[i].Suit, cards[i].Rank)];
            
            var gameObject = Instantiate(ButtonPrefab, new Vector3(x * (i + 1) + i * buttonImage.sprite.texture.width, y, 0), Quaternion.identity, transform);
            gameObject.GetComponent<Image>().overrideSprite = sprite;
        }
    }
}
