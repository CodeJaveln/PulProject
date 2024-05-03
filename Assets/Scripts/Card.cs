public struct Card
{
    public readonly Suit suit;
    public readonly Rank rank;
    public readonly int index;

    public Card(Suit suit, Rank rank, int index)
    {
        this.suit = suit;
        this.rank = rank;
        this.index = index;
    }

    public override bool Equals(object obj)
    {
        if (!(obj is Card))
            return false;

        return Equals((Card)obj);
    }

    private bool Equals(Card card)
    {
        return suit == card.suit && rank == card.rank;
    }

    // GetHashCode gets the cards hashcode by getting the hashcode from the card
    public override int GetHashCode()
    {
        return index;
    }
}
