using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(Rigidbody2D))]
public class NetworkRigidbody : NetworkBehaviour
{
    private Rigidbody2D rb;
    [SyncVar(hook = nameof(HandleVelocity))]
    private Vector2 velocity;
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

    private void HandleVelocity(Vector2 oldVal, Vector2 newVal) {
        if(rb.isKinematic)
            rb.velocity = newVal;
    }

    [Command]
    private void CmdSetVelocity(Vector2 velocity) {
        this.velocity = velocity;
        rb.velocity = velocity;
    }

    // Update is called once per frame
    void FixedUpdate() {
        if(!rb.isKinematic && hasAuthority) {
            CmdSetVelocity(rb.velocity);
        }
    }
}
