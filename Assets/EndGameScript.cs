using Components;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameScript : NetworkBehaviour
{
    private HealthComponent healthComponent;
    private bool gameIsDone = false;

    private void Start()
    {
        healthComponent = GameObject.FindGameObjectWithTag("MainHall").GetComponent<HealthComponent>();
        healthComponent.OnDeath += EndGame;
    }

    public void EndGame()
    {
        StartCoroutine(LostGame());
    }

    private void Update()
    {
        if (!gameIsDone)
        {
            List<Character> characters = FindObjectsOfType<Character>().ToList();

            foreach (Character character in characters)
            {
                if (character.IsAlive)
                {
                    return;
                }
            }
            EndGame();
            gameIsDone = true;
        }
    }

    private IEnumerator LostGame()
    {
        NetworkServer.Destroy(FindObjectOfType<PlayerSpawnSystem>().gameObject);
        //show end screen
        yield return new WaitForSeconds(2);
        Debug.Log("end game");
        // NetworkServer.Shutdown();
        // NetworkServer.DisconnectAll();
        // SceneManager.LoadScene("Lobby");
        FindObjectOfType<NetworkManagerLobby>().StopHost();
    }
}
