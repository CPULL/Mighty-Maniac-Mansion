using UnityEngine;

public class Woods : MonoBehaviour {
  public Tree[] Trees; // Border trees
  public Grass[] Grass;
  public WoodsDoor[] doors;
  public Room room;
  public Tree[] MiddleTrees;
  readonly byte[] spots = new byte[5]; // 0=no door, 1=next random, 2=home, 3=cemetery
  public bool Generated = false;


  public void Generate(bool home, bool cemetery, int nextDirection) {
    Generated = true;
    foreach (Tree t in MiddleTrees)
      if (t.gameObject.activeSelf) t.Randomize(room.minY, 3, 2);

    foreach (Tree t in Trees)
      if (t.gameObject.activeSelf) t.Randomize(room.minY);
    foreach (Grass g in Grass)
      if (g.gameObject.activeSelf)
        g.Randomize(room.minY);


    if (nextDirection == -2) return; // Not needed, it is the cemetery


    // (0)L=0 (1)R=180 (2)D=270 (3)tr=45 (4)tl=135

    // Randomize the spots. Have 3, 4, or 5 doors
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
    // If nextDirection is specified set it
    if (nextDirection != -1) spots[nextDirection] = 1;
    if (cemetery) spots[nextDirection] = 3;

    pos = 0;
    foreach (WoodsDoor wd in doors) {
      wd.Generate(spots[pos++], room.minY, room.maxY, room.scalePerc);
    }

  }

  public void SetActorRandomDoorPosition(Actor actor, int nextDirection) {
    actor.Stop();
    int pos = Random.Range(0, 5);
    while ((spots[pos] != 1 && spots[pos] != 2) || pos == nextDirection)
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

  public int GetDoorPosition(Item door) {
    if (door == null) return -99;
    for (int i = 0; i < doors.Length; i++) {
      if (doors[i].DoorFake == door || doors[i].DoorHome == door || doors[i].DoorCemetery == door) return i;
    }
    return -99;
  }
}

