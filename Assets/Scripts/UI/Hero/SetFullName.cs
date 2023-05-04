using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetFullName : SelectableMenu {

  Text firstNameTxt, lastNameTxt, nickNameTxt;

  public MyInputField myInputField;

  int sex;
  List<Transform> sexTfs = new List<Transform>();
  bool isSexMode = false;

  private void Awake() {
    Transform firstNameTf, lastNameTf, nickNameTf;

    menuItemTfList.Add( firstNameTf = transform.Find( "FirstNameRow" ) );
    menuItemTfList.Add( lastNameTf = transform.Find( "LastNameRow" ) );
    menuItemTfList.Add( nickNameTf = transform.Find( "NickNameRow" ) );
    menuItemTfList.Add( transform.Find( "SexRow" ) );
    menuItemTfList.Add( transform.Find( "ConfirmBtn" ) );

    firstNameTxt = firstNameTf.Find( "FirstNameTxt" ).GetComponent<Text>();
    lastNameTxt = lastNameTf.Find( "LastNameTxt" ).GetComponent<Text>();
    nickNameTxt = nickNameTf.Find( "NickNameTxt" ).GetComponent<Text>();

    sexTfs.Add( transform.Find( "SexRow/Item1" ) );
    sexTfs.Add( transform.Find( "SexRow/Item2" ) );
    sexTfs.Add( transform.Find( "SexRow/Item3" ) );
  }

  // Start is called before the first frame update
  void Start() {
    setupBase( 5, false, true );
  }

  public void Setup( string firstName, string lastName, string nickName, int sex, Action callback, Action<dynamic> next = null ) {
    Setup( callback, next );
    firstNameTxt.text = firstName;
    lastNameTxt.text = lastName;
    nickNameTxt.text = nickName;
    this.sex = sex;
    updateSexSelected();
    reset();
  }

  protected override void Update() {
    base.Update();

    if (isSexMode) {
      if (Input.GetButtonDown( "Left" )) {
        EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/menuMove" ), 3 );
        addSex( -1 );
      }
      else if (Input.GetButtonDown( "Right" )) {
        EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/menuMove" ), 3 );
        addSex( 1 );
      }
      updateSexSelected();
    }

    if (Input.GetButtonDown( "Start" )) {
      EffectSoundController.PLAY_MENU_CONFIRM();
      toNext();
    }
  }

  protected override void confirm() {
    EffectSoundController.PLAY_MENU_CONFIRM();

    switch (currentSelected) {
      case 0: //firstname
        enabled = false;
        myInputField.gameObject.SetActive( true );
        myInputField.Setup( "名字", 13, firstNameTxt.text, OtherCallback, txt => NextFromInputField( 0, txt ) );
        break;
      case 1: //lastname
        enabled = false;
        myInputField.gameObject.SetActive( true );
        myInputField.Setup( "姓氏", 12, lastNameTxt.text, OtherCallback, txt => NextFromInputField( 1, txt ) );
        break;
      case 2: //nickname
        enabled = false;
        myInputField.gameObject.SetActive( true );
        myInputField.Setup( "愛稱", 12, nickNameTxt.text, OtherCallback, txt => NextFromInputField( 2, txt ) );
        break;
      case 3: //Sex
        toSex();
        break;
      case 4:
        toNext();
        break;
      default:
        break;
    }


  }

  void toSex() {
    //addSex( 1 );
  }

  void toNext() {
    gameObject.SetActive( false );
    next?.Invoke( new {
      firstName = firstNameTxt.text,
      lastName = lastNameTxt.text,
      nickName = nickNameTxt.text,
      sex
    } );
  }

  protected override void UpdateSelectedMore() {
    isSexMode = currentSelected == 3;
  }

  void NextFromInputField( int index, string inputText ) {
    Text target;
    if (index == 0)
      target = firstNameTxt;
    else if (index == 1)
      target = lastNameTxt;
    else
      target = nickNameTxt;

    target.text = inputText;

    enabled = true;
  }

  void addSex( int add ) {
    sex = sex + add;
    if (sex < 1) sex = 3;
    else if (sex > 3) sex = 1;
  }
  
  void updateSexSelected() {
    sexTfs.ForEach( tf => tf.Find( "Selected" ).gameObject.SetActive( false ) );
    sexTfs[sex-1].Find( "Selected" ).gameObject.SetActive( true );
  }

}
