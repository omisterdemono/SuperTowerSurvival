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

    private void Awake()
    {
        healthComponent = GetComponent<HealthComponent>();
    }

    private void Start()
    {
        healthComponent.OnDeath += End;
    }

    private void End()
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
            End();
            gameIsDone = true;
        }
    }

    private IEnumerator LostGame()
    {
        //show end screen
        yield return new WaitForSeconds(10);
        Debug.Log("end game");
        NetworkServer.Shutdown();
        FindObjectOfType<NetworkManagerLobby>().StopServer();
        NetworkServer.DisconnectAll();
        SceneManager.LoadScene("Lobby");
    }
}
