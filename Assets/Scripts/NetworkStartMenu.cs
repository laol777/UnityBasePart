using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

public class NetworkStartMenu : MonoBehaviour 
{
    SocketsDiscovery discovery;
    [SerializeField]Text status;
    [SerializeField]GameObject mainMenuPanel;
    [SerializeField]GameObject stopButton;

    void Awake()
    {
        discovery = FindObjectOfType<SocketsDiscovery>();

    }

    void SearchGame()
    {
        discovery.StartListening();
        status.text = "Searching for a game to connect to.";
    }

    public void StartHost()
    {
        PlayerTypeDecision.type = PlayerTypeDecision.PlayerType.PLAYER;
        NetworkManager.singleton.StartHost();
    }

    public void StartClient()
    {
        PlayerTypeDecision.type = PlayerTypeDecision.PlayerType.PLAYER;
        SearchGame();
        SwitchPanels();
    }

    public void StartSpectator()
    {
        PlayerTypeDecision.type = PlayerTypeDecision.PlayerType.SPECTATOR;
        SearchGame();
        SwitchPanels();
    }

    void SwitchPanels()
    {
        bool current = mainMenuPanel.activeSelf;
        mainMenuPanel.SetActive(!current);
        stopButton.SetActive(current);
    }

    public void StopSearch()
    {
        discovery.StopDiscovery();
        SwitchPanels();
        status.text = "";
    }
}