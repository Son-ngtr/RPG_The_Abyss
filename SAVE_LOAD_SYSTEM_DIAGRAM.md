# 📀 SƠ ĐỒ HỆ THỐNG LƯU / LOAD DATA
## RPG: The Abyss - Kiến trúc Save/Load chi tiết

---

## 1. TỔNG QUAN KIẾN TRÚC

```
┌─────────────────────────────────────────────────────────────────────────────────┐
│                           HỆ THỐNG SAVE / LOAD                                    │
├─────────────────────────────────────────────────────────────────────────────────┤
│                                                                                   │
│   ┌──────────────┐         ┌──────────────┐         ┌──────────────────────┐    │
│   │ SaveManager  │ ──────► │   GameData   │ ◄────── │  ISaveable (nhiều)   │    │
│   │  (Singleton) │         │  (1 instance)│         │  SaveData(ref data)  │    │
│   └──────┬───────┘         └──────┬───────┘         │  LoadData(data)      │    │
│          │                         │                 └──────────────────────┘    │
│          │ SaveGame()              │ JsonUtility.ToJson / FromJson               │
│          │ LoadGame()               │                                             │
│          ▼                         ▼                                              │
│   ┌──────────────┐         ┌──────────────────────────────────────────────┐    │
│   │FileDataHandler│        │  File: doantotnghiep.json                      │    │
│   │ Save(gameData)│        │  Path: Application.persistentDataPath         │    │
│   │ LoadData()    │        │  (VD: .../AppData/LocalLow/.../RPG_The_Abyss)   │    │
│   │ DeleteData()  │        │  Optional: XOR encryption (codeword)           │    │
│   └──────────────┘         └──────────────────────────────────────────────┘    │
│                                                                                   │
└─────────────────────────────────────────────────────────────────────────────────┘
```

---

## 2. LUỒNG LƯU (SAVE)

```
                    SaveGame() được gọi
                    (GameManager.ChangeScene / OnApplicationQuit)
                                    │
                                    ▼
┌───────────────────────────────────────────────────────────────────────────────────┐
│  SaveManager.SaveGame()                                                              │
│    1. Duyệt tất cả ISaveable (FindAllSaveables)                                      │
│    2. Gọi saveable.SaveData(ref gameData)  → mỗi component ghi vào CÙNG 1 GameData  │
│    3. dataHandler.Save(gameData)           → ghi file từ GameData                    │
└───────────────────────────────────────────────────────────────────────────────────┘
                                    │
        ┌───────────────────────────┼───────────────────────────┐
        ▼                           ▼                           ▼
┌───────────────┐         ┌─────────────────┐         ┌─────────────────┐
│ GameManager   │         │ Inventory_Player │         │Inventory_Storage │
│ SaveData()    │         │ SaveData()       │         │ SaveData()       │
│ • lastScene   │         │ • gold           │         │ • storageItems   │
│ • lastPos     │         │ • inventory      │         │ • storageMaterials│
└───────────────┘         │ • equippedItems  │         └─────────────────┘
        │                 └─────────────────┘                   │
        │                           │                          │
        ▼                           ▼                          ▼
┌───────────────┐         ┌─────────────────┐         ┌─────────────────┐
│ Object_       │         │ Player_Quest    │         │ UI_SkillTree     │
│ CheckPoint    │         │ Manager         │         │ SaveData()       │
│ SaveData()    │         │ SaveData()      │         │ • skillPoints    │
│ • unlocked    │         │ • activeQuests  │         │ • skillTreeUI     │
│   CheckPoints │         │ • completedQuests│        │ • skillUpgrades  │
└───────────────┘         └─────────────────┘         └─────────────────┘
        │                           │                          │
        ▼                           ▼                          ▼
┌───────────────┐         ┌─────────────────┐         ┌─────────────────┐
│ Object_Portal │         │ UI_Quest        │         │ (Các ISaveable   │
│ SaveData()    │         │ SaveData()      │         │  khác nếu có)    │
│ • inScene     │         │ (empty body)    │         │                  │
│   Portals     │         │                 │         │                  │
│ • portalDest  │         │                 │         │                  │
│ • returnTown  │         │                 │         │                  │
└───────────────┘         └─────────────────┘         └─────────────────┘
                                    │
                                    ▼
                    FileDataHandler.Save(gameData)
                    1. GameData → JSON (JsonUtility.ToJson)
                    2. (Tuỳ chọn) Encrypt XOR
                    3. Ghi file: fullPath = persistentDataPath + "doantotnghiep.json"
```

