using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UserpanelLoader : MonoBehaviour
{ 


    public void LoadUserPanelScene()
    {
       // string name = EventSystem.current.currentSelectedGameObject.name;
        //int current = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene("UserPanel");
      

    }

    public void LoadGameScene()
    {
        // string name = EventSystem.current.currentSelectedGameObject.name;
        //int current = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene("Game_tabel");
       

    }


    public void LoadLoginPanel()
    {
        SceneManager.LoadScene("HomePage");


    }

    public void LoadRegisterPanel()
    {
        SceneManager.LoadScene("RegisterPanel");   
      

    }




}
