# Rename Objects

**Menu:** Tools → RenameObjectsTool · **Shortcut:** ``Shift+` ``

Batch-renames whatever is selected in the Hierarchy or Project window. Every rule is applied live — the **Preview** section shows `Old Name → New Name` for each object before you commit.

## Rules

Applied in this order:

1. **Add** — prefix and/or suffix.
2. **Replace** — replace all occurrences of a substring.
3. **Insert** — insert text before or after the first occurrence of an anchor string.
4. **Options** — convert to lowercase; trim the last word (cuts everything after the last `,` `.` ` ` `_`).