---

## 3. LUỒNG LOAD (LOAD)

```
                    LoadGame() được gọi
                    (SaveManager.Start → yield null → LoadGame())
                                    │
                                    ▼
┌───────────────────────────────────────────────────────────────────────────────────┐
│  SaveManager.LoadGame()                                                             │
│    1. gameData = dataHandler.LoadData()  → đọc file hoặc null                       │
│    2. Nếu gameData == null → gameData = new GameData() (new game)                   │
│    3. Duyệt tất cả ISaveable → saveable.LoadData(gameData)                           │
└───────────────────────────────────────────────────────────────────────────────────┘
                                    │
                    FileDataHandler.LoadData()
                    1. Đọc file fullPath (nếu tồn tại)
                    2. (Tuỳ chọn) Decrypt XOR
                    3. JSON → GameData (JsonUtility.FromJson<GameData>)
                    4. return loadData (hoặc null)
                                    │
        ┌───────────────────────────┼───────────────────────────┐
        ▼                           ▼                           ▼
┌───────────────┐         ┌─────────────────┐         ┌─────────────────┐
│ GameManager   │         │ Inventory_Player │         │Inventory_Storage │
│ LoadData()    │         │ LoadData()       │         │ LoadData()       │
│ • lastScene   │         │ • gold           │         │ • itemList từ   │
│ • lastPos     │         │ • itemList từ    │         │   storageItems   │
│ • dataLoad    │         │   data.inventory │         │ • materialStash  │
│   Completed   │         │ • equipList từ   │         │   từ storage     │
│   = true      │         │   equippedItems  │         │   Materials      │
└───────────────┘         └─────────────────┘         └─────────────────┘
        │                           │                          │
        ▼                           ▼                          ▼
┌───────────────┐         ┌─────────────────┐         ┌─────────────────┐
│ Object_       │         │ Player_Quest    │         │ UI_SkillTree     │
│ CheckPoint    │         │ Manager         │         │ LoadData()       │
│ LoadData()    │         │ LoadData()      │         │ • skillPoints    │
│ • ActiveCheck │         │ • activeQuests  │         │ • node.Unlock    │
│   Point(...)  │         │   từ data       │         │   WithSaveData()  │
└───────────────┘         └─────────────────┘         │ • skill.SetSkill │
        │                           │                  │   Upgrade(...)   │
        ▼                           ▼                  └─────────────────┘
┌───────────────┐         ┌─────────────────┐
│ Object_Portal │         │ UI_Quest        │
│ LoadData()    │         │ LoadData()      │
│ • position từ│         │ • currentGameData│
│   inScene     │         │   = data        │
│   Portals     │         │ (để check quest │
│ • returnScene │         │  taken/done)    │
└───────────────┘         └─────────────────┘
```

---

## 4. CẤU TRÚC GameData (Dữ liệu lưu trong file)

```
GameData
├── gold                              (int)
├── inventory                         (SerializableDictionary<string, int>)   // saveID → tổng stackSize
├── storageItems                      (SerializableDictionary<string, int>)   // saveID → stackSize (kho đồ thường)
├── storageMaterials                  (SerializableDictionary<string, int>)   // saveID → stackSize (material stash)
├── equippedItems                     (SerializableDictionary<string, ItemType>) // saveID → slotType
├── skillTreeUI                       (SerializableDictionary<string, bool>)  // skill displayName → unlocked
├── skillPoints                       (int)
├── skillUpgrades                     (SerializableDictionary<SkillType, SkillUpgradeType>)
├── unlockedCheckPoints               (SerializableDictionary<string, bool>)  // checkpointID → unlocked
├── inScenePortals                    (SerializableDictionary<string, Vector3>) // sceneName → portal position
├── portalDestinationSceneName        (string)
├── returningFromTown                 (bool)
├── lastScenePlayedName               (string)
├── lastPlayerPosition                (Vector3)
├── completedQuests                   (SerializableDictionary<string, bool>)  // questSaveID → completed
└── activeQuests                      (SerializableDictionary<string, int>)  // questSaveID → progressAmount
```

---

## 5. BẢNG COMPONENT ISaveable – Ghi/Đọc gì

