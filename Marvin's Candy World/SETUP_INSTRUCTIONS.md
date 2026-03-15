# Marvin's Candy World — Unity Setup Instructions

## Prerequisites
- Unity 2021.3 LTS or newer (2022+ recommended)
- 3D Core project template

---

## Step 0 — Import Scripts

1. Open your Unity project.
2. In the **Project** window, create a folder: `Assets/Scripts`.
3. Copy all five `.cs` files from this repository into that folder.

---

## Step 1 — Add Both Scenes to Build Settings

1. Menu: **File > Build Settings**.
2. Click **Add Open Scenes** — add `NormalWorld` first (index 0).
3. Create a new scene (`File > New Scene`), save it as **CandyWorld**.
4. Add it to Build Settings (index 1).

---

## Step 2 — Build the NormalWorld Scene

Open `NormalWorld`.

### 2a. Ground

| Action | Values |
|--------|--------|
| `GameObject > 3D Object > Plane` | Rename: **Ground** |
| Scale | X=5, Y=1, Z=5 (makes a 50×50 m floor) |
| Material color | Green (`#7EC850`) |

### 2b. Swimming Pool

Create the pool with two objects:

**Pool Visual (the blue box):**
| Action | Values |
|--------|--------|
| `3D Object > Cube` | Rename: **Pool** |
| Position | X=0, Y=0.1, Z=8 |
| Scale | X=4, Y=0.5, Z=4 |
| Material color | Blue (`#4A90D9`), set Rendering Mode to **Transparent**, Alpha ≈ 180 |

**Pool Trigger (invisible detection zone):**
| Action | Values |
|--------|--------|
| `Create Empty` (child of Pool) | Rename: **PoolTrigger** |
| Position | Y=0.5 (sits at water surface level) |
| Add Component | Box Collider |
| Box Collider | ✅ **Is Trigger**, Size X=4, Y=1, Z=4 |
| Add Component | **PoolPortal** script |
| PoolPortal Inspector | `Candy World Scene Name` = `CandyWorld` |

### 2c. Player

| Action | Values |
|--------|--------|
| `3D Object > Capsule` | Rename: **Player** |
| Position | X=0, Y=1, Z=0 |
| Tag | Set Tag to **Player** |
| Add Component | **Character Controller** |
| CharacterController | Center Y=0, Height=2, Radius=0.5 |
| Add Component | **PlayerController** script |
| PlayerController Inspector | Leave `Camera Transform` empty (auto-finds) |

> **Remove the default Capsule Collider** — CharacterController provides its own. Select the Capsule Collider component and hit Remove Component.

### 2d. Main Camera

| Action | Values |
|--------|--------|
| Select **Main Camera** | Already exists in the scene |
| Position | X=0, Y=4, Z=-6 |
| Add Component | **CameraFollow** script |
| CameraFollow Inspector | Drag **Player** into the `Target` field |

### 2e. GameManager + HUD

This is the most involved step — build the hierarchy below:

```
GameManager          ← Empty GameObject
└── HUD              ← UI > Canvas  (set Render Mode: Screen Space - Overlay)
    ├── ScoreText    ← UI > Legacy > Text
    ├── WinText      ← UI > Legacy > Text
    └── FadePanel    ← UI > Image
```

**GameManager object:**
- `Create Empty`, rename **GameManager**
- Add Component: **GameManager** script

**HUD Canvas:**
- Right-click GameManager → `UI > Canvas`
- Rename: **HUD**
- Canvas component: Render Mode = **Screen Space - Overlay**
- Add **Canvas Scaler**: UI Scale Mode = **Scale With Screen Size**, Reference = 1920×1080

**ScoreText:**
- Right-click HUD → `UI > Legacy > Text`
- Rename: **ScoreText**
- Anchor: Top-Left corner
- Pos X=20, Pos Y=-20
- Width=300, Height=60
- Text: `Candies: 0 / 10`
- Font Size: 28, Color: White
- Check **Best Fit** if you want it to auto-resize

