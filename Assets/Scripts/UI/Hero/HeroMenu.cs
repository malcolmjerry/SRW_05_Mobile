using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HeroMenu : SelectableMenu {

  Image photoImg;
  Text firstName, lastName, nickName, sexText, dobText, bloodText, characterText, descText;

  Transform choosePhoto, setFullName, setDob, setBlood, setCharacter;

  Hero hero = null;
  HeroData preData = new HeroData();
  int? denyPicNo;

  private void Awake() {
    menuItemTfList.Add( transform.Find( "Menu/MenuItems/Face" ) );
    menuItemTfList.Add( transform.Find( "Menu/MenuItems/Name" ) );
    menuItemTfList.Add( transform.Find( "Menu/MenuItems/DOB" ) );
    menuItemTfList.Add( transform.Find( "Menu/MenuItems/Blood" ) );
    menuItemTfList.Add( transform.Find( "Menu/MenuItems/Character" ) );
    menuItemTfList.Add( transform.Find( "Menu/MenuItems/Confirm" ) );

    photoImg = transform.Find( "Photo/PhotoImg" ).GetComponent<Image>();

    var personTf = transform.Find( "Person" );
    firstName = personTf.Find( "FirstNameTxt" ).GetComponent<Text>();
    lastName = personTf.Find( "LastNameTxt" ).GetComponent<Text>();
    nickName = personTf.Find( "NickNameTxt" ).GetComponent<Text>();
    sexText = personTf.Find( "SexTxt" ).GetComponent<Text>();
    dobText = personTf.Find( "DobTxt" ).GetComponent<Text>();
    bloodText = personTf.Find( "BloodTxt" ).GetComponent<Text>();
    characterText = personTf.Find( "CharacterTxt" ).GetComponent<Text>();
    descText = transform.Find( "Desc" ).GetComponent<Text>();

    choosePhoto = transform.Find( "ChoosePhoto" );
    choosePhoto.GetComponent<ChoosePhoto>().Setup( OtherCallback, NextFromChoosePhoto );

    setFullName = transform.Find( "SetFullName" );
    setDob = transform.Find( "SetDOB" );
    setBlood = transform.Find( "SetBlood" );
    setCharacter = transform.Find( "SetCharacter" );

    //callback = backToStartMenu;
  }

  // Start is called before the first frame update
  void Start() {
    setupBase( 6, true, false, 1 );

    if (hero == null)
      hero = preData.GetRandomHero();
  }

  public void Setup( Hero hero, int? denyPicNo, string desc, Action callback, Action<dynamic> next ) {
    Setup( callback, next, false );
    this.hero = hero;
    this.denyPicNo = denyPicNo;
    photoImg.sprite = Resources.Load<Sprite>( $"Character/{hero.PicNo}_1" );
    firstName.text = hero.FirstName;
    lastName.text = hero.LastName;
    nickName.text = hero.ShortName;
    sexText.text = hero.GetSexStr();
    dobText.text = $"{hero.Year} 年 {hero.Month} 月 {hero.Day} 日";
    bloodText.text = $"{hero.GetBloodStr()}型";
    characterText.text = $"{hero.GetCharacterStr()}";
    descText.text = desc;
    reset();
  }

  private void OnEnable() {
    UpdateSelectedItem();
  }

  // Update is called once per frame
  protected override void Update() {
    base.Update();
  }

  protected override void confirm() {
    EffectSoundController.PLAY_MENU_CONFIRM();
    //enabled = false;
    //GetComponent<ChooseParts>().enabled = true;
    //menuItemTfList[row - 1].Find( "Name" ).GetComponent<Text>().color = Color.yellow;

    /*
    switch () { 
    
    }
    */
    switch (currentSelected) {
      case 0: toChoosePhoto(); break;
      case 1: toSetFullName(); break;
      case 2: toDOB(); break;
      case 3: toBlood(); break;
      case 4: toCharacter(); break;
      default: toNext(); break;
    }
  }

  void toChoosePhoto() {
    //Debug.Log( "ToChoosePhoto()" );
    enabled = false;
    choosePhoto.gameObject.SetActive( true );
    CoroutineCommon.CallWaitForOneFrame( () => choosePhoto.GetComponent<ChoosePhoto>().Setup( hero.PicNo, denyPicNo ) );
  }

  void NextFromChoosePhoto( dynamic picNo ) {
    hero.PicNo = picNo;
    photoImg.sprite = Resources.Load<Sprite>( $"Character/{picNo}_1" );
    CoroutineCommon.CallWaitForOneFrame( () => enabled = true );
  }

  void toSetFullName() {
    enabled = false;
    setFullName.gameObject.SetActive( true );
    setFullName.GetComponent<SetFullName>().Setup( hero.FirstName, hero.LastName, hero.ShortName, hero.Sex, OtherCallback, NextFromSetName );
  }

  void NextFromSetName( dynamic nameTuple ) {
    hero.FirstName = nameTuple.firstName;
    hero.LastName = nameTuple.lastName;
    hero.ShortName = nameTuple.nickName;
    hero.Sex = nameTuple.sex;
    firstName.text = hero.FirstName;
    lastName.text = hero.LastName;
    nickName.text = hero.ShortName;
    sexText.text = hero.GetSexStr();

    CoroutineCommon.CallWaitForOneFrame( () => enabled = true );
  }

  void toDOB() {
    enabled = false;
    setDob.gameObject.SetActive( true );
    setDob.GetComponent<SetDOB>().Setup( hero.Year, hero.Month, hero.Day, OtherCallback, NextFromSetDOB );
  }

  void NextFromSetDOB( dynamic nameTuple ) {
    hero.Year = nameTuple.year;
    hero.Month = nameTuple.month;
    hero.Day = nameTuple.day;
    dobText.text = $"{hero.Year} 年 {hero.Month} 月 {hero.Day} 日";
    CoroutineCommon.CallWaitForOneFrame( () => enabled = true );
  }

  void toBlood() {
    enabled = false;
    setBlood.gameObject.SetActive( true );
    setBlood.GetComponent<SetByIndex>().Setup( hero.Blood, maxRow: 2, OtherCallback, NextFromSetBlood );
  }

  void NextFromSetBlood( dynamic blood ) {
    hero.Blood = blood; 
    bloodText.text = $"{hero.GetBloodStr()}型";
    CoroutineCommon.CallWaitForOneFrame( () => enabled = true );
  }

  void toCharacter() {
    enabled = false;
    setCharacter.gameObject.SetActive( true );
    setCharacter.GetComponent<SetByIndex>().Setup( hero.Character, maxRow: 6, OtherCallback, NextFromSetCharaccter );
  }

  void NextFromSetCharaccter( dynamic value ) {
    hero.Character = value;
    characterText.text = $"{hero.GetCharacterStr()}";
    CoroutineCommon.CallWaitForOneFrame( () => enabled = true );
  }

  void toNext() {
    enabled = false;
    next( hero );
  }

  /*
  void backToStartMenu() {
    var myCanvas = GameObject.Find( "MyCanvas" );
    GameObject.Find( "MyCanvas" ).transform.Find( "ConfirmDialog" ).GetComponent<ConfirmDialog>().Setup(
       "退回到主選單, 當前編輯的信息將會丟失",
       () => myCanvas.GetComponent<FadeInOut>().FadeOut( 1f,
              () => {
                SceneManager.UnloadSceneAsync( SceneManager.GetActiveScene() );
                SceneManager.LoadScene( "Title", LoadSceneMode.Additive );
              }
            ),
       () => { enabled = true; }   //Cancel
    );
  }
  */
}
