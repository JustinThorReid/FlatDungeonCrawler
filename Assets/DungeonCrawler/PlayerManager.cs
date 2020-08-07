using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerManager : NetworkBehaviour {
    public GameObject avatarPrefab;

    public override void OnStartServer() {
        //DungeonNetworkManager.OnServerReadied += SpawnPlayer;
        //DungeonNetworkManager.OnServerSceneReadied += SpawnAllPlayers;
    }

    [Server]
    public void SpawnPlayer(string playerName, NetworkConnection conn) {
        GameObject avatar = Instantiate(avatarPrefab);
        avatar.transform.position = new Vector2(1.8f, -23);
        avatar.GetComponent<Entity>().displayName = playerName;
        
        NetworkServer.Spawn(avatar, conn);
        //gameObject.GetComponent<NetworkIdentity>().AssignClientAuthority(conn);
    }
}
