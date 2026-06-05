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

Each **guide** = one scroll in game (list of PNGs in config). File order = `01-`, `02-`, … in the guide folder.

## Config

`BepInEx/config/gillez.sailingguide.cfg`:

```ini
[Guide]
PagesDirectory = pages/en
PageFileNames = alankh/01-cover.png,alankh/02-legend.png,...
```

For French in-game, set `PagesDirectory = pages/fr` (or install the FR release zip).

## Releases (EN / FR)

Same `SailingGuide.dll` in both packages; only the `pages/<lang>/` tree differs.

| Package | Contents |
|---------|----------|
| **EN** | DLL + `pages/en/**` |
| **FR** | DLL + `pages/fr/**` |

Optional later: a **pages-only** FR add-on zip for users who already have the EN plugin.

Authoring sources: `dev/Assets/guides/archipelago/<id>/pages/en/svg/`.
