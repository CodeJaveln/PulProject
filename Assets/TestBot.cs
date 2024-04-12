using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class TestBot : Player
{
    public TestBot() 
        : base("TestBot")
    {
    }

    public override int StickBidAmount()
    {
        return 0;
    }
}
