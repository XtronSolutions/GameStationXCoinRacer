using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class RaceManager : MonoBehaviour
{

    [SerializeField] private List<WayPoint> _wayPoints = new List<WayPoint>();
    [SerializeField] private int _requiredNumberOfLaps = 3;
    [SerializeField] private GameObject _pasueMenuObject = null;
    [SerializeField] private GameObject _raceOverMenuObject = null;
    [SerializeField] private TextMeshProUGUI LapText;
    
    [SerializeField] private AudioClip _buttonPressClip = null;
    [SerializeField] private AudioSource _audioSource = null;
    
    private int _currentWayPointIndex = 1;
    private int _lapsCounter;

    public static RaceManager Instance;
    
    void Start()
    {
        Instance = this;
        LapText.text = "Lap " + _lapsCounter.ToString() + "/" + _requiredNumberOfLaps.ToString();
        foreach (var wayPoint in _wayPoints)
        {
            wayPoint.WayPointDataObservable.Subscribe(OnWayPointData).AddTo(this);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    private void OnWayPointData(WayPointData data)
    {
        int indexOfPayPoint = _wayPoints.IndexOf(data.Waypoint);

        if (indexOfPayPoint % _wayPoints.Count == _currentWayPointIndex)
        {
            _currentWayPointIndex++;
            _currentWayPointIndex %= _wayPoints.Count;

            if (_currentWayPointIndex == 1)
            {
                _lapsCounter++;
                LapText.text = "Lap " + _lapsCounter.ToString() + "/" + _requiredNumberOfLaps.ToString();
                if (_lapsCounter == _requiredNumberOfLaps)
                {
                    OnRaceDone();
                }
            }
            else
            {
                print($"cross waypoint {_currentWayPointIndex}");
            }
        }
    }

    private void OnRaceDone()
    {
        if (TimeHandler.Instance)
            TimeHandler.Instance.timerIsRunning = false;


        if(GamePlayUIHandler.Instance)
        {
            GamePlayUIHandler.Instance.ToggleInputScreen_InputFieldUI(true);
            GamePlayUIHandler.Instance.SetWallet_InputFieldUI(FirebaseManager.Instance.PlayerData.WalletAddress);
            GamePlayUIHandler.Instance.SetInputUsername_InputFieldUI(FirebaseManager.Instance.PlayerData.UserName);
        }

        Time.timeScale = 0.1f;
    }
    public void RaceEnded()
    {
        //_raceOverMenuObject.SetActive(true);

        if(GamePlayUIHandler.Instance)
        {
            LeaderboardManager.Instance.EnableGameplayLeaderboard();
        }
    }

    public void TogglePauseMenu()
    {
        _pasueMenuObject.SetActive(!_pasueMenuObject.activeSelf);
        Time.timeScale = _pasueMenuObject.activeSelf ? 0 : 1;
    }

    public void ReplayLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1;
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(Constants.MAIN_MENU_SCENE_NAME);
        Time.timeScale = 1;
    }
    
    public Vector3 GetNextWayPointPosition()
    {
        return _wayPoints[_currentWayPointIndex].transform.position;
    }
   
    public void PlayButtonDownAudioClip()
    {
        _audioSource.PlayOneShot(_buttonPressClip);
    }
}
