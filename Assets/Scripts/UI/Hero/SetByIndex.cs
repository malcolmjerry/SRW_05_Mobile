using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetByIndex : SelectableMenu {

  void Start() {
  }

  public void Setup( int index, int maxRow, Action callback, Action<dynamic> next = null ) {
    Setup( callback, next );
    setupBase( menuItemTfList.Count, false, true, maxRow );
    SetPageAndRowBySelected( index );
  }

  protected override void Update() {
    base.Update();

    if (Input.GetButtonDown( "Start" )) {
      confirm();
    }
  }

  protected override void confirm() {
    EffectSoundController.PLAY_MENU_CONFIRM();
    gameObject.SetActive( false );
    next?.Invoke( currentSelected );
  }

}
