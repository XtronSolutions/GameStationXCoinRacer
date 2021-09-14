using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RaceManager : MonoBehaviour
{

    [SerializeField] private List<WayPoint> _wayPoints = new List<WayPoint>();
    [SerializeField] private int _requiredNumberOfLaps = 3;
    [SerializeField] private GameObject _pasueMenuObject = null;
    [SerializeField] private GameObject _raceOverMenuObject = null;
    private int _currentWayPointIndex = 1;
    private int _lapsCounter;
    
    
    // Start is called before the first frame update
    void Start()
    {
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
                print($"lap {_lapsCounter}");
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
        _raceOverMenuObject.SetActive(true);
        Time.timeScale = 0;
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
   
}
