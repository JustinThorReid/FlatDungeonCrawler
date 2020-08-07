using UnityEngine;
using Mirror;

public class AutoEnable : NetworkManager
{
    public override void Start() {
        base.Start();

        playerPrefab = new GameObject();
        playerPrefab.AddComponent<NetworkIdentity>();

        StartHost();
    }
}
