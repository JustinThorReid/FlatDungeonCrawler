using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerManager : NetworkBehaviour {
    public GameObject avatarPrefab;

    List<(string, NetworkConnection)> playersNeedingRespawn = new List<(string, NetworkConnection)>();

    public override void OnStartServer() {
        //DungeonNetworkManager.OnServerReadied += SpawnPlayer;
        //DungeonNetworkManager.OnServerSceneReadied += SpawnAllPlayers;

        InvokeRepeating(nameof(RespawnQueue), 10, 10);
    }

    [Server]
    private void RespawnQueue() {
        foreach((var name, var conn) in playersNeedingRespawn) {
            SpawnPlayer(name, conn);
        }
        playersNeedingRespawn.Clear();
    }

    [Server]
    public void SpawnPlayer(string playerName, NetworkConnection conn) {
        GameObject avatar = Instantiate(avatarPrefab);
        avatar.transform.position = new Vector2(1.8f, -23);
        avatar.GetComponent<Entity>().displayName = playerName;
        
        NetworkServer.Spawn(avatar, conn);
    }

    [Server]
    public void PlayerDeath(GameObject deadPlayer) {
        Entity entity = deadPlayer.GetComponent<Entity>();
        string playerName = entity.displayName;

        playersNeedingRespawn.Add((playerName, deadPlayer.GetComponent<NetworkIdentity>().connectionToClient));
    }
}
