using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    [SerializeField] private List<CarSelection> _selecteableCars = new List<CarSelection>();
    [SerializeField] private TextMeshProUGUI _versionText = null;
    [SerializeField] private TextMeshProUGUI _selectedCarName = null;

    private int _currentSelectedCarIndex = 0;


    
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

    // private void OnNextCar()
    // {
    //     int newIndex = _currentSelectedCarIndex + 1;
    //     newIndex %= _selecteableCars.Count;
    //     UpdateSelectedCarVisual(newIndex);
    // }

    // private void OnPrevCar()
    // {
    //     int newIndex = _currentSelectedCarIndex;
    //     newIndex--;
    //     if (newIndex < 0)
    //     {
    //         newIndex = _selecteableCars.Count + newIndex;
    //     }
    //
    //     UpdateSelectedCarVisual(newIndex);
    // }

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
        //todo: show loading screen
        SceneManager.LoadScene("Level1");
    }
}