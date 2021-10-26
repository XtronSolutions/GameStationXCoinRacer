using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

[Serializable]
public class PopMessageUI
{
    public GameObject PopUpScreen;
    public TextMeshProUGUI MsgText;
}

[Serializable]
public class InputFieldUI
{
    public GameObject InputScreen;
    public TextMeshProUGUI WalletTitleText;
    public TextMeshProUGUI WalletValueText;
    public TextMeshProUGUI UsernameTitle;
    public TMP_InputField UsernameInput;
    public Button SubmitButton;
}
public class GamePlayUIHandler : MonoBehaviour
{
    public static GamePlayUIHandler Instance;
    public InputFieldUI UIInputField;
    public PopMessageUI MessagePopUI;

    private void Start()
    {
        Instance = this;
    }

    [HideInInspector]
    public string _username;

    # region InputFieldUI
    public void SetWallet_InputFieldUI(string _add)
    {
        UIInputField.WalletValueText.text = _add;
    }

    public void ToggleInputScreen_InputFieldUI(bool _state)
    {
        UIInputField.InputScreen.SetActive(_state);
        UIInputField.UsernameInput.text = "";
    }

    public void OnUsernameChanged_InputFieldUI(string _name)
    {
        _username = _name;
    }

    public void SetInputUsername_InputFieldUI(string _name)
    {
        _username = _name;
        UIInputField.UsernameInput.text = _name;
    }

    public void SubmitData_InputFieldUI()
    {
        if (_username == "")
        {
            ShowToast(0.1f, "Please enter username.");
        }
        else
        {
            if (FirebaseManager.Instance)
            {
                FirebaseManager.Instance.PlayerData.UserName = _username;
                FirebaseManager.Instance.PlayerData.TimeSeconds = TimeHandler.Instance.TotalSeconds;
                FirebaseManager.Instance.UpdatedFireStoreData(FirebaseManager.Instance.PlayerData);

            }

            ToggleInputScreen_InputFieldUI(false);
           
        }

    }
    #endregion

    #region PopMessageUI
    public void ShowToast(float _time, string _msg)
    {
        MessagePopUI.PopUpScreen.SetActive(true);
        MessagePopUI.MsgText.text = _msg;
        StartCoroutine(DisableToast(_time));
    }

    IEnumerator DisableToast(float _sec)
    {
        yield return new WaitForSeconds(_sec);
        MessagePopUI.PopUpScreen.SetActive(false);
    }
    #endregion

    
}
