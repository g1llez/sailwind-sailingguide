# CaptureOrtho (MelonLoader)

Presets T1 pour sessions capture ortho — **MelonLoader seulement** (pas BepInEx).

| Touche | Action |
|--------|--------|
| **F8** | Environnement : midi figé, god mode, hints off, vent/météo/vagues calmes, eau sans reflets |
| **F9** | Nord en haut + ortho (`orthographic` size 250, Crest off) — remplace la boussole |
| **F10** | Toggle Crest ocean ON/OFF (screenshots shallow / island) |

Screenshots : **CinematicUnityExplorer** (manuel). Freecam / clip plane / recul : manuel.

F8/F9 utilisent le clavier Windows direct (CUE intercepte `UnityEngine.Input`).
F9 cible la freecam CUE (`ourCamera`) si active, sinon caméra non-VR détectée.
Vagues : `SailwindWavesInertial` + `SailwindWindChange` (`OceanWaveSpectrum._multiplier = 0`).

## Build

```powershell
cd "D:\Games\Sailwind\mods\SailingGuide\dev\plugin\capture_ortho"
.\build.ps1
```

Déploie vers `Sailwind/Mods/CaptureOrtho.dll`.

## Session T1

1. Désinstaller BepInEx, installer MelonLoader
2. Charger la save île, activer freecam CinematicUnityExplorer (mode New ou Gameplay)
3. **F8** → survoler l'île → **F9** (nord + ortho) → **F10** selon shots → captures CUE
4. ALT-F4 sans sauvegarder, retirer MelonLoader, remettre BepInEx

## Config

`UserData/MelonPreferences.cfg` — section `CaptureOrtho` (`OrthographicSize`, touches).
