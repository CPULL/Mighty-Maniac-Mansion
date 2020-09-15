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

  public PortraitClickHandler ActorPortrait1;
  public PortraitClickHandler ActorPortrait2;
  public PortraitClickHandler ActorPortrait3;

  public Actor[] actors;
  Actor actor1;
  Actor actor2;
  Actor actor3;
  readonly Actor kidnappedActor;

  Actor currentActor = null;
  Room currentRoom;
  Color32 unselectedActor = new Color32(0x6D, 0x7D, 0x7C, 255);
  Color32 selectedActor = new Color32(200, 232, 152, 255);

  public Room debugr;
  GameStatus status = GameStatus.IntroDialogue;


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

  public LayerMask pathLayer;
  public UnityEngine.UI.Image BlackFade;

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
// FIXME        Debug.Log(currentAction.ToString());

        if (currentAction.type == ActionType.Teleport) {
          Actor a = GetActor(currentAction);
          a.transform.position = currentAction.pos;
          a.SetDirection(currentAction.dir);
          RaycastHit2D hit = Physics2D.Raycast(currentAction.pos, cam.transform.forward, 10000, pathLayer);
          if (hit.collider != null) {
            Path p = hit.collider.GetComponent<Path>();
            a.WalkTo(currentAction.pos, currentAction.dir, p);
          }
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


    // Handle camera
    Vector2 cpos = cam.WorldToScreenPoint(currentActor.transform.position);
    if (cam.transform.position.x < currentRoom.minL) {
      Vector3 p = cam.transform.position;
      p.x = currentRoom.minL;
      cam.transform.position = p;
    }
    else if (cam.transform.position.x > currentRoom.maxR) {
      Vector3 p = cam.transform.position;
      p.x = currentRoom.maxR;
      cam.transform.position = p;
    }
    else if (cpos.x < .3f * Screen.width) {
      if (cam.transform.position.x > currentRoom.minL) {
        cam.transform.position -= cam.transform.right * Time.deltaTime * (.3f * Screen.width - cpos.x) / 10;
      }
    }
    else if (cpos.x > .7f * Screen.width) {
      if (cam.transform.position.x < currentRoom.maxR) {
        cam.transform.position += cam.transform.right * Time.deltaTime * (cpos.x - .7f * Screen.width) / 10;
      }
    }



    if (c.status != GameStatus.NormalGamePlay) return;

    // LMB -> Walk or secondary action
    // RMB -> Default action

    if (Input.GetMouseButtonDown(0)) {
      RaycastHit2D hit = Physics2D.Raycast(cam.ScreenToWorldPoint(Input.mousePosition), cam.transform.forward, 10000, pathLayer);
      if (hit.collider != null) {
        Path p = hit.collider.GetComponent<Path>();

        // FIXME We need to check if we may need to follow a series of paths (A*-ish?)

        currentActor.WalkTo(hit.point, CalculateDirection(hit.point), p);
      }

    }
    else if (currentActor.IsWalking() && Input.GetMouseButton(0) && (overObject == null || overObject.type != ItemType.Stairs)) {
// FIXME continous walking
    }
    if (Input.GetMouseButtonDown(1)) {
      if (overObject != null) {
        if (overObject.type == ItemType.Readable) {
          ActionSay(currentActor, overObject);
        }
        else if (overObject.type == ItemType.Openable) {
          ActionOpen(currentActor, overObject);
        }
        else if (overObject.type == ItemType.Activable) {
          ActionActivate(currentActor, overObject);
        }
        else if (overObject.type == ItemType.Walkable) { // This should be Open/Close, normal mouse to walk, and msg if it is closed
          ActionChangeRoom(currentActor, overObject);
        }
      }
    }



    /*
     
    Actions should have a condition. And probably a sequence.

    If the condition is true, as soon there are no actions to do, the sequence should start (conditions are on sequences)
    We may need to do things in parallel with multiple actors



     */


  }



  Dir CalculateDirection(Vector3 point) {
    Vector3 ap = cam.WorldToScreenPoint(currentActor.transform.position);
    Vector3 mp = cam.WorldToScreenPoint(point);

    float dx = ap.x - mp.x;
    float dy = ap.y - mp.y;

    // Vert or Horiz?
    if (Mathf.Abs(dx) * 1.2f < Mathf.Abs(dy)) {
      // Vert
      if (dy < 0) return Dir.B;
      return Dir.F;
    }
    else {
      // Horiz
      if (dx < 0) return Dir.R;
      return Dir.L;
    }
  }




  void ActionSay(Actor actor, Item item) {
    Vector3 one = actor.transform.position;
    one.y = 0;
    Vector3 two = item.InteractionPosition;
    two.y = 0;
    float dist = Vector3.Distance(one, two);
    if (dist > 1) { // Need to walk
      RaycastHit2D hit = Physics2D.Raycast(overObject.InteractionPosition, cam.transform.forward, 10000, pathLayer);
      if (hit.collider != null) {
        Path p = hit.collider.GetComponent<Path>();
        currentActor.WalkTo(overObject.InteractionPosition, CalculateDirection(overObject.InteractionPosition), p, new System.Action(() => {
          actor.SetDirection(item.preferredDirection);
          actor.Say(item.Description);
        }));
      }
      return;
    }
    else {
      actor.SetDirection(item.preferredDirection);
      actor.Say(item.Description);
    }
  }
  void ActionOpen(Actor actor, Item item) {
    Vector3 one = actor.transform.position;
    one.y = 0;
    Vector3 two = item.InteractionPosition;
    two.y = 0;
    float dist = Vector3.Distance(one, two);
    if (dist > 1) { // Need to walk
      RaycastHit2D hit = Physics2D.Raycast(overObject.InteractionPosition, cam.transform.forward, 10000, pathLayer);
      if (hit.collider != null) {
        Path p = hit.collider.GetComponent<Path>();
        currentActor.WalkTo(overObject.InteractionPosition, CalculateDirection(overObject.InteractionPosition), p, new System.Action(() => {
          actor.SetDirection(item.preferredDirection);
          if (item.Open()) actor.Say("Is locked");
        }));
      }
      return;
    }
    else {
      actor.SetDirection(item.preferredDirection);
      if (item.Open()) actor.Say("Is locked");
    }
  }
  void ActionActivate(Actor actor, Item item) {
    Vector3 one = actor.transform.position;
    one.y = 0;
    Vector3 two = item.InteractionPosition;
    two.y = 0;
    float dist = Vector3.Distance(one, two);
    if (dist > 1) { // Need to walk
      RaycastHit2D hit = Physics2D.Raycast(overObject.InteractionPosition, cam.transform.forward, 10000, pathLayer);
      if (hit.collider != null) {
        Path p = hit.collider.GetComponent<Path>();
        currentActor.WalkTo(overObject.InteractionPosition, CalculateDirection(overObject.InteractionPosition), p, new System.Action(() => {
          actor.SetDirection(item.preferredDirection);
          if (item.Open()) actor.Say("Does not work");
        }));
      }
      return;
    }
    else {
      actor.SetDirection(item.preferredDirection);
      if (item.Open()) actor.Say("Does not work");
    }
  }
  void ActionChangeRoom(Actor actor, Item item) {
    Vector3 one = actor.transform.position;
    one.y = 0;
    Vector3 two = item.InteractionPosition;
    two.y = 0;
    float dist = Vector3.Distance(one, two);
    if (dist > 1) { // Need to walk
      RaycastHit2D hit = Physics2D.Raycast(overObject.InteractionPosition, cam.transform.forward, 10000, pathLayer);
      if (hit.collider != null) {
        Path p = hit.collider.GetComponent<Path>();
        currentActor.WalkTo(overObject.InteractionPosition, CalculateDirection(overObject.InteractionPosition), p, new System.Action(() => {
          if (!item.isOpen && item.Open()) {
            actor.Say("Is locked");
            return;
          }
          StartCoroutine(ChangeRoom(actor, (item as Door)));
        }));
      }
      return;
    }
    else {
      if (!item.isOpen && item.Open()) {
        actor.Say("Is locked");
        return;
      }
      StartCoroutine(ChangeRoom(actor, (item as Door)));
    }
  }

  private CursorTypes forcedCursor = CursorTypes.None;
  private Item overObject = null;
  // FIXME private Item usedObject = null;
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


  internal static void SendEventData(IPointerClickHandler handler) {
    if (c.status != GameStatus.NormalGamePlay) return;
    PortraitClickHandler h = (PortraitClickHandler)handler;
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
      Debug.Log(" the ground!"); // Not used
    }
    else
      Debug.LogError("What called us?");
  }

  internal static void SendActorEventData(Actor actor, bool click) {
    c.ShowName(actor.name);
    if (c.status != GameStatus.NormalGamePlay || !click) return;
    if (actor == c.actor1) {
      c.currentActor = c.actor1;
      c.ShowName("Selected: " + c.actor1.name);
      c.ActorPortrait1.GetComponent<UnityEngine.UI.RawImage>().color = c.selectedActor;
      c.ActorPortrait2.GetComponent<UnityEngine.UI.RawImage>().color = c.unselectedActor;
      c.ActorPortrait3.GetComponent<UnityEngine.UI.RawImage>().color = c.unselectedActor;
    }
    else if (actor == c.actor2) {
      c.currentActor = c.actor2;
      c.ShowName("Selected: " + c.actor2.name);
      c.ActorPortrait1.GetComponent<UnityEngine.UI.RawImage>().color = c.unselectedActor;
      c.ActorPortrait2.GetComponent<UnityEngine.UI.RawImage>().color = c.selectedActor;
      c.ActorPortrait3.GetComponent<UnityEngine.UI.RawImage>().color = c.unselectedActor;
    }
    else if (actor == c.actor3) {
      c.currentActor = c.actor3;
      c.ShowName("Selected: " + c.actor3.name);
      c.ActorPortrait1.GetComponent<UnityEngine.UI.RawImage>().color = c.unselectedActor;
      c.ActorPortrait2.GetComponent<UnityEngine.UI.RawImage>().color = c.unselectedActor;
      c.ActorPortrait3.GetComponent<UnityEngine.UI.RawImage>().color = c.selectedActor;
    }
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
    else if (item.type == ItemType.Walkable) {
      c.forcedCursor = CursorTypes.None;
      c.overObject = item;
      c.ShowName(item.ItemName);
    }
    else if (item.type == ItemType.Stairs) {
      c.forcedCursor = CursorTypes.None;
      c.overObject = item;
    }

  }


  internal static void OverDoor(Door door) {
    // Highlight
    // Set currentItem
    // Show open/close cursor
    SetCurrentItem(door);
  }

  GameSequence currentSequence;
  GameAction currentAction;
  public List<GameSequence> sequences;

  void LoadSequences() {
    string path = Application.dataPath + "/Actions/";

    foreach(string file in System.IO.Directory.GetFiles(path, "*.json")) {
//FIXME Debug.Log(file);
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
            a.SetPos(val["pos"][0].AsFloat, val["pos"][1].AsFloat, val["pos"][1].AsFloat);
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


  private IEnumerator ChangeRoom(Actor actor, Door door) {
    // Disable gameplay
    status = GameStatus.RoomTransition;
    yield return null;

    // Enable dst
    door.dst.gameObject.SetActive(true);

    // Get dst camera pos + door dst pos
    Vector3 orgp = cam.transform.position;
    Vector3 dstp = door.camerapos;

    // Move camera quickly from current to dst
    float time = 0;
    while (time < .125f) {
      // Fade black
      BlackFade.color = new Color32(0, 0, 0, (byte)(255 * (time * 8)));
      cam.transform.position = (1 - time * 4) * orgp + (time * 4) * dstp;
      time += Time.deltaTime;
      yield return null;
    }
    actor.transform.position = door.correspondingDoor.InteractionPosition;
    yield return null;
    while (time < .25f) {
      // Fade black
      BlackFade.color = new Color32(0, 0, 0, (byte)(255 * (1 - (8 * (time - .125f)))));
      cam.transform.position = (1 - time * 4) * orgp + (time * 4) * dstp;
      time += Time.deltaTime;
      yield return null;
    }
    cam.transform.position = dstp;

    // Disable src
    door.src.gameObject.SetActive(false);
    actor.Stop();

    // Move actor to dst door pos
    currentRoom = door.dst;
    currentActor = actor;
    actor.transform.position = door.correspondingDoor.InteractionPosition;
    actor.currentRoom = currentRoom;
    yield return null;

    // Disable actors not in current room
    foreach (Actor a in actors) {
      if (a == null) continue;
      a.gameObject.SetActive(a.currentRoom == currentRoom);
    }

    // Enable gmaeplay
    status = GameStatus.NormalGamePlay;
  }



}


