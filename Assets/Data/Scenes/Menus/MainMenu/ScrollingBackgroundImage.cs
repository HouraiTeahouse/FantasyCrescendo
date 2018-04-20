using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HouraiTeahouse.FantasyCrescendo.UI {

public class ScrollingBackgroundImage : MonoBehaviour {

  public RawImage image;
  public Vector2 ScrollSpeed;

	// Update is called once per frame
	void Update () {
    if (image == null) return;
    var rect = image.uvRect;
    rect.position += ScrollSpeed * Time.deltaTime;
    image.uvRect = rect;
	}

}

}