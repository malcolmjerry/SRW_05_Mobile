using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoosePhoto : SelectableMenu {

  //[HideInInspector] public string HeroPicName;

  int? denyPicSeq = null;

  private void Awake() {
    menuItemTfList.Add( transform.Find( "Items1" ) );
    menuItemTfList.Add( transform.Find( "Items2" ) );
    menuItemTfList.Add( transform.Find( "Items3" ) );
    menuItemTfList.Add( transform.Find( "Items4" ) );
    menuItemTfList.Add( transform.Find( "Items5" ) );
    menuItemTfList.Add( transform.Find( "Items6" ) );
    menuItemTfList.Add( transform.Find( "Items7" ) );
    menuItemTfList.Add( transform.Find( "Items8" ) );
  }

  // Start is called before the first frame update
  void Start() {
    setupBase( 8, false, true, 2 );
  }

  public void Setup( int picNo, int? denyPicNo ) {
    int picSeq = picNo - 20000;
    if (picSeq <= menuItemTfList.Count)
      SetPageAndRowBySelected( picSeq - 1 );

    if (denyPicNo.HasValue) {
      denyPicSeq = denyPicNo.Value - 20000;
      menuItemTfList.ForEach( m => m.Find( "deny" ).gameObject.SetActive( false ) );   
      menuItemTfList[denyPicSeq.Value - 1].Find( "deny" ).gameObject.SetActive( true );
    }
  }

  protected override void Update() {
    base.Update();
  }

  protected override void confirm() {
    if (currentSelected == (denyPicSeq - 1)) {
      EffectSoundController.PLAY_ACTION_FAIL();
      return;
    }

    EffectSoundController.PLAY_MENU_CONFIRM();
    gameObject.SetActive( false );
    next?.Invoke( currentSelected + 1 + 20000 );

  }

}

