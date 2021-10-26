using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class LeaderboardUI : MonoBehaviour
{
    public TextMeshProUGUI PositionText;
    public TextMeshProUGUI NameText;
    public TextMeshProUGUI WalletText;
    public TextMeshProUGUI TimeText;

    float timeSpanConversionHours;//var to hold hours after converstion from seconds
    float timeSpanConversiondMinutes;//var to hold minutes after converstion from seconds
    float timeSpanConversionSeconds;//var to hold seconds after converstion from float seconds

    string textfieldHours;//string store converstion of hours into string for display
    string textfieldMinutes;//string store converstion of minutes into string for display
    string textfieldSeconds;//string store converstion of seconds into string for display

    string MainTime;
    public void SetPrefabData(string _pos,string _name,string _wallet,float _time)
    {
        PositionText.text = _pos;
        NameText.text = _name;

        char[] charArr = _wallet.ToCharArray();
        string FirstPart = "";
        string MidPart = "********";
        string EndPart = "";

        for (int i = 0; i < 4; i++)
            FirstPart += charArr[i];

        for (int j = charArr.Length - 4; j < charArr.Length; j++)
            EndPart += charArr[j];

        WalletText.text= FirstPart + MidPart + EndPart;
        ConvertTime(_time);
    }

    public void ConvertTime(float _sec)
    {
        //Store TimeSpan into variable.
        timeSpanConversionHours = TimeSpan.FromSeconds(_sec).Hours;
        timeSpanConversiondMinutes = TimeSpan.FromSeconds(_sec).Minutes;
        timeSpanConversionSeconds = TimeSpan.FromSeconds(_sec).Seconds;

        //Convert TimeSpan variables into strings for textfield display
        textfieldHours = timeSpanConversionHours.ToString();
        textfieldMinutes = timeSpanConversiondMinutes.ToString();
        textfieldSeconds = timeSpanConversionSeconds.ToString();

        LeaderboardTime();
    }

    public void LeaderboardTime()
    {
        //Display the time given the number of digits.
        if (textfieldMinutes.Length == 2 && textfieldSeconds.Length == 2)
            MainTime = textfieldHours + ":" + textfieldMinutes + ":" + textfieldSeconds;
        else if (textfieldMinutes.Length == 2 && textfieldSeconds.Length == 1)
            MainTime = textfieldHours + ":" + textfieldMinutes + ":0" + textfieldSeconds;
        else if (textfieldMinutes.Length == 1 && textfieldSeconds.Length == 1)
            MainTime = textfieldHours + ":0" + textfieldMinutes + ":0" + textfieldSeconds;
        else if (textfieldMinutes.Length == 1 && textfieldSeconds.Length == 2)
            MainTime = textfieldHours + ":0" + textfieldMinutes + ":" + textfieldSeconds;
        else
            MainTime = textfieldHours + ":" + textfieldMinutes + ":" + textfieldSeconds;

        TimeText.text = MainTime;
    }


}
