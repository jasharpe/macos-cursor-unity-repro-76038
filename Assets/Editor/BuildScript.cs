using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class BuildScript {
	// Invoked from the command line with -executeMethod BuildScript.Build.
	public static void Build() {
		const string scenePath = "Assets/Main.unity";

		var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
		EditorSceneManager.SaveScene(scene, scenePath);

		PlayerSettings.companyName = "CursorTest";
		PlayerSettings.productName = "UnityCursorTest";
		PlayerSettings.runInBackground = true;
		PlayerSettings.defaultIsNativeResolution = true;
		PlayerSettings.fullScreenMode = FullScreenMode.ExclusiveFullScreen;

		var options = new BuildPlayerOptions {
			scenes = new[] { scenePath },
			locationPathName = "Build/UnityCursorTest.app",
			target = BuildTarget.StandaloneOSX,
			targetGroup = BuildTargetGroup.Standalone,
			options = BuildOptions.None,
		};

		var report = BuildPipeline.BuildPlayer(options);
		Debug.Log("BUILD RESULT: " + report.summary.result + " errors=" + report.summary.totalErrors);
		EditorApplication.Exit(report.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded ? 0 : 1);
	}
}
