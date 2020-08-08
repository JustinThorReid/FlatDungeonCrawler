using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Door : NetworkBehaviour, IInteractable
{
    private Animator animator;
    private void Start() {
        animator = GetComponent<Animator>();
    }

    [SyncVar(hook = nameof(HandleDoorOpen))]
    bool isOpen = false;

    [Server]
    public void ServerInteract(GameObject source) {
        isOpen = !isOpen;
        HandleDoorOpen(!isOpen, isOpen);
    }
    
    private void HandleDoorOpen(bool oldVal, bool newVal) {
        animator.SetInteger("DoorOpen", newVal ? 1 : 0);
        GetComponent<Collider2D>().isTrigger = isOpen;
    }
}
