using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityORM;

[Serializable]
public class GameData {

  [Key( AutoIncrement = false )]
  public int SaveSlot;

  public long Money;

  public long HistoryMoney;

  public int Turns = 1;

  public int HistoryTurns = 1;

  public int Flags;

  public int Chapter;  //表面顯示的話數

  public int Stage;    //實際通關的版號

  public string SaveTime;

  //之後可能需要優化, 把個人資料抽出

  //MaxLength = 10
  private string heroName = "";
  public string HeroName { 
    set { heroName = value == null? "" : value.Substring( 0, Math.Min( value.Length, 10 ) ); }
    get { return heroName; }
  }

  //public string HeroFirst;
  //public string HeroLast;

  private int heroLv;
  public int HeroLv {
    set { heroLv = Math.Min( value, 99 ); }
    get { return heroLv; }
  }

  private int heroKills;
  public int HeroKills {
    set { heroKills = Math.Min( value, 99999999 ); }
    get { return heroKills; }
  }

  //MaxLength = 10
  // string Herorine;
  private string herorine = "";
  public string Herorine {
    set { herorine = value == null? "" : value.Substring( 0, Math.Min( value.Length, 10 ) ); }
    get { return herorine; }
  }

  //public string HerorineFirst;
  //public string HerorineLast;


}
