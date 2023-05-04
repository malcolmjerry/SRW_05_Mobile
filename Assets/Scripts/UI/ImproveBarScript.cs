using UnityEngine;
using UnityEngine.UI;

public class ImproveBarScript : MonoBehaviour {

  public Color32 FillColor;
  public Color32 PreColor;
  public Color32 EmptyColor;

  public static int MAX_LV = 10;
  private int level;

  // Use this for initialization
  void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

  public void SetLevel( int lv ) {
    level = lv;
    for ( int i=1; i<=MAX_LV; i++) {
      transform.Find( $"Fill{i}" ).GetComponent<Image>().color = i <= lv? FillColor : EmptyColor;
    }
  }

  public void SetPreLevel( int preLv ) {
    for (int i = level+1; i<=level+preLv; i++) {
      if (i > MAX_LV)
        break;
      transform.Find( $"Fill{i}" ).GetComponent<Image>().color = PreColor;
    }
  }


}
