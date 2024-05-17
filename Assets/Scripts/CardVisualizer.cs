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
    private List<Button> CardButtons;

    private void Start()
    {
        Cards = new();
        PlayerCardsObjects = new();
        CardButtons = new List<Button>();
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
    }

    public void VisualizeCards(List<Card> cards)
    {
        if (cards == null || cards.Count == 0) 
            throw new ArgumentNullException("cards is null", nameof(cards));

        foreach (var cardObject in PlayerCardsObjects)
        {
            Destroy(cardObject);
        }

        float x = 150 * (cards.Count - 1) / -2f;


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
            int cardIndex = cards[i].Index;
            gameObject.GetComponent<Button>().onClick.AddListener(() =>
            {
                GameScript.Instance.PlayCardServerRpc(cardIndex, new Unity.Netcode.ServerRpcParams());
            });

            PlayerCardsObjects.Add(gameObject);
        }
    }
}
