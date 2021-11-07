using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FirebaseWebGL.Examples.Utils;
using FirebaseWebGL.Scripts.FirebaseBridge;
using FirebaseWebGL.Scripts.Objects;
using Newtonsoft.Json;
using System.Linq;
using System.Text;
public class UserData
{
    public string UserName { get; set; }
    public string WalletAddress { get; set; }
    public double TimeSeconds { get; set; }
    public int amountOfFreeTries { get; set; }
    public int FreeTryGAMER { get; set; }
    public int FreeTrysGAMER { get; set; }
}


public class FirebaseManager : MonoBehaviour
{
    private int key=129;

    private string UID = "";
    public UserData PlayerData;
    public UserData[] PlayerDataArray;
    //private DependencyStatus dependencyStatus;
    //private FirebaseFirestore DatabaseInstance;
    public static FirebaseManager Instance;

    [HideInInspector]
    public bool WalletConnected = false;

    string DocPath = "users";
    [HideInInspector]
    public bool DocFetched = false;
    [HideInInspector]
    public bool ResultFetched = false;
    void Start()
    {
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        //AuthenticateFirebase();
        OnAuthChanged();
    }

    public void AuthenticateFirebase()
    {
        FirebaseAuth.AuthenticateAnonymous(gameObject.name, "OnAuthSuccess", "OnAuthError");
    }

    public void OnAuthSuccess(string info)
    {
        Debug.Log(info);        
    }


    public void OnAuthError(string error)
    {
        Debug.LogError(error);
    }

    public void OnAuthChanged()
    {
        FirebaseAuth.OnAuthStateChanged(gameObject.name, "OnAuthChangedSuccess", "OnAuthChangedError");
    }

    public void OnAuthChangedSuccess(string user)
    {
        var parsedUser = StringSerializationAPI.Deserialize(typeof(FirebaseUser), user) as FirebaseUser;
        UID = parsedUser.uid;
        //DisplayData($"Email: {parsedUser.email}, UserId: {parsedUser.uid}, EmailVerified: {parsedUser.isEmailVerified}");
    }

    public void OnAuthChangedError(string info)
    {
        UID = "";
        Debug.LogError(info);
    }

    public void AddFireStoreData(UserData _data)
    {
        string _json = JsonConvert.SerializeObject(_data);
        FirebaseFirestore.SetDocument(DocPath, _data.WalletAddress, _json, gameObject.name, "OnAddData", "OnAddDataError");
    }
    public void OnAddData(string info)
    {
        Debug.Log("Data successfully added");
        //Debug.Log(info);
    }

    public void OnAddDataError(string error)
    {
        Debug.LogError(error);
    }

    public void GetFireStoreData(string _collectionID,string _docID)
    {
        DocFetched = false;
        ResultFetched = false;
        FirebaseFirestore.GetDocument(_collectionID, _docID, gameObject.name, "OnDocGet", "OnDocGetError");
    }

    public void OnDocGet(string info)
    {
        Debug.Log("doc was fetched successfully");

        if (info == null || info=="null")
        {
            DocFetched = false;
            ResultFetched = true;
            Debug.LogError("info is null for OnDocGet");
        }
        else
        {
            PlayerData = JsonConvert.DeserializeObject<UserData>(info);
            DocFetched = true;
            ResultFetched = true;
           // Debug.Log(info);
            //Debug.Log("info is not null");
        }
    }

    public void OnDocGetError(string error)
    {
        DocFetched = false;
        ResultFetched = true;
        Debug.LogError(error);
    }

     public IEnumerator CheckCreateUserDB(string _walletID,string _username)
    {
        GetFireStoreData(DocPath, _walletID);
        yield return new WaitUntil(() => ResultFetched == true);
        
        if(DocFetched==true) //document existed
        {
            Debug.Log("user already exists!");
            Debug.Log(_walletID);
            Debug.Log(PlayerData.WalletAddress);
            Debug.Log(PlayerData.UserName);
            Debug.Log(PlayerData.amountOfFreeTries);
            Debug.Log(PlayerData.FreeTryGAMER);
            Debug.Log(PlayerData.TimeSeconds);
            Debug.Log(PlayerData.FreeTrysGAMER);

            if (MainMenuViewController.Instance)
                MainMenuViewController.Instance.TriesText.text = PlayerData.amountOfFreeTries.ToString();
        }
        else
        {
            Debug.Log("user does not exists, creating new entry in database!");
            PlayerData = new UserData();
            PlayerData.WalletAddress = _walletID;
            PlayerData.UserName = _username;
            PlayerData.amountOfFreeTries = 0;
            PlayerData.TimeSeconds = 0;
            PlayerData.FreeTryGAMER = 0;
            PlayerData.FreeTrysGAMER = 0;

            if (MainMenuViewController.Instance)
                MainMenuViewController.Instance.TriesText.text = "0";

            AddFireStoreData(PlayerData);
        }

        if(Constants.PushingTime)
        {
            Constants.PushingTime = false;
            GamePlayUIHandler.Instance.SubmitTime();
        }
    }

    public void UpdatedFireStoreData(UserData _data)
    {
        string _json = JsonConvert.SerializeObject(_data);
        FirebaseFirestore.UpdateDocument(DocPath, _data.WalletAddress, _json, gameObject.name, "OnDocUpdate", "OnDocUpdateError");
    }

    public void OnDocUpdate(string info)
    {
        RaceManager.Instance.RaceEnded();
        Debug.Log("Doc Updated");
        Debug.Log(info);
    }

    public void OnDocUpdateError(string error)
    {
        Debug.LogError(error);
    }

    public void QueryDB(string _field,string _type)
    {
        FirebaseFirestore.QueryDB(DocPath,_field, _type, gameObject.name, "OnQueryUpdate", "OnQueryUpdateError");
    }

    public void OnQueryUpdate(string info)
    {
        Debug.Log("leaderboard query completed");
        //Debug.Log(info);
        PlayerDataArray = JsonConvert.DeserializeObject<UserData[]>(info);
        //System.Array.Reverse(PlayerDataArray);
        LeaderboardManager.Instance.PopulateLeaderboardData(PlayerDataArray);
    }

    public void OnQueryUpdateError(string error)
    {
        Debug.LogError(error);
    }

    public string EncryptDecrypt(string textToEncrypt)
    {
        StringBuilder inSb = new StringBuilder(textToEncrypt);
        StringBuilder outSb = new StringBuilder(textToEncrypt.Length);
        char c;
        for (int i = 0; i < textToEncrypt.Length; i++)
        {
            c = inSb[i];
            c = (char)(c ^ key);
            outSb.Append(c);
        }
        return outSb.ToString();
    }
}
