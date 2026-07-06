# Material Property Cleaner

**Menu:** Tools → Material Property Cleaner

When a material's shader changes, values for properties the new shader doesn't use stay serialized in the material forever. This tool shows them and cleans them up.

## Usage

Select one or more materials in the Project window — each appears as a card with its properties grouped by type (Textures, Ints, Floats, Colors) and a status badge:

- **Exists** — the shader still uses this property.
- **Old Reference** — the shader no longer has it; a **Remove** button appears next to it.
- **Unknown** — the material has no valid shader.

Toolbar actions apply to all selected materials:

- **Remove All Old** / per-type buttons — bulk-remove stale properties.
- **Transfer Floats → Ints** — copies a saved Float value into the Int property of the same name (rounded) and deletes the Float. Useful after a shader property changed its type from Float to Int.