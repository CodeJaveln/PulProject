using Unity.Netcode;

class Player : NetworkBehaviour
{
    // TODO:
    // Implement a way to ask for client's bet
    // Implement a way to ask for client's card to stack

    public int StickBidAmount()
    {
        return 0;
    }

    public Card CardToStack()
    {
        return new Card(Suit.Joker, Rank.Ace, -1);
    }
}
