using UnityEngine;

public class TalkParam {

  //public Pilot Pilot = null;
  //public Hero Hero = null;

  public TalkParam( Pilot pilot, Vector3? defaultPos = null ) {
    //Pilot = pilot;
    //Hero = hero;
    PilotID = pilot.ID;
    PicId = pilot.ID;
    ShortName = pilot.ShortName;
    DefaultPosition = defaultPos;
  }

  public TalkParam( Hero hero, Vector3? defaultPos = null ) {
    //Pilot = pilot;
    //Hero = hero;
    HeroSeqNo = hero.SeqNo;
    PicId = hero.PicNo;
    ShortName = hero.ShortName;
    DefaultPosition = defaultPos;
  }

  public TalkParam( int picId, string shortName, Vector3? defaultPos = null ) {
    PicId = picId;
    ShortName = shortName;
    Position = defaultPos;
  }

  public int? PilotID = null;

  public int? HeroSeqNo = null;

  public int PicId;

  public string ShortName;

  public int FaceNo = 1;

  public string Say1 = "";

  public string Say2 = "";

  public string Say3 = "";

  public bool? UpDown = null;

  public Vector3? DefaultPosition = null;

  public Vector3? Position = null;

  //public int? PosByPilotId = null;

  public float Waiting = 0;

  public bool NoDisplay = false;

  public string BGM = null;

  //public bool IsPlace = false;

}

