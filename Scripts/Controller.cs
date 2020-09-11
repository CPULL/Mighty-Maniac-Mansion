using UnityEngine;
using UnityEngine.EventSystems;

public class Controller : MonoBehaviour {
  private static Controller c;
  public Texture2D[] Cursors;
  float cursorTime = 0;
  Vector2 center32 = new Vector2(32, 32);
  Camera cam;

  public Actor bernard;


  public GameObject debugObj;
  public Collider2D walkable;

  int rm = 0;
  string[] rndmsg = {
    "I am Bernard",
    "Small",
    "A very long\nmessage, that should\ngo on three lines.",
    "One\nTwo\nThree\nFour",
    "One One One One One One \nTwo Two Two Two Two Two \nThree Three Three Three Three \nFour Four Four Four Four Four\nFive"
  };


  private void Awake() {
    c = this;
    cam = Camera.main;
  }

  void Update() {
    cursorTime += Time.deltaTime;
    HandleCursor();


    // LMB -> Walk or secondary action
    // RMB -> Default action

    if (Input.GetMouseButtonDown(0)) {
      // Check intersection?
      Vector3 worldPoint = cam.ScreenToWorldPoint(Input.mousePosition);
      if (walkable.OverlapPoint(worldPoint)) {
        worldPoint.z = 0;
        bernard.WalkTo(worldPoint);
      }
      else
        Debug.Log("Outside");

    }
    if (Input.GetMouseButtonDown(1)) {
      if (overObject != null && overObject.type == ItemType.Readable) {
        bernard.Say(overObject.description);
      }
    }
  
  
    if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(2)) {
      bernard.Say(rndmsg[rm]);
      rm++;
      if (rm >= rndmsg.Length) rm = 0;
    }
  
  }

  private CursorTypes forcedCursor = CursorTypes.None;
  private Item overObject = null;
  private Item usedObject = null;
  private Texture2D oldCursor = null;
  void HandleCursor() {
    if (forcedCursor == CursorTypes.Examine) {
      if (oldCursor != Cursors[3]) {
        Cursor.SetCursor(Cursors[3], center32, CursorMode.Auto);
        oldCursor = Cursors[3];
      }
      return;
    }

    if (0 <= cursorTime && cursorTime <= .5f) {
      if (oldCursor != Cursors[0]) {
        Cursor.SetCursor(Cursors[0], center32, CursorMode.Auto);
        oldCursor = Cursors[0];
      }
    }
    else if (.5f < cursorTime && cursorTime <= .75f) {
      if (oldCursor != Cursors[1]) {
        Cursor.SetCursor(Cursors[1], center32, CursorMode.Auto);
        oldCursor = Cursors[1];
      }
    }
    else if (.75f < cursorTime && cursorTime <= .9f) {
      if (oldCursor != Cursors[2]) {
        Cursor.SetCursor(Cursors[2], center32, CursorMode.Auto);
        oldCursor = Cursors[2];
      }
    }
    else if (.9f < cursorTime && cursorTime <= 1.05f) {
      if (oldCursor != Cursors[1]) {
        Cursor.SetCursor(Cursors[1], center32, CursorMode.Auto);
        oldCursor = Cursors[1];
      }
    }
    else {
      cursorTime = 0;
      if (oldCursor != Cursors[0]) {
        Cursor.SetCursor(Cursors[0], center32, CursorMode.Auto);
        oldCursor = Cursors[0];
      }
    }
  }



  internal static void SendEventData(PointerEventData eventData) {
    c.debugObj.transform.position = eventData.position;
  }

  internal static void SetCurrentItem(Item item) {
    if (item == null) {
      c.forcedCursor = CursorTypes.None;
      c.overObject = null;
      return;
    }

    if (item.type == ItemType.Readable) {
      c.forcedCursor = CursorTypes.Examine;
      c.overObject = item;
    }

  }



}


