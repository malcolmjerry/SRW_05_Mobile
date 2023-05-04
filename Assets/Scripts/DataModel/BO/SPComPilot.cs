using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Serializable]
public class SPComPilot {

  public int PilotID;

  public int SPComID;
  public SPCommand SPCommand { set; get; }

  public int Level;

  public int SP;

}

