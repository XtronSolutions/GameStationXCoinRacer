using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Constants;

public class MainMenuViewController : MonoBehaviour
{
    public static CarSettings SelectedCar;

    [SerializeField] private GameObject GameModeSelectionObject = null;
    [SerializeField] private GameObject CarSelectionObject = null;
    [SerializeField] private GameObject CarSelection3dObject = null;
    [SerializeField] private GameObject MapSelection = null;
    [SerializeField] private Button _singlePlayerButton = null;
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

    private int _currentSelectedCarIndex = 0;
    private int _currentlySelectedLevelIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        _singlePlayerButton.onClick.AddListener(OnGoToCarSelection);
        _backToModeSelectionButton.onClick.AddListener(OnGoBackToModeSelection);
        _goToMapSelectionButton.onClick.AddListener(OnGoToMapSelection);
        _backToCarSelectionButton.onClick.AddListener(OnGoToCarSelection);
        _startRaceButton.onClick.AddListener(StartRace);

        _currentSelectedCarIndex = 0;
        UpdateSelectedCarVisual(_currentSelectedCarIndex);
        _versionText.text = APP_VERSION;
        
        _nextCarButton.onClick.AddListener(OnNextCar);
        _prevCarButton.onClick.AddListener(OnPrevCar);
        _nextMapButton.onClick.AddListener(OnNextMap);
        _prevMapButton.onClick.AddListener(OnPrevMap);

        // for (int i = 0; i < _selectCarButtons.Count; i++)
        // {
        //     int index = i;
        //     _selectCarButtons[i].image.sprite = _selecteableCars[i].carSettings.Icon;
        //     _selectCarButtons[i].onClick.AsObservable().Subscribe(_ => UpdateSelectedCarVisual(index)).AddTo(this);
        // }
        //
        // for (int i = 0; i < _levelsSettings.Count; i++)
        // {
        //     int index = i;
        //     _SelectLevelButtons[i].image.sprite = _levelsSettings[i].Icon;
        //     _SelectLevelButtons[i].onClick.AsObservable().Subscribe(_ => OnLevelSelected(index)).AddTo(this);
        // }

        OnLevelSelected(0);
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
}