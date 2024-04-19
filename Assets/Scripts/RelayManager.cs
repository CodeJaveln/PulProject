using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using UnityEngine;

public class RelayManager : MonoBehaviour
{
    public string JoinCode = null;
    public event Action OnRelayCreated;

    async void Start()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in as " + AuthenticationService.Instance.PlayerId);
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void CreateRelay()
    {
        try
        {
            Allocation allocation = await Relay.Instance.CreateAllocationAsync(3);

            JoinCode = await Relay.Instance.GetJoinCodeAsync(allocation.AllocationId);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData
                );

            NetworkManager.Singleton.StartHost();

            OnRelayCreated?.Invoke();
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void JoinRelay(string joinCode)
    {
        try
        {
            JoinAllocation joinAllocation = await Relay.Instance.JoinAllocationAsync(joinCode);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(
                joinAllocation.RelayServer.IpV4,
                (ushort)joinAllocation.RelayServer.Port,
                joinAllocation.AllocationIdBytes,
                joinAllocation.Key,
                joinAllocation.ConnectionData,
                joinAllocation.HostConnectionData
                );

            NetworkManager.Singleton.StartClient();
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }
}