| Component | File | SaveData(ref GameData) | LoadData(GameData) |
|-----------|------|------------------------|--------------------|
| **GameManager** | GameManager.cs | lastPlayerPosition, lastScenePlayedName (bỏ qua nếu scene = MainMenu) | lastScenePlayedName, lastPlayerPosition; dataLoadCompleted = true |
| **Inventory_Player** | Inventory_Player.cs | gold; inventory (saveID→stackSize); equippedItems (saveID→slotType) | gold; load inventory + equip từ itemDataBase; giữ health % |
| **Inventory_Storage** | Inventory_Storage.cs | Gọi base (player); storageItems; storageMaterials | Clear list; load storageItems → itemList; storageMaterials → materialStash |
| **Object_CheckPoint** | Object_CheckPoint.cs | Nếu isActive: unlockedCheckPoints[checkpointID] = true | ActiveCheckPoint(data.unlockedCheckPoints[checkpointID]) |
| **Object_Portal** | Object_Portal.cs | returnFromTown; inScenePortals[sceneName]=position; portalDestinationSceneName | position từ inScenePortals; returnScene; returningFromTown |
| **Player_QuestManager** | Player_QuestManager.cs | activeQuests (questSaveID→progress); completedQuests | activeQuests → QuestData từ questDatabase; restore progress |
| **UI_SkillTree** | UI_SkillTree.cs | skillPoints; skillTreeUI (name→unlocked); skillUpgrades (type→upgradeType) | skillPoints; node.UnlockWithSaveData(); skill.SetSkillUpgrade(...) |
| **UI_Quest** | UI_Quest.cs | (empty) | currentGameData = data (để UI check quest taken/completed) |
| **Inventory_Base** | Inventory_Base.cs | (base: empty) | (base: empty) |

---

## 6. THỜI ĐIỂM GỌI SAVE / LOAD

| Sự kiện | Hành động |
|---------|-----------|
| **Vào game (Start)** | SaveManager.Start → yield null → LoadGame() (đọc file, rồi LoadData cho từng ISaveable) |
| **Đổi scene** | GameManager.ChangeScene() → SaveManager.SaveGame() (trước khi load scene mới) |
| **Thoát game** | OnApplicationQuit → SaveManager.SaveGame() |
| **Xoá save (Context Menu)** | SaveManager.DeleteSaveData() → xoá file → LoadGame() (reset về new GameData) |

---

## 7. FILE VÀ ĐƯỜNG DẪN

| Thành phần | Chi tiết |
|------------|----------|
| **Tên file** | `doantotnghiep.json` (SerializeField trong SaveManager) |
| **Thư mục** | `Application.persistentDataPath` (VD Windows: `%userprofile%\AppData\LocalLow\<CompanyName>\<ProductName>`) |
| **Định dạng** | JSON (JsonUtility). Có thể bật encryption = true → XOR với codeword "nguyenxuanson" |
| **FileDataHandler** | Tạo thư mục nếu chưa có; ghi đè file khi Save; đọc toàn bộ file khi Load |

---

## 8. SƠ ĐỒ PHỤ THUỘC (DEPENDENCIES)

```
                    ISaveable (interface)
                         │
     ┌───────────────────┼───────────────────┬───────────────────┬───────────────────┐
     ▼                   ▼                   ▼                   ▼                   ▼
GameManager    Inventory_Player    Inventory_Storage    Object_CheckPoint    Object_Portal
     │                   │                   │                   │                   │
     │                   │                   │                   │                   │
     └───────────────────┴───────────────────┴───────────────────┴───────────────────┘
                                        │
                                        ▼
                                 SaveManager
                                        │
                         ┌──────────────┴──────────────┐
                         ▼                             ▼
                  FileDataHandler                 GameData
                         │                             ▲
                         │                             │
                         └──────► File (JSON) ─────────┘
                                        (serialize/deserialize)
```

---

## 9. GHI CHÚ KỸ THUẬT

- **Một GameData**: Tất cả ISaveable dùng chung một instance GameData; SaveData(ref gameData) ghi vào cùng object, sau đó mới ghi file một lần.
- **Thứ tự Load**: SaveManager duyệt FindAllSaveables() (FindObjectsByType MonoBehaviour, OfType<ISaveable>). Thứ tự có thể phụ thuộc thứ tự object trong scene; nếu có phụ thuộc (VD: Player phải load trước Storage) cần sắp xếp hoặc init thủ công.
- **Item load**: Inventory load theo saveID → ItemDataSO từ itemDataBase.GetItemDataByID(saveID); tạo Inventory_Item mới, add vào list (stacking do AddItem hoặc logic tương đương).
- **Encryption**: XOR đơn giản (codeword lặp lại); không bảo mật mạnh, chủ yếu tránh chỉnh file dễ dàng.

---

*Tài liệu dựa trên code trong project. Cập nhật: 2025.*
