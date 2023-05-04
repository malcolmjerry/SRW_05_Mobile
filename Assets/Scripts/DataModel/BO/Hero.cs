using UnityEngine;
using System.Collections;
using System;
using UnityORM;
using System.Collections.Generic;

[Serializable]
public class Hero {

  [Key]
  public int SaveSlot;

  [Key]
  public int SeqNo;

  public string ShortName;

  public string FirstName;

  public string LastName;

  public int Year;
   
  public int Month;
  
  public int Day;

  public int Sex;

  public int Character;

  public int Blood;

  public int PicNo;

  public string GetSexStr() {
    switch (Sex) {
      case 1: return "男";
      case 2: return "女";
      default: return "其他";
    }
  }

  public string GetBloodStr() {
    switch (Blood) {
      case 0: return "A+";
      case 1: return "A-";
      case 2: return "B+";
      case 3: return "B-";
      case 4: return "AB+";
      case 5: return "AB-";
      case 6: return "O+";
      case 7: return "O-";
      default: return "其他";
    }
  }

  public string GetCharacterStr() { 
    switch (Character) {
      case 0: return "認真、溫柔、熱血漢";  //超級
      case 1: return "雖是理論家但喜歡異性、頭腦清晰";  //真實
      case 2: return "冷酷且虛無主義、喜歡獨來獨往";   //平均
      case 3: return "有點奇怪的性格、有潔癖且執着";  //平均
      case 4: return "內向且優柔、沉着冷靜";    //真實
      default: return "樂觀且活力充沛、富正義感";  //超級
    }
  }

  public int GetPilotIdByCharacter() {
    switch (Character) {
      case 0: return 2204;  //超級
      case 1: return 2205;  //真實
      case 2: return 2203;   //平均
      case 3: return 2203;  //平均
      case 4: return 2205;    //真實
      default: return 2204;  //超級
    }
  }

    public int GetSubPilotIdByCharacter() {
    switch (Character) {
      case 0: return 9937;  //超級
      case 1: return 9938;  //真實
      case 2: return 9936;   //平均
      case 3: return 9936;  //平均
      case 4: return 9938;    //真實
      default: return 9937;  //超級
    }
  }
}
