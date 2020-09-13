using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using SimpleJSON;

public class Controller : MonoBehaviour {
  private static Controller c;
  public Texture2D[] Cursors;
  float cursorTime = 0;
  Vector2 center32 = new Vector2(32, 32);
  Camera cam;

  public ActorClickHandler ActorPortrait1;
  public ActorClickHandler ActorPortrait2;
  public ActorClickHandler ActorPortrait3;

  public Actor[] actors;
  Actor actor1;
  Actor actor2;
  Actor actor3;
  Actor kidnappedActor;

  Actor currentActor = null;
  Room currentRoom;
  Color32 unselectedActor = new Color32(0x6D, 0x7D, 0x7C, 255);
  Color32 selectedActor = new Color32(200, 232, 152, 255);

  public Room debugr;
  GameStatus status = GameStatus.IntroDialogue;

  int rm = 0; // FIXME remove
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

    actor1 = actors[(int)Chars.Dave];
    actor2 = actors[(int)Chars.Bernard];
    actor3 = actors[(int)Chars.Wendy];

    LoadSequences();
    PickValidSequence();


    currentRoom = debugr;
    StartCoroutine(StartDelayed());
    Cursor.SetCursor(Cursors[(int)CursorTypes.Wait], center32, CursorMode.Auto);
    status = GameStatus.IntroDialogue;
    currentActor = actor1;
    ActorPortrait1.GetComponent<UnityEngine.UI.RawImage>().color = c.selectedActor;
  }

  IEnumerator StartDelayed() {
    yield return new WaitForSeconds(.5f);
    ShowName(currentRoom.RoomName);
  }


  void Update() {
    cursorTime += Time.deltaTime;
    HandleCursor();

    // Handling of text messages
    if (textMsgTime > 0) {
      textMsgTime -= Time.deltaTime;
      switch (txtMsgMode) {
        case TextMsgMode.Appearing:
          TextMsgRT.sizeDelta = new Vector2(textSizeX * 4 * (.25f - textMsgTime), 50);
          TextBackRT.sizeDelta = TextMsgRT.sizeDelta;
          break;
        case TextMsgMode.Disappearing:
          TextMsgRT.sizeDelta = new Vector2(textSizeX * 4 * (textMsgTime), 50);
          TextBackRT.sizeDelta = TextMsgRT.sizeDelta;
          break;
      }
      if (textMsgTime <= 0) {
        switch (txtMsgMode) {
          case TextMsgMode.Appearing:
            txtMsgMode = TextMsgMode.Visible;
            textMsgTime = 3;
            break;
          case TextMsgMode.Disappearing:
            txtMsgMode = TextMsgMode.None;
            TextMsg.text = "";
            break;
          case TextMsgMode.Visible:
            txtMsgMode = TextMsgMode.Disappearing;
            textMsgTime = .25f;
            break;
          case TextMsgMode.None:
            break;
        }
      }
    }


    // Do we have a sequence?
    if (currentSequence != null) {
      // Do we have the action?

      // No action -> We may want to check if we have a sequence, if not get one valid
      // Action not playing
      // Action playing

      if (currentAction == null) {
        currentAction = currentSequence.GetNextAction();
        if (currentAction == null) currentSequence = null;
      }
      else if (currentAction.NotStarted()) {
        Debug.Log(currentAction.ToString());

        if (currentAction.type == ActionType.Teleport) {
          GetActor(currentAction).transform.position = currentAction.pos;
          GetActor(currentAction).SetDirection(currentAction.dir);
          currentAction.Complete();
        }
        else if (currentAction.type == ActionType.Speak) {
          GetActor(currentAction).Say(currentAction.msg, currentAction);
          GetActor(currentAction).SetDirection(currentAction.dir);
          currentAction.Play();
        }
        else if (currentAction.type == ActionType.Expression) {
          GetActor(currentAction).SetDirection(currentAction.dir);
          GetActor(currentAction).SetExpression(currentAction.expr);
          currentAction.Play();
        }
        else {
          // FIXME do the other actions
        }

      }
      else if (currentAction.IsPlaying()) {
        currentAction.AddTime(Time.deltaTime);
      }
      else if (currentAction.IsCompleted()) {
        currentAction = currentSequence.GetNextAction();
        if (currentAction == null) {
          status = GameStatus.NormalGamePlay;
        }
      }
    }



    if (c.status != GameStatus.NormalGamePlay) return;

    // LMB -> Walk or secondary action
    // RMB -> Default action

    if (Input.GetMouseButtonDown(0)) {
      Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
      if (Physics.Raycast(ray, out RaycastHit hit))
        currentActor.WalkTo(hit.point);

    }
    else if (!currentActor.IsWalking() && Input.GetMouseButton(0)) {
      Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
      if (Physics.Raycast(ray, out RaycastHit hit))
        currentActor.WalkTo(hit.point);

    }
    if (Input.GetMouseButtonDown(1)) {
      if (overObject != null) {
        if (overObject.type == ItemType.Readable) {
          // Are we close enough?
          float distx = Mathf.Abs(currentActor.transform.position.x - (overObject.transform.position.x + overObject.InteractionPosition.x));
          float disty = Mathf.Abs(currentActor.transform.position.z - (overObject.transform.position.z + overObject.InteractionPosition.y));
          if (distx > 1 || disty > .5f) { // Need to walk
            Vector3 dst = overObject.transform.position + overObject.InteractionPosition;
            Actor a = currentActor;
            Item o = overObject;
            currentActor.WalkTo(dst, new System.Action(() => {
              a.SetDirection(o.preferredDirection);
              a.Say(o.Description);
            }));
            return;
          }
          else {
            currentActor.SetDirection(overObject.preferredDirection);
            currentActor.Say(overObject.Description);
          }
        }
        else if (overObject.type == ItemType.Openable) {
          // Are we close enough?
          float distx = Mathf.Abs(currentActor.transform.position.x - (overObject.transform.position.x + overObject.InteractionPosition.x));
          float disty = Mathf.Abs(currentActor.transform.position.z - (overObject.transform.position.z + overObject.InteractionPosition.y));
          if (distx > 1 || disty > .5f) { // Need to walk
            Vector3 dst = overObject.transform.position + overObject.InteractionPosition;
            Actor a = currentActor;
            Item o = overObject;
            currentActor.WalkTo(dst, new System.Action(() => {
              a.SetDirection(o.preferredDirection);
              if (o.Open()) a.Say("Is locked");
            }));
            return;
          }
          else {
            currentActor.SetDirection(overObject.preferredDirection);
            if (overObject.Open()) currentActor.Say("Is locked");
          }
        }
        else if (overObject.type == ItemType.Activable) {
          // Are we close enough?
          float distx = Mathf.Abs(currentActor.transform.position.x - (overObject.transform.position.x + overObject.InteractionPosition.x));
          float disty = Mathf.Abs(currentActor.transform.position.z - (overObject.transform.position.z + overObject.InteractionPosition.y));
          if (distx > 1 || disty > .5f) { // Need to walk
            Vector3 dst = overObject.transform.position + overObject.InteractionPosition;
            Actor a = currentActor;
            Item o = overObject;
            currentActor.WalkTo(dst, new System.Action(() => {
              a.SetDirection(o.preferredDirection);
              if (o.Open()) a.Say("Does not work");
            }));
            return;
          }
          else {
            currentActor.SetDirection(overObject.preferredDirection);
            if (overObject.Open()) currentActor.Say("Does not work");
          }
        }
      }




    }

    // Handle camera
    Vector2 cpos = cam.WorldToScreenPoint(currentActor.transform.position);
    if (cpos.x < .2f * Screen.width) {
      if ((currentRoom.axis == Axis.X && cam.transform.position.x > currentRoom.minL) || (currentRoom.axis == Axis.Z && cam.transform.position.z > currentRoom.minL)) { 
        cam.transform.position -= cam.transform.right * Time.deltaTime * (.2f * Screen.width - cpos.x) / 10; 
      }
    }
    if (cpos.x > .8f * Screen.width) {
      if ((currentRoom.axis == Axis.X && cam.transform.position.x < currentRoom.maxR) || (currentRoom.axis == Axis.Z && cam.transform.position.z < currentRoom.maxR)) {
        cam.transform.position += cam.transform.right * Time.deltaTime * (cpos.x - .8f * Screen.width) / 10;
      }
    }


    if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(2)) { // FIXME remove it is just for debug
      currentActor.Say(rndmsg[rm]);
      rm++;
      if (rm >= rndmsg.Length) rm = 0;
    }


    /*
     
    Actions should have a condition. And probably a sequence.

    If the condition is true, as soon there are no actions to do, the sequence should start (conditions are on sequences)
    We may need to do things in parallel with multiple actors



     */


  }

  private CursorTypes forcedCursor = CursorTypes.None;
  private Item overObject = null;
  private Item usedObject = null;
  private Texture2D oldCursor = null;
  void HandleCursor() {
    if (c.status != GameStatus.NormalGamePlay) return;

    if (forcedCursor == CursorTypes.None) {
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
      return;
    }


    if (oldCursor != Cursors[(int)forcedCursor]) {
      Cursor.SetCursor(Cursors[(int)forcedCursor], center32, CursorMode.Auto);
      oldCursor = Cursors[(int)forcedCursor];
    }
  }



  public TextMeshProUGUI TextMsg;
  public RectTransform TextMsgRT;
  public RectTransform TextBackRT;
  TextMsgMode txtMsgMode = TextMsgMode.None;
  float textMsgTime = 0;
  float textSizeX = 0;

  void ShowName(string name) {
    if (TextMsg.text == name) return;
    textSizeX = TextMsg.GetPreferredValues(name).x;
    TextMsgRT.sizeDelta = new Vector2(0, 50);
    TextBackRT.sizeDelta = TextMsgRT.sizeDelta;
    TextMsg.text = name;
    textMsgTime = .25f;
    txtMsgMode = TextMsgMode.Appearing;
  }

  void HideName() {
    if (txtMsgMode == TextMsgMode.Disappearing || txtMsgMode == TextMsgMode.None) return;
    textMsgTime = .25f;
    txtMsgMode = TextMsgMode.Disappearing;
  }


  internal static void SendEventData(PointerEventData eventData, IPointerClickHandler handler) {
    if (c.status != GameStatus.NormalGamePlay) return;
    ActorClickHandler h = (ActorClickHandler)handler;
    if (h == c.ActorPortrait1) {
      c.currentActor = c.actor1;
      c.ShowName("Selected: " + c.actor1.name);
      c.ActorPortrait1.GetComponent<UnityEngine.UI.RawImage>().color = c.selectedActor;
      c.ActorPortrait2.GetComponent<UnityEngine.UI.RawImage>().color = c.unselectedActor;
      c.ActorPortrait3.GetComponent<UnityEngine.UI.RawImage>().color = c.unselectedActor;
    }
    else if (h == c.ActorPortrait2) {
      c.currentActor = c.actor2;
      c.ShowName("Selected: " + c.actor2.name);
      c.ActorPortrait1.GetComponent<UnityEngine.UI.RawImage>().color = c.unselectedActor;
      c.ActorPortrait2.GetComponent<UnityEngine.UI.RawImage>().color = c.selectedActor;
      c.ActorPortrait3.GetComponent<UnityEngine.UI.RawImage>().color = c.unselectedActor;
    }
    else if (h == c.ActorPortrait3) {
      c.currentActor = c.actor3;
      c.ShowName("Selected: " + c.actor3.name);
      c.ActorPortrait1.GetComponent<UnityEngine.UI.RawImage>().color = c.unselectedActor;
      c.ActorPortrait2.GetComponent<UnityEngine.UI.RawImage>().color = c.unselectedActor;
      c.ActorPortrait3.GetComponent<UnityEngine.UI.RawImage>().color = c.selectedActor;
    }
    else if (handler as GroundClickHandler) {
      Debug.Log(" the ground!");
    }
    else
      Debug.LogError("What called us?");
  }

  internal static void SetCurrentItem(Item item) {
    if (c.status != GameStatus.NormalGamePlay) return;
    if (item == null) {
      c.forcedCursor = CursorTypes.None;
      c.overObject = null;
      if (c.TextMsg.text != "") c.HideName();
      return;
    }

    if (item.type == ItemType.Readable) {
      c.forcedCursor = CursorTypes.Examine;
      c.overObject = item;
      c.ShowName("Examine " + item.ItemName);
    }
    else if (item.type == ItemType.Openable) {
      if (item.isOpen) {
        c.forcedCursor = CursorTypes.Close;
        c.ShowName("Close " + item.ItemName);
      }
      else {
        c.forcedCursor = CursorTypes.Open;
        c.ShowName("Open " + item.ItemName);
      }
      c.overObject = item;
    }
    else if (item.type == ItemType.Activable) {
      c.forcedCursor = CursorTypes.Use;
      c.overObject = item;
      c.ShowName((item.isOpen ? "Deactivate " : "Activate ") + item.ItemName);
    }
    else if (item.type == ItemType.Pickable) {
      c.forcedCursor = CursorTypes.PickUp;
      c.overObject = item;
      c.ShowName(item.ItemName);
    }
    else if (item.type == ItemType.Usable) {
      c.forcedCursor = CursorTypes.Use;
      c.overObject = item;
      c.ShowName("Use " + item.ItemName);
    }

  }

  GameSequence currentSequence;
  GameAction currentAction;
  public List<GameSequence> sequences;

  void LoadSequences() {
    string path = Application.dataPath + "/Actions/";

    foreach(string file in System.IO.Directory.GetFiles(path, "*.json")) {
      Debug.Log(file);
      string json = System.IO.File.ReadAllText(file);

      try {
        JSONNode j = JSON.Parse(json);

        GameSequence seq = new GameSequence(j["name"].Value);
        // FIXME conditions
        // Actions
        JSONNode.ValueEnumerator vals = j["actions"].Values;
        foreach (JSONNode val in vals) {
          GameAction a = new GameAction(val["type"].Value);
          if (a.type == ActionType.Teleport) {
            a.SetActor(val["actor"].Value);
            a.SetPos(val["pos"][0].AsFloat, val["pos"][1].AsFloat, val["pos"][2].AsFloat);
            a.SetDir(val["dir"].Value);
          }
          else if (a.type == ActionType.Speak) {
            a.SetActor(val["actor"].Value);
            a.SetOther(val["other"].Value);
            a.SetDir(val["dir"].Value);
            a.SetText(val["msg"].Value);
          }
          else if (a.type == ActionType.Expression) {
            a.SetActor(val["actor"].Value);
            a.SetDir(val["dir"].Value);
            a.SetExpr(val["expr"].Value);
          }
          a.SetWait(val["wait"].AsFloat);
          seq.actions.Add(a);
        }
        sequences.Add(seq);

      } catch (System.Exception e) {
        Debug.Log("ERROR (" + file + "): " + e.Message);
      }
    }

  }

  void PickValidSequence() {
    // Check all conditions and pick a valid sequence. In case there are more pick one random
    currentSequence = sequences[0];
    currentSequence.Start();
    currentAction = currentSequence.GetNextAction();
  }


  private Actor GetActor(GameAction a) {
    if (a.actor == Chars.Actor1) return actor1;
    if (a.actor == Chars.Actor2) return actor2;
    if (a.actor == Chars.Actor3) return actor3;
    return actors[(int)a.actor];
  }



}


