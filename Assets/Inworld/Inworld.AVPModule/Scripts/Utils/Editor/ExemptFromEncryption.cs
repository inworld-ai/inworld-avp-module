using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.iOS.Xcode;

// Modified from https://gist.github.com/TiborUdvari/4679d636b17ddff0d83065eefa399c04
public class ExemptFromEncryption : IPostprocessBuildWithReport // Will execute after XCode project is built
{
    public int callbackOrder {
        get { return 0; }
    }

    public void OnPostprocessBuild(BuildReport report)
    {
        if (report.summary.platform == BuildTarget.iOS ||
            report.summary.platform == BuildTarget.VisionOS) // Check if the build is for iOS / VisionOS
        {
            string plistPath = report.summary.outputPath + "/Info.plist";

            PlistDocument plist = new PlistDocument(); // Read Info.plist file into memory
            plist.ReadFromString(File.ReadAllText(plistPath));

            PlistElementDict rootDict = plist.root;
            rootDict.SetBoolean("ITSAppUsesNonExemptEncryption", false);

            File.WriteAllText(plistPath, plist.WriteToString()); // Override Info.plist
        }
    }
}