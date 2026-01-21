# ⚔️ RPG: The Abyss ⚔️

*Descend into darkness, emerge as legend*

---

## 🌟 About The Game

**RPG: The Abyss** is a 2D action-RPG adventure where you embark on an epic journey as a legendary warrior. Armed with sword and determination, you'll navigate treacherous landscapes, master fluid combat mechanics, and uncover the mysteries that lie within the depths of the abyss.

### 🎮 Core Features

- **⚔️ Dynamic Combat System**: Execute devastating attacks with frame-perfect timing
- **🏃‍♂️ Fluid Movement**: Run, jump, dash, and wall-slide through challenging terrain  
- **🎯 State-Driven Architecture**: Sophisticated state machine for responsive character control
- **🎨 Pixel-Perfect Animations**: Beautifully crafted warrior sprites with smooth transitions
- **🎮 Modern Input System**: Full controller and keyboard support with customizable controls

---

## 🛡️ The Warrior's Arsenal

Your warrior comes equipped with a vast array of abilities:

### Combat Abilities
- **Basic Attack Combos** - Chain devastating sword strikes
- **Dash Attacks** - Swift, powerful strikes while in motion
- **Defensive Maneuvers** - Crouch and evade enemy attacks

### Movement Mastery  
- **Parkour System** - Wall slides, edge grabs, and ladder climbing
- **Aerial Control** - Precise jumping and falling mechanics
- **Ground Movement** - Running and sliding across varied terrain

### Special Effects
- **Dynamic Visual Effects** - Dust particles, combat impacts, and death sequences
- **Responsive Animations** - Over 100+ individual sprite frames for fluid motion

---

## 🔧 Technical Architecture

Built with **Unity 6.0.2.13f1** using modern game development patterns:

### 🏗️ Core Systems
- **State Machine Pattern**: Modular character state management
- **Entity-Component System**: Flexible and extensible game object architecture  
- **Input System Integration**: Unity's new Input System for precise control
- **Animation Controller**: Blend trees and state transitions for seamless animation

### 📁 Project Structure
```
Assets/
├── 🎬 Animation/          # Character animations & controllers
├── 🎮 InputSystem/        # Input mappings & action sets
├── 📜 Scripts/            # Core game logic & state machines
├── 🎨 Warrior/            # Character sprites & assets
├── 🌍 Scenes/             # Game levels & environments
└── ⚙️ Settings/           # Rendering & project configuration
```

### 🎯 Key Components
- **`Player.cs`** - Main player controller with input handling
- **`StateMachine.cs`** - Generic state machine implementation
- **`EntityState.cs`** - Base class for all character states
- **State Classes** - Idle, Move, Attack, and more specialized behaviors

---

## 🚀 Getting Started

### Prerequisites
- **Unity 6.0.2.13f1** or compatible version
- **Git** for version control
- **Visual Studio** or **VS Code** for scripting

### Installation

📖 **Installation Guides**
- **English**: [INSTALLATION_GUIDE.md](INSTALLATION_GUIDE.md)
- **Tiếng Việt**: [HUONG_DAN_CAI_DAT.md](HUONG_DAN_CAI_DAT.md)

Quick Start:
1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/RPG_The_Abyss.git
   ```

2. **Open in Unity Hub**
   - Add the project folder to Unity Hub
   - Ensure you're using Unity 6.0.2.13f1 or compatible version

3. **Configure Input System**
   - The project uses Unity's new Input System
   - Input mappings are pre-configured in `Assets/InputSystem/`

4. **Play & Develop**
   - Open `Assets/Scenes/MainMenu.unity` or `Assets/Scenes/Level_0.unity`
   - Press Play to start your adventure!

### 🎮 Controls
- **WASD** / **Arrow Keys** - Move
- **Space** - Jump  
- **Left Click** / **J** - Attack
- **Shift** - Dash
- **S** / **Down Arrow** - Crouch
- **E** - Interact (Hold)

---

## 🎨 Asset Credits

### Character Sprites
- **Artist**: Clembod (@Clembod)
- **License**: Personal & Commercial Use (Modification Allowed)
- **Contact**: 
  - 🐦 Twitter: [@Clembod](https://twitter.com/Clembod)
  - 📸 Instagram: [@Clembod](https://instagram.com/Clembod)
  - 🎮 Itch.io: [clembod.itch.io](https://clembod.itch.io)
  - 🎨 ArtStation: [artstation.com/clembod](https://www.artstation.com/clembod)

*Credit is not required but greatly appreciated*

---

## 🛠️ Development Status

### ✅ Implemented Features
- [x] Core player movement & physics
- [x] State machine architecture  
- [x] Basic attack system
- [x] Animation system integration
- [x] Input system setup
- [x] Character sprite integration

### 🔄 In Development

#### Short-term Goals
- [ ] Enemy AI systems
- [ ] Level design & environments
- [ ] Combat expansion (combos, special abilities)
- [ ] Audio integration
- [ ] UI/HUD systems
- [ ] Save/Load functionality
- [ ] Stat system
- [ ] Skill tree system
- [ ] UI improvements & polish

#### Long-term Goals
- [ ] Inventory system
- [ ] Equipment & item system
- [ ] Crafting system
- [ ] Storage system
- [ ] Shop system
- [ ] Loot system

### 🎯 Future Plans
- [ ] Multiple character classes
- [ ] RPG progression systems
- [ ] Inventory & equipment
- [ ] Story campaign
- [ ] Boss battles
- [ ] Multiplayer support

---

## 🤝 Contributing

Contributions are welcome! Whether you're fixing bugs, adding features, or improving documentation:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

---

## 📜 License

This project is licensed under the **MIT License** - see the [LICENSE](LICENSE) file for details.

**Note**: Character sprite assets have their own licensing terms (see Asset Credits section).

---

## 🌟 Acknowledgments

- Unity Technologies for the incredible game engine
- Clembod for the amazing warrior character sprites
- The indie game development community for inspiration and support

---

*Ready to face the abyss? The darkness awaits your blade...* ⚔️

---

<div align="center">

**[🎮 Play Now]** • **[📖 Documentation]** • **[🐛 Report Bug]** • **[💡 Request Feature]**

*Built with ❤️ and ⚔️ in Unity*

</div>