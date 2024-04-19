using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;

class Player : NetworkBehaviour
{
    // Need to implement to ask how much they bet they will win
    public int StickBidAmount()
    {
        return 0;
    }

    // Implement to ask the client for what card should go to stack
    public Card CardToStack()
    {
        return new Card(Suit.Clubs, Rank.Jack, -1);
    }
}
