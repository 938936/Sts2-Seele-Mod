# AGENTS.md

## Project Overview

Slay the Spire 2 character mod for 希儿 (Seele/Selee). C# / Godot 4.5.1 / .NET 9.0. Uses [BaseLib](https://alchyr.github.io/BaseLib-Wiki/docs/Features.html) modding framework and Harmony patching.

## Build & Run

- **Build**: `dotnet build` — auto-copies .dll, .pdb, and Selee.json to the game's mods folder
- **Publish**: `dotnet publish` — requires `GodotPath` set in `Directory.Build.props`; exports .pck via Godot headless
- **Prerequisite**: StS2 must be installed; `Sts2PathDiscovery.props` auto-discovers it from Steam. If not found, set `Sts2Path` manually in `Directory.Build.props`
- The game's `sts2.dll` and `0Harmony.dll` are referenced from `Sts2DataDir` (not packaged)
- `Krafs.Publicizer` makes originally private/protected game members accessible

## Architecture

```
SeleeCode/              # All C# mod code (namespace: Selee.SeleeCode.*)
  MainFile.cs           # Mod entry point — [ModInitializer], creates Harmony instance, PatchAll()
  Character/
    Selee.cs            # Character definition (PlaceholderCharacterModel), starting deck/relics/pools
    SeleeCardPool.cs    # Card pool (CustomCardPoolModel) — color, energy icons
    SeleeRelicPool.cs   # Relic pool
    SeleePotionPool.cs  # Potion pool
  Cards/
    SeleeCard.cs        # Abstract base for all cards — image paths, [Pool] annotation
  Powers/
    SeleePower.cs       # Abstract base for all powers — image paths, Type/StackType abstracts
  Relics/
    SeleeRelic.cs       # Abstract base for all relics — image paths, [Pool] annotation
  Potions/
    SeleePotion.cs      # Abstract base for all potions
  Patch/
    SeleeHook.cs        # Custom hook interface (ISeleeHook) for cross-entity interactions
    SeleeCardKeyword.cs # Custom card keywords via [CustomEnum]
  Extensions/
    StringExtensions.cs # Asset path helpers (ImagePath, CardImagePath, PowerImagePath, etc.)

Selee/                  # Godot resources (excluded from C# compilation)
  images/               # card_portraits/, powers/, relics/, charui/, scenes/
  localization/
    zhs/                # Chinese localization JSONs (cards, powers, relics, etc.)
    eng/                # English localization JSONs

docs/
  开发经验.md            # Hard-won implementation notes (read before implementing unknown effects)
```

## Key Conventions

- **Naming**: Chinese names → pinyin (e.g. 量子叠加 → `LiangZiDieJia`). Power classes must have `Power` suffix
- **Localization keys**: Class name → UPPER_SNAKE_CASE with `SELEE-` prefix (e.g. `TongYao` → `SELEE-TONG_YAO`)
- **Base classes**: All mod entities inherit from `SeleeCard`, `SeleePower`, `SeleeRelic`, or `SeleePotion` — these handle image paths and pool registration automatically
- **Pool registration**: `[Pool]` attribute on base classes auto-registers all derived types; no manual pool setup needed
- **Image paths**: Handled by `StringExtensions` methods; fallback images exist for missing assets

## Localization

- Both `zhs/` and `eng/` must be updated for every new entity
- Power localization requires **three** keys: `title`, `description`, `smartDescription` — missing `smartDescription` causes build error `STS001`
- Dynamic var format: `{VarName:diff()}` for values, `{Amount}` for power stack count
- `energyIcons()` **must** have a count argument: `{Energy:energyIcons(1)}` (not `energyIcons()`)

## External References

- Game source (read-only): `C:\Users\joyce\Documents\sts_mods\sts2_src\src\Core\Models\` — reference for how vanilla cards/powers/relics work
- Game localization: `C:\Users\joyce\Documents\sts_mods\sts2_src\localization\zhs\` — find vanilla entity names by description
- BaseLib source: `C:\Users\joyce\Documents\sts_mods\sts2_lib\BaseLib-StS2-master\` — mod API implementations
- When implementing unknown effects: check `docs/开发经验.md` first, then vanilla code