using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class FadeInOut : MonoBehaviour {

  private RawImage rawImg;

  private bool isFadeIn;

	// Use this for initialization
	void Start () {
    rawImg = transform.Find( "RawImage" ).GetComponent<RawImage>();
  }
	
	// Update is called once per frame
	void Update () {
		
	}

  public void FullOut() {
    rawImg.gameObject.SetActive( true );
    rawImg.color = new Color( rawImg.color.r, rawImg.color.g, rawImg.color.b, 1 );
  }

  public void FullIn( float sec = 0 ) {
    CoroutineCommon.CallWaitForSeconds( sec, () => {
      rawImg.color = new Color( rawImg.color.r, rawImg.color.g, rawImg.color.b, 0 );
      rawImg.gameObject.SetActive( false );
    } );
  }

  public void FadeIn( float sec, Action callcack ) {
    Debug.Log( "FadeIn()" );
    rawImg.gameObject.SetActive( true );
    isFadeIn = true;
    rawImg.color = new Color( rawImg.color.r, rawImg.color.g, rawImg.color.b, 1f );

    CoroutineCommon.CallWaitForSeconds( 1f, () => {
      StartCoroutine( doFadeIn( sec, callcack ) );
    } );
  }

  public void FadeOut( float sec, Action callback, Action doProcess = null, float blackTime = 0f, bool hold = false ) {
    isFadeIn = hold;
    rawImg.gameObject.SetActive( true );
    rawImg.color = new Color( rawImg.color.r, rawImg.color.g, rawImg.color.b, 0f );
    StartCoroutine( doFadeOut( sec, callback, doProcess, blackTime ) );
  }

  private IEnumerator doFadeIn( float sec, Action callback ) {
    float startTime = Time.time;

    while (Time.time < startTime + sec) {
      float alpha = 1 - (Time.time - startTime) / sec;
      rawImg.color = new Color( rawImg.color.r, rawImg.color.g, rawImg.color.b, alpha );
      yield return null;
    }

    rawImg.color = new Color( rawImg.color.r, rawImg.color.g, rawImg.color.b, 0 );
    rawImg.gameObject.SetActive( false );
    callback();
  }

  private IEnumerator doFadeOut( float sec, Action callback, Action doProcess = null, float blackTime = 1f ) {
    Task task = null;
    if (doProcess != null)
      task = Task.Run( doProcess );

    float startTime = Time.time;

    while (Time.time < startTime + sec) {
      float alpha = (Time.time - startTime) / sec;
      rawImg.color = new Color( rawImg.color.r, rawImg.color.g, rawImg.color.b, alpha );
      yield return null;
    }
    rawImg.color = new Color( rawImg.color.r, rawImg.color.g, rawImg.color.b, 1 );

    if (task != null) {
      while (!task.IsCompleted) {
        yield return null;
      }
    }
    
    CoroutineCommon.CallWaitForSeconds( blackTime, () => {
      callback();
      CoroutineCommon.CallWaitForOneFrame( () => { AfterFadeOut(); } );
    } );

  }

  public void AfterFadeOut() {
    if (!isFadeIn)
      rawImg.gameObject.SetActive( false );
  }

  public string Display() {
    return $"rawImg.gameObject.active: {rawImg.gameObject.activeSelf} color: {rawImg.color} ";
  }
}

