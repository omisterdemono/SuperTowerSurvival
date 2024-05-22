using Components;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameScript : NetworkBehaviour
{
    [SerializeField] Canvas gameOverScreen;

    private HealthComponent healthComponent;
    private bool gameIsDone = false;
    private List<Canvas> _canvases;
     

    private void Start()
    {
        healthComponent = GameObject.FindGameObjectWithTag("MainHall").GetComponent<HealthComponent>();
        healthComponent.OnDeath += EndGame;
        _canvases = FindObjectsOfType<Canvas>().ToList();
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
        // NetworkServer.Destroy(FindObjectOfType<PlayerSpawnSystem>().gameObject);
        //show end screen
        foreach (var canvas in _canvases)
        {
            canvas.gameObject.SetActive(false);
        }
        gameOverScreen.gameObject.SetActive(true);
        yield return new WaitForSeconds(5);
        Debug.Log("end game");
        FindObjectOfType<NetworkManagerLobby>().StopHost();
    }
}
