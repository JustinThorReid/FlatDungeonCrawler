using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class DungeonNetworkManager : NetworkManager {
    [Scene]
    [SerializeField]
    private string menuScene = string.Empty;

    public static event Action<NetworkConnection> OnClientConnected;
    public static event Action<NetworkConnection> OnClientDisconnected;
    public static event Action<NetworkConnection> OnServerReadied;
    public static event Action<string> OnServerSceneReadied;

    public override void OnStartServer() {
        base.OnStartServer();

        foreach(var prefab in Resources.LoadAll<GameObject>("SpawnablePrefabs")) {
            if(prefab.GetComponent<NetworkIdentity>() != null)
                spawnPrefabs.Add(prefab);
        }
    }

    public override void OnStartClient() {
        base.OnStartClient();

        foreach(var prefab in Resources.LoadAll<GameObject>("SpawnablePrefabs")) {
            if(prefab.GetComponent<NetworkIdentity>() != null)
                ClientScene.RegisterPrefab(prefab);
        }
    }

    public override void OnServerReady(NetworkConnection conn) {
        base.OnServerReady(conn);

        OnServerReadied?.Invoke(conn);
    }

    public override void OnServerSceneChanged(string sceneName) {
        base.OnServerSceneChanged(sceneName);
        OnServerSceneReadied?.Invoke(sceneName);
    }
    
    public override void OnClientConnect(NetworkConnection conn) {
        base.OnClientConnect(conn);

        OnClientConnected?.Invoke(conn);
    }

    public override void OnServerConnect(NetworkConnection conn) {
        base.OnServerConnect(conn);
    }

    public override void OnClientDisconnect(NetworkConnection conn) {
        base.OnClientDisconnect(conn);

        OnClientDisconnected?.Invoke(conn);
    }

    public override void OnServerAddPlayer(NetworkConnection conn) {
        base.OnServerAddPlayer(conn);
    }
}
