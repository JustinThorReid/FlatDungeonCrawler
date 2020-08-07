using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class NetworkPlayer : NetworkBehaviour {
    private bool isReady = false;

    public override void OnStopClient() {
        base.OnStopClient();

        isReady = false;
    }

    //public override void OnStartClient() {      
    //    CmdReady(SceneManager.GetActiveScene().name);
    //}

    public override void OnStartAuthority() {
        CmdReady(SceneManager.GetActiveScene().name);
    }

    [Command]
    void CmdReady(string sceneName) {
        if(SceneManager.GetActiveScene().name != sceneName)
            return;

        var manager = FindObjectOfType<PlayerManager>();

        if(manager != null) {
            manager.SpawnPlayer(connectionToClient);
        }
    }
}
