using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerManager : NetworkBehaviour
{
    public GameObject avatarPrefab;
    GameObject avatar;

    public override void OnStartServer() => DungeonNetworkManager.OnServerReadied += SpawnPlayer;

    [Server]
    public void SpawnPlayer(NetworkConnection conn) {
        avatar = Instantiate(avatarPrefab);
        avatar.transform.position = new Vector2(1.8f, -23);
        
        NetworkServer.Spawn(avatar, conn);
    }
}
