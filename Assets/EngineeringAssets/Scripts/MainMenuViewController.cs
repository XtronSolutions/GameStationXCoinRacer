using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Constants;

[Serializable]
public class TournamentUI
{
    public GameObject MainScreen;
    public TextMeshProUGUI LowerHeaderText;
    public TextMeshProUGUI TimerText;
    public TextMeshProUGUI FotterText;
    public TextMeshProUGUI DisclaimerText;
    public GameObject LoaderObj;
    public TextMeshProUGUI TournamentStartText;
}

[Serializable]
public class TournamentSelectionUI
{
    public GameObject MainScreen;
    public Button FreeTry;
    public Button PlayFromTries;
    public Button Buy12Tries;
    public Button PlayOnce;
    public Button Cancel;
}
public class MainMenuViewController : MonoBehaviour
{
    public static CarSettings SelectedCar;

    public static MainMenuViewController Instance;

    [SerializeField] private GameObject GameModeSelectionObject = null;
    [SerializeField] private GameObject CarSelectionObject = null;
    [SerializeField] private GameObject CarSelection3dObject = null;
    [SerializeField] private GameObject MapSelection = null;
    [SerializeField] private Button _singlePlayerButton = null;
    [SerializeField] private Button _tournamentButton = null;
    [SerializeField] private Button _backToModeSelectionButton = null;
    [SerializeField] private Button _goToMapSelectionButton = null;
    [SerializeField] private Button _backToCarSelectionButton = null;
    [SerializeField] private Button _startRaceButton = null;
    [SerializeField] private Button _nextCarButton = null;
    [SerializeField] private Button _prevCarButton = null;
    [SerializeField] private Button _nextMapButton = null;
    [SerializeField] private Button _prevMapButton = null;
    [SerializeField] private List<CarSelection> _selecteableCars = new List<CarSelection>();
    [SerializeField] private TextMeshProUGUI _versionText = null;
    [SerializeField] private TextMeshProUGUI _selectedCarName = null;
    [SerializeField] private List<LevelSettings> _levelsSettings = new List<LevelSettings>();
    [SerializeField] private Image _selectedMapImage = null;
    [SerializeField] private TextMeshProUGUI _levelNameText = null;
    [SerializeField] private GameObject LoadingScreen = null;
    [SerializeField] private AudioClip _buttonPressClip = null;
    [SerializeField] private AudioSource _audioSource = null;

    [SerializeField] private GameObject MessageUI;
    [SerializeField] private TextMeshProUGUI ToastMsgText = null;
    public TextMeshProUGUI TriesText = null;
    public TextMeshProUGUI SGamerText = null;

    public TournamentUI UITournament;
    public TournamentSelectionUI TournamentUISelection;

    private int _currentSelectedCarIndex = 0;
    private int _currentlySelectedLevelIndex = 0;

    // Start is called before the first frame update

    private void OnEnable()
    {
        Instance = this;
    }

    void Start()
    {
        _audioSource.GetComponent<AudioSource>();
        _singlePlayerButton.onClick.AddListener(OnGoToCarSelection);
        _tournamentButton.onClick.AddListener(OnGoToCarSelectionTournament);
        _backToModeSelectionButton.onClick.AddListener(OnGoBackToModeSelection);
        _goToMapSelectionButton.onClick.AddListener(OnGoToMapSelection);
        _backToCarSelectionButton.onClick.AddListener(OnGoToCarSelection);
        _startRaceButton.onClick.AddListener(StartRace);

        _currentSelectedCarIndex = 0;
        UpdateSelectedCarVisual(_currentSelectedCarIndex);
        //_versionText.text = APP_VERSION;
        
        _nextCarButton.onClick.AddListener(OnNextCar);
        _prevCarButton.onClick.AddListener(OnPrevCar);
        _nextMapButton.onClick.AddListener(OnNextMap);
        _prevMapButton.onClick.AddListener(OnPrevMap);

        OnLevelSelected(0);
        TournamentSelection_EventListeners();
    }

    public void ToggleTournamentSelectionScreen(bool _state)
    {
        TournamentUISelection.MainScreen.SetActive(_state);
    }

    public void TournamentSelection_EventListeners()
    {
        TournamentUISelection.FreeTry.onClick.AddListener(OnButtonPressed_FreeTry);
        TournamentUISelection.Buy12Tries.onClick.AddListener(OnButtonPressed_Buy12Tries);
        TournamentUISelection.PlayFromTries.onClick.AddListener(OnButtonPressed_PlayFromTry);
        TournamentUISelection.PlayOnce.onClick.AddListener(OnButtonPressed_BuyOnceAndPlay);
        TournamentUISelection.Cancel.onClick.AddListener(DisableTournamentSelection);
    }

    public void DisableTournamentSelection()
    {
        ToggleTournamentSelectionScreen(false);
    }

