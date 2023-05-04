using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TotalBox : MonoBehaviour {

  public Text TurnTxt;
  public Text MoneyTxt;

  void OnEnable() {
    TurnTxt.text = DIContainer.Instance.GameDataService.GameData.Turns.ToString();
    MoneyTxt.text = DIContainer.Instance.GameDataService.GameData.Money.ToString();
  }
}
