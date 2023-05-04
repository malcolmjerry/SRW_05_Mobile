using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HeroData {

  public List<Hero> HeroList = new List<Hero> {
    new Hero() { FirstName = "イルムガルト", LastName = "カザハラ", ShortName = "イルム", Blood = 0, Month = 11, Day = 10, Year = 1997, Character = 1, Sex = 1, PicNo = 20001 },
    new Hero() { FirstName = "リン", LastName = "マオ", ShortName = "リン", Blood = 2, Month = 4, Day = 14, Year = 1997, Character = 2, Sex = 2, PicNo = 20002 },
    new Hero() { FirstName = "レナンジェス", LastName = "スターロード", ShortName = "ジェス", Blood = 6, Month = 7, Day = 23, Year = 1997, Character = 0, Sex = 1, PicNo = 20003 },
    new Hero() { FirstName = "ミーナ", LastName = "ライクリング", ShortName = "ミーナ", Blood = 6, Month = 9, Day = 21, Year = 1997, Character = 3, Sex = 2, PicNo = 20004 },
    new Hero() { FirstName = "アーウィン", LastName = "ドースティン", ShortName = "ウィン", Blood = 2, Month = 3, Day = 1, Year = 1997, Character = 2, Sex = 1, PicNo = 20005 },
    new Hero() { FirstName = "グレース", LastName = "ウリジン", ShortName = "グレース", Blood = 4, Month = 1, Day = 31, Year = 1997, Character = 1, Sex = 2, PicNo = 20006 },
    new Hero() { FirstName = "アサキム", LastName = "ドーウィン", ShortName = "アサキム", Blood = 5, Month = 6, Day = 6, Year = 2007, Character = 2, Sex = 1, PicNo = 20007 },
    new Hero() { FirstName = "セツコ", LastName = "オハラ", ShortName = "セツコ", Blood = 0, Month = 9, Day = 3, Year = 2007, Character = 4, Sex = 2, PicNo = 20008 }
  };

  public Hero GetRandomHero( int? sex = null, int? picNo = null ) {
    var heroListQ = HeroList.AsQueryable();
    if (sex.HasValue)
      heroListQ = heroListQ.Where( h => h.Sex != sex );

    if (picNo.HasValue)
      heroListQ = heroListQ.Where( h => h.PicNo != picNo );

    List<Hero> list = heroListQ.ToList();

    int randomIndex = new System.Random().Next( 0, list.Count ); 

    return list[randomIndex];
  }

  public Hero GetLover( int picNo ) {
    switch (picNo) {
      case 1: return HeroList[1];
      case 2: return HeroList[0];
      case 3: return HeroList[3];
      case 4: return HeroList[2];
      case 5: return HeroList[5];
      case 6: return HeroList[4];
      case 7: return HeroList[7];
      case 8: return HeroList[6];
      default: return HeroList[0];
    }
  }

}
