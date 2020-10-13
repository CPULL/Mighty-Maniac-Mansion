using System.Collections.Generic;
using UnityEngine;

public class AllObjects : MonoBehaviour {
  public List<Item> itemsList;
  public List<Room> roomsList;
  public List<FlagStatus> flagsList;

  private void Awake() {
    GD.a = this;

    flagsList = new List<FlagStatus>();
    foreach (GameFlag gf in System.Enum.GetValues(typeof(GameFlag)))
      flagsList.Add(new FlagStatus(gf, FlagValue.No));
  }

  internal Room GetRoom(string id) {
    foreach (Room r in roomsList)
      if (r.ID == id) {
        return r;
      }
    Debug.LogError("Cannot find room with id: \"" + id + "\"");
    return null;
  }

  internal Item FindItemByID(ItemEnum id) {
    foreach (Item i in itemsList) {
      if (i.Item == id) {
        return i;
      }
    }
    Debug.LogWarning("Cannot find Item with id: \"" + id + "\"");
    return null;
  }

  internal Item FindItemByID(string sid) {
    string val = sid.Trim().ToLowerInvariant();
    ItemEnum id = ItemEnum.Undefined;
    if (val == "sign") id = ItemEnum.Sign;
    if (val == "genericinvisibledoor") id = ItemEnum.GenericInvisibleDoor;
    if (val == "mailbox") id = ItemEnum.Mailbox;
    if (val == "mailboxflag") id = ItemEnum.MailboxFlag;
    if (val == "frontdoorkey") id = ItemEnum.FrontDoorKey;
    if (val == "grass") id = ItemEnum.Grass;
    if (val == "doorbell") id = ItemEnum.DoorBell;
    if (val == "doormat") id = ItemEnum.Doormat;
    if (val == "grate") id = ItemEnum.Grate;
    if (val == "basementpassage") id = ItemEnum.BasementPassage;
    if (val == "tedgrave") id = ItemEnum.TedGrave;
    if (val == "frontdoor") id = ItemEnum.FrontDoor;

    if (id == ItemEnum.Undefined) return null;
    return FindItemByID(id);
  }

  internal bool CheckFlag(GameFlag flag, FlagValue value) {
    foreach (FlagStatus fs in flagsList)
      if (fs.flag == flag) return fs.value == FlagValue.NA || fs.value == value;
    return false;
  }
}


/// <summary>
/// List of all items that are in game and can be referenced by actions by the ID (the ID is this enum value)
/// </summary>
public enum ItemEnum {
  Undefined,
  Sign,
  GenericInvisibleDoor,
  Mailbox,
  MailboxFlag,
  FrontDoorKey,
  Grass,
  DoorBell,
  Doormat,
  Grate,
  BasementPassage,
  TedGrave,
  FrontDoor,
  Flashlight,
  Cheese,
  Batteries,
  Hamster,
  PiggyBank,
  Coin,
  DeveloperBottle,

}

/// <summary>
/// List of all the actions that have an ID (mostly sequences)
/// </summary>
public enum ActionEnum {
  NoAction,
  Intro,
  Doorbell,
}



public enum GameFlag {
  GreenTentacleIsFed,
  LightIsOut,
  PoolHasWater,
  PowerIsDown,
  PackageIsDeliveredStamps,
}
public enum FlagValue {
  Yes, No, NA
}

public class FlagStatus {
  public GameFlag flag;
  public FlagValue value;

  public FlagStatus(GameFlag f, FlagValue v) {
    flag = f;
    value = v;
  }
}