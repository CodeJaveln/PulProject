using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Networking;

public class TestPlayer : NetworkBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float speed;

    private void Update()
    {
        if (!IsOwner) return;

        Vector2 input;
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");

        rb.velocity = input.normalized * speed;

        { };
    }
}
