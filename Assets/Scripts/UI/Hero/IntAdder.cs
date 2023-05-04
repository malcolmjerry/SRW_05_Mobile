using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntAdder : SelectableMenu {

  List<int> addComList;
  int myValue, min, max;
  Text valueText;

  bool countOK;
  int half;

  float lastTime;
  float maxTime;
  readonly float defaultMaxTime = 0.1f;

  private void Awake() {
    setupBase( menuItemTfList.Count, false, false, 1 );
    half = menuItemTfList.Count / 2 - 1;
    SetIndexToHalf();
    valueText = transform.Find( "ValueTxt" ).GetComponent<Text>();
  }

  // Start is called before the first frame update
  void Start() {
  }

  private void OnEnable() {
    lastTime = 0;
    maxTime = defaultMaxTime * 5;
  }

  public IntAdder Setup( List<int> addComList, int min, int max, int myValue, Action callback, Action<dynamic> next ) {
    Setup( callback, next );
    this.min = min;
    this.max = max;

    if (myValue < min) myValue = min;
    else if (myValue > max) myValue = max;

    this.myValue = myValue;
    valueText.text = this.myValue.ToString();

    if (addComList.Count == menuItemTfList.Count) {
      this.addComList = addComList;
      countOK = true;
    }
    else {
      countOK = false;
    }

    return this;
  }

  protected override void Update() {
    base.Update();

    if (Input.GetButtonDown( "NextUnit" )) {
      //EffectSoundController.PLAY_MENU_MOVE();
      SetPageAndRowBySelected( half + 1 );
      confirm();

    }
    else if (Input.GetButtonDown( "NextEnemy" )) {
      //EffectSoundController.PLAY_MENU_MOVE();
      SetPageAndRowBySelected( half );
      confirm();
    }

    preConfirm();
  }

  bool preConfirm() {
    if (Input.GetButton( "Confirm" ) || Input.GetButton( "NextUnit" ) || Input.GetButton( "NextEnemy" )) {
      lastTime += Time.deltaTime;
      if (lastTime > maxTime) {
        doConfirm();
        lastTime = 0;
        maxTime = defaultMaxTime;
      }
      return true;
    }
    return false;
  }

  protected override void confirm() {
    lastTime = 0;
    maxTime = defaultMaxTime * 5;
    doConfirm();
    return;
  }

  void doConfirm() {
    //Debug.Log( "IntAdder doConfirm " + DateTime.Now.ToString( "yyyy-MM-dd HH:mm:ss" ) );
    EffectSoundController.PLAY_MENU_MOVE();
    if (countOK) {
      myValue += addComList[currentSelected];

      if (myValue < min) myValue = max;
      else if (myValue > max) myValue = min;

      valueText.text = myValue.ToString();
      next( myValue );
    }
  }

  protected override void closeSelf() {
    if (callback != null) {
      enabled = false;
      callback.Invoke();
    }
  }

  public void SetIndexToHalf() {
    //Debug.Log( "SetIndexToHalf -> menuItemTfList.Count: " + menuItemTfList.Count );
    SetPageAndRowBySelected( half );
  }

}
