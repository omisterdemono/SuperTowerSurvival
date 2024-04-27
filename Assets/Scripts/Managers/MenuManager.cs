using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject InGameMenu;
    [SerializeField] private GameObject mainMenu;

    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private GameObject soundOptionsMenu;
    [SerializeField] private GameObject controllsOptionsMenu;
    [SerializeField] private GameObject graphicsOptionsMenu;
    
    
    //private GameObject mainMenu;
    // Start is called before the first frame update
    void Start()
    {
        InGameMenu.SetActive(false);
        //mainMenu = InGameMenu.GetComponentInChildren<GameObject>();
        //mainMenu = InGameMenu.GetComponentInChildren<GameObject>();
    }

    private void OpenHideMenu(bool toOpen)
    {
        InGameMenu.SetActive(toOpen);
        mainMenu.SetActive(toOpen);
        optionsMenu.SetActive(!toOpen);
        soundOptionsMenu.SetActive(!toOpen);
        controllsOptionsMenu.SetActive(!toOpen);
        graphicsOptionsMenu.SetActive(!toOpen);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            //InGameMenu.SetActive(!InGameMenu.activeSelf);
            this.OpenHideMenu(!InGameMenu.activeSelf);
        }
    }
}
