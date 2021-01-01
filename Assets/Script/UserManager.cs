using Firebase.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

[RequireComponent(typeof(UserpanelLoader))]
public class UserManager : MonoBehaviour
{
    private UserpanelLoader panellaoder;
    static protected Firebase.Auth.FirebaseAuth auth;
    protected Firebase.Auth.FirebaseAuth otherAuth;
    protected Dictionary<string, Firebase.Auth.FirebaseUser> userByAuth =
      new Dictionary<string, Firebase.Auth.FirebaseUser>();
    private bool fetchingToken = false;
    public TMP_Text toast_txt;
    public Image toast_back;


    protected string displayName = "";
    bool UIEnabled = true;
    Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;

    static string user_name="default";
    static int win=0;
    static int loss=0;
    



    public virtual void Start()
    {
        toast_txt.enabled = false;
        toast_back.enabled = false;

        panellaoder = GetComponent<UserpanelLoader>();

        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                InitializeFirebase();
            }
            else
            {
                Debug.LogError(
                  "Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
    }

    // Handle initialization of the necessary firebase modules:
    protected void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth");
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        auth.IdTokenChanged += IdTokenChanged;
        // Specify valid options to construct a secondary authentication object.
        if (otherAuthOptions != null &&
            !(String.IsNullOrEmpty(otherAuthOptions.ApiKey) ||
              String.IsNullOrEmpty(otherAuthOptions.AppId) ||
              String.IsNullOrEmpty(otherAuthOptions.ProjectId)))
        {
            try
            {
                otherAuth = Firebase.Auth.FirebaseAuth.GetAuth(Firebase.FirebaseApp.Create(
                  otherAuthOptions, "Secondary"));
                otherAuth.StateChanged += AuthStateChanged;
                otherAuth.IdTokenChanged += IdTokenChanged;
            }
            catch (Exception)
            {
                Debug.Log("ERROR: Failed to initialize secondary authentication object.");
            }
        }
        AuthStateChanged(this, null);
    }

