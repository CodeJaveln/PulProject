using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public struct PlayerInfo
{
    public ulong NetworkId;
    //public FixedString32Bytes Name;

    public PlayerInfo(ulong networkId/*, FixedString32Bytes name*/)
    {
        NetworkId = networkId;
        //Name = name;
    }
}
