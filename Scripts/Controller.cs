using System.Collections;
using System.Collections.Concurrent;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Controller : MonoBehaviour {
  [HideInInspector] public Camera cam;
  public Transform Moon;
  public GameObject BackSky;
  public LayerMask pathLayer;
  public LayerMask doorLayer;
  public Image BlackFade;
  public Transform PickedItems;
  public AudioSource MusicPlayer;
  private Vector3 MoonPos = new Vector3(5.9f, 3.35f, 10);
  bool blockingScenesPlaying = false;

  public TextMeshProUGUI DbgMsg;
  public static void Dbg(string txt) {
    GD.c.DbgMsg.text = txt;
  }

  static string dbgu, dbgc;
  public static void DbgU(string txt) {
    dbgu = txt;
//    GD.c.DbgMsg.text = "UsedItem = " + dbgu + "\nCursor = " + dbgc;
  }

  public static void DbgC(string txt) {
    dbgc = txt;
//    GD.c.DbgMsg.text = "UsedItem = " + dbgu + "\nCursor = " + dbgc;
  }

  #region *********************** Mouse and Interaction *********************** Mouse and Interaction *********************** Mouse and Interaction ***********************

  private void OnApplicationFocus(bool focus) {
    if (focus && GD.status != GameStatus.NotYetLoaded) CursorHandler.ResetCursor();
  }

  private void SelectInventoryItem() {
    usedItem = overInventoryItem;
    overInventoryItem = null;
    EnableActorSelection(true);
    CursorHandler.Set(CursorTypes.Normal, CursorTypes.Normal, usedItem);
    if (!string.IsNullOrEmpty(usedItem.Description)) currentActor.Say(usedItem.Description);
  }

  private void UseInventoryItem() {
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
    UpdateInventory();
  }

  private void UseInventoryItemTogether() {
    // Can we use the two items together?
    string res = usedItem.UseTogether(currentActor, overInventoryItem);
    if (!string.IsNullOrEmpty(res)) currentActor.Say(res);
    UpdateInventory();
    usedItem = null;
    overItem = null;
    overInventoryItem = null;
    EnableActorSelection(false);
    Inventory.SetActive(false);
  }

  bool prevlmb = false;
  bool prevrmb = false;
  float lastClickTimeL = 0;
  float lastClickTimeR = 0;
  readonly float doubleClickDelay = .35f;

  void Update() {
    if (Options.IsActive()) return;
    if (GD.status == GameStatus.StartGame) {
      StartGame();
      return;
    }
    if (GD.status != GameStatus.NormalGamePlay) return;


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

    blockingScenesPlaying = GameScenesManager.NoUserControl();
    CursorHandler.WaitMode(blockingScenesPlaying);
    FrontActors.enabled = blockingScenesPlaying;
    if (blockingScenesPlaying) return;

    #region Handle camera
    if (!CameraFadingToActor && CameraPanningInstance == null) {
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
          cam.transform.position -= cam.transform.right * Time.deltaTime * (.4f * Screen.width - cpos.x) / 25;
        }
      }
      else if (cpos.x > .7f * Screen.width) {
        if (cam.transform.position.x < currentRoom.maxR) {
          cam.transform.position += cam.transform.right * Time.deltaTime * (cpos.x - .6f * Screen.width) / 25;
        }
      }
      MoonPos.x = 5.9f - cam.transform.position.x / 100;
      Moon.localPosition = MoonPos;
    }
    #endregion

    if (currentActor.currentRoom != currentRoom && !CameraFadingToActor) {
      StartCoroutine(FadeToRoomActor());
    }

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



    bool notOverUI = !EventSystem.current.IsPointerOverGameObject();
    bool lmb = false;
    bool rmb = false;
    bool dblL = false;
    bool dblR = false;
    bool nowlmb = Input.GetMouseButtonDown(0);
    bool nowrmb = Input.GetMouseButtonDown(1);

    if (nowlmb) {
      if (Time.time - lastClickTimeL < doubleClickDelay) {
        lmb = true;
        dblL = true;
        prevlmb = false;
      }
      else {
        prevlmb = true;
        lastClickTimeL = Time.time;
      }
    }
    else if (prevlmb && Time.time - lastClickTimeL > doubleClickDelay * .25f) {
      lmb = prevlmb;
      prevlmb = false;
    }
    if (nowrmb) {
      if (Time.time - lastClickTimeR < doubleClickDelay) {
        rmb = true;
        dblR = true;
        prevrmb = false;
      }
      else {
        prevrmb = true;
        lastClickTimeR = Time.time;
      }
    }
    else if (prevrmb && Time.time - lastClickTimeR > doubleClickDelay * .25f) {
      rmb = prevrmb;
      prevrmb = false;
    }

    #region FlashLight

    if (GD.flashLight) {
      Vector3 fl = Input.mousePosition;
      if (fl.x < 0) fl.x = 0;
      if (fl.x > 1920) fl.x = 1920;
      if (fl.y < 0) fl.y = 0;
      if (fl.y > 1080) fl.y = 1080;
      fl.z = -7;
      FlashLightRT.localPosition = fl;

      GD.FlashLight().SetFloat("_FlickerPercent", 0);
      switch (batteriesUsed) {
        case BatteriesUsed.OldBatteries:
          flashlightTimeOld -= Time.deltaTime;
          if (flashlightTimeOld < 30) {
            if (!messageOldBatteries) {
              currentActor.Say("The batteries are dying");
              messageOldBatteries = true;
            }
            GD.FlashLight().SetFloat("_FlickerPercent", .1f - flashlightTimeOld * .0033333f);
          }
          if (flashlightTimeOld <= 0) {
            currentActor.Say("The batteries died");
            batteriesUsed = BatteriesUsed.NoBatteries;
            FlashLightPanel.enabled = false;
            BackSky.SetActive(true);
            GD.flashLight = false;
            currentRoom.UpdateLights();
          }
          break;

        case BatteriesUsed.Batteries:
          flashlightTime -= Time.deltaTime;
          if (flashlightTime < 30) {
            if (!messageBatteries) {
              currentActor.Say("The batteries are dying");
              messageBatteries = true;
            }
            GD.FlashLight().SetFloat("_FlickerPercent", .1f - flashlightTimeOld * .0033333f);
          }
          if (flashlightTime <= 0) {
            currentActor.Say("The batteries died");
            batteriesUsed = BatteriesUsed.NoBatteries;
            FlashLightPanel.enabled = false;
            BackSky.SetActive(true);
            GD.flashLight = false;
            currentRoom.UpdateLights();
          }
          break;

        case BatteriesUsed.PlutoniumBatteries:
          flashlightTimePlutonium -= Time.deltaTime;
          if (flashlightTimePlutonium < 30) {
            if (!messagePuBatteries) {
              currentActor.Say("The batteries are dying");
              messagePuBatteries = true;
            }
            GD.FlashLight().SetFloat("_FlickerPercent", .1f - flashlightTimeOld * .0033333f);
          }
          if (flashlightTimePlutonium <= 0) {
            currentActor.Say("The batteries died");
            batteriesUsed = BatteriesUsed.NoBatteries;
            FlashLightPanel.enabled = false;
            BackSky.SetActive(true);
            GD.flashLight = false;
            currentRoom.UpdateLights();
          }
          break;
      }
    }

    #endregion

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
      walkDelay = .25f / walkSpeed;
      RaycastHit2D hit = Physics2D.Raycast(cam.ScreenToWorldPoint(Input.mousePosition), cam.transform.forward, 10000, pathLayer);
      if (hit.collider != null) {
        PathNode p = hit.collider.GetComponent<PathNode>();
        if (p != null && !p.isStair)
          currentActor.WalkToPos(hit.point, p);
      }
    }

    #region Mouse control
    if (!lmb && !rmb) return;


    if ((lmb || rmb) && overInventoryItem == null && (inventoryMode == 1 || inventoryMode == 4)) { // click not on items will close the inventory
      Inventory.SetActive(false);
    }

    if (overInventoryItem != null) {
      if (inventoryMode == 0 || inventoryMode == 1) { // no auto-close -------------------------------------------------------------------------------------------------------------------------------------------------
        if (lmb && usedItem == null) { // LClick and no item -> select
          SelectInventoryItem();
        }
        else if (lmb && usedItem != null) { // LClick and item -> select or unselect if the same
          if (usedItem == overInventoryItem) {
            usedItem = null;
            EnableActorSelection(false);
            CursorHandler.Set(CursorTypes.Normal, CursorTypes.Normal, null);
          }
          else
            SelectInventoryItem();
        }
        else if (rmb && usedItem == null) { // RClick and no item -> use
          UseInventoryItem();
        }
        else if (rmb && usedItem != null) { // RClick and item -> use together
          UseInventoryItemTogether();
        }
      }
      else if (inventoryMode == 2) { // double-click select and close----------------------------------------------------------------------------------------------------------------------------
        if (lmb && usedItem == null) { // LClick and no item -> select
          SelectInventoryItem();
          if (dblL) Inventory.SetActive(false);
        }
        else if (lmb && usedItem != null) { // LClick and item -> select or unselect if the same
          if (usedItem == overInventoryItem) {
            usedItem = null;
            EnableActorSelection(false);
            CursorHandler.Set(CursorTypes.Normal, CursorTypes.Normal, null);
          }
          else
            SelectInventoryItem();
          if (dblL) Inventory.SetActive(false);
        }
        else if (rmb && usedItem == null) { // RClick and no item -> use
          UseInventoryItem();
        }
        else if (rmb && usedItem != null) { // RClick and item -> use together
          UseInventoryItemTogether();
        }

      }
      else if (inventoryMode == 3 || inventoryMode == 4) {
        // 3 double-click select and close, double right click use and close -------------------------------------------------------------------------------------------
        // 4 Double-click and clicking outside close inventory ----------------------------------------------------------------------------------------------------------
        if (lmb && usedItem == null) { // LClick and no item -> select
          SelectInventoryItem();
          if (dblL) Inventory.SetActive(false);
        }
        else if (lmb && usedItem != null) { // LClick and item -> select or unselect if the same
          if (usedItem == overInventoryItem) {
            usedItem = null;
            EnableActorSelection(false);
            CursorHandler.Set(CursorTypes.Normal, CursorTypes.Normal, null);
          }
          else
            SelectInventoryItem();
          if (dblL) Inventory.SetActive(false);
        }
        else if (rmb && usedItem == null) { // RClick and no item -> use
          UseInventoryItem();
          if (dblR) Inventory.SetActive(false);
        }
        else if (rmb && usedItem != null) { // RClick and item -> use together
          UseInventoryItemTogether();
          if (dblR) Inventory.SetActive(false);
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
                ShowName(currentActor.name + " got " + item.Name);
                if (!actor.inventory.Contains(item))
                  actor.inventory.Add(item);
                item.transform.parent = PickedItems;
                item.gameObject.SetActive(false);
                item.owner = Chars.None;
                if (actor == actor1) item.owner = Chars.Actor1;
                else if (actor == actor2) item.owner = Chars.Actor2;
                else if (actor == actor3) item.owner = Chars.Actor3;
                // Change the default actions to Read/Use
                item.whatItDoesL = WhatItDoes.Read;
                item.whatItDoesR = WhatItDoes.Use;
                if (item.Usable == Tstatus.Pickable) item.Usable = Tstatus.Usable;
                item.PlayActions(currentActor, null, When.Pick, null);
                item = null;
                CursorHandler.Set();
                DbgC("Update 348");
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
          Vector3 worldPoint = cam.ScreenToWorldPoint(Input.mousePosition);
          WalkAndAction(currentActor, overItem,
            new System.Action<Actor, Item>((actor, item) => {
              if (item == null) {
                Debug.Log("Null item in callback");
                CursorHandler.Set();
                return;
              }
              actor.SetDirection(item.dir);
              string res = item.Use(currentActor);
              if (!string.IsNullOrEmpty(res))
                actor.Say(res);

              // Here we should raycast and check what should the item be with the actions (if any)
              RaycastHit2D[] hits = Physics2D.RaycastAll(worldPoint, cam.transform.forward);
              foreach (RaycastHit2D hit in hits) {
                Item hitItem = hit.collider.gameObject.GetComponent<Item>();
                if (hitItem != null) SetItem(hitItem);
              }
            }));
        }

        else if ((lmb && overItem.whatItDoesL == WhatItDoes.Walk) || (rmb && overItem.whatItDoesR == WhatItDoes.Walk)) { /* walk */
          if (aDoor == null) {
            if (overItem.CompareTag("WoodsDoor")) {
              Item door = overItem;
              WalkAndAction(currentActor, overItem, new System.Action<Actor, Item>((actor, item) => {
                StartCoroutine(ChangeWoods(actor, door));
              }));
            }
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
          Vector3 worldPoint = cam.ScreenToWorldPoint(Input.mousePosition);
          WalkAndAction(currentActor, overItem,
            new System.Action<Actor, Item>((actor, item) => {
              // Can we use the two items together?
              string res = usedItem.UseTogether(currentActor, item);
              if (!string.IsNullOrEmpty(res)) currentActor.Say(res);
              UpdateInventory();
              usedItem = null;
              overItem = null;
              DbgU("Update 416");
              EnableActorSelection(false);
              Inventory.SetActive(false);

              // Here we should raycast and check what should the item be with the actions (if any)
              RaycastHit2D[] hits = Physics2D.RaycastAll(worldPoint, cam.transform.forward);
              foreach (RaycastHit2D hit in hits) {
                Item hitItem = hit.collider.gameObject.GetComponent<Item>();
                if (hitItem != null) SetItem(hitItem);
              }
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
          CursorHandler.Set();
          DbgC("Update 457");
          usedItem = null;
          DbgU("Update 455");
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
        DbgU("Update 469");
        CursorHandler.Set();
        DbgC("Update 475");
        EnableActorSelection(false);
        return;
      }
    }

    #endregion
  }


  internal static void HandleToolbarClicks(IPointerClickHandler handler) {
    if (Options.IsActive()) return;
    if (GameScenesManager.BlockingScene()) return;

    PortraitClickHandler h = (PortraitClickHandler)handler;
    if (h == GD.c.ActorPortrait1 && !GD.c.actor1.dead) {
      SelectActor(GD.c.actor1);
    }
    else if (h == GD.c.ActorPortrait2 && !GD.c.actor2.dead) {
      SelectActor(GD.c.actor2);
    }
    else if (h == GD.c.ActorPortrait3 && !GD.c.actor3.dead) {
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
    BackSky.SetActive(true);

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

    Material normal = GD.Normal();
    foreach (Actor a in allActors)
      if (a != null) {
        a.Face.material = normal;
        a.Arms.material = normal;
        a.Legs.material = normal;
      }
    foreach (Actor a in allEnemies)
      if (a != null) {
        a.Face.material = normal;
        a.Arms.material = normal;
        a.Legs.material = normal;
      }

    ActorsButtons.SetActive(true);
    StartIntroCutscene();

    /*
    Item FIXME = AllObjects.GetItem(ItemEnum.WoodsMap);
    FIXME.whatItDoesR = WhatItDoes.Use;
    FIXME.Usable = Tstatus.Usable;
    FIXME.owner = currentActor.id;
    currentActor.inventory.Add(FIXME);
    FIXME = AllObjects.GetItem(ItemEnum.Coat);
    FIXME.whatItDoesR = WhatItDoes.Use;
    FIXME.Usable = Tstatus.Usable;
    FIXME.owner = currentActor.id;
    currentActor.inventory.Add(FIXME);
    FIXME = AllObjects.GetItem(ItemEnum.Shovel);
    FIXME.whatItDoesR = WhatItDoes.Use;
    FIXME.Usable = Tstatus.Usable;
    FIXME.owner = currentActor.id;
    currentActor.inventory.Add(FIXME);

    for (int i = 0; i < (int)ItemEnum.ReedGrave; i++) {
      FIXME = AllObjects.GetItem((ItemEnum)i);
      if (FIXME == null) continue;
      FIXME.whatItDoesR = WhatItDoes.Use;
      FIXME.Usable = Tstatus.Usable;
      FIXME.owner = currentActor.id;
      currentActor.inventory.Add(FIXME);
    }
    */
  }

  #endregion


  #region *********************** Cutscenes and Actions *********************** Cutscenes and Actions *********************** Cutscenes and Actions ***********************

  [HideInInspector] public GameScene currentCutscene; // REMOVE FIXME

  void StartIntroCutscene() {
    FrontActors.enabled = true;
    GameScene introScene = AllObjects.GetCutscene(CutsceneID.Intro);
    CursorHandler.WaitMode(true);
    GameScenesManager.StartScene(introScene);
  }



  #endregion

  #region *********************** Inventory and Items *********************** Inventory and Items *********************** Inventory and Items ***********************
  public GameObject Inventory;
  public GameObject InventoryItemTemplate;
  private Item overItem = null; // Items we are over with the mouse
  private Item overInventoryItem = null; // Items we are over with the mouse in the inventory
  private Item usedItem = null; // Item that is being used (and visible on the cursor)

  public void ResetItems() {
    usedItem = null;
    overInventoryItem = null;
    overItem = null;
  }

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
      if (!item.isEnabled) continue;
      GameObject ii = Instantiate(InventoryItemTemplate, Inventory.transform);
      ii.gameObject.SetActive(true);
      InventoryItem it = ii.GetComponent<InventoryItem>();
      it.text.text = item.Name;
      it.front.sprite = item.iconImage;
      it.item = item;
    }
  }

  internal static void SetItem(Item item, bool fromInventory = false) {
    if (GD.status != GameStatus.NormalGamePlay && GD.c.blockingScenesPlaying) return;

    if (fromInventory) {
      if (item == null) {
        GD.c.overItem = null;
        GD.c.overInventoryItem = null;
        if (GD.c.TextMsg.text != "") GD.c.HideName();
        CursorHandler.Set(CursorTypes.Normal, CursorTypes.Normal, GD.c.usedItem);
        DbgC("SetItem 633 norm");
        return;
      }
      GD.c.overInventoryItem = item;
      if (GD.c.usedItem == null) {
        CursorHandler.Set(CursorTypes.Pick, CursorTypes.Use);
        DbgC("SetItem 639 norm");
        GD.c.overInventoryItem = item;
        GD.c.ShowName(item.Name);
      }
      return;
    }

    if (item == null) {
      if (GD.c.usedItem == null) {
        CursorHandler.Set();
        DbgC("SetItem 649");
      }
      GD.c.overItem = null;
      if (GD.c.TextMsg.text != "") GD.c.HideName();
      return;
    }

    if (item.owner != Chars.None) {
      GD.c.overItem = item;
      return;
    }

    CursorTypes onL = CursorTypes.Normal;
    CursorTypes onR = CursorTypes.Normal;

    // Right
    if (item.whatItDoesR == WhatItDoes.Walk) {
      Door d = item as Door;
      if (d != null) {
        switch (d.transition) {
          case TransitionDirection.Left:  onR = CursorTypes.WalkAwayL; break;
          case TransitionDirection.Right: onR = CursorTypes.WalkAwayR; break;
          case TransitionDirection.Up:    onR = CursorTypes.WalkAwayU; break;
          case TransitionDirection.Down:  onR = CursorTypes.WalkAwayD; break;
          case TransitionDirection.In:    onR = CursorTypes.WalkAwayI; break;
          case TransitionDirection.Out:   onR = CursorTypes.WalkAwayO; break;
        }
      }
      GD.c.overItem = item;
    }
    else if (item.whatItDoesR == WhatItDoes.Pick) {
      onR = CursorTypes.Pick;
      GD.c.overItem = item;
      GD.c.ShowName(item.Name);
    }
    else if (item.whatItDoesR == WhatItDoes.Use) {
      onR = CursorTypes.Use;
      if (item.Usable == Tstatus.Swithchable)
        onR = item.IsOpen() ? CursorTypes.Off : CursorTypes.On;
      else if ((item as Door) != null || (item as Container) != null) {
        if (GD.c.usedItem == null)
          onR = item.IsOpen() ? CursorTypes.Close : CursorTypes.Open;
        else
          onR = CursorTypes.Use;
      }

      GD.c.overItem = item;
      GD.c.ShowName(item.Name);
    }
    else if (item.whatItDoesR == WhatItDoes.Read) {
      onR = CursorTypes.Read;
      GD.c.overItem = item;
      GD.c.ShowName(item.Name);
    }
    
    
    
    // Left
    if (item.whatItDoesL == WhatItDoes.Walk) {
      Door d = item as Door;
      if (d != null) {
        switch (d.transition) {
          case TransitionDirection.Left:  onL = CursorTypes.WalkAwayL; break;
          case TransitionDirection.Right: onL = CursorTypes.WalkAwayR; break;
          case TransitionDirection.Up:    onL = CursorTypes.WalkAwayU; break;
          case TransitionDirection.Down:  onL = CursorTypes.WalkAwayD; break;
          case TransitionDirection.In:    onL = CursorTypes.WalkAwayI; break;
          case TransitionDirection.Out:   onL = CursorTypes.WalkAwayO; break;
        }
      }
      GD.c.overItem = item;
    }
    else if (item.whatItDoesL == WhatItDoes.Pick) {
      onL = CursorTypes.Pick;
      GD.c.overItem = item;
      GD.c.ShowName(item.Name);
    }
    else if (item.whatItDoesL == WhatItDoes.Use) {
      onL = CursorTypes.Use;
      if (item.Usable == Tstatus.Swithchable)
        onL = item.IsOpen() ? CursorTypes.Off : CursorTypes.On;
      else if ((item as Door) != null || (item as Container) != null) {
        if (GD.c.usedItem == null)
          onL = item.IsOpen() ? CursorTypes.Close : CursorTypes.Open;
        else
          onL = CursorTypes.Use;
      }

      GD.c.overItem = item;
      GD.c.ShowName(item.Name);
    }
    else if (item.whatItDoesL == WhatItDoes.Read) {
      onL = CursorTypes.Read;
      GD.c.overItem = item;
      GD.c.ShowName(item.Name);
    }

    if (GD.c.overItem != null && GD.c.usedItem != null) {
      onL = CursorTypes.Normal;
      onR = CursorTypes.Use;
    }

    CursorHandler.Set(onL, onR, GD.c.usedItem);
    DbgC("SetItem 747");
  }

  internal static bool IsItemCollected(ItemEnum itemID) {
    foreach(Item item in GD.c.actor1.inventory) {
      if (item.ID == itemID) return true;
    }
    foreach(Item item in GD.c.actor2.inventory) {
      if (item.ID == itemID) return true;
    }
    foreach(Item item in GD.c.actor3.inventory) {
      if (item.ID == itemID) return true;
    }
    return false;
  }


  private float flashlightTime = 6 * 60 * 60; // 6 hours
  private float flashlightTimeOld = 60; // 1 minute
  private float flashlightTimePlutonium = 900; // 15 minutes
  bool messageOldBatteries = false;
  bool messageBatteries = false;
  bool messagePuBatteries = false;
  public RectTransform FlashLightRT;
  public Canvas FlashLightPanel;
  [HideInInspector] public BatteriesUsed batteriesUsed = BatteriesUsed.NoBatteries;

  internal static void SwitchFlashLight(BatteriesUsed batteries) {
    if (batteries == BatteriesUsed.NoBatteries) { // Shut down flashlight <<< --------------------------------------------------------------------------------

      if (GD.c.batteriesUsed == BatteriesUsed.NoBatteries) {
        GD.c.currentActor.Say("No batteries inside...");
      }
      else if (GD.c.currentActor.HasItem(ItemEnum.Flashlight)) {
        GD.c.FlashLightPanel.enabled = false;
        GD.c.BackSky.SetActive(true);
        GD.flashLight = false;
        GD.c.currentRoom.UpdateLights();
        if (GD.c.batteriesUsed == BatteriesUsed.Batteries)
          GD.c.currentActor.inventory.Add(AllObjects.GetItem(ItemEnum.Batteries));
        else if (GD.c.batteriesUsed == BatteriesUsed.OldBatteries)
          GD.c.currentActor.inventory.Add(AllObjects.GetItem(ItemEnum.OldBatteries));
        else if (GD.c.batteriesUsed == BatteriesUsed.PlutoniumBatteries)
          GD.c.currentActor.inventory.Add(AllObjects.GetItem(ItemEnum.PlutoniumBatteries));
        GD.c.batteriesUsed = BatteriesUsed.NoBatteries;
        UpdateInventory();
      }
      else {
        GD.c.currentActor.Say("I do not have a flashlight...");
      }

    }
    else {
      if (GD.c.currentRoom.HasLights()) { // Say not needed <<< --------------------------------------------------------------------------------
        GD.c.currentActor.Say("No need to consume the batteries.");
        GD.c.batteriesUsed = BatteriesUsed.NoBatteries;
        GD.flashLight = false;
      }
      else { // Use flashlight <<< --------------------------------------------------------------------------------
        GD.c.batteriesUsed = batteries;
        GD.c.FlashLightPanel.enabled = (batteries != BatteriesUsed.NoBatteries);
        GD.c.BackSky.SetActive(!GD.c.FlashLightPanel.enabled);
        GD.flashLight = true;
        GD.c.currentRoom.UpdateLights();
        if (batteries == BatteriesUsed.OldBatteries) GD.c.currentActor.inventory.Remove(AllObjects.GetItem(ItemEnum.OldBatteries));
        if (batteries == BatteriesUsed.Batteries) GD.c.currentActor.inventory.Remove(AllObjects.GetItem(ItemEnum.Batteries));
        if (batteries == BatteriesUsed.PlutoniumBatteries) GD.c.currentActor.inventory.Remove(AllObjects.GetItem(ItemEnum.PlutoniumBatteries));
        UpdateInventory();
      }
    }
  }


  public GameObject MapImage;
  public Image[] MapArrows;
  public float[] MapArrowsAngles; // (0)L=0 (1)R=180 (2)D=270 (3)tr=45 (4)tl=135
  int[] mapDirections;
  int mapPos = -1;
  int woodSteps = 0;

  void GenerateMap() {
    if (mapDirections == null) { // This is the first time the map is shown, generate the random path
      mapDirections = new int[5];

      for (int i = 0; i < 4; i++) {
        mapDirections[i] = Random.Range(0, 5);
        while (i > 0 && mapDirections[i] == mapDirections[i - 1])
          mapDirections[i] = Random.Range(0, 5);
      }
      mapPos = -1;
    }
    mapDirections[4] = Random.Range(0, 2);
    for (int i = 0; i < MapArrows.Length; i++) {
      MapArrows[i].rectTransform.rotation = Quaternion.Euler(0, 0, MapArrowsAngles[mapDirections[i]]);
    }
  }

  public void ShowMap() {
    GenerateMap();
    MapImage.SetActive(!MapImage.activeSelf);
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
    if (GameScenesManager.SkipScenes()) {
      Fader.RemoveFade();
    }
    CursorHandler.Set();
    DbgC("SelActor 1038");
    GD.c.usedItem = null;
    DbgU("SelActor 1029");
    GD.c.EnableActorSelection(false);

    GD.c.currentActor = actor;
    GD.c.ActorPortrait1.GetComponent<RawImage>().color = GD.c.unselectedActor;
    GD.c.ActorPortrait2.GetComponent<RawImage>().color = GD.c.unselectedActor;
    GD.c.ActorPortrait3.GetComponent<RawImage>().color = GD.c.unselectedActor;
    if (actor == GD.c.actor1 && !GD.c.actor1.dead) {
      GD.c.ActorPortrait1.GetComponent<RawImage>().color = GD.c.selectedActor;
    }
    else if (actor == GD.c.actor2 && !GD.c.actor1.dead) {
      GD.c.ActorPortrait2.GetComponent<RawImage>().color = GD.c.selectedActor;
    }
    if (actor == GD.c.actor3 && !GD.c.actor1.dead) {
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
    if (actor != null) {
      if (!actor.IsVisible || !actor.gameObject.activeSelf) return false;
      if (actor == GD.c.currentActor) return true;
    }
    GD.c.overActor = actor;
    if (actor != null && GD.c.usedItem != null)
      CursorHandler.Set(CursorTypes.Normal, CursorTypes.Give, GD.c.usedItem);
    else
      CursorHandler.Set(CursorTypes.Normal, CursorTypes.Normal, GD.c.usedItem);
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
    actor.SetScaleAndPosition(door.correspondingDoor.HotSpot);
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
    CursorHandler.Set();
    overItem = null;
    ShowName(currentRoom.name);

    if (currentRoom.ID.Equals("Woods")) {
      GenerateMap(); // It will do nothing if the map is already generated
      if (currentActor.HasItem(ItemEnum.WoodsMap)) {
        Debug.Log("Generating woods, first step");
        mapPos = 0;
        woods.Generate(true, false, mapDirections[mapPos]);
        woods.SetActorRandomDoorPosition(actor, mapDirections[mapPos]);
      }
      else {
        Debug.Log("Generating woods without map");
        mapPos = -1;
        woods.Generate(true, false, -1);
        woods.SetActorRandomDoorPosition(actor, -1);
      }
      woodSteps = 0;
      StarsBlink.SetWoods(woodSteps);
    }
    else if (currentRoom.ID.Equals("Cemetery") && !cemetery.Generated) {
      cemetery.Generate(false, false, -2);
      StarsBlink.SetWoods(4);
    }
    else {
      StarsBlink.SetWoods(0);
    }

    if (currentRoom.ID.Equals("Pool")) { // Code specific for the pool
      Item water = AllObjects.GetItem(ItemEnum.PoolWater);
      Item watervalve = AllObjects.GetItem(ItemEnum.PoolValve);
      Animator anim = water.GetComponent<Animator>();
      if (watervalve.IsOpen()) {
        AnimatorStateInfo si = anim.GetCurrentAnimatorStateInfo(0);
        if (si.IsName("Pool Water Animation Up") || si.IsName("Pool Water Animation Idle Up") || si.IsName("Pool Water Animation Idle")) {
          anim.Play("Pool Water Animation Idle Down");
        }
      }
      else {
        AnimatorStateInfo si = anim.GetCurrentAnimatorStateInfo(0);
        if (si.IsName("Pool Water Animation Down") || si.IsName("Pool Water Animation Idle Down") || si.IsName("Pool Water Animation Idle")) {
          anim.Play("Pool Water Animation Idle Up");
        }
      }
    }

    BlackFade.color = new Color32(0, 0, 0, 0);
  }

  bool CameraFadingToActor = false;

  public IEnumerator FadeToRoomActor() {
    if (currentRoom == null || currentActor == null || currentActor.currentRoom == null) yield break;
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
    Vector2 cpos = cam.WorldToScreenPoint(currentActor.transform.position);
    if (cpos.x < .3f * Screen.width) {
      if (dstp.x > currentRoom.minL) {
        dstp -= cam.transform.right * (.3f * Screen.width - cpos.x);
      }
    }
    else if (cpos.x > .7f * Screen.width) {
      if (dstp.x < currentRoom.maxR) {
        dstp += cam.transform.right * (cpos.x - .7f * Screen.width);
      }
    }
    if (dstp.x < currentRoom.minL) {
      dstp.x = currentRoom.minL;
    }
    else if (dstp.x > currentRoom.maxR) {
      dstp.x = currentRoom.maxR;
    }
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
    CursorHandler.Set();
    overItem = null;
    CameraFadingToActor = false;

    if (currentRoom.ID.Equals("Pool")) { // Code specific for the pool
      Item water = AllObjects.GetItem(ItemEnum.PoolWater);
      Item watervalve = AllObjects.GetItem(ItemEnum.PoolValve);
      Animator anim = water.GetComponent<Animator>();
      if (watervalve.IsOpen()) {
        AnimatorStateInfo si = anim.GetCurrentAnimatorStateInfo(0);
        if (si.IsName("Pool Water Animation Up") || si.IsName("Pool Water Animation Idle Up") || si.IsName("Pool Water Animation Idle")) {
          anim.Play("Pool Water Animation Idle Down");
        }
      }
      else {
        AnimatorStateInfo si = anim.GetCurrentAnimatorStateInfo(0);
        if (si.IsName("Pool Water Animation Down") || si.IsName("Pool Water Animation Idle Down") || si.IsName("Pool Water Animation Idle")) {
          anim.Play("Pool Water Animation Idle Up");
        }
      }
    }
    BlackFade.color = new Color32(0, 0, 0, 0);
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

  public Woods woods;
  public Woods cemetery;

  private IEnumerator ChangeWoods(Actor actor, Item door) {
    // Fade out
    float time = 0;
    while (time < .125f) {
      // Fade black
      BlackFade.color = new Color32(0, 0, 0, (byte)(255 * (time * 8)));
      time += Time.deltaTime;
      yield return null;
    }

    // Do we have the map and the path is the right one?
    bool goodPath = mapPos >= 0 && mapPos < 5 && mapDirections[mapPos] == woods.GetDoorPosition(door);

    Debug.Log("g=" + goodPath + " pos=" + mapPos + " gdp=" + woods.GetDoorPosition(door) + " " + mapDirections[0] + ", " + mapDirections[1] + ", " + mapDirections[2] + ", " + mapDirections[3] + ", " + mapDirections[4]);

    if (goodPath && currentActor.HasItem(ItemEnum.WoodsMap)) {
      //  yes-> check if we are following the right path, and be sure to generate the correct direction
      mapPos++;
      woods.Generate(Random.Range(0, 3) == 0, mapPos == 4, mapDirections[mapPos]);
      woods.SetActorRandomDoorPosition(actor, mapDirections[mapPos]); // Place the actor on any of the doors except the one that is the right direction
    }
    else {
      //  no->do not generate the cemetery, after a few steps say something
      woods.Generate(Random.Range(0, 3) == 0, false, -1);
      woods.SetActorRandomDoorPosition(actor, -1); // Place the actor on any of the non valid but visible doors
    }

    // Fade in
    while (time < .25f) {
      // Fade black
      BlackFade.color = new Color32(0, 0, 0, (byte)(255 * (1 - (8 * (time - .125f)))));
      time += Time.deltaTime;
      yield return null;
    }

    // Enable gameplay
    CursorHandler.Set();
    overItem = null;
    ShowName(currentRoom.name);

    if (woodSteps > 3 && Random.Range(0, 11 - woodSteps) == 0) {
      currentActor.Say("I am getting lost.\nBetter to go back...");
    }

    StarsBlink.SetWoods(woodSteps);
    woodSteps++;
    if (woodSteps > 10) woodSteps = 10;
  }

  #endregion

  #region *********************** UI and Options *********************** UI and Options *********************** UI and Options *********************** UI and Options ***********************
  public static float walkSpeed;
  public static float textSpeed;
  public static int inventoryMode = 0;
  public static int c64mode;
  public Options options;

  public TextMeshProUGUI TextMsg;
  public RectTransform TextMsgRT;
  public RectTransform TextBackRT;
  TextMsgMode txtMsgMode = TextMsgMode.None;
  float textMsgTime = 0;
  float textSizeX = 0;

  public GameObject ActorsButtons;

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


public enum SceneStatus {
  NoScenes, NonSkippableCutscene, SkippableCutscene, SkippedCutscene, BackgroundScenes
}