using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mirror;

public class MainMenu : MonoBehaviour {
    [SerializeField]
    private DungeonNetworkManager networkManager;
    [SerializeField]
    private TMP_InputField nameInput;
    [SerializeField]
    private Button offline;
    [SerializeField]
    private Button online;
    [SerializeField]
    private GameObject onlineMenu;
    [SerializeField]
    private GameObject mainMenu;
    private const string charName = "CharacterName";

    // Start is called before the first frame update
    void Start()
    {
        string name = PlayerPrefs.GetString(charName);

        if(name != null) {
            nameInput.text = name;
        }

        nameInput.onValueChanged.AddListener(text => {
            offline.interactable = !string.IsNullOrEmpty(text);
            online.interactable = !string.IsNullOrEmpty(text);
        });
    }

    public void StartOffline() {
        if(string.IsNullOrEmpty(nameInput.text))
            return;

        PlayerPrefs.SetString(charName, nameInput.text);
        networkManager.StartHost();
    }

    public void StartOnline() {
        if(string.IsNullOrEmpty(nameInput.text))
            return;

        PlayerPrefs.SetString(charName, nameInput.text);
        mainMenu.SetActive(false);
        onlineMenu.SetActive(true);
    }

    public void StartHost() {
        networkManager.StartHost();
        onlineMenu.SetActive(false);

        //networkManager.ServerChangeScene(firstScene);
    }

    public void StartClient() {
        networkManager.StartClient();
    }
}
