using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HelpDesc : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

  private FontStyle fontStyle;
  public string Desc = "";
  private bool isShowing;

  private void Awake() {
    fontStyle = GetComponent<Text>().fontStyle;
  }

  public void OnPointerEnter( PointerEventData eventData ) {
    if (string.IsNullOrWhiteSpace( Desc ))
      return;

    Vector2 viewportPositionUnit = Camera.main.ScreenToViewportPoint( transform.position );
    MyCanvas.SHOW_HELP_DESC( Desc, viewportPositionUnit.y <= 0.5f );
    GetComponent<Text>().fontStyle = FontStyle.Bold;
    isShowing = true;
  }

  public void OnPointerExit( PointerEventData eventData ) {
    if (string.IsNullOrWhiteSpace( Desc ))
      return;
    MyCanvas.HIDE_HELP_DESC();
    GetComponent<Text>().fontStyle = fontStyle;
    isShowing = false;
  }

  void OnDisable() {   
    if (string.IsNullOrWhiteSpace( Desc ))
      return;
    if (isShowing == true) {
      MyCanvas.HIDE_HELP_DESC();
      GetComponent<Text>().fontStyle = fontStyle;
      isShowing = false;
    }
    
  }


}
