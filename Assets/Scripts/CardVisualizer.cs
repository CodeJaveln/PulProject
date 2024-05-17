using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CardVisualizer : MonoBehaviour
{
    [SerializeField] private Sprite[] PossibleCards;
    [SerializeField] private GameObject ButtonPrefab;

    // Dictionary of sprites based on the tuple of suit and rank
    private Dictionary<(Suit, Rank), Sprite> CardIdentifier;

    // instance of instance
    public static CardVisualizer Instance;

    private List<Card> Cards;
    private List<GameObject> PlayerCardsObjects;

    private void Start()
    {
        Cards = new();
        PlayerCardsObjects = new();
        
        // instance is instance not destroyed
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
            if (int.TryParse(PossibleCards[i].name.Substring(1), out int n))
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

    private void Update()
    {
        for (int i = 0; i < Cards.Count; i++)
        {
            if (GameScript.IsCardEligible(Cards[i], Suit.Joker, Suit.Joker, Cards))
            {
                PlayerCardsObjects[i].GetComponent<Button>().interactable = true;
            }
            else
            {
                PlayerCardsObjects[i].GetComponent<Button>().interactable = false;
            }
        }
    }

    public void VisualizeCards(List<Card> cards)
    {
        if (cards == null || cards.Count == 0) 
            throw new ArgumentNullException("cards is null", nameof(cards));
        
        Image buttonImage = ButtonPrefab.GetComponent<Image>();

        float x = 150 * (cards.Count - 1) / -2f;

        //float x = (1600 - cards.Count * buttonImage.sprite.texture.width) / (float)(1 + cards.Count);
        //float y = buttonImage.sprite.texture.height / 2f;

        for (int i = 0; i < cards.Count; i++)
        {
            var sprite = CardIdentifier[(cards[i].Suit, cards[i].Rank)];
            
            var gameObject = Instantiate(ButtonPrefab, Vector3.zero, Quaternion.identity, transform);
            gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(x + i * 150f, 0f);
            var image = gameObject.GetComponent<Image>();
            image.overrideSprite = sprite;
            if (!GameScript.IsCardEligible(cards[i], Suit.Joker, Suit.Joker, cards))
            {
                //image.color = Color.gray;
                gameObject.GetComponent<Button>().interactable = false;
            }
            PlayerCardsObjects.Add(gameObject);
        }
    }
}
