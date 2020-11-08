using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CursorHandler : MonoBehaviour {
  public Image Cursor;
  public Image CursorL;
  public Image CursorR;
  public static CursorHandler me;

  RectTransform crt;
  RectTransform crtl;
  RectTransform crtr;
  float phase = 0;
  Color32 color;
  CursorTypes onLeft = CursorTypes.Normal;
  CursorTypes onRight = CursorTypes.Normal;
  CursorTypes prevCentral = CursorTypes.Normal;

  private void Awake() {
    me = this;
  }

  private void Start() {
    crt = Cursor.GetComponent<RectTransform>();
    crtl = CursorL.GetComponent<RectTransform>();
    crtr = CursorR.GetComponent<RectTransform>();
    UnityEngine.Cursor.lockState = CursorLockMode.None;
    UnityEngine.Cursor.visible = false;
    color = new Color32(255, 255, 255, 255);
  }

  private void Update() {
    Vector2 mouse = Input.mousePosition;
    crt.position = mouse;
    mouse.y += 32;
    mouse.x -= 40;
    crtl.position = mouse;
    mouse.x += 80;
    crtr.position = mouse;

    phase += Time.deltaTime;
    if (phase > 1.5f) phase -= 1.5f;
    float val = (Mathf.Sin(4.195f * phase + 1.57f) + 1.75f) * .37f;
    val = Mathf.Clamp(255 * val, 64, 255);
    color.r = (byte)val;
    color.g = (byte)val;
    color.b = (byte)val;

    Cursor.color = color;

    CursorL.enabled = (onLeft != onRight && onLeft != CursorTypes.Normal);
    CursorR.enabled = (onLeft != onRight && onRight != CursorTypes.Normal);
    if (onLeft == onRight && prevCentral != onLeft && onLeft != CursorTypes.Object && onLeft != CursorTypes.Give) {
      prevCentral = onLeft;
      Cursor.sprite = Cursors[(int)prevCentral];
    }
  }

  CursorTypes savedL;
  CursorTypes savedR;

  public static void SaveCursor() {
    me.savedL = me.onLeft;
    me.savedR = me.onRight;
  }
  public static void ResetCursor() {
    me.onLeft=me.savedL;
    me.onRight= me.savedR;
    me.prevCentral = me.onLeft;
  }

  public static void SetLeft(CursorTypes c, bool notIfObject = false) {
    if (notIfObject && me.prevCentral == CursorTypes.Object) return;
    if (me.onLeft == c) return;
    me.onLeft = c;
    me.CursorL.sprite = me.Cursors[(int)c];
  }

  public static void SetRight(CursorTypes c, bool notIfObject = false) {
    if (notIfObject && me.prevCentral == CursorTypes.Object) return;
    if (me.onRight == c) return;
    me.onRight = c;
    me.CursorR.sprite = me.Cursors[(int)c];
  }

  public static void SetBoth(CursorTypes c, bool notIfObject = false) {
    if (notIfObject && me.prevCentral == CursorTypes.Object) return;
    me.onLeft = c;
    me.onRight = c;
  }


  public Sprite[] Cursors;

  internal static void SetObject(Sprite cursorImage) {
    if (cursorImage != null) {
      SetBoth(CursorTypes.Object);
      me.Cursor.sprite = cursorImage;
    }
    else {
      SetBoth(CursorTypes.Normal);
      me.Cursor.sprite = me.Cursors[0];
    }
  }

  internal static void SoftCleanObject() {
    if (me.prevCentral == CursorTypes.Object) {
      SetBoth(CursorTypes.Normal);
      me.Cursor.sprite = me.Cursors[0];
    }
  }


  internal static bool IsItemCursor() {
    return me.prevCentral == CursorTypes.Object;
  }
}

public enum CursorTypes {
  Normal=0,
  Wait=1,
  Object=2,
  Read=3,
  Open=4,
  Close=5,
  On=6,
  Off=7,
  Use=8,
  WalkAwayL=9,
  WalkAwayR=10,
  WalkAwayU=11,
  WalkAwayD=12,
  WalkAwayI=13,
  WalkAwayO=14,
  Give=15,
  Pick=16
}