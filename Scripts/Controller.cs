using System;
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
  
  
  
    if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(1)) {
      Balloon.Show(rndmsg[rm], bernard.transform);
      rm++;
      if (rm >= rndmsg.Length) rm = 0;
    }
  
  }

  void HandleCursor() {
    if (0 <= cursorTime && cursorTime <= .5f) {
      Cursor.SetCursor(Cursors[0], center32, CursorMode.Auto);
    }
    else if (.5f < cursorTime && cursorTime <= .75f) {
      Cursor.SetCursor(Cursors[1], center32, CursorMode.Auto);
    }
    else if (.75f < cursorTime && cursorTime <= .9f) {
      Cursor.SetCursor(Cursors[2], center32, CursorMode.Auto);
    }
    else if (.9f < cursorTime && cursorTime <= 1.05f) {
      Cursor.SetCursor(Cursors[1], center32, CursorMode.Auto);
    }
    else {
      cursorTime = 0;
      Cursor.SetCursor(Cursors[0], center32, CursorMode.Auto);
    }
  }



  internal static void SendEventData(PointerEventData eventData) {
    c.debugObj.transform.position = eventData.position;
  }

  internal static void SetCurrentItem(Item item) {
    if (item == null) return;

    if (item.type == ItemType.Readable) {
      Balloon.Show(item.description, c.bernard.transform);
    }

  }



}


public enum ItemType { None, Readable };