**WinText:**
- Right-click HUD → `UI > Legacy > Text`
- Rename: **WinText**
- Anchor: Center-Middle
- Width=600, Height=200
- Text: `You WIN!`
- Font Size: 48, Color: Yellow, Alignment: Center
- (Will be hidden at runtime by the script)

**FadePanel:**
- Right-click HUD → `UI > Image`
- Rename: **FadePanel**
- Anchor: Stretch to fill entire screen (click the anchor preset while holding Alt+Shift to also set position)
- Left=0, Right=0, Top=0, Bottom=0
- Color: Black (`#000000`), Alpha = **0** (fully transparent at start)

**Wire up GameManager references:**
- Select the **GameManager** object
- In the Inspector under GameManager script:
  - `Score Text` → drag **ScoreText**
  - `Win Text` → drag **WinText**
  - `Fade Panel` → drag **FadePanel**

---

## Step 3 — Build the CandyWorld Scene

Open `CandyWorld`.

### 3a. Ground

| Action | Values |
|--------|--------|
| `3D Object > Plane` | Rename: **CandyGround** |
| Scale | X=5, Y=1, Z=5 |
| Material color | Hot Pink (`#FF69B4`) or Mint (`#98FF98`) |

### 3b. Candy Pickups (place at least 10)

For each candy:

| Action | Values |
|--------|--------|
| `3D Object > Sphere` (or Capsule) | Rename: **Candy_01**, **Candy_02**, etc. |
| Scale | X=0.5, Y=0.5, Z=0.5 |
| Position | Spread around the ground — vary X, Z. Y=0.5 |
| Material color | Bright colors: red, yellow, orange, purple, green, etc. |
| Add Component | **Sphere Collider** |
| Sphere Collider | ✅ **Is Trigger** |
| Add Component | **CandyPickup** script |

Repeat for 10+ candies. Space them at least 2–3 units apart so they're easy to run to.

> **Tip:** After making one candy correctly, right-click it and **Duplicate** (Ctrl/Cmd+D) to make copies, then just move them around.

### 3c. Player (in CandyWorld)

Set up exactly the same Player + Camera as in NormalWorld:
- Capsule with CharacterController + PlayerController, Tag = **Player**
- Camera with CameraFollow pointing at it
- Place the Player at X=0, Y=1, Z=0

> No GameManager is needed in CandyWorld — it carries over automatically from NormalWorld via `DontDestroyOnLoad`.

---

## Step 4 — Test Play

1. Make sure **NormalWorld** is scene index 0 in Build Settings.
2. Press **Play** in NormalWorld.
3. Move the player toward the pool (WASD), then jump into it (Space).
4. The screen should fade to black and load CandyWorld.
5. Run around and collect 10 candies.
6. The win message should appear.

---

## Controls Summary

| Key | Action |
|-----|--------|
| W / A / S / D | Move |
| Space | Jump |
| Mouse X | Rotate camera |

---

## Script Summary

| Script | Attach To | Purpose |
|--------|-----------|---------|
| `PlayerController.cs` | Player (Capsule) | Movement, jumping, camera-relative input |
| `CameraFollow.cs` | Main Camera | Orbiting third-person camera |
| `PoolPortal.cs` | PoolTrigger (empty child of Pool) | Detects player entering pool, triggers scene load |
| `CandyPickup.cs` | Each candy object | Spin/bob animation, detects collection |
| `GameManager.cs` | GameManager (empty root object) | Score tracking, HUD, fade transition, win condition |

---

## Troubleshooting

| Problem | Fix |
|---------|-----|
| Player falls through the ground | Make sure CharacterController is on the Player, not just a Capsule Collider |
| Pool doesn't trigger | Ensure PoolTrigger has a Box Collider with **Is Trigger** checked and `PoolPortal.cs` attached |
| Candies don't register | Check that the candy's Sphere Collider has **Is Trigger** checked and player Tag is exactly **Player** |
| Screen stays black after transition | Make sure FadePanel's Alpha is **0** in the Inspector at start |
| Score doesn't show | Check that ScoreText is wired up in the GameManager Inspector |
| Camera spins wildly | Lower `Mouse Sensitivity` in CameraFollow Inspector (default 120 is good for most setups) |
