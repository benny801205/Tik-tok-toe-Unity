using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;




[RequireComponent(typeof(UserManager))]
public class UserPanelUI : MonoBehaviour
{

    private UserManager um;
    public TMP_Text record;
    public TMP_Text displayname;

    public void LogOut()
    {

        um.SignOut();



    }

    void Start()
    {
        um = GetComponent<UserManager>();

        record.text = "W:" + um.get_win() + "  L:" + um.get_loss();
        displayname.text = um.get_user_name();


    }


}
