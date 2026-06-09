# SailingGuide — The Sailor's Diary

Pilot charts, island guides, and the BepInEx plugin that loads custom parchemin pages in-game.

| Part | Path |
|------|------|
| **Pages (runtime)** | `pages/en/`, `pages/fr/` |
| **Authoring** | `dev/Assets/guides/`, `dev/Assets/shared/` |
| **Plugin** | `dev/plugin/` → `SailingGuide.dll` |
| **Local WIP** | `dev/_workspace/` (gitignored) |

## Layout

```
SailingGuide/
  pages/
    en/<guide-id>/    PNG shipped in EN release
    fr/<guide-id>/    PNG shipped in FR release
  dev/
    Assets/guides/    SVG masters per scroll
    _workspace/       notes, fonts, vanilla refs
    reference/        how to extract game refs locally
    plugin/
  build.ps1
```

## Build & test

```powershell
cd "D:\Games\Sailwind\mods\SailingGuide"
.\build.ps1
```

- DLL → `BepInEx/plugins/SailingGuide/`
- Copies `pages/en/`, `pages/fr/`, … into the game `pages/` folder

Config: `BepInEx/config/gillez.sailingguide.cfg`

```ini
[Guide]
ReplaceTutorialScroll = true
PagesDirectory = pages/en
GuideId = alankh
PageFileNames =
ScrollDisplayName = The Sailor's Diary — Al'Ankh
```

**v0.1.1+** — leave `PageFileNames` empty to auto-load all `*.png` in `pages/<lang>/<GuideId>/`, sorted by filename (`01-cover.png`, `15-lionsfang.png`, …). Set an explicit comma-separated list to override order or exclude pages. Prefix WIP files with `_` to skip them.

Use `PagesDirectory = pages/fr` for French in-game (same filenames under `fr/alankh/`).

**v0.1.x** replaces the tutorial scroll (guide `alankh`). **v0.2.0+** = extra scrolls per guide (e.g. `seamanship`).

## EN / FR releases

Same DLL; different language tree:

| Zip | Contents |
|-----|----------|
| EN | `SailingGuide.dll` + `pages/en/**` |
| FR | `SailingGuide.dll` + `pages/fr/**` |

Default cfg uses `pages/en`. FR install sets `PagesDirectory = pages/fr`.

## License

MIT — see [LICENSE](LICENSE). Sailwind is not affiliated with this mod.
