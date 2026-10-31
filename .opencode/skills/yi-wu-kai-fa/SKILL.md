---
name: yi-wu-kai-fa
description: 创建、修改Selee角色mod专属遗物。Use ONLY when the user asks to create or modify a relic for the Selee mod. Triggers on keywords: 遗物, relic, 新遗物, 添加遗物.
---

# 遗物开发

本skill指导如何为Selee角色模组创建和修改遗物。

## 目录

1. [创建遗物代码文件](#1-创建遗物代码文件)
2. [实现遗物类](#2-实现遗物类)
3. [添加本地化文本](#3-添加本地化文本)
4. [实现遗物效果](#4-实现遗物效果)
5. [遗物常用Hook参考](#5-遗物常用hook参考)
6. [值修改方法参考](#6-值修改方法参考)
7. [计数器与状态](#7-计数器与状态)
8. [其它内容](#8-其它内容)

所有代码操作默认在 `SeleeCode/` 目录下进行。

---

## 1. 创建遗物代码文件

在 `SeleeCode/Relics/` 目录下创建遗物的 `.cs` 文件。

**命名规则：**
- 中文遗物名 → 拼音命名，如"童谣"→ `TongYao.cs`

---

## 2. 实现遗物类

参考模板文件 `SeleeCode/Relics/SeleeRelic.cs` 创建。Selee角色mod的遗物基类为 `SeleeRelic`，其继承自 `CustomRelicModel` 并已处理了遗物图像路径和遗物池注解。

### 2.1 构造函数

遗物需继承 `SeleeRelic` 类。`SeleeRelic` 无需构造函数参数。

**示例：**
```csharp
public class MyRelic() : SeleeRelic
```

注意：`SeleeRelic` 基类已通过 `[Pool(typeof(SeleeRelicPool))]` 注解将所有派生遗物自动添加到Selee遗物池，无需手动设置遗物池。

### 2.2 必须重载的项

每个遗物类**必须**重载以下属性：

| 项目 | 签名 | 说明 |
|------|------|------|
| 遗物稀有度 | `override RelicRarity Rarity` | 遗物的稀有度等级 |

**RelicRarity 枚举：**
```csharp
public enum RelicRarity
{
    None,
    Starter,    //初始遗物
    Common,     //普通
    Uncommon,   //罕见
    Rare,       //稀有
    Shop,       //商店
    Event,      //事件
    Ancient,    //先古（boss遗物）
}
```

**示例：**
```csharp
public override RelicRarity Rarity => RelicRarity.Uncommon;
```

### 2.3 模板代码

```csharp
using MegaCrit.Sts2.Core.Entities.Relics;

namespace Selee.SeleeCode.Relics;

public class _TemplateRelic() : SeleeRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Var", 1m),
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [];

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        Flash();
    }
}
```

---

## 3. 添加本地化文本

打开 `Selee/localization/zhs/relics.json`，在JSON对象中添加三项：

```json
"SELEE-{RELIC_NAME}.description": "遗物描述文本",
"SELEE-{RELIC_NAME}.flavor": "遗物风味文本",
"SELEE-{RELIC_NAME}.title": "遗物中文名称"
```

**RELIC_NAME 命名规则：** 类名转为大写+下划线格式。
- 类名 `TongYao` → `TONG_YAO`

**描述文本中的动态变量格式：** 与卡牌相同
- `{VarName:diff()}` — 显示数值，升级时显示差值
- `[gold]文本[/gold]` — 金色高亮文本

**示例：**
```json
"SELEE-TONG_YAO.description": "战斗开始时，获得两层[gold]量子叠加[/gold]",
"SELEE-TONG_YAO.flavor": "[red]这个遗物的细节将在未来揭晓……[/red]",
"SELEE-TONG_YAO.title": "童谣"
```

---

## 4. 实现遗物效果

实现遗物效果时，若遇到实现方法未知的需求，应先查阅 `docs/开发经验.md` 寻找是否有相关经验记录，没有相关经验再去查阅游戏本体代码或 [BaseLib开发文档](https://alchyr.github.io/BaseLib-Wiki/docs/models/custom-relic.html) 以找到实现方法。

### 步骤

1. **查阅开发经验**：打开 `docs/开发经验.md`，搜索是否有与目标效果相关的经验记录

2. **查找原版遗物描述**：打开 `%USERPROFILE%\Documents\sts_mods\sts2_src\localization\zhs\relics.json`，搜索与目标效果相关的描述，并找到对应的原版遗物名称

3. **找到对应代码文件**：根据遗物名称转为驼峰命名，在 `%USERPROFILE%\Documents\sts_mods\sts2_src\src\Core\Models\Relics` 目录下查找对应的 `.cs` 文件
   - 例如：`AKABEKO` → `Akabeko.cs`

4. **参考实现**：阅读原版遗物的代码，参考以下内容：
   - 使用了哪些Hook方法
   - `DynamicVars` 的初始化和应用
   - 值修改方法（`Modify*`）的返回值
   - 状态追踪（`[SavedProperty]`、计数器等）

5. **也可参考本mod内其它遗物**：`SeleeCode/Relics/` 目录下的现有遗物

### 常用效果参考

**触发遗物闪烁动画：**
```csharp
Flash();
```
在遗物效果生效时调用，播放遗物激活动画和音效。

**施加能力(Power)：**

若遗物需要施加自定义能力，请参考 **能力开发skill** (`neng-li-kai-fa`) 获取完整的能力开发指南。能力类命名规则：中文能力名 → 拼音 + `Power` 后缀，如"量子叠加"→ `LiangZiDieJiaPower`。

```csharp
await PowerCmd.Apply<SomePower>(
    new ThrowingPlayerChoiceContext(),
    base.Owner.Creature,
    base.DynamicVars["VarName"].IntValue,
    base.Owner.Creature, null);
```

**获得格挡：**
```csharp
await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars["Block"], cardPlay);
```

**抽牌：**
```csharp
await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner);
```

---

## 5. 遗物常用Hook参考

遗物通过重载生命周期Hook方法来响应游戏事件。以下按类别列出常用Hook：

### 战斗生命周期

| Hook | 签名 | 触发时机 |
|------|------|----------|
| `BeforeCombatStart` | `Task BeforeCombatStart()` | 战斗开始前 |
| `AfterCombatEnd` | `Task AfterCombatEnd(CombatRoom room)` | 战斗结束后 |
| `AfterCombatVictory` | `Task AfterCombatVictory(CombatRoom room)` | 战斗胜利后 |

### 回合生命周期

| Hook | 签名 | 触发时机 |
|------|------|----------|
| `AfterSideTurnStart` | `Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)` | 任一方回合开始后 |
| `AfterPlayerTurnStart` | `Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)` | 玩家回合开始后 |
| `BeforeSideTurnEnd` | `Task BeforeSideTurnEnd(PlayerChoiceContext, CombatSide, IEnumerable<Creature>)` | 任一方回合结束前 |
| `AfterSideTurnEnd` | `Task AfterSideTurnEnd(PlayerChoiceContext, CombatSide, IEnumerable<Creature>)` | 任一方回合结束后 |

### 卡牌相关

| Hook | 签名 | 触发时机 |
|------|------|----------|
| `BeforeCardPlayed` | `Task BeforeCardPlayed(CardPlay)` | 卡牌打出前 |
| `AfterCardPlayed` | `Task AfterCardPlayed(PlayerChoiceContext, CardPlay)` | 卡牌打出后 |
| `AfterCardDrawn` | `Task AfterCardDrawn(PlayerChoiceContext, CardModel, bool fromHandDraw)` | 抽牌后 |
| `AfterCardDiscarded` | `Task AfterCardDiscarded(PlayerChoiceContext, CardModel)` | 弃牌后 |
| `AfterCardExhausted` | `Task AfterCardExhausted(PlayerChoiceContext, CardModel, bool causedByEthereal)` | 消耗牌后 |

### 伤害/生命相关

| Hook | 签名 | 触发时机 |
|------|------|----------|
| `BeforeAttack` | `Task BeforeAttack(AttackCommand)` | 攻击执行前 |
| `AfterAttack` | `Task AfterAttack(PlayerChoiceContext, AttackCommand)` | 攻击执行后 |
| `AfterDamageReceived` | `Task AfterDamageReceived(PlayerChoiceContext, Creature target, DamageResult, ValueProp, Creature? dealer, CardModel?)` | 受到伤害后 |
| `AfterDamageGiven` | `Task AfterDamageGiven(PlayerChoiceContext, Creature? dealer, DamageResult, ValueProp, Creature target, CardModel?)` | 造成伤害后 |
| `AfterCurrentHpChanged` | `Task AfterCurrentHpChanged(Creature, decimal delta)` | HP变化后 |

### 格挡相关

| Hook | 签名 | 触发时机 |
|------|------|----------|
| `BeforeBlockGained` | `Task BeforeBlockGained(Creature, decimal, ValueProp, CardModel?)` | 获得格挡前 |
| `AfterBlockGained` | `Task AfterBlockGained(Creature, decimal, ValueProp, CardModel?)` | 获得格挡后 |

### 能量/抽牌相关

| Hook | 签名 | 触发时机 |
|------|------|----------|
| `AfterEnergyReset` | `Task AfterEnergyReset(Player)` | 能量重置后 |
| `AfterEnergySpent` | `Task AfterEnergySpent(CardModel, int amount)` | 消耗能量后 |
| `BeforeHandDraw` | `Task BeforeHandDraw(Player, PlayerChoiceContext, ICombatState)` | 抽手牌前 |

### 能力(Power)相关

| Hook | 签名 | 触发时机 |
|------|------|----------|
| `BeforePowerAmountChanged` | `Task BeforePowerAmountChanged(PowerModel, decimal, Creature target, Creature? applier, CardModel?)` | 能力层数变化前 |
| `AfterPowerAmountChanged` | `Task AfterPowerAmountChanged(PlayerChoiceContext, PowerModel, decimal, Creature?, CardModel?)` | 能力层数变化后 |

### 遗物生命周期

| Hook | 签名 | 触发时机 |
|------|------|----------|
| `AfterObtained` | `virtual Task AfterObtained()` | 获得遗物后 |
| `AfterRemoved` | `virtual Task AfterRemoved()` | 遗物被移除后 |

### 房间/地图相关

| Hook | 签名 | 触发时机 |
|------|------|----------|
| `AfterRoomEntered` | `Task AfterRoomEntered(AbstractRoom)` | 进入房间后 |
| `AfterItemPurchased` | `Task AfterItemPurchased(Player, MerchantEntry, int goldSpent)` | 购买物品后 |
| `AfterGoldGained` | `Task AfterGoldGained(Player)` | 获得金币后 |
| `AfterRestSiteHeal` | `Task AfterRestSiteHeal(Player, bool isMimicked)` | 篝火休息后 |
| `AfterRestSiteSmith` | `Task AfterRestSiteSmith(Player)` | 篝火升级后 |

---

## 6. 值修改方法参考

遗物可以通过重载 `Modify*` 方法来修改游戏中的数值。这些方法返回修改后的值，不会消耗额外的游戏行动。

### 伤害修改

| 方法 | 签名 | 默认返回 | 说明 |
|------|------|----------|------|
| `ModifyDamageAdditive` | `decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp, Creature? dealer, CardModel?)` | `0m` | 加法修改伤害（返回增加量） |
| `ModifyDamageMultiplicative` | `decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp, Creature? dealer, CardModel?)` | `1m` | 乘法修改伤害（返回乘数，如2m表示翻倍） |

### 格挡修改

| 方法 | 签名 | 默认返回 | 说明 |
|------|------|----------|------|
| `ModifyBlockAdditive` | `decimal ModifyBlockAdditive(Creature, decimal block, ValueProp, CardModel?, CardPlay?)` | `0m` | 加法修改格挡 |
| `ModifyBlockMultiplicative` | `decimal ModifyBlockMultiplicative(Creature, decimal block, ValueProp, CardModel?, CardPlay?)` | `1m` | 乘法修改格挡 |

### 能量修改

| 方法 | 签名 | 默认返回 | 说明 |
|------|------|----------|------|
| `ModifyMaxEnergy` | `decimal ModifyMaxEnergy(Player, decimal amount)` | `amount` | 修改最大能量 |
| `ModifyEnergyGain` | `decimal ModifyEnergyGain(Player, decimal amount)` | `amount` | 修改能量获取量 |

### 抽牌修改

| 方法 | 签名 | 默认返回 | 说明 |
|------|------|----------|------|
| `ModifyHandDraw` | `decimal ModifyHandDraw(Player, decimal count)` | `count` | 修改抽牌数量 |

### 生命流失修改

| 方法 | 签名 | 默认返回 | 说明 |
|------|------|----------|------|
| `ModifyHpLostBeforeOsty` | `decimal ModifyHpLostBeforeOsty(Creature, decimal, ValueProp, Creature?, CardModel?)` | `amount` | 修改HP流失（奥斯提之前） |
| `ModifyHpLostAfterOsty` | `decimal ModifyHpLostAfterOsty(Creature, decimal, ValueProp, Creature?, CardModel?)` | `amount` | 修改HP流失（奥斯提之后） |

### 能力层数修改

| 方法 | 签名 | 默认返回 | 说明 |
|------|------|----------|------|
| `ModifyPowerAmountGiven` | `decimal ModifyPowerAmountGiven(PowerModel, Creature, decimal, Creature?, CardModel?)` | `amount` | 修改施加的能力层数 |

### 攻击/卡牌修改

| 方法 | 签名 | 默认返回 | 说明 |
|------|------|----------|------|
| `ModifyAttackHitCount` | `int ModifyAttackHitCount(AttackCommand, int)` | `hitCount` | 修改攻击次数 |
| `ModifyXValue` | `int ModifyXValue(CardModel, int)` | `originalValue` | 修改X耗能卡牌的X值 |

### 商店/奖励修改

| 方法 | 签名 | 默认返回 | 说明 |
|------|------|----------|------|
| `ModifyMerchantPrice` | `decimal ModifyMerchantPrice(Player, MerchantEntry, decimal)` | `cost` | 修改商店价格 |
| `ModifyCardRewardCreationOptions` | `CardCreationOptions ModifyCardRewardCreationOptions(Player, CardCreationOptions)` | `options` | 修改卡牌奖励选项 |

### 常用值修改示例

**增加固定伤害（如遗物使攻击伤害+2）：**
```csharp
public override decimal ModifyDamageAdditive(Creature? target, decimal amount,
    ValueProp props, Creature? dealer, CardModel? cardSource)
{
    if (!props.IsPoweredAttack() || cardSource == null) return 0m;
    if (dealer != base.Owner.Creature && dealer != base.Owner.Osty) return 0m;
    return 2m;
}
```

**伤害翻倍（条件触发时）：**
```csharp
public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount,
    ValueProp props, Creature? dealer, CardModel? cardSource)
{
    if (someCondition) return 2m;
    return 1m;
}
```

**减少HP流失（如钨合金棒）：**
```csharp
public override decimal ModifyHpLostAfterOsty(Creature target, decimal amount,
    ValueProp props, Creature? dealer, CardModel? cardSource)
{
    if (target != base.Owner.Creature) return amount;
    return Math.Max(0m, amount - base.DynamicVars["HpLossReduction"].BaseValue);
}

public override Task AfterModifyingHpLostAfterOsty()
{
    Flash();
    return Task.CompletedTask;
}
```
注意：`Modify*` 修改值后，对应 `AfterModifying*` Hook会被调用，可在其中 `Flash()` 遗物。

**增加每回合抽牌数：**
```csharp
public override decimal ModifyHandDraw(Player player, decimal count)
{
    if (player != base.Owner) return count;
    return count + 1;
}
```

---

## 7. 计数器与状态

### 显示计数器

遗物可以显示一个计数器数字徽章：

```csharp
public override bool ShowCounter => true;

public override int DisplayAmount => _counter;

private int _counter;

[SavedProperty]
public int Counter
{
    get => _counter;
    private set { AssertMutable(); _counter = value; UpdateDisplay(); }
}
```

- `ShowCounter` → 是否显示计数器
- `DisplayAmount` → 显示的数字
- `[SavedProperty]` → 标记字段需要存档保存
- `AssertMutable()` → 在setter中调用，确保在运行实例上修改
- `UpdateDisplay()` → 通知UI更新显示
- 修改计数器值后调用 `InvokeDisplayAmountChanged()` 也可更新UI

### 遗物状态

可以设置遗物的视觉状态：

```csharp
base.Status = RelicStatus.Normal;   // 正常
base.Status = RelicStatus.Active;   // 激活发光（即将触发）
base.Status = RelicStatus.Disabled; // 灰色（已消耗）
```

### 获取遗物后触发效果

如果遗物在获取时需要立即执行效果（如添加诅咒、提供奖励选择）：

```csharp
public override bool HasUponPickupEffect => true;

public override async Task AfterObtained()
{
    Flash();
    // 执行效果...
}
```

设置 `HasUponPickupEffect => true` 会使遗物不可交易。

---

## 8. 其它内容

### DynamicVar类型

与卡牌相同，遗物也使用 `CanonicalVars` 声明动态变量：

| 类型 | 用法 | 示例 |
|------|------|------|
| `DynamicVar` | 通用变量 | `new DynamicVar("VarName", 7m)` |
| `DamageVar` | 伤害变量 | `new DamageVar(6m, ValueProp.Move)` |
| `BlockVar` | 格挡变量 | `new BlockVar(4m, ValueProp.Move)` |
| `PowerVar<T>` | 能力变量 | `new PowerVar<SomePower>(1)` |

### ExtraHoverTips

为遗物添加鼠标悬停提示（如能力关键词说明）：

```csharp
protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [HoverTipFactory.FromPower<SomePower>()];
```

注意：泛型参数使用能力的完整类名（含 `Power` 后缀），如 `HoverTipFactory.FromPower<LiangZiDieJiaPower>()`。

### 常用命名空间

```csharp
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Saves.Runs;
```

### 根据设计文档开发

当用户要求"根据设计文档开发"遗物时，使用设计文档读取脚本获取遗物设计数据：

1. **列出所有sheet页**：`python .opencode/scripts/read_design_doc.py`
2. **读取遗物sheet全部内容**：`python .opencode/scripts/read_design_doc.py 遗物`
3. **搜索特定遗物**：`python .opencode/scripts/read_design_doc.py 遗物 {遗物名称}`

搜索结果为JSON数组，每个元素是一个字典，key为表头（名称、类型/稀有度、说明）。

读取设计数据后，按照上述步骤1-7实现遗物。

### 其它引用

- Baselib库，该库封装了许多杀戮尖塔2的mod api，源码可查看`%USERPROFILE%\Documents\sts_mods\sts2_lib\BaseLib-StS2`目录
- BaseLib遗物开发文档：https://alchyr.github.io/BaseLib-Wiki/docs/models/custom-relic.html
- SeleeRelic基类源码：`SeleeCode/Relics/SeleeRelic.cs`
- 原版遗物代码目录：`%USERPROFILE%\Documents\sts_mods\sts2_src\src\Core\Models\Relics`
