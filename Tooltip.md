# CÂU HỎI HỘI ĐỒNG CHẤM ĐỒ ÁN & GỢI Ý TRẢ LỜI
## RPG: The Abyss – Chuẩn bị bảo vệ

---

## A. HỆ THỐNG LƯU / LOAD (Save/Load)

### Câu 1. Thứ tự thực thi – SaveManager có chạy trước GameManager không?

**Trả lời:**  
**Không có gì đảm bảo** SaveManager chạy trước GameManager. Unity không đảm bảo thứ tự Awake/Start giữa các script trừ khi dùng **Script Execution Order**.  
Code chỉ đảm bảo **Load chạy sau mọi Start** nhờ `yield return null` trong SaveManager.Start(), chứ không đảm bảo SaveManager “chạy đầu tiên”.

**Nếu GameManager.Awake chạy trước SaveManager.Awake:**  
- `SaveManager.instance` có thể còn null → gọi `SaveManager.instance.GetGameData()` sẽ NullReferenceException (ví dụ trong GameManager.ContinuePlay, ChangeScene).  
**Cách xử lý đề xuất:**  
- Set **Script Execution Order** cho SaveManager (ví dụ -200) để Awake/Start chạy trước, hoặc  
- Trong GameManager không gọi SaveManager trong Awake; chỉ gọi trong Start hoặc khi đổi scene (lúc đó SaveManager đã có).

---

### Câu 2. Nhiều scene, một SaveManager – SaveManager có bị destroy khi chuyển scene?

**Trả lời:**  
Trong code hiện tại SaveManager **không** dùng DontDestroyOnLoad (bị comment).  
Khi gọi `SceneManager.LoadScene(sceneName)`:
- Toàn bộ object của scene cũ (trừ DontDestroyOnLoad) bị destroy, **kể cả SaveManager** nếu nó nằm trong scene đó.
- Scene mới load lên sẽ có **SaveManager mới** (nếu scene mới có GameObject gắn SaveManager).
- **gameData trong memory** của SaveManager cũ bị mất theo scene cũ.
- **Luồng thực tế:** Trước khi load scene mới, `GameManager.ChangeScene()` đã gọi `SaveManager.instance.SaveGame()` → **đã ghi ra file**. Scene mới vào → SaveManager mới Start → `LoadGame()` **đọc lại từ file** → gameData được tạo lại từ file.  
Vậy **không bị mất data** vì đã persist ra file trước khi đổi scene; nhưng cần **mỗi scene có SaveManager** (hoặc dùng DontDestroyOnLoad cho SaveManager) thì mới đúng thiết kế.

---

### Câu 3. LoadData và thứ tự – Thứ tự gọi LoadData có cố định không?

**Trả lời:**  
**Không cố định.** `FindAllSaveables()` dùng `FindObjectsByType<MonoBehaviour>().OfType<ISaveable>()` – thứ tự phụ thuộc thứ tự object trong scene / thứ tự Unity trả về, **không được đảm bảo** trong code.

**Rủi ro:**  
- Nếu GameManager.LoadData chạy sau Inventory_Player.LoadData, vẫn ổn vì GameManager chỉ đọc/ghi lastScene, lastPosition.  
- Nếu có ISaveable A phụ thuộc dữ liệu của ISaveable B (ví dụ B phải load xong trước A), hiện tại **có thể bị lỗi** vì thứ tự không kiểm soát.  
**Đề xuất:** Cần thì sắp xếp thứ tự load (ví dụ sort allSaveables theo priority, hoặc đăng ký ISaveable theo phase: trước load player, sau load player).

---

### Câu 4. DataLoadCompleted và race – Nếu scene mới không có SaveManager thì có chờ vô hạn không?

**Trả lời:**  
**Có.** Trong `ChangeSceneCo` có `while (dataLoadCompleted == false) { yield return null; }`.  
`dataLoadCompleted` chỉ được set `true` trong `GameManager.LoadData()`, mà LoadData chỉ được gọi khi SaveManager.LoadGame() chạy.  
Nếu scene mới **không có SaveManager** (hoặc SaveManager bị lỗi, không gọi LoadGame), thì GameManager.LoadData() không bao giờ chạy → **coroutine chờ vô hạn**.

**Đề xuất:** Thêm timeout (ví dụ sau 2–3 giây) hoặc kiểm tra “scene đã có SaveManager chưa”; nếu quá thời gian thì set `dataLoadCompleted = true` và log warning, tránh treo game.

