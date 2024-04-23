#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using System.Linq;
using UnityEngine;

namespace CrowdControlSampleGame
{
    public class DependencyChecker
    {
        static ListRequest listRequest;

        [InitializeOnLoadMethod]
        static void CheckDependencies()
        {
            listRequest = Client.List();
            EditorApplication.update += Progress;
        }

        static void Progress() {
            if (listRequest.IsCompleted) {
                EditorApplication.update -= Progress;
                if (listRequest.Status == StatusCode.Success) {
                    var vectorGraphicsPackage = listRequest.Result.FirstOrDefault(pkg => pkg.name == "com.unity.vectorgraphics");
                    if (vectorGraphicsPackage == null) {
                        Debug.LogError("The Unity Vector Graphics package is not installed. Please install it via the Unity Package Manager. " +
                            "Click the '+' button at the top-left of the Package Manager, select 'Add package from git URL...', " +
                            "and enter 'com.unity.vectorgraphics'.");

                        // Show a dialog with simplified instructions
                        EditorUtility.DisplayDialog("Dependency Missing",
                            "The Unity Vector Graphics package is not installed. Please install it via the Unity Package Manager. " +
                            "To do this, click the '+' button at the top-left of the Package Manager, choose 'Add package from git URL...', " +
                            "and use the URL 'com.unity.vectorgraphics'.",
                            "OK");
                    }
                }
                else if (listRequest.Status >= StatusCode.Failure) {
                    Debug.LogError("Error checking packages: " + listRequest.Error.message);
                }
            }
        }
    }
}
#endif
