using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class DungeonNetworkManager : NetworkManager {

    public static event Action<NetworkConnection> OnServerReadied;

    public override void OnServerReady(NetworkConnection conn) {
        base.OnServerReady(conn);

        OnServerReadied?.Invoke(conn);
    }
}