    public void OnButtonPressed_FreeTry()
    {
        if (Constants.TournamentActive == true)
        {
            LoadingScreen.SetActive(true);
            if (WalletManager.Instance)
            {
                if (WalletManager.Instance.CheckBalanceORTryForFreeTournament())
                {
                    FirebaseManager.Instance.PlayerData.FreeTryGAMER = 1;
                    FirebaseManager.Instance.UpdatedFireStoreData(FirebaseManager.Instance.PlayerData);
                    StartTournament(true);
                    //WalletManager.Instance.TransferToken(TournamentManager.Instance.DataTournament.TicketPrice);
                }
                else
                {
                    LoadingScreen.SetActive(false);
                    ShowToast(3f, "Insufficient $GAMER value or free try already availed");
                }
            }
            else
            {
                LoadingScreen.SetActive(false);
                Debug.LogError("WalletManager instance is null");
                ShowToast(3f, "something went wrong, please try again");
            }
        }
        else
        {
            LoadingScreen.SetActive(false);
            ShowToast(3f, "No active tournament at the moment.");
        }
    }

    public void OnButtonPressed_PlayFromTry()
    {
        if (Constants.TournamentActive == true)
        {
            LoadingScreen.SetActive(true);
            if (WalletManager.Instance)
            {
                if (FirebaseManager.Instance.PlayerData.amountOfFreeTries>0)
                {
                    FirebaseManager.Instance.PlayerData.amountOfFreeTries -= 1;
                    ChangeTriesText(FirebaseManager.Instance.PlayerData.amountOfFreeTries.ToString());
                    FirebaseManager.Instance.UpdatedFireStoreData(FirebaseManager.Instance.PlayerData);
                    StartTournament(true);
                }
                else
                {
                    LoadingScreen.SetActive(false);
                    ShowToast(3f, "Insufficient number of tries.");
                }
            }
            else
            {
                LoadingScreen.SetActive(false);
                Debug.LogError("WalletManager instance is null");
                ShowToast(3f, "something went wrong, please try again");
            }
        }
        else
        {
            LoadingScreen.SetActive(false);
            ShowToast(3f, "No active tournament at the moment.");
        }
    }

    public void OnBuyTires(bool _success)
    {
        LoadingScreen.SetActive(false);
        if (_success)
        {
            FirebaseManager.Instance.PlayerData.amountOfFreeTries += Constants.NumberTriesToBuy;
            ChangeTriesText(FirebaseManager.Instance.PlayerData.amountOfFreeTries.ToString());
            FirebaseManager.Instance.UpdatedFireStoreData(FirebaseManager.Instance.PlayerData);
            ShowToast(3f, Constants.NumberTriesToBuy.ToString()+" tries were successfully added, total tries are "+ FirebaseManager.Instance.PlayerData.amountOfFreeTries.ToString());
        }
        else
        {
            ShowToast(3f, "something went wrong or transaction was rejected, please try again");
        }
    }
    public void OnButtonPressed_Buy12Tries()
    {
        if (Constants.TournamentActive == true)
        {
            LoadingScreen.SetActive(true);
            if (WalletManager.Instance)
            {
                if (WalletManager.Instance.CheckBalanceForBuyingTournament())
                {
                    WalletManager.Instance.TransferToken(Constants.Tries12Fees,true);
                }
                else
                {
                    LoadingScreen.SetActive(false);
                    ShowToast(3f, "Insufficient $GAMER value, need "+Constants.Tries12Fees.ToString()+ " $GAMER");
                }
            }
            else
            {
                LoadingScreen.SetActive(false);
                Debug.LogError("WalletManager instance is null");
                ShowToast(3f, "something went wrong, please try again");
            }
        }
        else
        {
            LoadingScreen.SetActive(false);
            ShowToast(3f, "No active tournament at the moment.");
        }
    }

    public void OnButtonPressed_BuyOnceAndPlay()
    {
        if (Constants.TournamentActive == true)
        {
            LoadingScreen.SetActive(true);
            if (WalletManager.Instance)
            {
                if (WalletManager.Instance.CheckBalanceTournament())
                {
                    WalletManager.Instance.TransferToken(TournamentManager.Instance.DataTournament.TicketPrice, false);
                }
                else
                {
                    LoadingScreen.SetActive(false);
                    ShowToast(3f, "Insufficient $GAMER value, need " + TournamentManager.Instance.DataTournament.TicketPrice.ToString() + " $GAMER");
                }
            }
            else
            {
                LoadingScreen.SetActive(false);
                Debug.LogError("WalletManager instance is null");
                ShowToast(3f, "something went wrong, please try again");
            }
        }
        else
        {
            LoadingScreen.SetActive(false);
            ShowToast(3f, "No active tournament at the moment.");
        }
    }

    public void ChangeTriesText(string _text)
    {
        TriesText.text = _text;
    }

    public void ChangesGamerText(string _text)
    {
        SGamerText.text = _text;
    }

    private void OnNextMap()
    {
        int newIndex = _currentlySelectedLevelIndex + 1;
        newIndex %= _levelsSettings.Count;
        OnLevelSelected(newIndex);
    }

