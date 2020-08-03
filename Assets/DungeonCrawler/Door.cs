using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Door : NetworkBehaviour, IInteractable
{
    [SyncVar]
    bool isOpen = false;

    [Server]
    public void ServerInteract(GameObject source) {
        RpcInteract(gameObject);
    }
    
    private void Interact(GameObject source) {
        isOpen = !isOpen;
        GetComponent<Collider2D>().isTrigger = isOpen;
        GetComponent<SpriteRenderer>().enabled = !isOpen;
    }

    [ClientRpc]
    private void RpcInteract(GameObject source) {
        Interact(gameObject);
    }

}
