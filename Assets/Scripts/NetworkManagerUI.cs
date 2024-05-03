using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private RelayManager RelayManager;

    [SerializeField] private Button HostButton;
    [SerializeField] private Button ClientButton;
    [SerializeField] private TMP_InputField JoinCodeTextField;
    
    [SerializeField] private Button StartGameButton;

    private void Awake()
    {
        HostButton.onClick.AddListener(() =>
        {
            RelayManager.CreateRelay();
        });

        ClientButton.onClick.AddListener(() =>
        {
            RelayManager.JoinRelay(JoinCodeTextField.text);
        });

        RelayManager.OnRelayCreated += () =>
        {
            JoinCodeTextField.text = RelayManager.JoinCode;
        };


        
    }
}
