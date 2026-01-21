# 📖 HƯỚNG DẪN CÀI ĐẶT CHƯƠNG TRÌNH
## RPG: The Abyss

---

> English version: [INSTALLATION_GUIDE.md](INSTALLATION_GUIDE.md)

## 📋 MỤC LỤC

1. [Yêu cầu hệ thống](#yêu-cầu-hệ-thống)
2. [Cài đặt Unity](#cài-đặt-unity)
3. [Tải và mở dự án](#tải-và-mở-dự-an)
4. [Cấu hình dự án](#cấu-hình-dự-án)
5. [Chạy chương trình](#chạy-chương-trình)
6. [Xử lý lỗi thường gặp](#xử-lý-lỗi-thường-gặp)
7. [Cấu trúc dự án](#cấu-trúc-dự-án)

---

## 💻 YÊU CẦU HỆ THỐNG

### Yêu cầu tối thiểu:
- **Hệ điều hành**: Windows 10/11 (64-bit), macOS 10.15+, hoặc Linux Ubuntu 18.04+
- **CPU**: Intel Core i5 hoặc AMD tương đương
- **RAM**: 8 GB (khuyến nghị 16 GB)
- **Ổ cứng**: Ít nhất 10 GB dung lượng trống
- **Card đồ họa**: DirectX 11 tương thích
- **Unity Editor**: Unity 6.0.2.13f1 hoặc tương thích

### Phần mềm cần thiết:
- **Unity Hub** (bắt buộc)
- **Unity Editor 6.0.2.13f1** hoặc phiên bản tương thích
- **Visual Studio 2022** hoặc **Visual Studio Code** (cho việc chỉnh sửa code)
- **Git** (nếu clone từ repository)

---

## 🎮 CÀI ĐẶT UNITY

### Bước 1: Tải Unity Hub

1. Truy cập trang web: https://unity.com/download
2. Tải **Unity Hub** về máy
3. Cài đặt Unity Hub theo hướng dẫn

### Bước 2: Cài đặt Unity Editor

1. Mở **Unity Hub**
2. Chuyển sang tab **Installs**
3. Nhấn nút **Install Editor**
4. Chọn phiên bản **Unity 6.0.2.13f1** hoặc phiên bản tương thích
5. Trong cửa sổ **Add modules**, đảm bảo chọn các module sau:
   - ✅ **Microsoft Visual Studio Community** (hoặc Visual Studio Code)
   - ✅ **Windows Build Support (IL2CPP)** (nếu build cho Windows)
   - ✅ **Android Build Support** (nếu build cho Android)
   - ✅ **iOS Build Support** (nếu build cho iOS - chỉ trên macOS)
6. Nhấn **Install** và chờ quá trình cài đặt hoàn tất

**Lưu ý**: Quá trình cài đặt có thể mất 15-30 phút tùy vào tốc độ internet.

---

## 📥 TẢI VÀ MỞ DỰ ÁN

### Cách 1: Clone từ Git Repository

1. Mở **Git Bash** hoặc **Command Prompt**
2. Di chuyển đến thư mục bạn muốn lưu dự án:
   ```bash
   cd D:\My Projects
   ```
3. Clone repository:
   ```bash
   git clone <URL_REPOSITORY>
   ```
4. Hoặc nếu bạn đã có thư mục dự án, chỉ cần mở Unity Hub và chọn **Add** để thêm dự án

### Cách 2: Mở dự án từ thư mục có sẵn

1. Mở **Unity Hub**
2. Nhấn nút **Open** hoặc **Add**
3. Duyệt đến thư mục dự án: `D:\My Projects\RPG_The_Abyss` (Tùy thuộc vào bạn lưu ở folder ở đâu)
4. Chọn thư mục và nhấn **Open**

### Bước tiếp theo:

Unity sẽ tự động nhận diện phiên bản Unity Editor phù hợp và mở dự án. Lần đầu mở có thể mất vài phút để Unity import các assets.

---

## ⚙️ CẤU HÌNH DỰ ÁN

### Bước 1: Kiểm tra phiên bản Unity

1. Sau khi mở dự án, kiểm tra phiên bản Unity ở góc dưới bên phải Unity Editor
2. Đảm bảo phiên bản là **Unity 6.0.2.13f1** hoặc tương thích
3. Nếu Unity yêu cầu upgrade hoặc downgrade, làm theo hướng dẫn

### Bước 2: Cấu hình Input System

Dự án sử dụng **Unity Input System** mới. Nếu gặp cảnh báo:

1. Vào menu **Edit** → **Project Settings**
2. Chọn **Player** → **Other Settings**
3. Tìm **Active Input Handling** và chọn:
   - **Input System Package (New)** hoặc
   - **Both** (nếu muốn hỗ trợ cả Input Manager cũ)

### Bước 3: Cài đặt các Package cần thiết

Unity sẽ tự động cài đặt các package từ file `Packages/manifest.json`, bao gồm:
- ✅ Input System (1.14.2)
- ✅ Universal RP (17.2.0)
- ✅ Cinemachine (3.1.4)
- ✅ Timeline (1.8.9)
- ✅ Visual Scripting (1.9.8)

Nếu package nào chưa được cài đặt:
1. Vào **Window** → **Package Manager**
2. Tìm package cần thiết và nhấn **Install**

### Bước 4: Cấu hình Visual Studio (Tùy chọn)

1. Vào **Edit** → **Preferences** → **External Tools**
2. Chọn **External Script Editor** và trỏ đến:
   - **Visual Studio 2022**: `C:\Program Files\Microsoft Visual Studio\2022\Community\Common7\IDE\devenv.exe`
   - Hoặc **Visual Studio Code**: `C:\Users\<TênUser>\AppData\Local\Programs\Microsoft VS Code\Code.exe`

---

## 🎯 CHẠY CHƯƠNG TRÌNH

### Cách 1: Chạy từ Unity Editor

1. Trong cửa sổ **Project**, mở thư mục `Assets/Scenes`
2. Chọn một trong các scene sau:
   - **MainMenu.unity** - Menu chính của game (Nơi bắt đầu màn game, nên chọn scene này làm scene bắt đầu)
   - **Level_0.unity** - Level đầu tiên
   - **Level_1.unity** - Level thứ hai
   - **SampleScene.unity** - Scene mẫu để test
   - **Test_Level.unity** - Scene test
3. Nhấn nút **Play** (▶️) ở trên cùng Unity Editor
4. Game sẽ chạy trong cửa sổ **Game** view

### Cách 2: Build và chạy file .exe (Windows)

1. Vào menu **File** → **Build Settings**
2. Chọn platform **PC, Mac & Linux Standalone**
3. Chọn **Target Platform**: Windows
4. Nhấn **Add Open Scenes** để thêm scene hiện tại
5. Nhấn **Build** và chọn thư mục lưu file build
6. Sau khi build xong, vào thư mục và chạy file `.exe`

### Điều khiển trong game:

> Mapping bên dưới được lấy theo **Input System** của dự án: `Assets/InputSystem/Player_InputSet.inputactions` và script `Assets/Scripts/Player/Player.cs`.

#### Điều khiển nhân vật (Gameplay)
- **Di chuyển**: `A / D` (trái / phải)
- **Nhảy**: `Space`
- **Dash**: `Left Shift`
- **Tấn công thường**: `Chuột trái`
- **Đỡ / Counter Attack**: `Q`
- **Dùng Skill (Spell)**: `E`
- **Đánh xa (Range Attack)**: `Chuột phải`
- **Ultimate**: `R`
- **Tương tác (Interact)**: `F`
- **Quick item slot**: `1` (slot 1), `2` (slot 2)

#### Điều khiển UI
- **Mở/đóng Option (Pause/Option UI)**: `Esc`
- **Mở/đóng Skill Tree**: `L`
- **Mở/đóng Inventory**: `C`
- **Giữ input phụ (Alternative Input)**: `Left Ctrl` (thường dùng để thao tác “full stack” trong UI)
- **Tương tác trong Dialogue**: `F`
- **Di chuyển trong Dialogue**: `W / S`

#### Phím debug/test đang có trong Scripts (nếu bạn giữ lại)
- **Merchant fill shop list**: `Z` (trong `Assets/Scripts/Interactive_Objects/Object_Merchant.cs`)
- **Reaper teleport test**: `V` (trong `Assets/Scripts/Enemy/Boss/Enemy_Reaper.cs`)
- **Force drop item (test)**: `X` (trong `Assets/Scripts/Entity/Entity_DropManager.cs`)
- **Kill player (test)**: `N` (trong `Assets/Scripts/Player/Player_Health.cs`)

---

## 🔧 XỬ LÝ LỖI THƯỜNG GẶP

### Lỗi 1: "Input System not found" / "CS0246: InputAction not found"

**Nguyên nhân**: Package Input System chưa được cài đặt hoặc chưa được kích hoạt.

**Giải pháp tự động** (Khuyến nghị):
- Dự án này có **script tự động setup** (`Assets/Scripts/Editor/ProjectSetupHelper.cs`) sẽ tự động kiểm tra và cài đặt Input System khi bạn mở project.
- Nếu gặp lỗi này, **đóng Unity hoàn toàn** và **mở lại project**. Script sẽ tự chạy.
- Kiểm tra **Console** để xem thông báo như `[ProjectSetupHelper] ✓ Input System package is installed.`

**Giải pháp thủ công** (nếu tự động không hoạt động):
1. Vào **Window** → **Package Manager**
2. Nhấn **+** → **Add package by name...**
3. Nhập: `com.unity.inputsystem`
4. Nhấn **Add**
5. Vào **Edit** → **Project Settings** → **Player** → **Other Settings**
6. Đặt **Active Input Handling** thành **Input System Package (New)** hoặc **Both**
7. Restart Unity Editor

### Lỗi 2: "Script compilation errors"

**Nguyên nhân**: Có lỗi trong code hoặc thiếu package.

**Giải pháp**:
1. Kiểm tra cửa sổ **Console** (Window → General → Console)
2. Đọc thông báo lỗi và sửa code tương ứng
3. Đảm bảo tất cả package đã được cài đặt

### Lỗi 3: "Scene not found" hoặc "Missing references"

**Nguyên nhân**: File scene hoặc asset bị thiếu hoặc đường dẫn sai.

**Giải pháp**:
1. Kiểm tra file `.meta` có tồn tại không
2. Nhấn chuột phải vào asset bị lỗi → **Reimport**
3. Nếu vẫn lỗi, kiểm tra lại cấu trúc thư mục

### Lỗi 4: Unity Editor chạy chậm

**Nguyên nhân**: Dự án lớn hoặc máy tính yếu.

**Giải pháp**:
1. Tắt các cửa sổ không cần thiết trong Unity
2. Giảm chất lượng đồ họa trong Game view
3. Đóng các ứng dụng khác để giải phóng RAM

### Lỗi 5: "Package version mismatch"

**Nguyên nhân**: Phiên bản package không khớp với Unity version.

**Giải pháp**:
1. Xóa thư mục `Library` và `Temp`
2. Mở lại Unity Editor
3. Unity sẽ tự động import lại và cài đặt package đúng phiên bản

---

## 📁 CẤU TRÚC DỰ ÁN

```
RPG_The_Abyss/
│
├── Assets/                    # Thư mục chứa tất cả assets của game
│   ├── Animation/            # Animations và Animation Controllers
│   ├── Audio/                # File âm thanh (.wav, .mp3)
│   ├── Data/                 # ScriptableObjects và dữ liệu game
│   ├── Graphics/             # Sprites, textures, và hình ảnh
│   ├── InputSystem/          # Cấu hình Input System
│   ├── Materials/            # Materials và Physics Materials
│   ├── Prefab/               # Prefabs của các GameObject
│   ├── Scenes/               # Các scene của game
│   │   ├── MainMenu.unity
│   │   ├── Level_0.unity
│   │   ├── Level_1.unity
│   │   ├── SampleScene.unity
│   │   └── Test_Level.unity
│   ├── Scripts/              # Tất cả các script C#
│   ├── Settings/             # Cài đặt project
│   └── UI/                   # Giao diện người dùng
│
├── Library/                  # Thư mục cache của Unity (không chỉnh sửa)
├── Logs/                     # File log của Unity
├── Packages/                 # Package manifest và lock file
├── ProjectSettings/          # Cài đặt dự án Unity
├── README.md                 # File README chính
├── HUONG_DAN_CAI_DAT.md     # File hướng dẫn này
└── Assembly-CSharp.csproj   # Project file cho Visual Studio
```

---

## 📝 GHI CHÚ QUAN TRỌNG

1. **Không chỉnh sửa thư mục Library**: Thư mục này được Unity tự động tạo và quản lý.

2. **Backup trước khi chỉnh sửa**: Luôn backup dự án trước khi thực hiện thay đổi lớn.

3. **Kiểm tra version control**: Nếu sử dụng Git, đảm bảo file `.gitignore` đã bao gồm:
   - `Library/`
   - `Temp/`
   - `obj/`
   - `*.csproj`
   - `*.sln`

4. **Cập nhật Unity**: Nếu gặp lỗi không giải quyết được, thử cập nhật Unity lên phiên bản mới nhất.

---

## 🆘 HỖ TRỢ

Nếu gặp vấn đề không được liệt kê ở trên:

1. Kiểm tra file log trong thư mục `Logs/`
2. Xem Console trong Unity Editor để biết chi tiết lỗi
3. Tham khảo tài liệu Unity: https://docs.unity3d.com/
4. Liên hệ với nhóm phát triển

---

## ✅ KIỂM TRA CÀI ĐẶT THÀNH CÔNG

Sau khi cài đặt, bạn có thể kiểm tra bằng cách:

1. ✅ Unity Editor mở được dự án không có lỗi
2. ✅ Console không hiển thị lỗi màu đỏ
3. ✅ Có thể nhấn Play và game chạy được
4. ✅ Nhân vật có thể di chuyển và tấn công
5. ✅ Animation chạy mượt mà

---

**Chúc bạn cài đặt thành công và có trải nghiệm tuyệt vời với RPG: The Abyss!** ⚔️

*Cập nhật lần cuối: 2026*
