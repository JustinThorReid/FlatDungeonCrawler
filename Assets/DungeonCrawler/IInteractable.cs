using UnityEngine;
using Mirror;
using System;

public interface IInteractable {
    [Server]
    void ServerInteract(GameObject source);
}