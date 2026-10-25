using UnityEngine;

// Minimal bench to compare macOS fullscreen cursor behaviour against Godot (godotengine/godot#76038).
// Bootstraps itself, so the scene can stay empty.
public class CursorTest : MonoBehaviour {
	private Texture2D _magenta;
	private CursorMode _mode = CursorMode.Auto;
	private string _last = "-";

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	private static void Bootstrap() {
		var go = new GameObject("CursorTest");
		go.AddComponent<CursorTest>();
		DontDestroyOnLoad(go);
	}

	private void Awake() {
		Application.runInBackground = true;
		Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
		_magenta = MakeCircle(48, new Color(1f, 0f, 1f, 1f));
		Apply();
	}

	// Same unmistakable magenta disc used in the Godot bench.
	private static Texture2D MakeCircle(int size, Color color) {
		var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
		float r = size / 2f - 2f;
		for (int y = 0; y < size; y++) {
			for (int x = 0; x < size; x++) {
				float dx = x - size / 2f, dy = y - size / 2f;
				tex.SetPixel(x, y, dx * dx + dy * dy <= r * r ? color : Color.clear);
			}
		}
		tex.Apply();
		return tex;
	}

	private void Apply() {
		Cursor.SetCursor(_magenta, new Vector2(24, 24), _mode);
	}

	private void Update() {
		if (Input.GetKeyDown(KeyCode.Escape)) { Application.Quit(); }

		if (Input.GetKeyDown(KeyCode.Alpha1)) { Screen.fullScreenMode = FullScreenMode.Windowed; _last = "Windowed"; }
		if (Input.GetKeyDown(KeyCode.Alpha2)) { Screen.fullScreenMode = FullScreenMode.FullScreenWindow; _last = "FullScreenWindow"; }
		if (Input.GetKeyDown(KeyCode.Alpha3)) { Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen; _last = "ExclusiveFullScreen"; }

		if (Input.GetKeyDown(KeyCode.V)) { Cursor.visible = true; Cursor.lockState = CursorLockMode.None; _last = "visible"; }
		if (Input.GetKeyDown(KeyCode.H)) { Cursor.visible = false; _last = "hidden"; }
		if (Input.GetKeyDown(KeyCode.C)) { Cursor.lockState = CursorLockMode.Locked; _last = "locked"; }
		if (Input.GetKeyDown(KeyCode.N)) { Cursor.lockState = CursorLockMode.Confined; _last = "confined"; }

		// Hardware cursor is the path comparable to Godot's NSCursor use.
		// Software draws it in-game, which would hide the bug entirely.
		if (Input.GetKeyDown(KeyCode.M)) {
			_mode = _mode == CursorMode.Auto ? CursorMode.ForceSoftware : CursorMode.Auto;
			Apply();
			_last = "cursor mode " + _mode;
		}
		if (Input.GetKeyDown(KeyCode.R)) { Apply(); _last = "re-applied cursor"; }
	}

	private void OnGUI() {
		GUI.skin.label.fontSize = 22;
		GUI.Label(new Rect(40, 40, 1400, 400),
			"Unity macOS cursor bench\n\n" +
			"unity        : " + Application.unityVersion + "\n" +
			"screen mode  : " + Screen.fullScreenMode + "\n" +
			"resolution   : " + Screen.width + "x" + Screen.height + "\n" +
			"cursor mode  : " + _mode + "   visible: " + Cursor.visible + "   lock: " + Cursor.lockState + "\n" +
			"last action  : " + _last + "\n\n" +
			"1 windowed   2 fullscreen window   3 exclusive fullscreen\n" +
			"V visible    H hidden   C locked   N confined\n" +
			"M hardware/software cursor    R re-apply cursor    ESC quit\n\n" +
			"Brush the menu bar at the top, then move back down.");
	}
}