    // Track state changes of the auth object.
    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        Firebase.Auth.FirebaseAuth senderAuth = sender as Firebase.Auth.FirebaseAuth;
        Firebase.Auth.FirebaseUser user = null;
        if (senderAuth != null) userByAuth.TryGetValue(senderAuth.App.Name, out user);
        if (senderAuth == auth && senderAuth.CurrentUser != user)
        {
            bool signedIn = user != senderAuth.CurrentUser && senderAuth.CurrentUser != null;
            if (!signedIn && user != null)
            {
                Debug.Log("Signed out " + user.UserId);
            }
            user = senderAuth.CurrentUser;
            userByAuth[senderAuth.App.Name] = user;
            if (signedIn)
            {
                Debug.Log("AuthStateChanged Signed in " + user.UserId);
                displayName = user.DisplayName ?? "";
               // DisplayDetailedUserInfo(user, 1);
            }
        }
    }

    // Track ID token changes.
    void IdTokenChanged(object sender, System.EventArgs eventArgs)
    {
        Firebase.Auth.FirebaseAuth senderAuth = sender as Firebase.Auth.FirebaseAuth;
        if (senderAuth == auth && senderAuth.CurrentUser != null && !fetchingToken)
        {
            senderAuth.CurrentUser.TokenAsync(false).ContinueWithOnMainThread(
              task => Debug.Log(String.Format("Token[0:8] = {0}", task.Result.Substring(0, 8))));
        }
    }


    void OnDestroy()
    {
        if (auth != null)
        {
            auth.StateChanged -= AuthStateChanged;
            auth.IdTokenChanged -= IdTokenChanged;
            auth = null;
        }
        if (otherAuth != null)
        {
            otherAuth.StateChanged -= AuthStateChanged;
            otherAuth.IdTokenChanged -= IdTokenChanged;
            otherAuth = null;
        }
    }
    void DisableUI()
    {
        UIEnabled = false;
    }

    void EnableUI()
    {
        UIEnabled = true;
    }
    // Options used to setup secondary authentication object.
    private Firebase.AppOptions otherAuthOptions = new Firebase.AppOptions
    {
        ApiKey = "",
        AppId = "",
        ProjectId = ""
    };


  


    // Create a user with the email and password.
    public Task CreateUserWithEmailAsync(string email, string password,string displayname)
    {
   

        Debug.Log(String.Format("Attempting to create user {0}...", email));
        DisableUI();

        // This passes the current displayName through to HandleCreateUserAsync
        // so that it can be passed to UpdateUserProfile().  displayName will be
        // reset by AuthStateChanged() when the new user is created and signed in.
        string newDisplayName = displayName;
        return auth.CreateUserWithEmailAndPasswordAsync(email, password)
          .ContinueWithOnMainThread((task) => {
              EnableUI();


              //
              if (task.IsCanceled)
              {
                  Debug.Log("register canceled.");
              }
              else if (task.IsFaulted)
              {
                  Debug.Log("register is failed");
                  foreach (Exception exception in task.Exception.Flatten().InnerExceptions)
                  {
                      string authErrorCode = "";
                      Firebase.FirebaseException firebaseEx = exception as Firebase.FirebaseException;
                      if (firebaseEx != null)
                      {
                          authErrorCode = String.Format("AuthError.{0}: ",
                            ((Firebase.Auth.AuthError)firebaseEx.ErrorCode).ToString());
                      }
                      Debug.Log("Here Print" + exception.ToString());
                      showToast(exception.ToString(), 3);
                  }
              }
              else if (task.IsCompleted)
              {
                  Debug.Log("succeesful sign up");

                  user_name = displayname;
                  win = 0;
                  loss = 0;

                  UpdateUserProfileAsync(displayname+"&0&0");
                  //load user panel
                  panellaoder.LoadUserPanelScene();
              }



              return task;
          }).Unwrap();
    }
    // Sign-in with an email and password.
    public Task SigninWithEmailAsync(string email,string password)
    {
        Debug.Log(String.Format("Attempting to sign in as {0}...", email));
        DisableUI();
              return auth.SignInWithEmailAndPasswordAsync(email, password)
              .ContinueWithOnMainThread(HandleSignInWithUser);
        
    }

    // Called when a sign-in without fetching profile data completes.
    void HandleSignInWithUser(Task<Firebase.Auth.FirebaseUser> task)
    {

        if (task.IsCanceled)
        {
            Debug.Log("sign in canceled.");
        }
        else if (task.IsFaulted)
        {
            Debug.Log("SignIn is failed");
            foreach (Exception exception in task.Exception.Flatten().InnerExceptions)
            {
                string authErrorCode = "";
                Firebase.FirebaseException firebaseEx = exception as Firebase.FirebaseException;
                if (firebaseEx != null)
                {
                    authErrorCode = String.Format("AuthError.{0}: ",
                      ((Firebase.Auth.AuthError)firebaseEx.ErrorCode).ToString());
                }
                Debug.Log("Here Print" + exception.ToString());
                showToast(exception.ToString(), 3);
            }
        }
        else if (task.IsCompleted)
        {
            Debug.Log("succeesful sign in");
            
            handel_(auth.CurrentUser.DisplayName);
            //load user panel
            panellaoder.LoadUserPanelScene();
        }

        /*
        EnableUI();
        if (LogTaskCompletion(task, "Sign-in"))
        {
            Debug.Log(String.Format("{0} signed in", task.Result.DisplayName));
        }

        */
    }




    private void handel_(string str)
    {
        var arr=str.Split('&');

        user_name = arr[0];
        win = Int32.Parse(arr[1]);
        loss = Int32.Parse(arr[2]);


    }





    // Log the result of the specified task, returning true if the task
    // completed successfully, false otherwise.
    protected bool LogTaskCompletion(Task task, string operation)
    {
        bool complete = false;
        if (task.IsCanceled)
        {
            Debug.Log(operation + " canceled.");
        }
        else if (task.IsFaulted)
        {
            Debug.Log(operation + " encounted an error.");
            foreach (Exception exception in task.Exception.Flatten().InnerExceptions)
            {
                string authErrorCode = "";
                Firebase.FirebaseException firebaseEx = exception as Firebase.FirebaseException;
                if (firebaseEx != null)
                {
                    authErrorCode = String.Format("AuthError.{0}: ",
                      ((Firebase.Auth.AuthError)firebaseEx.ErrorCode).ToString());
                }
                Debug.Log("Here Print"+ authErrorCode + exception.ToString());
            }
        }
        else if (task.IsCompleted)
        {
            Debug.Log(operation + " completed");
            complete = true;
        }
        return complete;
    }

    public Task UpdateUserProfileAsync(string newDisplayName = null)
    {
        if (auth.CurrentUser == null)
        {
            Debug.Log("Not signed in, unable to update user profile");
            return Task.FromResult(0);
        }
        displayName = newDisplayName ?? displayName;
        Debug.Log("Updating user profile");
        DisableUI();
        return auth.CurrentUser.UpdateUserProfileAsync(new Firebase.Auth.UserProfile
        {

            DisplayName = displayName,
            PhotoUrl = auth.CurrentUser.PhotoUrl,
        }).ContinueWithOnMainThread(task => {
            EnableUI();
            if (LogTaskCompletion(task, "User profile"))
            {
                //DisplayDetailedUserInfo(auth.CurrentUser, 1);
            }
        });
    }

    // Sign out the current user.
    public void SignOut()
    {
        Debug.Log("Signing out.");
        auth.SignOut();
        panellaoder.LoadLoginPanel();
    }



    //toast help method
    public void showToast(string text,int duration)
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


    public string get_user_name()
    {

        return user_name;
    }

    public int get_win()
    {

        return win;
    }

    public int get_loss()
    {

        return loss;
    }


    public void isWin(bool g)
    {

        if (g)
        {
            win++;
        }
        else
        {
            loss++;
        }


        UpdateUserProfileAsync(user_name+"&"+win+"&"+loss);


    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
