using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class MyInputField : MonoBehaviour {

  public InputField m_InputField;
  public Text Desc;
  public Text Title;

  int maxChar = 13;
  Action callback;
  Action<string> next;

  void Start() {

  }

  // Update is called once per frame
  void Update() {
    if (Input.GetButtonDown( "Start" ))
      PressConfirmBtn();
  }

  public void Setup( string title, int maxChar, string defaultStr, Action callback, Action<string> next ) {
    this.maxChar = maxChar;
    this.callback = callback;
    this.next = next;
    m_InputField.text = defaultStr;

    m_InputField.ActivateInputField();
    CoroutineCommon.CallWaitForOneFrame( () => m_InputField.MoveTextEnd( false ) );
    Title.text = title;
    Desc.text = $"(最長 {maxChar} 位, 英數以外佔 2 位)";
  }

  public void CheckLen() {
    int len = m_InputField.text.Length;
    int actualLen = 0;

    //Debug.Log( $"m_InputField.text: {m_InputField.text}" );

    foreach (var c in m_InputField.text) {
      char _char = c;
      if (_char == '　')
        _char = ' ';
      if (!Regex.IsMatch( _char.ToString(), "[A-Za-z1-9'_ \\!\\@\\#\\$\\%\\^\\&\\*\\(\\)\\-\\=\\[\\]\\{\\}_\\+\"\\<\\>\\,\\.\\/\\?`~\\\\;:]" ))
        actualLen += 2;
      else actualLen++;
    }

    if (actualLen > maxChar)
      m_InputField.text = m_InputField.text.Substring( 0, len - 1 );
  }

  public void PressConfirmBtn() {
    EffectSoundController.PLAY_MENU_CONFIRM();
    //Debug.Log( $"Confirmed: {m_InputField.text}" );
    gameObject.SetActive( false );
    next( m_InputField.text );
  }

  public void EndEdit() {
    //Debug.Log( "End Edit" );
    m_InputField.text = m_InputField.text.Trim();
    Regex regex = new Regex( "\\s{2,}" );
    m_InputField.text = regex.Replace( m_InputField.text, " " );
  }

}
