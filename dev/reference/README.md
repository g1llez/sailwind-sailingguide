# Game reference assets (local only)

Vanilla maps and scroll pages **must not** be committed to this public repository. They are copyrighted Sailwind / developer material.

## Extract on your machine

1. Own [Sailwind](https://store.steampowered.com/) and install [BepInEx](https://docs.bepinex.dev/).
2. Use [SailwindHack](../../SailwindHack/) in-game to export scrolls/maps.
3. Copy exports into:

   `dev/_workspace/reference/ingame/<game-version>/`

   Example layout (after copy from `BepInEx/plugins/SailwindHack/extracted/`):

   ```
   ingame/0.36/
     maps/     map_alankh.png, ports_on_maps.txt, …
     scrolls/  slot_*_page*.png, manifest.txt, …
   ```

Use the **game version** folder name (e.g. `0.36`), not a date.

That folder is gitignored via `dev/_workspace/`. Keep it for layout reference and icon alignment only.
