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
  CursorTypes onCenter = CursorTypes.Normal;

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
    if (UnityEngine.Cursor.visible) return;
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
  }

  CursorTypes savedL;
  CursorTypes savedR;
  CursorTypes savedC;

  bool waitMode = false;
  public static void WaitMode(bool wait) {
    if (me.waitMode == wait) return;
    me.waitMode = wait;
    if (wait) {
      me.CursorL.enabled = false;
      me.CursorR.enabled = false;
      me.Cursor.sprite = me.Cursors[(int)CursorTypes.Wait];
    }
    else
      me.Cursor.sprite = me.Cursors[(int)CursorTypes.Normal];
  }

  public static void SaveCursor() {
    me.savedL = me.onLeft;
    me.savedR = me.onRight;
    me.savedC = me.onCenter;
    UnityEngine.Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    UnityEngine.Cursor.visible = true;
    me.crt.gameObject.SetActive(false);
    me.crtl.gameObject.SetActive(false);
    me.crtr.gameObject.SetActive(false);
  }
  public static void ResetCursor() {
    me.onLeft = me.savedL;
    me.onRight = me.savedR;
    me.onCenter = me.savedC;
    UnityEngine.Cursor.visible = false;
    me.crt.gameObject.SetActive(true);
    me.crtl.gameObject.SetActive(true);
    me.crtr.gameObject.SetActive(true);
  }


  public static void Set(CursorTypes l = CursorTypes.Normal, CursorTypes r = CursorTypes.Normal, Item item = null) {
    System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace();
    string str = "";
    int num = st.FrameCount;
    if (num > 3) num = 3;
    for (int i = 1; i < num; i++) {
      var sf = st.GetFrame(i);
      str += sf.GetFileName() + ": " + sf.GetMethod() + ": " + sf.GetFileLineNumber() + "\n";
    }

    if (l == CursorTypes.Normal) {
      me.CursorL.enabled = false;
      me.onLeft = CursorTypes.Normal;
    }
    else {
      me.CursorL.enabled = true;
      if (me.onLeft != l) {
        me.onLeft = l;
        me.CursorL.sprite = me.Cursors[(int)l];
      }
    }

    if (r == CursorTypes.Normal) {
      me.CursorR.enabled = false;
      me.onRight = CursorTypes.Normal;
    }
    else {
      me.CursorR.enabled = true;
      if (me.onRight != r) {
        me.onRight = r;
        me.CursorR.sprite = me.Cursors[(int)r];
      }
    }

    if (l == r) {
      if (item == null || item.iconImage == null) {
        me.CursorL.enabled = false;
        me.CursorR.enabled = false;
        me.onCenter = l;
        me.Cursor.sprite = me.Cursors[(int)l];
        me.onCenter = CursorTypes.Normal;
      }
      else {
        if (me.Cursor.sprite != item.iconImage) {
          me.Cursor.sprite = item.iconImage;
          me.onCenter = CursorTypes.Object;
        }
      }
    }
    else {
      if (item != null && item.iconImage != null) {
        if (me.Cursor.sprite != item.iconImage) {
          me.Cursor.sprite = item.iconImage;
          me.onCenter = CursorTypes.Object;
        }
      }
      else if (me.Cursor.sprite != me.Cursors[0]) {
        me.Cursor.sprite = me.Cursors[0];
        me.onCenter = CursorTypes.Normal;
      }
    }
  }


  public Sprite[] Cursors;

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