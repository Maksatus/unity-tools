# Scene Selector

**Menu:** Tools → Scene Selector · **Shortcut:** ``Ctrl+` ``

A compact popup listing every scene in `Assets/`, grouped by folder.

## Usage

- **Search** — type to filter scenes by name. Empty folders are hidden.
- **Click a row** — opens the scene in *Single* mode. Unsaved changes prompt a save dialog first.
- **`+` button** — loads the scene *additively*, keeping the current ones.
- **`−` button** — unloads an additively loaded scene. Loaded scenes are highlighted in blue.
- **`≡` button** — toggles between folder grouping and a flat alphabetical list.

The list refreshes automatically when scenes are added, removed, or opened elsewhere in the editor.

## Exclusions

Hide scenes you never open by hand via **Project Settings → Unity Tools → Scene Selector**:

- **Excluded Folders** — path prefixes, e.g. `Assets/ThirdParty`.
- **Excluded Scenes** — a scene name (`Boot`) or a full path (`Assets/Scenes/Boot.unity`).

The lists are stored per project in `ProjectSettings/SceneSelectorSettings.asset`, so they can be committed and shared with the team.
