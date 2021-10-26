using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using TMPro;
using System.Numerics;
using System;

[Serializable]
public class UIData
{
    [Tooltip("UI Reference for Connect Button as gameObject")]
    public GameObject ConnectBtn;
    [Tooltip("UI Reference for Connected Button as gameObject")]
    public GameObject ConnectedBtn;
    [Tooltip("UI Reference for Coin text as texMeshGUI")]
    public TextMeshProUGUI CoinText;
}

public class WalletManager : MonoBehaviour
{
    [Tooltip("Instance of the serialized class")]
    public UIData MainUI;

#if UNITY_WEBGL
    [DllImport("__Internal")]
    private static extern void SetStorage(string key,string val);

    [DllImport("__Internal")]
    private static extern string GetStorage(string key,string ObjectName,string callback);

    [DllImport("__Internal")]
    private static extern void Web3Connect();

    [DllImport("__Internal")]
    private static extern string ConnectAccount();

    [DllImport("__Internal")]
    private static extern void SetConnectAccount(string value);
#endif

    private string account; //string to store wallet address
    BigInteger MainbalanceOf;//Biginteger to store balance of token inside wallet
    BigInteger decimals;//Biginteger to store decimal settings for the token in wallet
    string symbol;//string to store symbol of token in wallet
    BigInteger totalSupply;//Biginteger to store totalSupply of tokens
    string nameContract;//string to store name of the contract (BEP20)
    BigInteger DecimalValue; //stores Decimal calculation with power of 10
    BigInteger ActualBalance;//stores actual balance after dividing with 'DecimalValue'

    private string chain = "binance";//name of the chain
    private string network = "mainnet";//name of the network
    private string contract = "0xFBb4F2f342c6DaaB63Ab85b0226716C4D1e26F36";//address of the BEP20 contract
    //private string MainAccount = "0x5eef79b1df4d44e76e340408d98c338dc8ea6dc9";

    private void Start()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        string _address= GetStorage("Account",this.gameObject.name,"OnGetAcount");

        //Debug.Log("address received : "+_address);
        //if(_address!=string.Empty && _address!=null)
        //{
        //    ConnectWallet();
        //}
#endif
        BEPName();
        BEPSymbol();
        BEPDecimals();
        BEPTotalSupply();
    }

    public void OnGetAcount(string info)
    {
        Debug.Log("Got addresssss");
        Debug.Log(info);

        if(info!="null" && info!="")
        {
            ConnectWallet();
        }
    }
    /// <summary>
    /// Display token balance on screen after connecting to wallet
    /// </summary>
    public void DisplayBalance()
    {
        DecimalValue = (BigInteger)Math.Pow(10, (double)decimals);
        ActualBalance = MainbalanceOf / DecimalValue;
        Debug.Log("Balance after decimal calculation: "+ ActualBalance);
        MainUI.CoinText.text = ActualBalance.ToString();
    }
    /// <summary>
    /// Called to connect wallet from "Connect Wallet" Button
    /// </summary>
    public void ConnectWallet()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
            Web3Connect();
            OnConnected();
#else
        Debug.Log("Cannot call inside editor, has support of webgl build only");
    #endif
    }

    /// <summary>
    /// Aysnc call when user is connected to a wallet
    /// </summary>
    async private void OnConnected()
    {
        account = ConnectAccount();
        while (account == "")
        {
            await new WaitForSeconds(1f);
            account = ConnectAccount();
        };

        // save account for next scene
        PlayerPrefs.SetString("Account", account);

#if UNITY_WEBGL && !UNITY_EDITOR
        SetStorage("Account", PlayerPrefs.GetString("Account"));
#endif

        FirebaseManager.Instance.WalletConnected = true;
        FirebaseManager.Instance.StartCoroutine(FirebaseManager.Instance.CheckCreateUserDB(PlayerPrefs.GetString("Account"), ""));

        Debug.Log("OnConnected Called");
        Debug.Log(PlayerPrefs.GetString("Account"));

        // reset login message
        SetConnectAccount("");

        MainUI.ConnectBtn.SetActive(false);
        MainUI.ConnectedBtn.SetActive(true);
        PrintWalletAddress();
        BEPBalanceOf();

    }

    /// <summary>
    /// Called ro print connected wallet address on Connected button in short form
    /// </summary>
    public void PrintWalletAddress()
    {
        char[] charArr = account.ToCharArray();
        string FirstPart = "";
        string MidPart = "*****************";
        string EndPart = "";

        for (int i = 0; i < 4; i++)
            FirstPart += charArr[i];

        for (int j= charArr.Length-4 ; j < charArr.Length; j++)
            EndPart += charArr[j];

        MainUI.ConnectedBtn.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "Connected"+"\n"+ FirstPart+ MidPart+ EndPart;
    }

    /// <summary>
    /// Call to get balance of specific BEp20/ERC20 contract
    /// </summary>
    async public void BEPBalanceOf()
    {
        MainbalanceOf = await ERC20.BalanceOf(chain, network, contract, account);
        print("Balance: "+ MainbalanceOf);
        DisplayBalance();
    }

    /// <summary>
    /// Call to get name of specific BEp20/ERC20 contract
    /// </summary>
    async public void BEPName()
    {
        nameContract = await ERC20.Name(chain, network, contract);
        print("name: "+ nameContract);
    }

    /// <summary>
    /// Call to get symbol of specific BEp20/ERC20 contract
    /// </summary>
    async public void BEPSymbol()
    {
        symbol = await ERC20.Symbol(chain, network, contract);
        print("Symbol: "+symbol);
    }

    /// <summary>
    /// Call to get decimal of specific BEp20/ERC20 contract
    /// </summary>
    async public void BEPDecimals ()
    {
        decimals = await ERC20.Decimals(chain, network, contract);
        print("Decimals: "+decimals);
    }

    /// <summary>
    /// Call to get total supply of specific BEp20/ERC20 contract
    /// </summary>
    async public void BEPTotalSupply()
    {
        totalSupply = await ERC20.TotalSupply(chain, network, contract);
        print("Total Supply: "+totalSupply);
    }

    public void OnSkip()
    {
        // save account for next scene
        PlayerPrefs.SetString("Account", "");

        // move to next scene
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