---

## B. KIẾN TRÚC & DESIGN

### Câu 5. ISaveable và Singleton – Unit test Save/Load có khó không?

**Trả lời:**  
**Có, khó hơn.** Nhiều ISaveable gọi trực tiếp `SaveManager.instance`, `GameManager.instance`, `Player.instance` – phụ thuộc cứng vào singleton.  
Để unit test Save/Load mà không chạy full scene cần: inject interface cho SaveManager/GameManager, hoặc tạo scene test có đủ object, hoặc mock static instance (phức tạp trong Unity).  
Hiện tại code **không** dùng dependency injection; test thường phải là **integration test** (chạy trong Unity, có scene).

---

### Câu 6. Tách data và logic – Có tách “serialization model” và “runtime model” không?

**Trả lời:**  
**Chưa tách rõ.** GameData vừa là “serialization model” (ghi/đọc JSON) vừa là nơi mỗi ISaveable ghi/đọc trực tiếp. Các component (Inventory, Player, Portal…) vừa đọc GameData vừa sửa trạng thái runtime (list, transform, v.v.).  
**Hạn chế:** Đổi format save (versioning, migration) hoặc tách bản build không cần Unity (server, tool) sẽ khó hơn.  
**Đề xuất:** Có thể giữ GameData cho tương thích hiện tại, sau này thêm lớp DTO (data transfer object) chỉ chứa data thuần, map qua lại với GameData và runtime model.

---

### Câu 7. FindAllSaveables mỗi scene – Chi phí và cache?

**Trả lời:**  
FindObjectsByType<MonoBehaviour>() quét toàn bộ MonoBehaviours trong scene; với scene lớn có thể tốn.  
Code **không** đo performance và **không** cache danh sách ISaveable; mỗi lần SaveManager.Start() đều gọi FindAllSaveables() một lần.  
Vì Start() chỉ chạy một lần khi vào scene, trong đồ án quy mô vừa thường chấp nhận được. Nếu mở rộng (nhiều scene, nhiều object), có thể cache allSaveables và chỉ refresh khi scene load hoặc khi có object ISaveable spawn/destroy.

---

## C. HỆ THỐNG ITEM / INVENTORY

### Câu 8. Lỗi giá bán Merchant – TrySellItem dùng buyPrice thay vì sellPrice?

**Trả lời:**  
**Đúng, đây là lỗi.** Trong `Inventory_Merchant.TrySellItem()`:

- Dòng 58: `int sellPrice = Mathf.FloorToInt(itemToSell.sellPrice);` được tính nhưng **không dùng**.
- Dòng 60: `playerInventory.gold += itemToSell.buyPrice;` → đang cộng **giá mua** thay vì **giá bán**.

**Hậu quả:** Player bán đồ được trả đúng bằng giá mua (mua bao nhiêu bán bấy nhiêu), sai thiết kế (thường sellPrice < buyPrice).  
**Sửa:** Dùng `sellPrice` đã tính: `playerInventory.gold += sellPrice;` (và có thể nhân với `amountToSell` nếu bán nhiều stack trong cùng vòng lặp, tùy logic hiện tại).

---

### Câu 9. Load inventory và stacking – LoadData dùng Add hay AddItem?

**Trả lời:**  
**LoadData không dùng AddItem.** Trong `Inventory_Player.LoadData()`:

- Với mỗi (saveID, stackSize), code chạy: `for (i = 0; i < stackSize; i++) { itemList.Add(new Inventory_Item(itemData)); }`.
- Tức là **mỗi đơn vị là một Inventory_Item riêng** được add thẳng vào itemList.

**Hậu quả:** Stacking **không** được áp dụng. Ví dụ save 99 item cùng loại sẽ thành 99 slot trong itemList thay vì 1 slot stack 99 (nếu maxStackSize cho phép).  
**Đề xuất:** Trong LoadData nên gọi `AddItem(new Inventory_Item(itemData))` (hoặc tạo một item với stackSize rồi AddItem một lần) để tận dụng logic stack có sẵn trong AddItem/FindStackable.

---

### Câu 10. Chuyển đồ Storage – Chuyển từng đơn vị, có vấn đề hiệu năng không?

