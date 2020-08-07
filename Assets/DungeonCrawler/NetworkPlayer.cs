using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class NetworkPlayer : NetworkBehaviour {
    private const string charName = "CharacterName";
    private bool isReady = false;

    public override void OnStopClient() {
        base.OnStopClient();

        isReady = false;
    }

    public override void OnStartClient() {
        if(!hasAuthority)
            return;
        name = name + "_authority";

        CmdReady(PlayerPrefs.GetString(charName), SceneManager.GetActiveScene().name);
    }

    [Command]
    void CmdReady(string playerName, string sceneName) {
        if(SceneManager.GetActiveScene().name != sceneName)
            return;

        var manager = FindObjectOfType<PlayerManager>();

        if(manager != null) {
            manager.SpawnPlayer(playerName, connectionToClient);
        }
    }
}
