using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using TMPro;
using System.Numerics;
using System;
using Newtonsoft.Json;

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
    private static extern void SetStorage(string key, string val);

    [DllImport("__Internal")]
    private static extern string GetStorage(string key, string ObjectName, string callback);

    [DllImport("__Internal")]
    private static extern void Web3Connect();

    [DllImport("__Internal")]
    private static extern string ConnectAccount();

    [DllImport("__Internal")]
    private static extern void SetConnectAccount(string value);
#endif

    public static WalletManager Instance;
    private string account; //string to store wallet address
    BigInteger MainbalanceOf;//Biginteger to store balance of token inside wallet
    BigInteger decimals;//Biginteger to store decimal settings for the token in wallet
    string symbol;//string to store symbol of token in wallet
    BigInteger totalSupply;//Biginteger to store totalSupply of tokens
    string nameContract;//string to store name of the contract (BEP20)

    BigInteger MainbalanceOfsGamer;//Biginteger to store balance of token inside wallet
    BigInteger decimalsGamer;//Biginteger to store decimal settings for the token in wallet
    string symbolsGamer;//string to store symbol of token in wallet
    BigInteger totalSupplysGamer;//Biginteger to store totalSupply of tokens
    string nameContractsGamer;//string to store name of the contract (BEP20)

    BigInteger DecimalValue; //stores Decimal calculation with power of 10
    BigInteger ActualBalance;//stores actual balance after dividing with 'DecimalValue'
    BigInteger MainBalance = 1000000000000000000;

    BigInteger DecimalValuesGamer; //stores Decimal calculation with power of 10
    BigInteger ActualBalancesGamer;//stores actual balance after dividing with 'DecimalValue'
    BigInteger MainBalancesGamer = 1000000000000000000;

    private string chain = "polygon";//name of the chain
    private string network = "mainnet";//name of the network
    private string contract = "0x3f6b3595ecf70735d3f48d69b09c4e4506db3f47";//address of the BEP20 contract // GAMER
    private string sGamercontract = "0xd1ecdc553651dab068486d9c4d066ecdc614416e";//address of the BEP20 contract // staked GAMER

    private string amount = "";
    private string toAccount = "0xe1E4160F4AcDf756AA0d2B02D786a42527560E82";
    private readonly string abi = "[ { \"inputs\": [ { \"internalType\": \"string\", \"name\": \"name_\", \"type\": \"string\" }, { \"internalType\": \"string\", \"name\": \"symbol_\", \"type\": \"string\" } ], \"stateMutability\": \"nonpayable\", \"type\": \"constructor\" }, { \"anonymous\": false, \"inputs\": [ { \"indexed\": true, \"internalType\": \"address\", \"name\": \"owner\", \"type\": \"address\" }, { \"indexed\": true, \"internalType\": \"address\", \"name\": \"spender\", \"type\": \"address\" }, { \"indexed\": false, \"internalType\": \"uint256\", \"name\": \"value\", \"type\": \"uint256\" } ], \"name\": \"Approval\", \"type\": \"event\" }, { \"anonymous\": false, \"inputs\": [ { \"indexed\": true, \"internalType\": \"address\", \"name\": \"from\", \"type\": \"address\" }, { \"indexed\": true, \"internalType\": \"address\", \"name\": \"to\", \"type\": \"address\" }, { \"indexed\": false, \"internalType\": \"uint256\", \"name\": \"value\", \"type\": \"uint256\" } ], \"name\": \"Transfer\", \"type\": \"event\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"owner\", \"type\": \"address\" }, { \"internalType\": \"address\", \"name\": \"spender\", \"type\": \"address\" } ], \"name\": \"allowance\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"spender\", \"type\": \"address\" }, { \"internalType\": \"uint256\", \"name\": \"amount\", \"type\": \"uint256\" } ], \"name\": \"approve\", \"outputs\": [ { \"internalType\": \"bool\", \"name\": \"\", \"type\": \"bool\" } ], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"account\", \"type\": \"address\" } ], \"name\": \"balanceOf\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"decimals\", \"outputs\": [ { \"internalType\": \"uint8\", \"name\": \"\", \"type\": \"uint8\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"spender\", \"type\": \"address\" }, { \"internalType\": \"uint256\", \"name\": \"subtractedValue\", \"type\": \"uint256\" } ], \"name\": \"decreaseAllowance\", \"outputs\": [ { \"internalType\": \"bool\", \"name\": \"\", \"type\": \"bool\" } ], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"spender\", \"type\": \"address\" }, { \"internalType\": \"uint256\", \"name\": \"addedValue\", \"type\": \"uint256\" } ], \"name\": \"increaseAllowance\", \"outputs\": [ { \"internalType\": \"bool\", \"name\": \"\", \"type\": \"bool\" } ], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"name\", \"outputs\": [ { \"internalType\": \"string\", \"name\": \"\", \"type\": \"string\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"symbol\", \"outputs\": [ { \"internalType\": \"string\", \"name\": \"\", \"type\": \"string\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [], \"name\": \"totalSupply\", \"outputs\": [ { \"internalType\": \"uint256\", \"name\": \"\", \"type\": \"uint256\" } ], \"stateMutability\": \"view\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"recipient\", \"type\": \"address\" }, { \"internalType\": \"uint256\", \"name\": \"amount\", \"type\": \"uint256\" } ], \"name\": \"transfer\", \"outputs\": [ { \"internalType\": \"bool\", \"name\": \"\", \"type\": \"bool\" } ], \"stateMutability\": \"nonpayable\", \"type\": \"function\" }, { \"inputs\": [ { \"internalType\": \"address\", \"name\": \"sender\", \"type\": \"address\" }, { \"internalType\": \"address\", \"name\": \"recipient\", \"type\": \"address\" }, { \"internalType\": \"uint256\", \"name\": \"amount\", \"type\": \"uint256\" } ], \"name\": \"transferFrom\", \"outputs\": [ { \"internalType\": \"bool\", \"name\": \"\", \"type\": \"bool\" } ], \"stateMutability\": \"nonpayable\", \"type\": \"function\" } ]";

    //private string MainAccount = "0x5eef79b1df4d44e76e340408d98c338dc8ea6dc9";

    private void OnEnable()
    {
        Instance = this;
        Constants.WalletConnected = false;
    }
    private void Start()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        string _address= GetStorage("Account",this.gameObject.name,"OnGetAcount");
