using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        return new Card(Suit.Clubs, Rank.Jack, -1);
    }
}
