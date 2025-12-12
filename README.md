# Winter Holiday Mod

Bring the warmth of the festive season into Eco with a handcrafted collection of holiday decorations:

- **Inflatable Santa:** A small, chubby, and adorable inflatable that squeezes into any yard while still kicking in custom housing value.
- **Christmas Trees:** Four sizes of the same twinkly, garland-wrapped tree so you can layer decorations without juggling different aesthetics.
- **Gift Wraps:** Usable presents that act as single-item storage, perfect for hiding real gifts while boosting home scores.
- **Snowman Companion:** A jolly, snow-proof friend crafted from everyday tailoring and farming materials.

## Getting Started

1. Drop the `WinterHolidayMod` folder into your `Mods/UserCode/` directory (or link it, as in this repo).
2. Ensure `Translations/WinterHolidayItems.csv` is copied to your `Mods/Translations/` folder so the new strings load correctly.
3. Restart your Eco server or client; the new recipes appear in Tailoring Tables, Carpentry Tables, and Workbenches depending on item size.
4. Craft, place, and mix the decorations to design cozy plazas, store-front displays, or town centers with synchronized housing bonuses.

## Contributing

- **Translations:** Add columns to `Translations/WinterHolidayItems.csv` for your language and fill in the localized text.
- **Balancing:** Adjust home values or recipe costs directly in `WinterHolidayItems.cs`; every item uses dedicated helper methods for quick tuning.
- **Assets:** Update or replace meshes/textures inside `Assets/christmas_dremixam.unity3d`, then reference them in the corresponding world objects.

Happy holidays, and enjoy spreading cheer across your Eco world!