#endif
        //BEPName();
        //BEPSymbol();
        //BEPDecimals();
        //BEPTotalSupply();
    }

    public void OnGetAcount(string info)
    {
        Debug.Log("Got addresssss");
        Debug.Log(info);

        if (info != "null" && info != "")
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
        Debug.Log("Balance after decimal calculation: " + ActualBalance);
        MainUI.CoinText.text = ActualBalance.ToString();
    }

    /// <summary>
    /// Display token sGamer balance on screen after connecting to wallet
    /// </summary>
    public void DisplayBalancesGamer()
    {
        DecimalValuesGamer = (BigInteger)Math.Pow(10, (double)decimalsGamer);
        ActualBalancesGamer = MainbalanceOfsGamer / DecimalValuesGamer;
        Debug.Log("Balance after decimal calculation sGamer: " + ActualBalancesGamer);
        MainMenuViewController.Instance.ChangesGamerText(ActualBalancesGamer.ToString());
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

        FirebaseManager.Instance.DocFetched = false;
        FirebaseManager.Instance.ResultFetched = true;
        FirebaseManager.Instance.StartCoroutine(FirebaseManager.Instance.CheckCreateUserDB(PlayerPrefs.GetString("Account"), ""));

        Debug.Log("wallet connected");
        //Debug.Log(PlayerPrefs.GetString("Account"));

        // reset login message
        SetConnectAccount("");

        MainUI.ConnectBtn.SetActive(false);
        MainUI.ConnectedBtn.SetActive(true);
        PrintWalletAddress();
        BEPBalanceOf();
        BEPBalanceOfsGamer();
        Constants.WalletConnected = true;

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

        for (int j = charArr.Length - 4; j < charArr.Length; j++)
            EndPart += charArr[j];

        MainUI.ConnectedBtn.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "Connected" + "\n" + FirstPart + MidPart + EndPart;
    }

    /// <summary>
    /// Call to get balance of specific BEp20/ERC20 contract
    /// </summary>
    async public void BEPBalanceOf()
    {
        MainbalanceOf = await ERC20.BalanceOf(chain, network, contract, account);
        print("Balance: " + MainbalanceOf);
        DisplayBalance();
    }

    /// <summary>
    /// Call to get balance of specific BEp20/ERC20 contract
    /// </summary>
    async public void BEPBalanceOfsGamer()
    {
        MainbalanceOfsGamer = await ERC20.BalanceOf(chain, network, sGamercontract, account);
        print("Balance of sGamer: " + MainbalanceOfsGamer);
        DisplayBalancesGamer();
    }


    /// <summary>
    /// Call to get name of specific BEp20/ERC20 contract
    /// </summary>
    async public void BEPName()
    {
        nameContract = await ERC20.Name(chain, network, contract);
        print("name: " + nameContract);
    }

    /// <summary>
    /// Call to get name of specific BEp20/ERC20 contract
    /// </summary>
    async public void BEPNamesGamer()
    {
        nameContractsGamer = await ERC20.Name(chain, network, sGamercontract);
        print("name sGamer: " + nameContractsGamer);
    }

    /// <summary>
    /// Call to get symbol of specific BEp20/ERC20 contract
    /// </summary>
    async public void BEPSymbol()
    {
        symbol = await ERC20.Symbol(chain, network, contract);
        print("Symbol: " + symbol);
    }

    async public void BEPSymbolsGamer()
    {
        symbolsGamer = await ERC20.Symbol(chain, network, sGamercontract);
        print("Symbol sGamer: " + symbolsGamer);
    }

    /// <summary>
    /// Call to get decimal of specific BEp20/ERC20 contract
    /// </summary>
    async public void BEPDecimals()
    {
        decimals = await ERC20.Decimals(chain, network, contract);
        print("Decimals: " + decimals);
    }

    /// <summary>
    /// Call to get decimal of specific BEp20/ERC20 contract
    /// </summary>
    async public void BEPDecimalsGamer()
    {
        decimalsGamer = await ERC20.Decimals(chain, network, sGamercontract);
        print("Decimals sGamer: " + decimalsGamer);
    }

    /// <summary>
    /// Call to get total supply of specific BEp20/ERC20 contract
    /// </summary>
    async public void BEPTotalSupply()
    {
        totalSupply = await ERC20.TotalSupply(chain, network, contract);
        print("Total Supply: " + totalSupply);
    }


    /// <summary>
    /// Call to get total supply of specific BEp20/ERC20 contract
    /// </summary>
    async public void BEPTotalSupplysGamer()
    {
        totalSupplysGamer = await ERC20.TotalSupply(chain, network, sGamercontract);
        print("Total Supply sGamer: " + totalSupplysGamer);
    }

    public void OnSkip()
    {
        // save account for next scene
        PlayerPrefs.SetString("Account", "");

        // move to next scene
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    /// <summary>
    /// Trnasfer BEP20 token
    /// </summary>
    async public void TransferToken(int _amount, bool BuyTries = false)
    {
        Constants.BuyingTries = BuyTries;
        BigInteger _mainAmount = _amount * MainBalance;
        amount = _mainAmount.ToString();
        string method = "transfer";
        // array of arguments for contract
        string[] obj = { toAccount, amount };
        string args = JsonConvert.SerializeObject(obj);
        // value in wei
        string value = "0";
        // gas limit OPTIONAL
        string gas = "210000";// 21000
        // connects to user's browser wallet (metamask) to send a transaction
        try
        {
            string response = await Web3GL.SendContract(method, abi, contract, args, value, gas);
            Debug.Log(response);
            BEPBalanceOf();
            BEPBalanceOfsGamer();

            if (Constants.BuyingTries)
            {
                Constants.BuyingTries = false;
                MainMenuViewController.Instance.OnBuyTires(true);
            }
            else
            {
                MainMenuViewController.Instance.StartTournament(true);
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e, this);  

            if (Constants.BuyingTries)
            {
                Constants.BuyingTries = false;
                MainMenuViewController.Instance.OnBuyTires(false);
            }
            else
            {
                MainMenuViewController.Instance.StartTournament(false);
            }
        }
    }

    public bool CheckBalanceTournament()
    {
        bool _havebalance = false;
        if (TournamentManager.Instance)
        {
            if (ActualBalance >= TournamentManager.Instance.DataTournament.TicketPrice)
            {
                _havebalance = true;
            }
        }
        else
        {
            Debug.LogError("TournamentManager instance is null");
        }

        return _havebalance;
    }

    public bool CheckBalanceORTryForFreeTournament()
    {
        bool _havebalance = false;
        if (TournamentManager.Instance)
        {
             if (ActualBalance >= Constants.FreeGamerPirce && FirebaseManager.Instance.PlayerData.FreeTryGAMER==0)
              {
                  _havebalance = true;
              }
        }
        else
        {
            Debug.LogError("TournamentManager instance is null");
        }

        return _havebalance;
    }

    public bool CheckBalanceForBuyingTournament()
    {
        bool _havebalance = false;
        if (TournamentManager.Instance)
        {
            if (ActualBalancesGamer >= Constants.FreesGamerPirce && FirebaseManager.Instance.PlayerData.FreeTrysGAMER == 0)
            {
                _havebalance = true;
            }
        }
        else
        {
            Debug.LogError("TournamentManager instance is null");
        }

        return _havebalance;
    }

    public void UpdateWallet()
    {
        BEPName();
        BEPSymbol();
        BEPDecimals();
        BEPTotalSupply();

        BEPNamesGamer();
        BEPSymbolsGamer();
        BEPDecimalsGamer();
        BEPTotalSupplysGamer();

        if (Constants.WalletConnected)
        {
            BEPBalanceOf();
            BEPBalanceOfsGamer();
        }
    }
}
