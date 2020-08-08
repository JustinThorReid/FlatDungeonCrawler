using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(Rigidbody2D))]
public class NetworkRigidbody : NetworkBehaviour
{
    private Rigidbody2D rb;
    //[SyncVar(hook = nameof(HandleVelocity))]
    //private Vector2 velocity;
    //[SyncVar(hook = nameof(HandlePosition))]
    //private Vector2 position;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if(!isServer && !hasAuthority) {
            rb.isKinematic = true;
        }
    }
    
    //[Client]
    //private void HandleVelocity(Vector2 old, Vector2 newVal) {
    //    rb.velocity = newVal;
    //}

    //[Client]
    //private void HandlePosition(Vector2 old, Vector2 newVal) {
    //    rb.position = newVal;
    //}

    //// Update is called once per frame
    //[ServerCallback]
    //void FixedUpdate() {
    //    velocity = rb.velocity;
    //    position = rb.position;
    //}
}
