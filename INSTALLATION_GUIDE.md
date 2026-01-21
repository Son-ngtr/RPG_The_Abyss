# 📖 INSTALLATION GUIDE
## RPG: The Abyss

---

## 📋 TABLE OF CONTENTS

1. [System requirements](#system-requirements)
2. [Install Unity](#install-unity)
3. [Download and open the project](#download-and-open-the-project)
4. [Project setup](#project-setup)
5. [Run the game](#run-the-game)
6. [Common issues](#common-issues)
7. [Project structure](#project-structure)

---

## 💻 SYSTEM REQUIREMENTS

### Minimum requirements
- **OS**: Windows 10/11 (64-bit), macOS 10.15+, or Linux Ubuntu 18.04+
- **CPU**: Intel Core i5 or equivalent AMD
- **RAM**: 8 GB (recommended 16 GB)
- **Storage**: At least 10 GB free space
- **GPU**: DirectX 11 compatible
- **Unity Editor**: Unity **6.0.2.13f1** or compatible

### Required software
- **Unity Hub** (required)
- **Unity Editor 6.0.2.13f1** (or compatible)
- **Visual Studio 2022** or **Visual Studio Code** (for editing C# scripts)
- **Git** (if you clone from a repository)

---

## 🎮 INSTALL UNITY

### Step 1: Install Unity Hub
1. Go to: `https://unity.com/download`
2. Download **Unity Hub**
3. Install Unity Hub

### Step 2: Install Unity Editor
1. Open **Unity Hub**
2. Go to **Installs**
3. Click **Install Editor**
4. Select **Unity 6.0.2.13f1** (or a compatible version)
5. In **Add modules**, it’s recommended to include:
   - ✅ **Microsoft Visual Studio Community** (or Visual Studio Code)
   - ✅ **Windows Build Support (IL2CPP)** (if you build for Windows)
   - ✅ **Android Build Support** (if you build for Android)
   - ✅ **iOS Build Support** (if you build for iOS — macOS only)
6. Click **Install** and wait until it finishes

**Note**: First-time installation may take 15–30 minutes depending on your internet speed.

---

## 📥 DOWNLOAD AND OPEN THE PROJECT

### Option 1: Clone from a Git repository
1. Open **Git Bash** or **Command Prompt**
2. Navigate to where you want to store the project:

```bash
cd D:\My Projects
```

3. Clone the repository:

```bash
git clone <REPOSITORY_URL>
```

4. Then open Unity Hub and click **Add** to add the project folder

### Option 2: Open an existing local folder
1. Open **Unity Hub**
2. Click **Open** (or **Add**)
3. Browse to the project folder, e.g. `D:\My Projects\RPG_The_Abyss` (depending on where you saved it)
4. Select the folder and click **Open**

### Next
Unity will detect the correct editor version and import assets. The first import can take several minutes.

---

## ⚙️ PROJECT SETUP

### Step 1: Verify Unity version
1. After the project opens, check the Unity version shown in the editor
2. Make sure it is **Unity 6.0.2.13f1** (or compatible)
3. If Unity asks to upgrade/downgrade, follow the prompts (keep a backup if needed)

### Step 2: Configure Input System
This project uses Unity’s **new Input System**. If Unity shows a warning:
1. Go to **Edit** → **Project Settings**
2. Select **Player** → **Other Settings**
3. Set **Active Input Handling** to:
   - **Input System Package (New)**, or
   - **Both** (if you need legacy Input Manager compatibility)

### Step 3: Install required packages
Unity installs packages from `Packages/manifest.json`, including:
- ✅ Input System (1.14.2)
- ✅ Universal RP (17.2.0)
- ✅ Cinemachine (3.1.4)
- ✅ Timeline (1.8.9)
- ✅ Visual Scripting (1.9.8)

If a package is missing:
1. Open **Window** → **Package Manager**
2. Find the package and click **Install**

### Step 4: Set up Visual Studio / VS Code (Optional)
1. Go to **Edit** → **Preferences** → **External Tools**
2. Set **External Script Editor** to:
   - **Visual Studio 2022**: `C:\Program Files\Microsoft Visual Studio\2022\Community\Common7\IDE\devenv.exe`
   - **VS Code**: `C:\Users\<YourUser>\AppData\Local\Programs\Microsoft VS Code\Code.exe`

---

## 🎯 RUN THE GAME

### Option 1: Run in Unity Editor
1. In the **Project** window, open `Assets/Scenes`
2. Choose a scene:
   - **MainMenu.unity** — Main menu (recommended as the starting scene)
   - **Level_0.unity** — First level
   - **Level_1.unity** — Second level
   - **SampleScene.unity** — Sample/test scene
   - **Test_Level.unity** — Test level
3. Press **Play** (▶️)
4. The game runs in the **Game** view

### Option 2: Build and run an .exe (Windows)
1. Go to **File** → **Build Settings**
2. Select **PC, Mac & Linux Standalone**
3. Set **Target Platform**: Windows
4. Click **Add Open Scenes**
5. Click **Build** and choose an output folder
6. After build completes, run the generated `.exe`

### Controls
> The mappings below are based on the project **Input System**: `Assets/InputSystem/Player_InputSet.inputactions` and script `Assets/Scripts/Player/Player.cs`.

#### Gameplay
- **Move**: `A / D` (left / right)
- **Jump**: `Space`
- **Dash**: `Left Shift`
- **Basic attack**: `Left Mouse Button`
- **Block / Counter Attack**: `Q`
- **Use Skill (Spell)**: `E`
- **Ranged attack**: `Right Mouse Button`
- **Ultimate**: `R`
- **Interact**: `F`
- **Quick item slots**: `1` (slot 1), `2` (slot 2)

#### UI
- **Open/close Options (Pause/Options UI)**: `Esc`
- **Open/close Skill Tree**: `L`
- **Open/close Inventory**: `C`
- **Hold alternative input**: `Left Ctrl` (commonly used for “full stack” UI actions)
- **Dialogue interaction**: `F`
- **Dialogue navigation**: `W / S`

#### Debug/test keys currently present in scripts (if you keep them)
- **Merchant fill shop list**: `Z` (in `Assets/Scripts/Interactive_Objects/Object_Merchant.cs`)
- **Reaper teleport test**: `V` (in `Assets/Scripts/Enemy/Boss/Enemy_Reaper.cs`)
- **Force drop items (test)**: `X` (in `Assets/Scripts/Entity/Entity_DropManager.cs`)
- **Kill player (test)**: `N` (in `Assets/Scripts/Player/Player_Health.cs`)

---

## 🔧 COMMON ISSUES

### Issue 1: "Input System not found" / "CS0246: InputAction not found"

**Cause**: Input System package is not installed/enabled.

**Automatic Fix** (Recommended):
- This project includes an **auto-setup script** (`Assets/Scripts/Editor/ProjectSetupHelper.cs`) that automatically checks and installs Input System when you open the project.
- If you see this error, **close Unity completely** and **reopen the project**. The script will run automatically.
- Check the **Console** for messages like `[ProjectSetupHelper] ✓ Input System package is installed.`

**Manual Fix** (if automatic doesn't work):
1. Open **Window** → **Package Manager**
2. Click **+** → **Add package by name...**
3. Enter: `com.unity.inputsystem`
4. Click **Add**
5. Go to **Edit** → **Project Settings** → **Player** → **Other Settings**
6. Set **Active Input Handling** to **Input System Package (New)** or **Both**
7. Restart Unity

### Issue 2: “Script compilation errors”
**Cause**: Code errors or missing packages.

**Fix**:
1. Check **Console** (Window → General → Console)
2. Read the error messages and fix the referenced scripts
3. Confirm all packages are installed

### Issue 3: “Scene not found” / “Missing references”
**Cause**: Missing scene/asset files or broken references.

**Fix**:
1. Verify the `.meta` files exist
2. Right-click the affected asset → **Reimport**
3. If it persists, re-check the folder structure and references

### Issue 4: Unity Editor is slow
**Cause**: Large project and/or limited hardware.

**Fix**:
1. Close unnecessary Unity windows/tabs
2. Lower quality settings in the Game view while testing
3. Close other apps to free RAM/CPU

### Issue 5: “Package version mismatch”
**Cause**: Package versions don’t match the Unity version.

**Fix**:
1. Delete `Library` and `Temp`
2. Reopen the project in Unity
3. Unity will reimport and restore packages

---

## 📁 PROJECT STRUCTURE

```
RPG_The_Abyss/
│
├── Assets/                    # All game assets
│   ├── Animation/            # Animations & controllers
│   ├── Audio/                # Audio files (.wav, .mp3)
│   ├── Data/                 # ScriptableObjects and game data
│   ├── Graphics/             # Sprites, textures
│   ├── InputSystem/          # Input System actions/config
│   ├── Materials/            # Materials / physics materials
│   ├── Prefab/               # Prefabs
│   ├── Scenes/               # Scenes
│   │   ├── MainMenu.unity
│   │   ├── Level_0.unity
│   │   ├── Level_1.unity
│   │   ├── SampleScene.unity
│   │   └── Test_Level.unity
│   ├── Scripts/              # C# scripts
│   ├── Settings/             # Project settings/assets
│   └── UI/                   # UI scripts/assets
│
├── Library/                  # Unity cache (do not edit)
├── Logs/                     # Unity logs
├── Packages/                 # Package manifest & lock
├── ProjectSettings/          # Unity project settings
├── README.md                 # Main README
├── HUONG_DAN_CAI_DAT.md      # Vietnamese installation guide
└── INSTALLATION_GUIDE.md     # English installation guide
```

---

## 📝 IMPORTANT NOTES
1. **Do not edit `Library/`**: Unity manages it automatically.
2. **Backup before big changes**: Especially before upgrading Unity.
3. **Version control**: If you use Git, your `.gitignore` should include:
   - `Library/`
   - `Temp/`
   - `obj/`
   - `*.csproj`
   - `*.sln`
4. **Unity version**: If something is hard to fix, verify you’re on the correct Unity version first.

---

## 🆘 SUPPORT
If you hit an issue not listed above:
1. Check logs in `Logs/`
2. Check the Unity **Console** for error details
3. See Unity docs: `https://docs.unity3d.com/`
4. Contact the project maintainers

---

## ✅ INSTALLATION CHECKLIST
After setup, you can confirm success by:
1. ✅ Unity opens the project without errors
2. ✅ Console shows no red errors
3. ✅ Pressing Play runs the game
4. ✅ Player can move and attack
5. ✅ Animations play smoothly

---

**Good luck and have fun with RPG: The Abyss!**

*Last updated: 2026*
