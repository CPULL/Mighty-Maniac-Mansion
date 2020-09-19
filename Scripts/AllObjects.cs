using System.Collections.Generic;
using UnityEngine;

public class AllObjects : MonoBehaviour {
  public List<GameItem> itemsList;
  public List<Room> roomsList;

  internal Room GetRoom(string id) {
    foreach (Room r in roomsList)
      if (r.ID == id) {
        return r;
      }
    Debug.LogError("Cannot find room with id: \"" + id + "\"");
    return null;
  }

  internal Item FindItemByID(ItemEnum id) {
    Debug.Log("Searching for: " + id);
    foreach (Item i in itemsList) {
      if (i.Item == id) {
        Debug.Log("Found: " + i);
        return i;
      }
    }
    Debug.LogError("Cannot find Item with id: \"" + id + "\"");
    return null;
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




}