    private void OnPrevMap()
    {
        int newIndex = _currentlySelectedLevelIndex;
        newIndex--;
        if (newIndex < 0)
        {
            newIndex = _levelsSettings.Count + newIndex;
        }
    
        OnLevelSelected(newIndex);
    }

    private void OnLevelSelected(int i)
    {
        _currentlySelectedLevelIndex = i;
        _selectedMapImage.sprite =  _levelsSettings[i].Icon;
        _levelNameText.text = _levelsSettings[i].LevelName;
    }
    
    private void OnGoToCarSelection()
    {
       Constants.IsTournament = false;
       Constants.IsPractice = true;
       GameModeSelectionObject.SetActive(false);
       CarSelectionObject.SetActive(true);
       CarSelection3dObject.SetActive(true);
       MapSelection.SetActive(false);
    }

    public void OnGoToCarSelectionTournament()
    {
        if (FirebaseManager.Instance.WalletConnected)
        {
            if (Constants.TournamentActive == true)
            {
                LoadingScreen.SetActive(false);
                ToggleTournamentSelectionScreen(true);
            }else
            {
                ShowToast(3f, "No active tournament at the moment.");
            }
               
            //if (Constants.TournamentActive == true)
            //{
            //    LoadingScreen.SetActive(true);
            //    if (WalletManager.Instance)
            //    {
            //        if (WalletManager.Instance.CheckBalanceTournament())
            //        {
            //            WalletManager.Instance.TransferToken(TournamentManager.Instance.DataTournament.TicketPrice);
            //        }
            //        else
            //        {
            //            LoadingScreen.SetActive(false);
            //            ShowToast(3f, "Insufficient $CRACE value.");
            //        }
            //    }
            //    else
            //    {
            //        LoadingScreen.SetActive(false);
            //        Debug.LogError("WalletManager instance is null");
            //    }
            //}else
            //{
            //    LoadingScreen.SetActive(false);
            //    ShowToast(3f, "No active tournament at the moment.");
            //}
        }
        else
        {
            LoadingScreen.SetActive(false);
            ShowToast(3f, "Please connect your wallet first.");
        }
    }

    public void StartTournament(bool _canstart=false)
    {
        if (_canstart)
        {
            //ShowToast(2f, "Transaction was was successful");
            LoadingScreen.SetActive(false);
            Constants.IsTournament = true;
            Constants.IsPractice = false;
            GameModeSelectionObject.SetActive(false);
            CarSelectionObject.SetActive(true);
            CarSelection3dObject.SetActive(true);
            MapSelection.SetActive(false);
        }else
        {
            LoadingScreen.SetActive(false);
            ShowToast(3f, "Transaction was not successful, please try again.");
            //Invoke("StartWithDelay", 4f);
        }
    }

    public void StartWithDelay()
    {
        LoadingScreen.SetActive(false);
        Constants.IsTournament = true;
        Constants.IsPractice = false;
        GameModeSelectionObject.SetActive(false);
        CarSelectionObject.SetActive(true);
        CarSelection3dObject.SetActive(true);
        MapSelection.SetActive(false);
    }

    private void OnGoBackToModeSelection()
    {
        GameModeSelectionObject.SetActive(true);
        CarSelectionObject.SetActive(false);
        CarSelection3dObject.SetActive(false);
        MapSelection.SetActive(false);
    }

    private void OnGoToMapSelection()
    {
        GameModeSelectionObject.SetActive(false);
        CarSelectionObject.SetActive(false);
        CarSelection3dObject.SetActive(false);
        MapSelection.SetActive(true);
    }

    private void OnNextCar()
    {
        int newIndex = _currentSelectedCarIndex + 1;
        newIndex %= _selecteableCars.Count;
        UpdateSelectedCarVisual(newIndex);
    }

    private void OnPrevCar()
    {
        int newIndex = _currentSelectedCarIndex;
        newIndex--;
        if (newIndex < 0)
        {
            newIndex = _selecteableCars.Count + newIndex;
        }
    
        UpdateSelectedCarVisual(newIndex);
    }

    public void UpdateSelectedCarVisual(int newIndex)
    {
        _selecteableCars[_currentSelectedCarIndex].Deactivate();
        _currentSelectedCarIndex = newIndex;
        _selecteableCars[_currentSelectedCarIndex].Activate();
        _selectedCarName.text = _selecteableCars[_currentSelectedCarIndex].carSettings.Name;
    }

    private void StartRace()
    {
        SelectedCar = _selecteableCars[_currentSelectedCarIndex].carSettings;
        SceneManager.LoadScene(_levelsSettings[_currentlySelectedLevelIndex].SceneName);
    }

    private void ShowToast(float _time,string _msg)
    {
        MessageUI.SetActive(true);
        ToastMsgText.text = _msg;
        StartCoroutine(DisableToast(_time));
    }

    IEnumerator DisableToast(float _sec)
    {
        yield return new WaitForSeconds(_sec);
        MessageUI.SetActive(false);
    }

    public void PlayButtonDownAudioClip()
    {
        _audioSource.PlayOneShot(_buttonPressClip);
    }
}