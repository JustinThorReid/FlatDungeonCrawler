using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEditor.Callbacks;

public class AutoEnable : NetworkManager
{
    public override void Start() {
        base.Start();

        playerPrefab = new GameObject();
        playerPrefab.AddComponent<NetworkIdentity>();

        StartHost();
    }
}
