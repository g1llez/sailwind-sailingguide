# dev/Assets/

Tracked authoring files (not loaded by the game directly).

```
Assets/
  shared/                 Icons (`icons/final/`), FONTS.md pointer
  guides/
    archipelago/
      <id>/               e.g. alankh
        pages/
          en/svg/         Parchemin page masters (EN)
          fr/svg/         (future) French masters
        maps/
          <island>/       Island map SVGs (e.g. neverdin/)
```

Export 2048×2048 PNGs to `pages/en/<id>/` (runtime). Version suffix lives on PNG exports (`_v0.1.0`), not on SVG masters.

Notes, fonts, WIP: `../_workspace/`.
