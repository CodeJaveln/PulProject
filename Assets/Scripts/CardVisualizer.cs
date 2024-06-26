using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CardVisualizer : MonoBehaviour
{
    [SerializeField] private Sprite[] PossibleCards;
    [SerializeField] private GameObject ButtonPrefab;
    [SerializeField] private GameObject CardStackPrefab;

    // Dictionary of sprites based on the tuple of suit and rank
    private Dictionary<(Suit, Rank), Sprite> CardIdentifier;

    // instance of instance
    public static CardVisualizer Instance;
    
    // Cards that are visualized
    private List<GameObject> PlayerCardsObjects;
    private Card[] PlayerCards;

    // Hmm
    private List<GameObject> StackCardsObjects;

    // First card in stack
    private Card FirstStackCard = new Card(Suit.Joker, Rank.Ace, -1);

    private void Start()
    {
        // Ensure only one instance
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        PlayerCardsObjects = new();
        
        // Prepares card indentifier
        CardIdentifier = new Dictionary<(Suit, Rank), Sprite>();
        for (int i = 0; i < PossibleCards.Length; i++)
        {
            // Checks suit of card
            Suit cardSuit = PossibleCards[i].name[0] switch
            {
                'H' => Suit.Hearts,
                'D' => Suit.Diamonds,
                'S' => Suit.Spades,
                'C' => Suit.Clubs,
                'J' => Suit.Joker,
                _ => (Suit)0
            };

            // Checks rank
            Rank cardRank;
            // If it isn't a kl�dd kort then try parse as it has a number on latest
            if (int.TryParse(PossibleCards[i].name[1..], out int n))
            {
                cardRank = (Rank)n;
            }
            // If it failed to convert to number, then check last character
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
        switch (GameScript.Instance.State.Value)
        {
            case GameState.PlayingStack:
                for (int i = 0; i < PlayerCardsObjects.Count; i++)
                {
                    if (GameScript.IsCardEligible(PlayerCards[i], FirstStackCard.Suit, GameScript.Instance.TrumpCard.Suit, PlayerCards.ToList()))
                        PlayerCardsObjects[i].GetComponent<Button>().interactable = true;
                    else
                        PlayerCardsObjects[i].GetComponent<Button>().interactable = false;
                }
                break;
            default:
                foreach (var card in PlayerCardsObjects)
                {
                    card.GetComponent<Button>().interactable = false;
                }
                break;
        }
    }

    public void VisualizeCardsAsButtons(List<Card> cards)
    {
        foreach (var cardObject in PlayerCardsObjects)
        {
            Destroy(cardObject);
        }
        PlayerCardsObjects = new List<GameObject>();
        if (cards == null || cards.Count == 0) return;
        float x = 150 * (cards.Count - 1) / -2f;


        for (int i = 0; i < cards.Count; i++)
        {
            var sprite = CardIdentifier[(cards[i].Suit, cards[i].Rank)];
            
            var gameObject = Instantiate(ButtonPrefab, Vector3.zero, Quaternion.identity, transform);
            gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(x + i * 150f, -250F);
            var image = gameObject.GetComponent<Image>();
            image.overrideSprite = sprite;
            int cardIndex = cards[i].Index;
            gameObject.GetComponent<Button>().onClick.AddListener(() =>
            {
                GameScript.Instance.PlayCardServerRpc(cardIndex, new Unity.Netcode.ServerRpcParams());
            });

            PlayerCardsObjects.Add(gameObject);
        }

        PlayerCards = cards.ToArray();
    }

    public void VisualizeStack(List<Card> stack)
    {
        if (stack == null || stack.Count == 0)
            return;

        FirstStackCard = stack[0];
        foreach (var card in StackCardsObjects)
        {
            Destroy(card);
        }
        StackCardsObjects = new();
        float x = 150 * (stack.Count - 1) / -2f;

        for (int i = 0; i < stack.Count; i++)
        {
            var sprite = CardIdentifier[(stack[i].Suit, stack[i].Rank)];

            var gameObject = Instantiate(CardStackPrefab, Vector3.zero, Quaternion.identity, transform);
            gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(x * i * 150f, 0F + 0);
            var image = gameObject.GetComponent<Image>();
            image.overrideSprite = sprite;

            StackCardsObjects.Add(gameObject);
        }
    }
}
