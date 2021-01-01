using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;




[RequireComponent(typeof(UserManager))]
public class RegisterUI : MonoBehaviour
{
    public TMP_Text email_txt;
    public TMP_Text password_txt;
    public TMP_Text name_txt;
    private UserManager um;


    public void CreatAccount()
    {
        if (name_txt.text == "")
        {
            um.showToast("name can not be empty",3);
            return;
        }




        string filter = name_txt.text.Replace("&", string.Empty);

        um.CreateUserWithEmailAsync(email_txt.text,password_txt.text, filter);



    }

    void Start()
    {
        um= GetComponent<UserManager>();
        name_txt.text = "";

    }


}
