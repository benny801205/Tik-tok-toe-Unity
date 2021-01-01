using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(UserManager))]
public class GameController : MonoBehaviour
{

    [SerializeField] Sprite o;
    [SerializeField] Sprite x;
    public TMP_Text toast_txt;
    public Image toast_back;
    public Button goback_button;
    int[] tabel = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    bool isEnd = false;
    int round = 0;
    public Button[] grids;
    private UserManager um;

    public void pick(Button btn)
    {
        
        int index = Int32.Parse(btn.name);
        if (isEnd || tabel[index]!=0)
            return;
        
        
        Debug.Log("round : " + round);

        if (round%2 == 0)
        {
            btn.image.sprite = o;
            btn.enabled = false;
            tabel[index] = 1;

        }
        else
        {
            btn.image.sprite = x;
            btn.enabled = false;
            tabel[index] = 2;

        }
        
        //check if win
        var winner=cheackwinner();

        if (winner == 1)
        {  //O win
            enable_goback_button();
            showToast("You Win", 2);
            um.isWin(true);
            return;
        }
        if(winner == 2)
        {  //X win
            enable_goback_button();
            showToast("You're Loser", 2);
            um.isWin(false);
            return;
        }

        //if tie
        if(round >= 8)
        {
            enable_goback_button();
            showToast("It's a tie", 2);
            return;

        }
        round++;

        AImove();
    }

    //need a go back button  image from hide to other image could be a idea


    private void AImove()
    {

        var list = new ArrayList();
        //get the remains list
        for(int i = 0; i < tabel.Length; i++)
        {
            if (tabel[i] == 0)
            {
                list.Add(i);

            }
        }
        System.Random r = new System.Random();

        int index = r.Next(0, list.Count);

        ////////////////////////
        int buton_id = (int)list[index];
        Button btn = grids[buton_id];
        if (isEnd || tabel[buton_id] != 0)
            return;


        Debug.Log("round : " + round);

        if (round % 2 == 0)
        {
            btn.image.sprite = o;
            btn.enabled = false;
            tabel[buton_id] = 1;

        }
        else
        {
            btn.image.sprite = x;
            btn.enabled = false;
            tabel[buton_id] = 2;

        }

        //check if win
        var winner = cheackwinner();

        if (winner == 1)
        {  //O win
            enable_goback_button();
            showToast("You Win", 2);
            um.isWin(true);
            return;
        }
        if (winner == 2)
        {  //X win
            enable_goback_button();
            showToast("You're Loser", 2);
            um.isWin(false);
            return;
        }

        //if tie
        if (round >= 8)
        {
            enable_goback_button();
            showToast("It's a tie", 2);
            return;

        }




        round++;

    }

    private int cheackwinner()
    {

        //0,1,2
        if(tabel[0]!=0 && tabel[0]==tabel[1] && tabel[0] == tabel[2])
        {
            isEnd = true;
            Debug.Log("0,1,2 win");
            return tabel[0];

        }




        //3,4,5
        else if(tabel[3]!=0 && tabel[3]==tabel[4] && tabel[3] == tabel[5])
        {
            isEnd = true;
            Debug.Log("3,4,5 win");
            return tabel[3];



        }


        //6,7,8
        else if (tabel[6] != 0 && tabel[6] == tabel[7] && tabel[6] == tabel[8])
        {

            isEnd = true;
            Debug.Log("6,7,8 win");
            return tabel[6];


        }



        //0,3,6
        else if (tabel[0] != 0 && tabel[0] == tabel[3] && tabel[0] == tabel[6])
        {

            isEnd = true;
            Debug.Log("0,3,6 win");
            return tabel[0];


        }



        //1,4,7
        else if (tabel[1] != 0 && tabel[1] == tabel[4] && tabel[1] == tabel[7])
        {

            isEnd = true;
            Debug.Log("1,4,7 win");
            return tabel[1];


        }



        //2,5,8
        else if (tabel[2] != 0 && tabel[2] == tabel[5] && tabel[2] == tabel[8])
        {
            isEnd = true;
            Debug.Log("2,5,8 win");
            return tabel[2];



        }



        //0,4,8
        else if (tabel[0] != 0 && tabel[0] == tabel[4] && tabel[0] == tabel[8])
        {

            isEnd = true;
            Debug.Log("0,4,7 win");
            return tabel[0];


        }



        //2,4,6
        else if (tabel[2] != 0 && tabel[2] == tabel[4] && tabel[2] == tabel[6])
        {

            isEnd = true;
            Debug.Log("2,4,6 win");
            return tabel[2];


        }

      
        return 0;
    }





    private void enable_goback_button()
    {

        goback_button.gameObject.SetActive(true);

        Debug.Log("enable goback button");

    }






    // Start is called before the first frame update
    void Start()
    {
        um = GetComponent<UserManager>();
        toast_txt.enabled = false;
        toast_back.enabled = false;
        goback_button.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    //toast help method
    void showToast(string text,
   int duration)
    {
        StartCoroutine(showToastCOR(text, duration));
    }


    private IEnumerator showToastCOR(string text,
        int duration)
    {
        Color orginalColor = toast_txt.color;

        toast_txt.text = text;
        toast_txt.enabled = true;
        toast_back.enabled = true;
        //Fade in
        yield return fadeInAndOut(toast_txt, true, 0.5f);

        //Wait for the duration
        float counter = 0;
        while (counter < duration)
        {
            counter += Time.deltaTime;
            yield return null;
        }

        //Fade out
        yield return fadeInAndOut(toast_txt, false, 0.5f);

        toast_txt.enabled = false;
        toast_back.enabled = false;
        toast_txt.color = orginalColor;
    }

    IEnumerator fadeInAndOut(TMP_Text targetText, bool fadeIn, float duration)
    {
        //Set Values depending on if fadeIn or fadeOut
        float a, b;
        if (fadeIn)
        {
            a = 0f;
            b = 1f;
        }
        else
        {
            a = 1f;
            b = 0f;
        }

        Color currentColor = Color.clear;
        float counter = 0f;

        while (counter < duration)
        {
            counter += Time.deltaTime;
            float alpha = Mathf.Lerp(a, b, counter / duration);

            targetText.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
            yield return null;
        }
    }




}
