using System.Collections;
using UnityEngine;

public class Woods : MonoBehaviour {
  public Tree[] Trees; // Border trees
  public Grass[] Grass;
  public WoodsDoor[] doors;
  public Room room;
  public Tree[] MiddleTrees;
  byte[] spots = new byte[5]; // 0=no door, 1=next random, 2=home, 3=cemetery
  public bool GenerationAtStart = false;

  private void Start() {
    if (GenerationAtStart) StartCoroutine(DelayedGeneration());
  }

  IEnumerator DelayedGeneration() {
    yield return new WaitForSeconds(1);
    Generate(false, false);
  }

  public void Generate(bool home, bool cemetery) {
    foreach (Tree t in MiddleTrees)
      if (t.gameObject.activeSelf) t.Randomize(room.minY, room.maxY, room.scalePerc, 3, 2);

    foreach (Tree t in Trees)
      if (t.gameObject.activeSelf) t.Randomize(room.minY, room.maxY, room.scalePerc);
    foreach (Grass g in Grass)
      if (g.gameObject.activeSelf)
        g.Randomize(room.minY, room.maxY, room.scalePerc);


    if (GenerationAtStart) return; // Not needed, it is the cemetery


    // Randomize the spots.
    // Have 3, 4, or 5 doors
    // If home or cemetery are specified, set one of them
    int pos;
    int num = Random.Range(3, 6);
    if (home) num--;
    if (cemetery) num--;
    for (int i = 0; i < 5; i++) {
      spots[i] = (byte)((i < num) ? 1 : 0);
    }
    for (int i = 0; i < 50; i++) {
      int a = Random.Range(0, 5);
      int b = Random.Range(0, 5);
      byte tmp = spots[a];
      spots[a] = spots[b];
      spots[b] = tmp;
    }
    if (home && cemetery) {
      if (Random.Range(0, 2) == 0) {
        spots[0] = 2;
        spots[1] = 3;
      }
      else {
        spots[0] = 3;
        spots[1] = 2;
      }
    }
    if (home) spots[Random.Range(0, 2)] = 2;
    if (cemetery) spots[Random.Range(0, 2)] = 3;

    pos = 0;
    foreach (WoodsDoor wd in doors) {
      wd.Generate(spots[pos++], room.minY, room.maxY, room.scalePerc);
    }

  }

  public void SetActorRandomDoorPosition(Actor actor) {
    actor.Stop();
    int pos = Random.Range(0, 5);
    while (spots[pos] != 1 && spots[pos] != 2)
      pos = Random.Range(0, 5);
    actor.SetScaleAndPosition(doors[pos].DoorFake.HotSpot);
    switch(doors[pos].DoorFake.dir) {
      case Dir.B: actor.SetDirection(Dir.F); break;
      case Dir.F: actor.SetDirection(Dir.B); break;
      case Dir.L: actor.SetDirection(Dir.R); break;
      case Dir.R: actor.SetDirection(Dir.L); break;
      case Dir.None: actor.SetDirection(Dir.None); break;
    }
  }
}

