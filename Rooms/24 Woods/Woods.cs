using UnityEngine;

public class Woods : MonoBehaviour {
  public Tree[] Trees; // Border trees
  public Grass[] Grass;
  public WoodsDoor[] doors;
  public Room room;
  public Tree[] MiddleTrees;

  private void Start() {

  }

  private void Update() {
    if (Input.GetKeyDown(KeyCode.Space)) {
      Generate((uint)Random.Range(0, 2048));
    }
  }

  public void Generate(uint key) {
    foreach (Tree t in MiddleTrees)
      if (t.gameObject.activeSelf) t.Randomize(room.minY, room.maxY, room.scalePerc, 3, 2);

    foreach (Tree t in Trees)
      if (t.gameObject.activeSelf) t.Randomize(room.minY, room.maxY, room.scalePerc);
    foreach (Grass g in Grass)
      if (g.gameObject.activeSelf)
        g.Randomize(room.minY, room.maxY, room.scalePerc);

    foreach (WoodsDoor wd in doors) {
      wd.Generate(key & 3, room.minY, room.maxY, room.scalePerc);
      key = key >> 2;
    }

    /*
      2 bits for each "door": 0=no door, 1=next random, 2=home, 3=cemetery
      a few bits for the trees in the center (at least 5 positions)



     
     */


  }

}

