using System;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveCtl : MonoBehaviour {

  public Text Top1, Top2, Top3, Mid1, Mid2, Mid3, Bot1, Bot2, Bot3;
  private Action callback;
  public bool IsEdited;

  private void Update() {
    if (Input.GetButtonDown( "Back" ) || Input.GetButtonDown( "Cancel" )) {
      EffectSoundController.PLAY_BACK_CANCEL();
      gameObject.SetActive( false );
      callback?.Invoke();
    }
    else if (Input.GetButtonDown( "Confirm" )) {
      EffectSoundController.PLAY_MENU_CONFIRM();
      gameObject.SetActive( false );
      callback?.Invoke();
    }
  }

  public void SetData( ObjectiveData objData, bool isEdited = false ) {
    this.IsEdited = isEdited;
    SetTop( objData.WinStrings );
    SetMid( objData.LoseStrings );
    SetBot( objData.HintStrings );
  }

  public void Setup( Action callback ) {
    this.callback = callback;
    gameObject.SetActive( true );
    IsEdited = false;
  }

  public void SetTop( string[] topStrings ) {
    if (topStrings == null)
      return;
    Top1.text = topStrings.Length > 0? topStrings[0] : "";
    Top2.text = topStrings.Length > 1? topStrings[1] : "";
    Top3.text = topStrings.Length > 2? topStrings[2] : "";
  }

  public void SetMid( string[] midStrings ) {
    if (midStrings == null)
      return;
    Mid1.text = midStrings.Length > 0? midStrings[0] : "";
    Mid2.text = midStrings.Length > 1? midStrings[1] : "";
    Mid3.text = midStrings.Length > 2? midStrings[2] : "";
  }

  public void SetBot( string[] botStrings ) {
    if (botStrings == null)
      return;
    Bot1.text = botStrings.Length > 0? botStrings[0] : "";
    Bot2.text = botStrings.Length > 1? botStrings[1] : "";
    Bot3.text = botStrings.Length > 2? botStrings[2] : "";
  }

}
