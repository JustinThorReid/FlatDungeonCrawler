using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public TMP_InputField nameInput;
    private const string charName = "CharacterName";

    // Start is called before the first frame update
    void Start()
    {
        string name = PlayerPrefs.GetString(charName);

        if(name != null) {
            nameInput.text = name;
        }
    }

    public void StartOffline() {
        if(string.IsNullOrEmpty(nameInput.text))
            return;

        PlayerPrefs.SetString(charName, nameInput.text);
    }

    public void StartOnline() {
        if(string.IsNullOrEmpty(nameInput.text))
            return;

        PlayerPrefs.SetString(charName, nameInput.text);
    }
}
