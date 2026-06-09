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
PageFileNames = alankh/01-cover.png,alankh/02-legend.png,alankh/15-lionsfang.png,alankh/18-neverdin-1.png,alankh/19-neverdin-2.png,alankh/20-oldankhtown-1.png,alankh/21-oldankhtown-2.png,alankh/22-oldankhtown-3.png
ScrollDisplayName = The Sailor's Diary — Al'Ankh
```

Use `pages/fr` + French `PageFileNames` for the FR package (same file names under `fr/alankh/`).

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