**Trả lời:**  
FromPlayerToStorage / FromStorageToPlayer đang chuyển **từng đơn vị** trong vòng for (transferAmount lần), mỗi lần RemoveOneItem + AddItem (hoặc Add bên storage).  
Với stack lớn (ví dụ 999) sẽ có 999 lần remove/add → nhiều lần duyệt list và có thể ảnh hưởng frame.  
**Đề xuất:** Có thể tối ưu bằng “chuyển theo stack”: tính số stack đủ chuyển (theo maxStackSize), rồi RemoveStack/AddItem theo từng stack thay vì từng 1 đơn vị.

---

## D. ENEMY / COMBAT

### Câu 11. Debug input còn trong build – KeyCode.V, Z, X, N?

**Trả lời:**  
Đúng, trong code vẫn có:
- Enemy_Reaper: `Input.GetKeyDown(KeyCode.V)` (teleport).
- Object_Merchant: `Input.GetKeyDown(KeyCode.Z)` (fill shop).
- Entity_DropManager: `Input.GetKeyDown(KeyCode.X)` (drop item).
- Player_Health: `Input.GetKeyDown(KeyCode.N)` (kill player).

Đây là **phím debug/test**, nếu build release mà không tắt thì người chơi có thể lợi dụng.  
**Đề xuất:** Bọc trong `#if UNITY_EDITOR` hoặc dùng cờ `bool enableDebugKeys` chỉ bật trong editor/build dev; build release tắt hẳn.

---

### Câu 12. Counter window và đồng bộ animation?

**Trả lời:**  
Cửa sổ counter được bật/tắt bởi **Animation Event** gọi `EnableCounterWindow()` / `DisableCounterWindow()` trong Enemy_AnimationTriggers.  
Nếu event gọi sai thời điểm (lệch vài frame so với hitbox hoặc animation state), có thể xảy ra “counter không ăn” hoặc “ăn counter khi không đúng thời điểm”.  
Code không có comment hoặc tài liệu nêu rõ frame nào mở/đóng; cần kiểm tra trên timeline animation và test thực tế. **Đề xuất:** Ghi rõ trong tài liệu hoặc comment khoảng frame/event nào tương ứng với “counter window”; test nhiều lần với các FPS khác nhau.

---

### Câu 13. Reaper spell – Prediction công thức và test?

**Trả lời:**  
Reaper dự đoán vị trí: `xOffset = playerMoving ? playerOffsetPrediction.x * playerScript.facingDirection : 0`, rồi spawn spell tại `player.position + (xOffset, playerOffsetPrediction.y)`.  
Công thức **có** trong code nhưng **chưa** được mô tả rõ trong tài liệu thiết kế (ENEMY_COMBAT_SYSTEM có thể bổ sung).  
**Test:** Nên test trường hợp player đứng yên (xOffset = 0), chạy sang trái/phải, và nhảy để xem spell có bám hợp lý không; có thể cần điều chỉnh playerOffsetPrediction theo feedback.

---

## E. BẢO MẬT & DỮ LIỆU

### Câu 14. Mã hóa save – XOR có phải encryption thật không?

**Trả lời:**  
XOR với codeword cố định ("nguyenxuanson") chỉ là **obfuscation** (che mắt), **không** phải encryption an toàn. Ai có file JSON (sau khi XOR lại) và biết codeword có thể sửa gold, item, level… dễ dàng.  
Code **không** có checksum/hash để phát hiện file bị sửa.  
**Đề xuất:** Trong báo cáo nên ghi rõ “obfuscation để tránh chỉnh file đơn giản”; nếu cần chống cheat mạnh hơn có thể thêm HMAC/checksum hoặc lưu critical data trên server (tùy yêu cầu đồ án).

---

### Câu 15. saveID và tham chiếu – Đổi asset, duplicate asset?

**Trả lời:**  
Item được nhận diện bằng `itemData.saveID` (thường là GUID của asset).  
Nếu đổi tên asset hoặc duplicate asset trong Unity, **saveID có thể thay đổi** (tùy cách Unity/OnValidate gán). Khi đó save cũ vẫn lưu saveID cũ → load sẽ không tìm thấy item trong itemDataBase (GetItemDataByID trả null) → **có thể mất item** hoặc bỏ qua (code đã có continue khi null).  
**Hiện tại** không thấy chiến lược versioning hay migration. **Đề xuất:** Không đổi saveID của asset đã dùng trong production; nếu bắt buộc đổi thì cần bảng map oldSaveID → newSaveID và xử lý khi LoadData.

