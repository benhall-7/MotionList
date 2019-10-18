Note: this repository is deprecated; if you are looking for up-to-date file extraction tools, please use [motion_lib](https://github.com/ultimate-research/motion_lib) instead

# MotionList
Class structure for Smash Ultimate motion_list.bin files.

# MotionXML
Command line converter between motion_list <-> XML. Requires: .NET Core 2.1 runtime, available from Microsoft

## Filetype limitations
1) Animation count cannot exceed 3. Although it is unknown why any Motion would require > 1 animation, exactly one article in the game has 2 (although it is possiblly unused). The limitation is due to how the game code calculates the offset to skip the animation hashes and unk bytes following them.
2) The number of ExtraHash'es determines which types are expected. The types and corresponding groups they can be written as are described in the following enums:
```cs
public enum ExtraHashKind
{
    Expression,
    Sound,
    Effect,
    Game2,
    Expression2,
    Sound2,
    Effect2
}
public enum ExtraHashGroup
{
    None     = 0,
    F        = 8,
    SF       = 0x10,
    XSF      = 0x18,
    SFG2S2F2 = 0x28
}
```
3) Fighters require that you use the `XSF` group, they will all use this by default so you don't need to change anything. They also need the "Extended" section, which is the 4 last parameters: `XluStart`, `XluEnd`, `CancelFrame`, and `NoStopIntp`. If any one of these exists, the program will search for the rest; one requires all.
