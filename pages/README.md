# pages/ — parchemin PNGs (2048×2048)

Runtime content copied by `build.ps1` into `BepInEx/plugins/SailingGuide/pages/`.

## Layout

```
pages/
  en/                 English pages (release zip EN)
    alankh/           Archipelago guide — tutorial replacement in v0.1.x
    seamanship/       (future) Boat handling, docking, …
  fr/                 French pages (release zip FR)
    alankh/
    seamanship/
```

Each **guide** = one scroll in game. File order = sorted PNG filenames in the guide folder (`01-`, `02-`, `15-`, …).

## Config

`BepInEx/config/gillez.sailingguide.cfg`:

```ini
[Guide]
PagesDirectory = pages/en
GuideId = alankh
PageFileNames =
```

Empty `PageFileNames` = auto-scan `pages/<lang>/<GuideId>/*.png` (v0.1.1+). Add new island pages → run `build.ps1` only (no DLL rebuild, no config edit). Optional: explicit `PageFileNames` list to override.

For French in-game, set `PagesDirectory = pages/fr`.

## Releases (EN / FR)

Same `SailingGuide.dll` in both packages; only the `pages/<lang>/` tree differs.

| Package | Contents |
|---------|----------|
| **EN** | DLL + `pages/en/**` |
| **FR** | DLL + `pages/fr/**` |

Optional later: a **pages-only** FR add-on zip for users who already have the EN plugin.

Authoring sources: `dev/Assets/guides/archipelago/pages/<id>/` (`.svg` + `.txt`).
