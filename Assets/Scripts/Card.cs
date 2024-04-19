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

    public override int GetHashCode()
    {
        return index;
    }
}
