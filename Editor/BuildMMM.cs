using UnityEditor;
using System.Diagnostics;

public class BuildMMM {
  [MenuItem("Build/Windows Build")]
  public static void BuildGame() {
    // Get filename.
    string path = @"C:\Users\claud\Unity\MMM\Build\";
    string[] levels = new string[] { "Assets/Scenes/MainScene.unity" };

    // Build player.
    BuildPipeline.BuildPlayer(levels, path + "/MMM.exe", BuildTarget.StandaloneWindows64, BuildOptions.None);

    // Copy a file from the project folder to the build folder, alongside the built game.
    FileUtil.CopyFileOrDirectory("Assets/Actions/", path + "MMM_Data\\Actions\\");

    // Run the game (Process class from System.Diagnostics).
    Process proc = new Process();
    proc.StartInfo.FileName = path + "/MMM.exe";
    proc.Start();
  }
}

