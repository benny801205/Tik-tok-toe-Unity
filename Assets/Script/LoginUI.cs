using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;




[RequireComponent(typeof(UserManager))]
public class LoginUI : MonoBehaviour
{
    public TMP_Text email_txt;
    public TMP_Text password_txt;
    private UserManager um;


    public void SignIn()
    {

        um.SigninWithEmailAsync(email_txt.text, password_txt.text);



    }

    void Start()
    {
        um = GetComponent<UserManager>();


    }


}
