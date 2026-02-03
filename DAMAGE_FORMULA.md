# Công thức tổng — Sát thương kẻ địch nhận (công thức cơ bản)

Chỉ công thức toán, không Clamp / Random trong biểu thức. Giới hạn (cap) ghi ở chú thích.

---

## Ký hiệu (attacker = A, defender = D)

| Ký hiệu | Ý nghĩa | Nguồn |
|--------|--------|--------|
| **Damage_A** | Sát thương cơ bản | offense.damage (A) |
| **Strength_A, Agility_A, Intelligence_A** | Chỉ số chính | major (A) |
| **CritDamage_A** | Crit damage % = CritDamage_A_stat + Strength_A×0.5 | offense + major (A) |
| **ArmorReduction_A** | Xuyên giáp = ArmorReduction_A_stat/100 | offense (A) |
| **Fire_A, Ice_A, Lightning_A** | Sát thương Fire/Ice/Lightning | offense (A) |
| **Armor_D, Vitality_D, Intelligence_D** | Giáp, máu, INT | defend/major (D) |
| **Resist_D** | Kháng đúng element (Fire/Ice/Lightning) % | defend (D) |
| **s_phys, s_elem** | Hệ số scale vật lý / nguyên tố | DamageScaleData, mặc định 1 |
| **H** | Damage nguyên tố cao nhất = max(Fire_A, Ice_A, Lightning_A) | |
| **O1, O2** | Damage hai nguyên tố còn lại | |

---

## Công thức cơ bản (không Clamp, không Random)

**EffectiveArmor** = (Armor_D + Vitality_D×1) × (1 − ArmorReduction_A)

**Mitigation** = EffectiveArmor / (EffectiveArmor + 100)

**Resistance** = (Resist_D + Intelligence_D×0.5) / 100

```
DamageTaken =

  (Damage_A + Strength_A×1) × k_crit × s_phys × (1 − Mitigation)

  +

  (H + 0.5×(O1 + O2) + Intelligence_A×1) × s_elem × (1 − Resistance)
```

- **k_crit:** hệ số crit = CritDamage_A/100 khi crit, = 1 khi không crit.
- **Chú thích:** Trong code, Mitigation bị giới hạn tối đa 0.85; Resistance (phần trăm) tối đa 75%; né tránh (evasion) tối đa 85%. Công thức trên là dạng gốc trước khi áp dụng các giới hạn đó.

---

## Code tham chiếu

- `Entity_Health.cs`: TakeDamage, áp dụng mitigation & resistance.
- `Entity_Stats.cs`: GetPhysicalDamage, GetElementalDamage, GetArmorMitigation, GetElementalResistance, GetArmorReduction.
- `AttackData.cs` + `DamageScaleData.cs`: s_phys, s_elem.
