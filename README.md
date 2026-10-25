# macos-cursor-unity-repro-76038

A minimal **Unity** reproduction of the macOS fullscreen cursor defect described
in [godotengine/godot#76038](https://github.com/godotengine/godot/issues/76038).

The point of this project is not Unity itself. It exists to show that the defect
is in **macOS**, not in Godot: an engine with a completely separate codebase
reproduces the same behaviour through the same operating system API.

The matching Godot reproduction is at
[jasharpe/macos-cursor-repro-76038](https://github.com/jasharpe/macos-cursor-repro-76038).

## The defect

Run a fullscreen build with a custom cursor. Move the pointer into the band at
the **top of the screen**, then move back down. The custom cursor (a magenta
disc) is replaced by the system arrow cursor, and it stays replaced.

The **menu bar is not involved**. You do not need to reach the top edge of the
screen or make the menu bar appear. In fullscreen the content view does not
cover the whole screen, and a band of approximately 37 points at the top
belongs to a window owned by macOS. Crossing that line and coming back is
enough.

macOS resets the cursor state of the application when the pointer leaves the
view at the top and returns, and sends no event for the change. This affects
both the cursor image and the visibility of the system cursor.

## Result

Reproduced with Unity `6000.3.22f1` on macOS 26, Apple Silicon, in a standalone
build:

| Cursor mode | Behaviour |
| --- | --- |
| `CursorMode.Auto` (hardware cursor, the default) | **Reverts to the system arrow cursor.** Same as Godot. |
| `CursorMode.ForceSoftware` | **Also affected, and arguably worse.** Unity keeps drawing its own cursor, but macOS also shows the system arrow cursor, so the screen has two cursors. |

Both modes fail, which shows that macOS resets more than the cursor image. A
software cursor needs the operating system cursor to stay hidden. macOS makes
the hidden cursor visible again when the pointer leaves the view at the top and
returns, and the application gets no event for this change either. Press `M` in the build to
switch between the two modes and compare.

## How to build and run

The Editor's Play mode is **not** sufficient. The defect needs a real fullscreen
application window, so a standalone build is required.

Open the project in Unity and use *File > Build Settings > macOS > Build*, or
build it from the command line:

```
/Applications/Unity/Hub/Editor/6000.3.22f1/Unity.app/Contents/MacOS/Unity \
  -quit -batchmode -nographics \
  -projectPath . \
  -executeMethod BuildScript.Build \
  -logFile -
```

This writes `Build/UnityCursorTest.app`. The build starts in exclusive
fullscreen.

| Key | Action |
| --- | --- |
| `1` `2` `3` | windowed / fullscreen window / exclusive fullscreen |
| `V` `H` `C` `N` | cursor visible / hidden / locked / confined |
| `M` | toggle hardware (`Auto`) and software (`ForceSoftware`) cursor |
| `R` | re-apply the cursor |
| `ESC` | quit |

Unity version, screen mode, resolution and cursor state are drawn on screen.

## Related reports

- [Chromium issue 430116472](https://issues.chromium.org/issues/430116472) —
  Chrome logged no `updateCursor` call when the cursor changed, and concluded
  the change comes from a window the application does not own.
- [Deskflow PR #10184](https://github.com/deskflow/deskflow/pull/10184) — the
  same defect on macOS 26, worked around with an undocumented menu bar
  notification.
- [Apple Developer Forums thread 803069](https://developer.apple.com/forums/thread/803069) —
  custom cursors revert when they touch the top screen edge in fullscreen
  applications.
