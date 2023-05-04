using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SetDOB : SelectableMenu {

  IntAdder yearAdder, monthAdder, dayAdder;
  //MonthController monthController;
  //DayController dayController;

  int year = 0, month, day;

  private void Awake() {
    Transform yearTf, monthTf, dayTf;

    menuItemTfList.Add( yearTf = transform.Find( "YearRow" ) );
    menuItemTfList.Add( monthTf = transform.Find( "MonthRow" ) );
    menuItemTfList.Add( dayTf = transform.Find( "DayRow" ) );
    menuItemTfList.Add( transform.Find( "ConfirmBtn" ) );

    yearAdder = yearTf.GetComponent<IntAdder>();
    monthAdder = monthTf.GetComponent<IntAdder>();
    dayAdder = dayTf.GetComponent<IntAdder>();
  }

  void Start() {
    setupBase( 4, false, true );
  }

  private void OnEnable() {
    //Debug.Log( "SetDOB OnEnable()" );
    //yearAdder.enabled = true;
    yearAdder.SetIndexToHalf();
    monthAdder.SetIndexToHalf();
    dayAdder.SetIndexToHalf();

  }

  public void Setup( int year, int month, int day, Action callback, Action<dynamic> next = null ) {
    //Debug.Log( "SetDOB Setup" );
    Setup( callback, next );
    this.year = year;
    this.month = month;
    this.day = day;
    //yearTxt.text = year.ToString();
    reset();

    yearAdder.Setup(
      new List<int> { -10, -1, 1, 10 },
      0, 99999, year,
      () => { },
      result => {
        this.year = result;
        updateDayList();
      }
    );
    monthAdder.Setup(
      new List<int> { -1, 1 },
      1, 12, month,
      () => { },
      result => {
        this.month = result;
        updateDayList();
      }
    );
    updateDayList();
  }

  protected override void Update() {
    base.Update();

    if (Input.GetButtonDown( "Start" )) {
      toNext();
    }
  }

  protected override void confirm() {
    //Debug.Log( "SetDOB currentSelected: " + currentSelected );
    if (currentSelected == 3)
      toNext();
  }

  protected override void UpdateSelectedMore() {
    disableAllMode();

    if (currentSelected == 0) {
      yearAdder.enabled = true;
    }
    else if (currentSelected == 1) {
      monthAdder.enabled = true;
    }
    else if (currentSelected == 2) {
      dayAdder.enabled = true;
    }
  }

  void disableAllMode() {
    yearAdder.enabled = false;
    monthAdder.enabled = false;
    dayAdder.enabled = false;
  }

  void toNext() {
    EffectSoundController.PLAY_MENU_CONFIRM();
    gameObject.SetActive( false );
    next?.Invoke( new { year, month, day } );
  }

  /*
  void AddYear( dynamic add ) {
    //Debug.Log( "SetDOB AddYear " + DateTime.Now.ToString( "yyyy-MM-dd HH:mm:ss" ) );
    //Debug.Log( "Year " + year );
    year += add;

    if (year < 0) year = 0;
    else if (year > 99999) year = 99999;

    yearTxt.text = year.ToString();
  }
  */

  void updateDayList() {
    //Debug.Log( "updateDayList -> month: " + month );
    int maxDay;
    if (new int[] { 1, 3, 5, 7, 8, 10, 12 }.Contains( month ))
      maxDay = 31;
    else if (new int[] { 4, 6, 9, 11 }.Contains( month ))
      maxDay = 30;
    else {
      bool isLeap = year % 4 == 0 && year % 100 != 0 || year % 400 == 0 && year % 3200 != 0 || year % 80000 == 0;
      maxDay = isLeap? 29 : 28;
    }
    if (day > maxDay) day = maxDay;
    //Debug.Log( "maxDay: " + maxDay );
    dayAdder.Setup(
      new List<int> { -1, 1 },
      1, maxDay, day,
      () => { },
      result => day = result
    );

  }
}
