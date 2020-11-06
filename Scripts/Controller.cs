using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Controller : MonoBehaviour {
  [HideInInspector] public Camera cam;
  public LayerMask pathLayer;
  public LayerMask doorLayer;
  public Image BlackFade;
  public Transform PickedItems;
  public AudioSource MusicPlayer;


  public TextMeshProUGUI DbgMsg;
  public static void Dbg(string txt) {
    GD.c.DbgMsg.text = txt;
  }

  #region *********************** Mouse and Interaction *********************** Mouse and Interaction *********************** Mouse and Interaction ***********************

  void Update() {
    if (Options.IsActive()) return;
    if (GD.status == GameStatus.StartGame) {
      StartGame();
      return;
    }
    if (GD.status != GameStatus.NormalGamePlay && GD.status != GameStatus.Cutscene) return;

    HandleCursor();

    #region Handling of text messages
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
            textMsgTime = 3 * textSpeed;
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
    #endregion

    #region Sequences and actions
    if (currentCutscene != null) { // Do we have a sequence?
      FrontActors.enabled = !currentCutscene.skippable && currentCutscene.status != GameSceneStatus.ShutDown;
      if (currentCutscene.Run(null, null)) {
        GD.status = GameStatus.Cutscene;
        if (currentCutscene.status == GameSceneStatus.ShutDown) {
          SceneSkipped = true;
        }
      }
      else { // Completed
        Debug.Log("Completed cutscene " + currentCutscene.ToString());
        currentCutscene = null;
        SceneSkipped = false;
        GD.status = GameStatus.NormalGamePlay;
        forcedCursor = CursorTypes.Normal;
        oldCursor = null;
        if (currentActor.currentRoom != currentRoom) StartCoroutine(FadeToRoomActor());
      }
    }
    #endregion

    #region Handle camera
    if (!CameraFadingToActor && CameraPanningInstance == null && (currentCutscene == null || SceneSkipped)) {
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
      else if (cpos.x < .4f * Screen.width) {
        if (cam.transform.position.x > currentRoom.minL) {
          cam.transform.position -= cam.transform.right * Time.deltaTime * (.4f * Screen.width - cpos.x) / 10;
        }
      }
      else if (cpos.x > .6f * Screen.width) {
        if (cam.transform.position.x < currentRoom.maxR) {
          cam.transform.position += cam.transform.right * Time.deltaTime * (cpos.x - .6f * Screen.width) / 10;
        }
      }
    }
    #endregion

    if (GD.status != GameStatus.NormalGamePlay && !SceneSkipped) return;
    FrontActors.enabled = false;
    if (currentActor.currentRoom != currentRoom) StartCoroutine(FadeToRoomActor());

    if (Input.GetKeyUp(KeyCode.Alpha1)) {
      SelectActor(GD.c.actor1);
    }
    else if (Input.GetKeyUp(KeyCode.Alpha2)) {
      SelectActor(GD.c.actor2);
    }
    else if (Input.GetKeyUp(KeyCode.Alpha3)) {
      SelectActor(GD.c.actor3);
    }
    else if (Input.GetKeyUp(KeyCode.I)) {
      if (GD.c.Inventory.activeSelf) { // Show/Hide inventory of current actor
        GD.c.Inventory.SetActive(false);
        GD.c.InventoryPortrait.GetComponent<RawImage>().color = new Color32(0x6D, 0x7D, 0x7C, 0xff);
        return;
      }
      else
        GD.c.ActivateInventory(GD.c.currentActor);
    }



    #region Mouse control
    bool notOverUI = !EventSystem.current.IsPointerOverGameObject();
    bool lmb = Input.GetMouseButtonDown(0);
    bool rmb = Input.GetMouseButtonDown(1);


    Door aDoor = null;
    if ((lmb || rmb) && walkDelay < 0 && notOverUI) { // Check if we have a door
      RaycastHit2D hit = Physics2D.Raycast(cam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 10000, doorLayer);
      if (hit.collider != null && hit.collider.gameObject != null) {
        Door d = hit.collider.gameObject.GetComponent<Door>();
        if (d != null) {
          aDoor = d;
        }
        Item i = hit.collider.gameObject.GetComponent<Item>();
        if (i != null) {
          overItem = i;
        }
      }
    }

    walkDelay -= Time.deltaTime;
    if (notOverUI && Input.GetMouseButton(0) && currentActor.IsWalking() && walkDelay < 0) {
      walkDelay = .25f;
      RaycastHit2D hit = Physics2D.Raycast(cam.ScreenToWorldPoint(Input.mousePosition), cam.transform.forward, 10000, pathLayer);
      if (hit.collider != null) {
        PathNode p = hit.collider.GetComponent<PathNode>();
        currentActor.WalkToPos(hit.point, p);
      }
    }

    if (!lmb && !rmb) return;

    if (overInventoryItem != null) {
      if (usedItem == overInventoryItem) {
        if (lmb) { /* lmb - remove used */
          forcedCursor = CursorTypes.Normal;
          oldCursor = null;
          usedItem = null;
          EnableActorSelection(false);
        }
        else { /* rmb - read */
          if (overInventoryItem.HasActions(When.Use)) {
            ActionRes res = overInventoryItem.PlayActions(currentActor, null, When.Use, null);
            if (res == null || res.actionDone)
              currentActor.Say(overInventoryItem.Description); // By default read what is written in the description of the object
            else
              currentActor.Say(res.res);
          }
          else {
            string msg = overInventoryItem.Description.Replace("%open", overInventoryItem.GetOpenStatus());
            currentActor.Say(msg);
          }
        }
      }
      else if (usedItem == null) {
        if (lmb) { /* lmb - set as used */
          usedItem = overInventoryItem;
          EnableActorSelection(true);
          overInventoryItem = null;
          oldCursor = null;
          forcedCursor = CursorTypes.Item;
          Cursor.SetCursor(usedItem.cursorImage, new Vector2(usedItem.cursorImage.width / 2, usedItem.cursorImage.height / 2), CursorMode.Auto);
        }
        else { /* rmb - use immediately */
          string res = overInventoryItem.Use(currentActor);
          if (!string.IsNullOrEmpty(res)) currentActor.Say(res);
        }

      }
      else {
        if (lmb) { /* lmb - Swap */
          usedItem = overInventoryItem;
          EnableActorSelection(true);
          overInventoryItem = null;
          oldCursor = null;
          forcedCursor = CursorTypes.Item;
          Cursor.SetCursor(usedItem.cursorImage, new Vector2(usedItem.cursorImage.width / 2, usedItem.cursorImage.height / 2), CursorMode.Auto);
        }
        else { /* rmb - Use together */
          // Can we use the two items together?
          string res = usedItem.UseTogether(currentActor, overInventoryItem);
          if (!string.IsNullOrEmpty(res)) currentActor.Say(res);
          UpdateInventory();
          forcedCursor = CursorTypes.Normal;
          oldCursor = null;
          usedItem = null;
          EnableActorSelection(false);
          Inventory.SetActive(false);
          return;
        }
      }

    }
    else if (notOverUI && overItem != null) {
      if (usedItem == overItem) { // Not possible
      }
      else if (usedItem == null) {
        if ((lmb && overItem.whatItDoesL == WhatItDoes.Read) || (rmb && overItem.whatItDoesR == WhatItDoes.Read)) { /* read */
          WalkAndAction(currentActor, overItem,
            new System.Action<Actor, Item>((actor, item) => {
              actor.SetDirection(item.dir);
              if (item.HasActions(When.Use)) {
                ActionRes res = item.PlayActions(currentActor, null, When.Use, null);
                if (res == null || res.actionDone)
                  actor.Say(item.Description); // By default read what is written in the description of the object
                else
                  actor.Say(res.res);
              }
              else {
                string msg = item.Description.Replace("%open", item.GetOpenStatus());
                actor.Say(msg);
              }
            }));
        }

        else if ((lmb && overItem.whatItDoesL == WhatItDoes.Pick) || (rmb && overItem.whatItDoesR == WhatItDoes.Pick)) { /* pick */
          // If we have a container prefer it as hotspot
          WalkAndAction(currentActor, overItem,
            new System.Action<Actor, Item>((actor, item) => {
              if (item.Usable == Tstatus.Pickable) {
                // Do we have a container?
                Container c = item.transform.parent.GetComponent<Container>();
                if (c != null) c.Collect(item, currentActor);
                ShowName(currentActor + " got " + item.Name);
                if (!actor.inventory.Contains(item))
                  actor.inventory.Add(item);
                item.transform.parent = PickedItems;
                item.gameObject.SetActive(false);
                item.owner = Chars.None;
                if (actor == actor1) item.owner = Chars.Actor1;
                else if (actor == actor2) item.owner = Chars.Actor2;
                else if (actor == actor3) item.owner = Chars.Actor3;
                item.PlayActions(currentActor, null, When.Pick, null);
                item = null;
                forcedCursor = CursorTypes.Normal;
                if (Inventory.activeSelf) ActivateInventory(currentActor);
              }
              else {
                // Just run the actions, the item is special
                ActionRes msg = item.PlayActions(currentActor, null, When.Pick, null);
                if (msg != null && !msg.actionDone) currentActor.Say(msg.res);
              }
            }));
          overItem = null;
        }

        else if ((lmb && overItem.whatItDoesL == WhatItDoes.Use) || (rmb && overItem.whatItDoesR == WhatItDoes.Use)) { /* use */
          WalkAndAction(currentActor, overItem,
            new System.Action<Actor, Item>((actor, item) => {
              if (item == null) {
                Debug.Log("Null item in callback");
                forcedCursor = CursorTypes.Normal;
                oldCursor = null;
                return;
              }
              actor.SetDirection(item.dir);
              string res = item.Use(currentActor);
              if (!string.IsNullOrEmpty(res))
                actor.Say(res);
              else {
                forcedCursor = CursorTypes.Normal;
                oldCursor = null;
              }
            }));
        }

        else if ((lmb && overItem.whatItDoesL == WhatItDoes.Walk) || (rmb && overItem.whatItDoesR == WhatItDoes.Walk)) { /* walk */
          if (aDoor == null) {
            WalkAndAction(currentActor, overItem, null);
          }
          else {
            WalkAndAction(currentActor, aDoor,
              new System.Action<Actor, Item>((actor, item) => {
                if (!item.IsOpen() && item.IsLocked()) {
                  actor.Say("It is locked");
                  return;
                }
                if (!item.IsOpen()) return;
                StartCoroutine(ChangeRoom(actor, (item as Door)));
              }));
          }
        }
      }
      else {
        if (lmb) { /* lmb Walk */
          WalkAndAction(currentActor, overItem, null);
        }
        else { /* rmb - Use together */
          WalkAndAction(currentActor, overItem,
            new System.Action<Actor, Item>((actor, item) => {
              // Can we use the two items together?
              string res = usedItem.UseTogether(currentActor, item);
              if (!string.IsNullOrEmpty(res)) currentActor.Say(res);
              UpdateInventory();
              forcedCursor = CursorTypes.Normal;
              oldCursor = null;
              usedItem = null;
              EnableActorSelection(false);
              Inventory.SetActive(false);
              return;
            }));
        }
      }
    }
    else if (notOverUI) {
      if (lmb) {
        if (!currentActor.IsWalking()) { /* lmb - walk */
          if (aDoor != null) {
            WalkAndAction(currentActor, aDoor,
              new System.Action<Actor, Item>((actor, item) => {
                if (!item.IsOpen() && item.IsLocked()) {
                  actor.Say("Is locked");
                  return;
                }
                StartCoroutine(ChangeRoom(actor, (item as Door)));
              }));
            return;
          }

          RaycastHit2D hit = Physics2D.Raycast(cam.ScreenToWorldPoint(Input.mousePosition), cam.transform.forward, 10000, pathLayer);
          if (hit.collider != null) {
            PathNode p = hit.collider.GetComponent<PathNode>();
            currentActor.WalkToPos(hit.point, p);
            walkDelay = 0;
          }
        }
        else { /* rmb - do nothing but unselect used items */
          forcedCursor = CursorTypes.Normal;
          oldCursor = null;
          usedItem = null;
          EnableActorSelection(false);
        }
      }
    }


    if (notOverUI && overActor != null) {
      if (rmb && usedItem != null) {
        if (currentActor == overActor) return;
        receiverActor = overActor;
        usedItem.Give(currentActor, receiverActor);
        Inventory.SetActive(false);
        usedItem = null;
        EnableActorSelection(false);
        oldCursor = null;
        forcedCursor = CursorTypes.Normal;
        return;
      }
    }

    #endregion
  }

  private CursorTypes forcedCursor = CursorTypes.Normal;
  private Texture2D oldCursor = null;
  public Texture2D[] Cursors;
  float cursorTime = 0;
  float cursorTimeSpeed = 1;
  private Vector2 middle = new Vector2(32, 32);

  void HandleCursor() {
    if (GD.status != GameStatus.NormalGamePlay && !SceneSkipped) {
      if (oldCursor != Cursors[(int)CursorTypes.Wait]) {
        Cursor.SetCursor(Cursors[(int)CursorTypes.Wait], middle, CursorMode.Auto);
        oldCursor = Cursors[(int)CursorTypes.Wait];
      }
      return;
    }

    if (forcedCursor == CursorTypes.Item) return;

    cursorTime += Time.deltaTime * cursorTimeSpeed;
    if (cursorTime > 1.5f) {
      cursorTime = 0;
      cursorTimeSpeed = Random.Range(.9f, 1.5f);
    }


    if (forcedCursor == CursorTypes.Normal) {
      float val = Mathf.Cos(cursorTime * 2.1f + 4.7f) * 4.9f;
      val = Mathf.Clamp(val, 0, 4);
      int c = 4 - Mathf.RoundToInt(val);
      if (oldCursor != Cursors[c]) {
        Cursor.SetCursor(Cursors[c], middle, CursorMode.Auto);
        oldCursor = Cursors[c];
      }
      return;
    }

    if (oldCursor != Cursors[(int)forcedCursor]) {
      Cursor.SetCursor(Cursors[(int)forcedCursor], new Vector2(Cursors[(int)forcedCursor].width / 2, Cursors[(int)forcedCursor].height / 2), CursorMode.Auto);
      oldCursor = Cursors[(int)forcedCursor];
    }
  }

  internal static CursorTypes GetCursor() {
    Cursor.SetCursor(GD.c.Cursors[0], new Vector2(GD.c.Cursors[0].width / 2, GD.c.Cursors[0].height / 2), CursorMode.Auto);
    return GD.c.forcedCursor;
  }


  internal static void SetCursor(CursorTypes cur) {
    GD.c.forcedCursor = cur;
    GD.c.oldCursor = null;
  }


  internal static void HandleToolbarClicks(IPointerClickHandler handler) {
    if (Options.IsActive()) return;
    if (GD.status != GameStatus.NormalGamePlay && (GD.c.currentCutscene == null || (!GD.c.currentCutscene.skippable && GD.c.currentCutscene.status != GameSceneStatus.ShutDown))) return;

    PortraitClickHandler h = (PortraitClickHandler)handler;
    if (h == GD.c.ActorPortrait1) {
      SelectActor(GD.c.actor1);
    }
    else if (h == GD.c.ActorPortrait2) {
      SelectActor(GD.c.actor2);
    }
    else if (h == GD.c.ActorPortrait3) {
      SelectActor(GD.c.actor3);
    }
    else if (h == GD.c.InventoryPortrait) {
      if (GD.c.Inventory.activeSelf) { // Show/Hide inventory of current actor
        GD.c.Inventory.SetActive(false);
        GD.c.InventoryPortrait.GetComponent<RawImage>().color = new Color32(0x6D, 0x7D, 0x7C, 0xff);
        return;
      }
      else
        GD.c.ActivateInventory(GD.c.currentActor);
    }
  }


  #endregion


  #region *********************** Initialization

  private void Awake() {
    GD.c = this;
    cam = Camera.main;
  }

  private void Start() {
    GD.LoadGameScenes();

    foreach (Room r in GD.a.roomsList) {
      r.gameObject.SetActive(false);
    }
    foreach (Actor a in allEnemies)
      if (a != null) a.SetVisible(false);
    foreach (Actor a in allActors)
      if (a != null) a.SetVisible(false);
    ActorsButtons.SetActive(false);

    currentRoom = GD.a.roomsList[0];
    currentActor = actor1;
    ActorPortrait1.GetComponent<RawImage>().color = selectedActor;

    Options.GetOptions();
    GD.ReadyToStart();
  }

  void StartGame() {
    foreach (Room r in GD.a.roomsList) {
      r.gameObject.SetActive(r == currentRoom);
    }
    GD.status = GameStatus.NormalGamePlay;
    foreach (Actor a in allActors) {
      if (a == null) continue;
      a.SetVisible(a.currentRoom == currentRoom);
    }
    foreach (Actor a in allEnemies) {
      if (a == null) continue;
      a.SetVisible(a.currentRoom == currentRoom);
    }
    SkyBackground.enabled = true;

    actor1 = GetActor(GD.actor1);
    ActorPortrait1.portrait.sprite = GetActorPortrait(GD.actor1);
    actor1.Player();
    actor2 = GetActor(GD.actor2);
    ActorPortrait2.portrait.sprite = GetActorPortrait(GD.actor2);
    actor2.Player();
    actor3 = GetActor(GD.actor3);
    ActorPortrait3.portrait.sprite = GetActorPortrait(GD.actor3);
    actor3.Player();
    kidnappedActor = GetActor(GD.kidnapped);
    currentActor = actor1;
    EnableActorSelection(false);

    ActorsButtons.SetActive(true);
    StartIntroCutscene();
  }

  #endregion


  #region *********************** Cutscenes and Actions *********************** Cutscenes and Actions *********************** Cutscenes and Actions ***********************
  [HideInInspector] public GameScene currentCutscene;
  [HideInInspector] public static bool SceneSkipped;

  void StartIntroCutscene() {
    FrontActors.enabled = true;
    currentCutscene = AllObjects.GetCutscene(CutsceneID.Intro);
    forcedCursor = CursorTypes.Wait;
    oldCursor = null;
    GD.status = GameStatus.NormalGamePlay;
    SceneSkipped = false;
  }


  public static void StartCutScene(GameScene scene) {
    GD.c.FrontActors.enabled = true;
    Chars main = scene.mainChar;
    AllObjects.StopScenes(main);
    GD.c.currentCutscene = scene;
    GD.c.currentCutscene.Reset();
    GD.c.forcedCursor = CursorTypes.Wait;
    GD.c.oldCursor = null;
    GD.status = GameStatus.NormalGamePlay;
    SceneSkipped = false;
  }

  public static void RemoveCutScene(GameScene scene) {
    if (GD.c.currentCutscene == scene) {
      SceneSkipped = false;
      GD.c.currentCutscene = null;
      GD.c.FrontActors.enabled = false;
    }
  }

  #endregion

  #region *********************** Inventory and Items *********************** Inventory and Items *********************** Inventory and Items ***********************
  public GameObject Inventory;
  public GameObject InventoryItemTemplate;
  private Item overItem = null; // Items we are over with the mouse
  private Item overInventoryItem = null; // Items we are over with the mouse in the inventory
  private Item usedItem = null; // Item that is being used (and visible on the cursor)

  internal static void UpdateInventory() {
    if (GD.c.Inventory.activeSelf)
      GD.c.ActivateInventory(GD.c.currentActor);
  }

  private void ActivateInventory(Actor actor) {
    Inventory.SetActive(true);
    InventoryPortrait.GetComponent<RawImage>().color = new Color32(0x7D, 0x8D, 0xfC, 0xff);
    foreach (Transform t in Inventory.transform)
      GameObject.Destroy(t.gameObject);

    foreach (Item item in actor.inventory) {
      GameObject ii = Instantiate(InventoryItemTemplate, Inventory.transform);
      ii.gameObject.SetActive(true);
      InventoryItem it = ii.GetComponent<InventoryItem>();
      it.text.text = item.Name;
      it.front.sprite = item.iconImage;
      it.item = item;
    }
  }

  internal static void SetItem(Item item, bool fromInventory = false) {
    if (GD.status != GameStatus.NormalGamePlay && !SceneSkipped) return;

    if (fromInventory) {
      if (item == null) {
        GD.c.overItem = null;
        GD.c.overInventoryItem = null;
        if (GD.c.TextMsg.text != "") GD.c.HideName();
        return;
      }
      GD.c.overInventoryItem = item;
      if (GD.c.usedItem == null) {
        GD.c.forcedCursor = CursorTypes.PickUp;
        GD.c.overInventoryItem = item;
        GD.c.ShowName(item.Name);
      }
      return;
    }

    if (item == null) {
      if (GD.c.forcedCursor != CursorTypes.Item) GD.c.forcedCursor = CursorTypes.Normal;
      GD.c.overItem = null;
      if (GD.c.TextMsg.text != "") GD.c.HideName();
      return;
    }

    if (item.owner != Chars.None) {
      GD.c.overItem = item;
      return;
    }

    // Right
    if (item.whatItDoesR == WhatItDoes.Walk) {
      if (GD.c.forcedCursor != CursorTypes.Item) GD.c.forcedCursor = CursorTypes.Normal;
      GD.c.overItem = item;
    }
    else if (item.whatItDoesR == WhatItDoes.Pick) {
      if (GD.c.forcedCursor != CursorTypes.Item) GD.c.forcedCursor = CursorTypes.PickUp;
      GD.c.overItem = item;
      GD.c.ShowName(item.Name);
    }
    else if (item.whatItDoesR == WhatItDoes.Use) {
      if (GD.c.forcedCursor != CursorTypes.Item) {

        Door d = item as Door;
        if (d != null) {
          if (d.IsOpen())
            GD.c.forcedCursor = CursorTypes.Close;
          else
            GD.c.forcedCursor = CursorTypes.Open;

        }
        else
          GD.c.forcedCursor = CursorTypes.Use;
      }
      GD.c.overItem = item;
      GD.c.ShowName(item.Name);
    }
    else if (item.whatItDoesR == WhatItDoes.Read) {
      if (GD.c.forcedCursor != CursorTypes.Item) GD.c.forcedCursor = CursorTypes.Examine;
      GD.c.overItem = item;
      GD.c.ShowName(item.Name);
    }
    // Left
    else if (item.whatItDoesL == WhatItDoes.Walk) {
      if (GD.c.forcedCursor != CursorTypes.Item) GD.c.forcedCursor = CursorTypes.Normal;
      GD.c.overItem = item;
    }
    else if (item.whatItDoesL == WhatItDoes.Pick) {
      if (GD.c.forcedCursor != CursorTypes.Item) GD.c.forcedCursor = CursorTypes.PickUp;
      GD.c.overItem = item;
      GD.c.ShowName(item.Name);
    }
    else if (item.whatItDoesL == WhatItDoes.Use) {
      if (GD.c.forcedCursor != CursorTypes.Item) GD.c.forcedCursor = CursorTypes.Use;
      GD.c.overItem = item;
      GD.c.ShowName(item.Name);
    }
    else if (item.whatItDoesL == WhatItDoes.Read) {
      if (GD.c.forcedCursor != CursorTypes.Item) GD.c.forcedCursor = CursorTypes.Examine;
      GD.c.overItem = item;
      GD.c.ShowName(item.Name);
    }
  }

  internal static bool IsItemCollected(ItemEnum itemID) {
    foreach(Item item in GD.c.actor1.inventory) {
      if (item.Item == itemID) return true;
    }
    foreach(Item item in GD.c.actor2.inventory) {
      if (item.Item == itemID) return true;
    }
    foreach(Item item in GD.c.actor3.inventory) {
      if (item.Item == itemID) return true;
    }
    return false;
  }

  #endregion

  #region *********************** Actors *********************** Actors *********************** Actors *********************** Actors *********************** Actors ***********************
  public PortraitClickHandler ActorPortrait1;
  public PortraitClickHandler ActorPortrait2;
  public PortraitClickHandler ActorPortrait3;
  public PortraitClickHandler InventoryPortrait;
  public Actor actor1;
  public Actor actor2;
  public Actor actor3;
  public Actor kidnappedActor;
  public Actor receiverActor;
  public Actor currentActor = null;
  Color32 unselectedActor = new Color32(0x6D, 0x7D, 0x7C, 255);
  Color32 selectedActor = new Color32(200, 232, 152, 255);
  public Actor[] allEnemies;
  public Actor[] allActors;
  public Sprite[] Portraits;
  public PressAction[] pressActions;
  float walkDelay = 0;
  public RawImage FrontActors;



  /// <summary>
  /// Gets the actual Actor from the Chars enum
  /// </summary>
  public static Actor GetActor(Chars actor) {
    switch (actor) {
      case Chars.None: return null;
      case Chars.Current: return GD.c.currentActor;
      case Chars.Actor1: return GD.c.actor1;
      case Chars.Actor2: return GD.c.actor2;
      case Chars.Actor3: return GD.c.actor3;
      case Chars.Kidnapped: return GD.c.kidnappedActor;
      case Chars.Receiver: return GD.c.receiverActor;
      case Chars.Self: return null;
      case Chars.Player: return GD.c.currentActor;
      case Chars.Fred: return GD.c.allEnemies[0];
      case Chars.Edna: return GD.c.allEnemies[1];
      case Chars.Ted: return GD.c.allEnemies[2];
      case Chars.Ed: return GD.c.allEnemies[3];
      case Chars.Edwige: return GD.c.allEnemies[4];
      case Chars.GreenTentacle: return GD.c.allEnemies[5];
      case Chars.PurpleTentacle: return GD.c.allEnemies[6];
      case Chars.BlueTentacle: return GD.c.allEnemies[7];
      case Chars.PurpleMeteor: return GD.c.allEnemies[8];
      case Chars.Dave: return GD.c.allActors[0];
      case Chars.Bernard: return GD.c.allActors[1];
      case Chars.Wendy: return GD.c.allActors[2];
      case Chars.Syd: return GD.c.allActors[3];
      case Chars.Hoagie: return GD.c.allActors[4];
      case Chars.Razor: return GD.c.allActors[5];
      case Chars.Michael: return GD.c.allActors[6];
      case Chars.Jeff: return GD.c.allActors[7];
      case Chars.Javid: return GD.c.allActors[8];
      case Chars.Laverne: return GD.c.allActors[9];
      case Chars.Ollie: return GD.c.allActors[10];
      case Chars.Sandy: return GD.c.allActors[11];
    }
    Debug.LogError("Invalid actor requested! " + actor);
    return null;
  }


  internal static bool ValidActor(Chars id, Actor actor) {
    switch (id) {
      case Chars.None: return false;
      case Chars.Current: return actor == GD.c.currentActor;
      case Chars.Actor1: return actor == GD.c.actor1;
      case Chars.Actor2: return actor == GD.c.actor2;
      case Chars.Actor3: return actor == GD.c.actor3;
      case Chars.Kidnapped: return actor == GD.c.kidnappedActor;
      case Chars.Receiver: return false;
      case Chars.Self: return false;
      case Chars.Player: return (actor == GD.c.actor1 || actor == GD.c.actor2 || actor == GD.c.actor3);
      case Chars.Enemy: return (actor != GD.c.actor1 && actor != GD.c.actor2 && actor != GD.c.actor3 && actor != GD.c.kidnappedActor);
      case Chars.Fred: return actor == GD.c.allEnemies[0];
      case Chars.Edna: return actor == GD.c.allEnemies[1];
      case Chars.Ted: return actor == GD.c.allEnemies[2];
      case Chars.Ed: return actor == GD.c.allEnemies[3];
      case Chars.Edwige: return actor == GD.c.allEnemies[4];
      case Chars.GreenTentacle: return actor == GD.c.allEnemies[5];
      case Chars.PurpleTentacle: return actor == GD.c.allEnemies[6];
      case Chars.BlueTentacle: return actor == GD.c.allEnemies[7];
      case Chars.PurpleMeteor: return actor == GD.c.allEnemies[8];

      case Chars.Dave: return actor == GD.c.allActors[0];
      case Chars.Bernard: return actor == GD.c.allActors[1];
      case Chars.Wendy: return actor == GD.c.allActors[2];
      case Chars.Syd: return actor == GD.c.allActors[3];
      case Chars.Hoagie: return actor == GD.c.allActors[4];
      case Chars.Razor: return actor == GD.c.allActors[5];
      case Chars.Michael: return actor == GD.c.allActors[6];
      case Chars.Jeff: return actor == GD.c.allActors[7];
      case Chars.Javid: return actor == GD.c.allActors[8];
      case Chars.Laverne: return actor == GD.c.allActors[9];
      case Chars.Ollie: return actor == GD.c.allActors[10];
      case Chars.Sandy: return actor == GD.c.allActors[11];

      case Chars.Male: return actor.male;
      case Chars.Female: return !actor.male;
    }
    return false;
  }


  public static Actor GetActorForSelection(int num) {
    if (num < 9) return GD.c.allEnemies[num];
    return GD.c.allActors[num - 9];
  }

  public Sprite GetActorPortrait(Chars actor) {
    int idx = (int)actor;
    if (idx < 10) return null;

    return Portraits[idx - 10];
  }

  public static Chars GetCharFromActor(Actor actor) {
    if (actor == null) return Chars.None;
    else if (actor == GD.c.actor1) return Chars.Actor1;
    else if (actor == GD.c.actor2) return Chars.Actor2;
    else if (actor == GD.c.actor3) return Chars.Actor3;
    else if (actor == GD.c.kidnappedActor) return Chars.Kidnapped;
    else if (actor == GD.c.receiverActor) return Chars.Receiver;
    else if (actor == GD.c.allEnemies[0]) return Chars.Fred;
    else if (actor == GD.c.allEnemies[1]) return Chars.Edna;
    else if (actor == GD.c.allEnemies[2]) return Chars.Ted;
    else if (actor == GD.c.allEnemies[3]) return Chars.Ed;
    else if (actor == GD.c.allEnemies[4]) return Chars.Edwige;
    else if (actor == GD.c.allEnemies[5]) return Chars.GreenTentacle;
    else if (actor == GD.c.allEnemies[6]) return Chars.PurpleTentacle;
    else if (actor == GD.c.allActors[0]) return Chars.Dave;
    else if (actor == GD.c.allActors[1]) return Chars.Bernard;
    else if (actor == GD.c.allActors[2]) return Chars.Wendy;
    else if (actor == GD.c.allActors[3]) return Chars.Syd;
    else if (actor == GD.c.allActors[4]) return Chars.Hoagie;
    else if (actor == GD.c.allActors[5]) return Chars.Razor;
    else if (actor == GD.c.allActors[6]) return Chars.Michael;
    else if (actor == GD.c.allActors[7]) return Chars.Jeff;
    else if (actor == GD.c.allActors[8]) return Chars.Javid;
    else if (actor == GD.c.allActors[9]) return Chars.Laverne;
    else if (actor == GD.c.allActors[10]) return Chars.Ollie;
    else if (actor == GD.c.allActors[11]) return Chars.Sandy;
    else if (actor == GD.c.currentActor) return Chars.Current;
    Debug.LogError("Invalid actor requested! " + actor);
    return Chars.None;
  }



  /// <summary>
  /// Moves the actor to the destination and execute the action callback when the destination is reached
  /// </summary>
  void WalkAndAction(Actor actor, Item item, System.Action<Actor, Item> action) {
    Vector3 one = actor.transform.position;
    one.z = 0;
    Vector3 two = item.HotSpot;
    // If we have a container prefer it as hotspot
    Container c = item.transform.parent.GetComponent<Container>();
    if (c != null) two = c.HotSpot;
    two.z = 0;
    float dist = Vector3.Distance(one, two);
    if (dist > .2f) { // Need to walk
      RaycastHit2D hit = Physics2D.Raycast(two, cam.transform.forward, 10000, pathLayer);
      if (hit.collider != null) {
        PathNode p = hit.collider.GetComponent<PathNode>();
        currentActor.WalkToPos(two, p, action, item);
      }
      return;
    }
    else {
      action?.Invoke(currentActor, item);
    }
  }

  public static void RemovePressAction(Actor a) {
    if (GD.c.pressActions[0].actor == a)  GD.c.pressActions[0].Reset(a);
    if (GD.c.pressActions[1].actor == a)  GD.c.pressActions[1].Reset(a);
    if (GD.c.pressActions[2].actor == a)  GD.c.pressActions[2].Reset(a);
  }


  internal static void SelectActor(Actor actor, bool force = false) {
    if (GD.status != GameStatus.NormalGamePlay && !force && (GD.c.currentCutscene == null || !GD.c.currentCutscene.skippable)) return;

    if (GD.c.currentCutscene != null && GD.c.currentCutscene.skippable) {
      SceneSkipped = true;
      Fader.RemoveFade();
    }
    GD.c.forcedCursor = CursorTypes.Normal;
    GD.c.oldCursor = null;
    GD.c.usedItem = null;
    GD.c.EnableActorSelection(false);

    GD.c.currentActor = actor;
    GD.c.ActorPortrait1.GetComponent<RawImage>().color = GD.c.unselectedActor;
    GD.c.ActorPortrait2.GetComponent<RawImage>().color = GD.c.unselectedActor;
    GD.c.ActorPortrait3.GetComponent<RawImage>().color = GD.c.unselectedActor;
    if (actor == GD.c.actor1) {
      GD.c.ActorPortrait1.GetComponent<RawImage>().color = GD.c.selectedActor;
    }
    else if (actor == GD.c.actor2) {
      GD.c.ActorPortrait2.GetComponent<RawImage>().color = GD.c.selectedActor;
    }
    if (actor == GD.c.actor3) {
      GD.c.ActorPortrait3.GetComponent<RawImage>().color = GD.c.selectedActor;
    }
    GD.c.ShowName("Selected: " + GD.c.currentActor.name);
    if (GD.c.currentActor.currentRoom != GD.c.currentRoom) { // Different room
      GD.c.StartCoroutine(GD.c.FadeToRoomActor());
    }
    if (GD.c.Inventory.activeSelf) GD.c.ActivateInventory(GD.c.currentActor);
  }

  Actor overActor = null;
  internal static bool OverActor(Actor actor) {
    if (actor == GD.c.currentActor) return true;
    GD.c.overActor = actor;
    return false;
  }

  void EnableActorSelection(bool enable) {
    foreach (Actor a in allActors)
      if (a != null) a.GetComponent<BoxCollider2D>().enabled = enable;
    foreach (Actor a in allEnemies)
      if (a != null) a.GetComponent<BoxCollider2D>().enabled = enable;
  }

#endregion

  #region *********************** Rooms and Transitions *********************** Rooms and Transitions *********************** Rooms and Transitions ***********************
  public Room currentRoom;

  private IEnumerator ChangeRoom(Actor actor, Door door) {
    if (door == null || door.dst == null) {
      Dbg("Not available in demo!");
      yield break;
    }

    // Disable gameplay
    GD.status = GameStatus.Cutscene;
    yield return null;

    // Enable dst
    door.dst.gameObject.SetActive(true);

    // Get dst camera pos + door dst pos
    Vector3 orgp = cam.transform.position;
    Vector3 dstp = orgp;
    float orthos = cam.orthographicSize;
    float orthod = orthos;
    switch (door.transition) {
      case TransitionDirection.Left: dstp += 1.5f * Vector3.left; break;
      case TransitionDirection.Right: dstp += 1.5f * Vector3.right; break;
      case TransitionDirection.Up: dstp += 1 * Vector3.up; break;
      case TransitionDirection.Down: dstp += 1 * Vector3.down; break;
      case TransitionDirection.In: orthod -= .5f; break;
      case TransitionDirection.Out: orthod += .5f; break;
    }

    // Move camera quickly from current to dst
    float time = 0;
    while (time < .125f) {
      // Fade black
      BlackFade.color = new Color32(0, 0, 0, (byte)(255 * (time * 8)));
      cam.transform.position = (1 - time * 4) * orgp + (time * 4) * dstp;
      cam.orthographicSize = (1 - time * 4) * orthos + (time * 4) * orthod;
      time += Time.deltaTime;
      yield return null;
    }
    actor.transform.position = door.correspondingDoor.HotSpot;
    yield return null;

    dstp = door.camerapos;
    orgp = dstp;
    orthos = 4;
    orthod = 4;
    switch (door.correspondingDoor.transition) {
      case TransitionDirection.Left: orgp += 1.5f * Vector3.left; break;
      case TransitionDirection.Right: orgp += 1.5f * Vector3.right; break;
      case TransitionDirection.Up: orgp += 1 * Vector3.up; break;
      case TransitionDirection.Down: orgp += 1 * Vector3.down; break;
      case TransitionDirection.In: orthos -= .5f; break;
      case TransitionDirection.Out: orthos += .5f; break;
    }
    actor.Stop();
    // Move actor to dst door pos
    currentRoom = door.dst;
    currentActor = actor;
    actor.transform.position = door.correspondingDoor.HotSpot;
    actor.currentRoom = currentRoom;
    actor.SetDirection(door.correspondingDoor.arrivalDirection);
    currentActor.SetScaleAndPosition(door.correspondingDoor.HotSpot);
    currentRoom.UpdateLights();
    yield return null;

    while (time < .25f) {
      // Fade black
      BlackFade.color = new Color32(0, 0, 0, (byte)(255 * (1 - (8 * (time - .125f)))));
      cam.transform.position = (1 - time * 4) * orgp + (time * 4) * dstp;
      cam.orthographicSize = (1 - time * 4) * orthos + (time * 4) * orthod;
      time += Time.deltaTime;
      yield return null;
    }
    cam.transform.position = dstp;

    // Disable src
    door.src.gameObject.SetActive(false);

    // Disable actors not in current room
    foreach (Actor a in allActors) {
      if (a == null) continue;
      a.SetVisible(a.currentRoom == currentRoom);
    }
    foreach (Actor a in allEnemies) {
      if (a == null) continue;
      a.SetVisible(a.currentRoom == currentRoom);
    }

    // Enable gameplay
    GD.status = GameStatus.NormalGamePlay;
    forcedCursor = CursorTypes.Normal;
    overItem = null;
    ShowName(currentRoom.name);
  }

  bool CameraFadingToActor = false;

  public IEnumerator FadeToRoomActor() {
    CameraFadingToActor = true;
    Room prev = currentRoom;
    currentRoom = currentActor.currentRoom;

    // Get dst camera pos + door dst pos
    Vector3 dstp = currentActor.transform.position;
    dstp.y = currentRoom.CameraGround;
    dstp.z = -10;

    // Move camera quickly from current to dst
    float time = 0;
    while (time < .125f) {
      // Fade black
      BlackFade.color = new Color32(0, 0, 0, (byte)(255 * (time * 8)));
      time += Time.deltaTime;
      yield return null;
    }
    prev.gameObject.SetActive(false);
    currentRoom.gameObject.SetActive(true);
    cam.transform.position = dstp;
    yield return null;

    // Disable actors not in current room
    foreach (Actor a in allActors) {
      if (a == null) continue;
      a.SetVisible(a.currentRoom == currentRoom);
    }
    foreach (Actor a in allEnemies) {
      if (a == null) continue;
      a.SetVisible(a.currentRoom == currentRoom);
    }


    while (time < .25f) {
      // Fade black
      BlackFade.color = new Color32(0, 0, 0, (byte)(255 * (1 - (8 * (time - .125f)))));
      time += Time.deltaTime;
      yield return null;
    }
    forcedCursor = CursorTypes.Normal;
    overItem = null;
    CameraFadingToActor = false;
  }

  internal static void StopPanningCamera() {
    if (GD.c.CameraPanningInstance != null) {
      GD.c.StopCoroutine(GD.c.CameraPanningInstance);
      GD.c.CameraPanningInstance = null;
    }
  }

  internal static void PanCamera(Vector3 rpos, float del) {
    if (del < .1f) del = 1;
    GD.c.CameraPanningInstance = GD.c.StartCoroutine(GD.c.CameraPanning(rpos, del));
  }

  Coroutine CameraPanningInstance = null;

  IEnumerator CameraPanning(Vector3 pos, float del) {
    Vector3 start = cam.transform.position;
    Vector3 dest = pos;
    float time = del;
    yield return null;

    while (time >= 0) {
      time -= Time.deltaTime;
      Vector3 here = (time / del) * start + (1 - time / del) * dest;
      cam.transform.position = here;
      yield return null;
    }
    cam.transform.position = pos;
    GD.c.CameraPanningInstance = null;
  }

  #endregion

  #region *********************** UI and Options *********************** UI and Options *********************** UI and Options *********************** UI and Options ***********************
  public static float walkSpeed;
  public static float textSpeed;
  public Options options;

  public TextMeshProUGUI TextMsg;
  public RectTransform TextMsgRT;
  public RectTransform TextBackRT;
  TextMsgMode txtMsgMode = TextMsgMode.None;
  float textMsgTime = 0;
  float textSizeX = 0;

  public GameObject ActorsButtons;
  public Canvas SkyBackground;

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

  #endregion

  #region *********************** Music and Sounds *********************** Music and Sounds *********************** Music and Sounds *********************** Music and Sounds ***********************
  public static void PlayMusic(AudioClip clip) {
    GD.c.MusicPlayer.Stop();
    GD.c.MusicPlayer.clip = clip;
    GD.c.MusicPlayer.Play();
  }

  public static void StopMusic() {
    GD.c.MusicPlayer.Stop();
  }

  public static void PauseMusic() {
    if (GD.c.MusicPlayer.isPlaying)
      GD.c.MusicPlayer.Pause();
    else
      GD.c.MusicPlayer.UnPause();
  }

  #endregion


}


