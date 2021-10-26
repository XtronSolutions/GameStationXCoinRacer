using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[Serializable]
public class LeaderBoardDataUI
{
    public GameObject MainScreen;
    public GameObject ObjectPrefab;
    public Transform ScrollContent;
    public GameObject LoaderObj;
    public int ContentHeight;

}
public class LeaderboardManager : MonoBehaviour
{
    public LeaderBoardDataUI LeaderBoardUIData;
    public static LeaderboardManager Instance;
    List<GameObject> BoardObjects = new List<GameObject>();
    #region Leaderboard

    private void OnEnable()
    {
        Instance = this;
        //PrintDummy();
    }
    public void EnableGameplayLeaderboard()
    {
        LeaderBoardUIData.MainScreen.SetActive(true);
        LeaderBoardUIData.LoaderObj.SetActive(true);

        FirebaseManager.Instance.QueryDB("TimeSeconds", "desc");
    }

    public void CloseLeaderBoard()
    {
        LeaderBoardUIData.MainScreen.SetActive(false);
    }

    public void PopulateLeaderboardData(UserData[] _data)
    {
        for (int i = 0; i < BoardObjects.Count; i++)
        {
            Destroy(BoardObjects[i]);
        }

        BoardObjects.Clear();

        for (int i = 0; i < _data.Length; i++)
        {
            GameObject _obj = Instantiate(LeaderBoardUIData.ObjectPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            LeaderboardUI _UIInstance = _obj.GetComponent<LeaderboardUI>();
            _UIInstance.SetPrefabData((i + 1).ToString(), _data[i].UserName, _data[i].WalletAddress, (float)_data[i].TimeSeconds);
            _obj.transform.SetParent(LeaderBoardUIData.ScrollContent);
            _obj.transform.localScale = new Vector3(1, 1, 1);
            LeaderBoardUIData.ScrollContent.GetComponent<RectTransform>().sizeDelta = new Vector2(0, LeaderBoardUIData.ContentHeight * (i + 1));

            BoardObjects.Add(_obj);
        }

        LeaderBoardUIData.LoaderObj.SetActive(false);
    }

    public void PrintDummy()
    {
        UserData[] maindata = new UserData[5];

        maindata[0] = new UserData();
        maindata[0].UserName = "Jhon123456";
        maindata[0].WalletAddress = "02225568";
        maindata[0].TimeSeconds = 152.3;

        maindata[1] = new UserData();
        maindata[1].UserName = "Orthelio";
        maindata[1].WalletAddress = "123456789";
        maindata[1].TimeSeconds = 120.8;

        maindata[2] = new UserData();
        maindata[2].UserName = "OceanLife";
        maindata[2].WalletAddress = "987654321";
        maindata[2].TimeSeconds = 85.4;

        maindata[3] = new UserData();
        maindata[3].UserName = "Smith Page";
        maindata[3].WalletAddress = "00002222";
        maindata[3].TimeSeconds = 185.1;

        maindata[4] = new UserData();
        maindata[4].UserName = "Lauura bane";
        maindata[4].WalletAddress = "11115555";
        maindata[4].TimeSeconds = 400.3;

        System.Array.Reverse(maindata);
        PopulateLeaderboardData(maindata);
    }
    #endregion
}