---

## F. HIỆU NĂNG & MỞ RỘNG

### Câu 16. Slime split – Giới hạn số slime?

**Trả lời:**  
Code **không** giới hạn số slime trong scene hay số lần split. Một slime chết tạo 2 slime con; mỗi slime con chết lại tạo 2 nữa → có thể bùng nổ số lượng nếu không có cơ chế giới hạn (theo số lần split hoặc theo tổng số slime trong scene).  
**Đề xuất:** Thêm maxSplitLevel hoặc maxSlimeInScene; khi vượt thì không spawn thêm hoặc không cho split nữa.

---

### Câu 17. RollDrops và danh sách item lớn?

**Trả lời:**  
RollDrops() duyệt toàn bộ dropData.itemDataList, roll từng item, sort theo rarity, rồi duyệt lại để chọn theo maxRarityAmount.  
Với list rất lớn (hàng trăm item) có thể tốn CPU; code **không** đo và **chưa** tối ưu (cache, nhóm theo rarity).  
Với quy mô đồ án (vài chục item) thường chấp nhận được. Nếu mở rộng, có thể nhóm item theo rarity trước, hoặc dùng cấu trúc dữ liệu phù hợp để giảm duyệt.

---

## G. TÀI LIỆU & QUY TRÌNH

### Câu 18. Sơ đồ Save/Load trong báo cáo?

**Trả lời:**  
Trong repo đã có file **SAVE_LOAD_SYSTEM_DIAGRAM.md** mô tả luồng Save/Load, GameData, ISaveable, thời điểm Save/Load khi chuyển scene.  
**Nên:** Đưa nội dung tương tự (hoặc rút gọn) vào báo cáo đồ án, phần “Kiến trúc hệ thống” hoặc “Thiết kế hệ thống lưu/load”, kèm sơ đồ (flowchart/sequence) để hội đồng dễ theo dõi.

---

### Câu 19. Bug đã biết và danh sách known issues?

**Trả lời:**  
Nên có một mục **Known issues / Hạn chế** trong báo cáo hoặc README, liệt kê rõ:
- Merchant sell dùng buyPrice (đã nêu ở câu 8).
- Load inventory không gộp stack (câu 9).
- Thứ tự LoadData không cố định (câu 3).
- Chờ dataLoadCompleted có thể vô hạn nếu scene thiếu SaveManager (câu 4).
- Debug key V, Z, X, N chưa tắt trong build (câu 11).

Ghi kèm hướng xử lý hoặc lý do chưa sửa (ưu tiên, thời gian). Điều này thể hiện ý thức về chất lượng và giúp hội đồng đánh giá công bằng hơn.

---

### Câu 20. Test và QA – Test case Save/Load?

**Trả lời:**  
Code **không** thấy Unity Test Framework hay script test tự động cho Save/Load.  
**Nên có** ít nhất test case thủ công (hoặc checklist) cho:
1. Save tại Level_0 → thoát → mở lại game → Load → kiểm tra scene, vị trí, inventory, gold.
2. Chuyển scene qua Portal (đi/về town) → Save → Load lại → kiểm tra portal, vị trí, quest.
3. Inventory đầy, quick slot có đồ, đã trang bị → Save → Load → kiểm tra slot, stack, equipment.

Nếu có thời gian, có thể viết test trong Unity (Assert scene name, player position, gold, số lượng item) để tái hiện lỗi khi sửa code sau này.

---

## TÓM TẮT NÊU KHI BẢO VỆ

- **Điểm mạnh:** Có kiến trúc Save/Load rõ (SaveManager, GameData, ISaveable), tài liệu sơ đồ, xử lý chuyển scene (Save trước khi load, Load sau khi scene mới vào).
- **Điểm cần cải thiện:** (1) Sửa lỗi bán đồ (sellPrice), (2) Load inventory nên dùng AddItem để đúng stacking, (3) Đảm bảo thứ tự/thời điểm SaveManager và Load (Execution Order hoặc timeout dataLoadCompleted), (4) Tắt hoặc bảo vệ debug key khi build release, (5) Bổ sung known issues và test case vào báo cáo.

---

*Tài liệu gợi ý trả lời cho đồ án RPG The Abyss. Cập nhật theo code và thảo luận với hội đồng.*
