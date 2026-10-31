---
name: neng-li-kai-fa
description: 创建、修改Selee角色mod专属能力(Power)。Use ONLY when the user asks to create or modify a power for the Selee mod. Triggers on keywords: 能力, power, 新能力, 添加能力, 状态效果.
---

# 能力开发

本skill指导如何为Selee角色模组创建和修改能力（Power，即状态效果）。

## 目录

1. [创建能力代码文件](#1-创建能力代码文件)
2. [实现能力类](#2-实现能力类)
3. [添加本地化文本](#3-添加本地化文本)
4. [实现能力效果](#4-实现能力效果)
5. [能力常用Hook参考](#5-能力常用hook参考)
6. [值修改方法参考](#6-值修改方法参考)
7. [持续时间与递减模式](#7-持续时间与递减模式)
8. [在卡牌/遗物中施加能力](#8-在卡牌遗物中施加能力)
9. [其它内容](#9-其它内容)

所有代码操作默认在 `SeleeCode/` 目录下进行。

---

## 1. 创建能力代码文件

在 `SeleeCode/Powers/` 目录下创建能力的 `.cs` 文件。

**命名规则：**
- 中文能力名 → 拼音命名 + `Power` 后缀，如"量子叠加"→ `LiangZiDieJiaPower.cs`
- **必须添加 `Power` 后缀**，以与卡牌、遗物等命名空间区分

---

## 2. 实现能力类

参考模板文件 `SeleeCode/Powers/SeleePower.cs` 创建。Selee角色mod的能力基类为 `SeleePower`，其继承自 `CustomPowerModel` 并已处理了能力图像路径。

### 2.1 构造函数

能力需继承 `SeleePower` 类。`SeleePower` 无需构造函数参数。

**示例：**
```csharp
public class LiangZiDieJiaPower() : SeleePower
```

注意：与卡牌和遗物不同，能力不需要池注解，能力通过 `PowerCmd.Apply<T>()` 动态施加给生物。

### 2.2 必须重载的项

每个能力类**必须**重载以下属性：

| 项目 | 签名 | 说明 |
|------|------|------|
| 能力类型 | `override PowerType Type` | Buff还是Debuff |
| 叠加方式 | `override PowerStackType StackType` | 再次施加时如何叠加 |

**PowerType 枚举：**
```csharp
public enum PowerType
{
    None,
    Buff,      //增益（绿色图标）
    Debuff,    //减益（红色图标）
}
```

**PowerStackType 枚举：**
```csharp
public enum PowerStackType
{
    None,      //不叠加
    Counter,   //层数叠加（最常见，再次施加时增加层数）
    Single,    //不叠加但更明确（如路障，要么有要么没有）
}
```

**示例：**
```csharp
public override PowerType Type => PowerType.Buff;
public override PowerStackType StackType => PowerStackType.Counter;
```

### 2.3 可选重载

| 项目 | 签名 | 默认值 | 说明 |
|------|------|--------|------|
| 允许负数 | `override bool AllowNegative` | `false` | 设为true允许层数为负数 |
| 显示层数 | `override int DisplayAmount` | `Amount` | 自定义显示的层数数字 |

### 2.4 模板代码

```csharp
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Selee.SeleeCode.Powers;

public class _TemplatePower() : SeleePower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [];
}
```

### 2.5 可叠加能力的效果数值应使用Amount

当能力对应的是能力牌（Power类型卡牌），且需要支持多张同名卡牌打出后效果叠加时，**效果的数值应使用 `base.Amount` 记录，而非 `DynamicVar`**。

原因：`PowerCmd.Apply` 对同一类型能力再次施加时会自动叠加 Amount 层数。如果使用 DynamicVar 记录数值，再次施加时 DynamicVar 不会叠加，效果丢失。

**能力侧：** 移除 `CanonicalVars` 中对应的效果数值变量，效果逻辑中直接使用 `base.Amount`：

```csharp
// 错误：使用DynamicVar记录效果数值
protected override IEnumerable<DynamicVar> CanonicalVars => [
    new DynamicVar("Value", 20m),
];
// 触发效果时使用 DynamicVars["Value"].BaseValue → 再次施加时不叠加

// 正确：使用Amount记录效果数值
// 无需在CanonicalVars中声明该变量
// 触发效果时使用 base.Amount → 再次施加时Amount叠加
await PowerCmd.Apply<OtherPower>(choiceContext, base.Owner, base.Amount, base.Owner, null);
```

**卡牌侧：** 使用 `PowerVar<T>` 声明变量，`OnPlay` 时以 DynamicVar 值作为 Amount 施加：

```csharp
protected override IEnumerable<DynamicVar> CanonicalVars => [
    new PowerVar<SomePower>(20),
];

protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
{
    await PowerCmd.Apply<SomePower>(choiceContext, base.Owner.Creature, DynamicVars["SomePower"].BaseValue, base.Owner.Creature, this);
}
```

**本地化：** `smartDescription` 中使用 `{Amount}` 替代 `{VarName:diff()}` 显示数值。

注意：此规则仅适用于能力牌对应的可叠加能力。对于技能牌/攻击牌施加的一次性能力，仍可使用 DynamicVar。

---

## 3. 添加本地化文本

打开 `Selee/localization/zhs/powers.json`，在JSON对象中添加三项：

```json
"SELEE-{POWER_NAME}.title": "能力中文名称",
"SELEE-{POWER_NAME}.description": "能力描述文本",
"SELEE-{POWER_NAME}.smartDescription": "智能描述文本"
```

**POWER_NAME 命名规则：** 类名转为大写+下划线格式。
- 类名 `LiangZiDieJiaPower` → `LIANG_ZI_DIE_JIA_POWER`

**重要：`smartDescription` 为必填项**，构建时若缺失会报错 `STS001: Localization ... not found`。

### description 与 smartDescription 的区别

- **description**：能力的完整描述，始终显示
- **smartDescription**：智能描述，当能力有层数时优先显示。可使用 `{Amount}` 引用当前层数

### 描述文本中的变量格式

- `{Amount}` — 当前能力层数（无需 `:diff()`）
- `{VarName:diff()}` — 显示DynamicVar数值
- `[gold]文本[/gold]` — 金色高亮文本
- `[blue]文本[/blue]` — 蓝色高亮文本

**示例：**
```json
"SELEE-LIANG_ZI_DIE_JIA_POWER.title": "量子叠加",
"SELEE-LIANG_ZI_DIE_JIA_POWER.description": "从卡牌中获得的格挡值+1。回合结束时减少1层。",
"SELEE-LIANG_ZI_DIE_JIA_POWER.smartDescription": "从卡牌中获得的格挡值增加[blue]{block_add:diff()}[/blue]。剩余[blue]{Amount}[/blue]回合。"
```

---

## 4. 实现能力效果

实现能力效果时，**应优先查询设计文档能力sheet页的"说明"和"备注"列**，其中通常包含能力的具体行为描述、触发条件、实现提示等关键信息。仅当设计文档无相关描述时，再参考现有模组能力或游戏本体能力的实现方式。

查询设计文档能力sheet的方式：

```bash
python .opencode/scripts/read_design_doc.py 能力 {能力名称}
```

返回结果包含"名称"、"说明"、"叠加方式"、"类型"、"备注"等字段。其中：

- **说明**列：能力的核心效果描述，是实现的主要依据
- **备注**列：补充的实现细节，如特殊实现方式、参考对象、特殊属性等

### 步骤

1. **查阅开发经验**：打开 `docs/开发经验.md`，搜索是否有与目标效果相关的经验记录

2. **查找原版能力描述**：打开 `%USERPROFILE%\Documents\sts_mods\sts2_src\localization\zhs\powers.json`，搜索与目标效果相关的描述，并找到对应的原版能力名称

3. **找到对应代码文件**：根据能力名称转为驼峰命名，在 `%USERPROFILE%\Documents\sts_mods\sts2_src\src\Core\Models\Powers` 目录下查找对应的 `.cs` 文件
   - 例如：`DEXTERITY_POWER` → `DexterityPower.cs`
   - 例如：`BLUR_POWER` → `BlurPower.cs`

4. **参考实现**：阅读原版能力的代码，参考以下内容：
   - 使用了哪些Hook方法和值修改方法
   - `DynamicVars` 的初始化和应用
   - 持续时间/递减的实现模式
   - 属主判断逻辑（`base.Owner` vs `target`）

5. **也可参考本mod内其它能力**：`SeleeCode/Powers/` 目录下的现有能力

### 常用效果参考

**触发能力闪烁动画：**
```csharp
Flash();
```
在能力效果生效时调用，播放能力激活动画。

**修改卡牌格挡值（参考DexterityPower）：**
```csharp
public override decimal ModifyBlockAdditive(Creature target, decimal block, ValueProp props, CardModel? cardSource, CardPlay? cardPlay)
{
    if (cardSource != null)
    {
        if (cardSource.Owner.Creature != base.Owner) return 0m;
    }
    else if (base.Owner != target)
    {
        return 0m;
    }
    if (!props.IsPoweredCardOrMonsterMoveBlock()) return 0m;
    return 1m; // 固定+1格挡
}
```
关键判断逻辑：
- `cardSource != null` → 来自卡牌的格挡，需判断卡牌所有者是否为能力持有者
- `cardSource == null` → 非卡牌来源的格挡，需判断目标是否为能力持有者
- `props.IsPoweredCardOrMonsterMoveBlock()` → 确保是"有来源"的格挡（卡牌或怪物行动），排除纯数值格挡

**修改卡牌伤害值（参考StrengthPower）：**
```csharp
public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
{
    if (!props.IsPoweredAttack() || cardSource == null) return 0m;
    if (dealer != base.Owner) return 0m;
    return base.Amount;
}
```

---

## 5. 能力常用Hook参考

能力通过重载生命周期Hook方法来响应游戏事件。以下按类别列出常用Hook：

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

### 伤害/生命相关

| Hook | 签名 | 触发时机 |
|------|------|----------|
| `BeforeAttack` | `Task BeforeAttack(AttackCommand)` | 攻击执行前 |
| `AfterAttack` | `Task AfterAttack(PlayerChoiceContext, AttackCommand)` | 攻击执行后 |
| `AfterDamageReceived` | `Task AfterDamageReceived(PlayerChoiceContext, Creature target, DamageResult, ValueProp, Creature? dealer, CardModel?)` | 受到伤害后 |
| `AfterDamageGiven` | `Task AfterDamageGiven(PlayerChoiceContext, Creature? dealer, DamageResult, ValueProp, Creature target, CardModel?)` | 造成伤害后 |

### 格挡相关

| Hook | 签名 | 触发时机 |
|------|------|----------|
| `BeforeBlockGained` | `Task BeforeBlockGained(Creature, decimal, ValueProp, CardModel?)` | 获得格挡前 |
| `AfterBlockGained` | `Task AfterBlockGained(Creature, decimal, ValueProp, CardModel?)` | 获得格挡后 |
| `AfterBlockCleared` | `Task AfterBlockCleared(Creature)` | 格挡清零后 |

### 能力相关

| Hook | 签名 | 触发时机 |
|------|------|----------|
| `BeforePowerAmountChanged` | `Task BeforePowerAmountChanged(PowerModel, decimal, Creature target, Creature? applier, CardModel?)` | 能力层数变化前 |
| `AfterPowerAmountChanged` | `Task AfterPowerAmountChanged(PlayerChoiceContext, PowerModel, decimal, Creature?, CardModel?)` | 能力层数变化后 |

### 战斗生命周期

| Hook | 签名 | 触发时机 |
|------|------|----------|
| `BeforeCombatStart` | `Task BeforeCombatStart()` | 战斗开始前 |
| `AfterCombatEnd` | `Task AfterCombatEnd(CombatRoom room)` | 战斗结束后 |

---

## 6. 值修改方法参考

能力可以通过重载 `Modify*` 方法来修改游戏中的数值。这些方法返回修改后的值，不会消耗额外的游戏行动。

### 伤害修改

| 方法 | 签名 | 默认返回 | 说明 |
|------|------|----------|------|
| `ModifyDamageAdditive` | `decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp, Creature? dealer, CardModel?)` | `0m` | 加法修改伤害（返回增加量） |
| `ModifyDamageMultiplicative` | `decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp, Creature? dealer, CardModel?)` | `1m` | 乘法修改伤害（返回乘数） |

### 格挡修改

| 方法 | 签名 | 默认返回 | 说明 |
|------|------|----------|------|
| `ModifyBlockAdditive` | `decimal ModifyBlockAdditive(Creature, decimal block, ValueProp, CardModel?, CardPlay?)` | `0m` | 加法修改格挡 |
| `ModifyBlockMultiplicative` | `decimal ModifyBlockMultiplicative(Creature, decimal block, ValueProp, CardModel?, CardPlay?)` | `1m` | 乘法修改格挡 |

### 生命流失修改

| 方法 | 签名 | 默认返回 | 说明 |
|------|------|----------|------|
| `ModifyHpLostBeforeOsty` | `decimal ModifyHpLostBeforeOsty(Creature, decimal, ValueProp, Creature?, CardModel?)` | `amount` | 修改HP流失（奥斯提之前） |
| `ModifyHpLostAfterOsty` | `decimal ModifyHpLostAfterOsty(Creature, decimal, ValueProp, Creature?, CardModel?)` | `amount` | 修改HP流失（奥斯提之后） |

### 能力层数修改

| 方法 | 签名 | 默认返回 | 说明 |
|------|------|----------|------|
| `ModifyPowerAmountGiven` | `decimal ModifyPowerAmountGiven(PowerModel, Creature, decimal, Creature?, CardModel?)` | `amount` | 修改施加的能力层数 |

### 抽牌修改

| 方法 | 签名 | 默认返回 | 说明 |
|------|------|----------|------|
| `ModifyHandDraw` | `decimal ModifyHandDraw(Player, decimal count)` | `count` | 修改抽牌数量 |

### 伤害上限

| 方法 | 签名 | 默认返回 | 说明 |
|------|------|----------|------|
| `ModifyDamageCap` | `decimal ModifyDamageCap(Creature?, ValueProp, Creature?, CardModel?)` | `decimal.MaxValue` | 修改伤害上限 |

### 常用值修改示例

**格挡+1（固定值，不随层数增加）：**
```csharp
public override decimal ModifyBlockAdditive(Creature target, decimal block, ValueProp props, CardModel? cardSource, CardPlay? cardPlay)
{
    if (cardSource != null)
    {
        if (cardSource.Owner.Creature != base.Owner) return 0m;
    }
    else if (base.Owner != target)
    {
        return 0m;
    }
    if (!props.IsPoweredCardOrMonsterMoveBlock()) return 0m;
    return 1m;
}
```

**格挡随层数增加（等效敏捷）：**
```csharp
public override decimal ModifyBlockAdditive(Creature target, decimal block, ValueProp props, CardModel? cardSource, CardPlay? cardPlay)
{
    if (cardSource != null)
    {
        if (cardSource.Owner.Creature != base.Owner) return 0m;
    }
    else if (base.Owner != target)
    {
        return 0m;
    }
    if (!props.IsPoweredCardOrMonsterMoveBlock()) return 0m;
    return base.Amount;
}
```

**格挡减少25%（等效脆弱）：**
```csharp
public override decimal ModifyBlockMultiplicative(Creature target, decimal block, ValueProp props, CardModel? cardSource, CardPlay? cardPlay)
{
    if (base.Owner != target) return 1m;
    if (!props.IsPoweredCardOrMonsterMoveBlock()) return 1m;
    return 0.75m;
}
```

**伤害+层数（等效力量）：**
```csharp
public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
{
    if (!props.IsPoweredAttack() || cardSource == null) return 0m;
    if (dealer != base.Owner) return 0m;
    return base.Amount;
}
```

**伤害受限为1（等效无形）：**
```csharp
public override decimal ModifyHpLostAfterOsty(Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
{
    if (target != base.Owner) return amount;
    if (amount < 1m) return amount;
    return 1m;
}
```

注意：`Modify*` 修改值后，对应 `AfterModifying*` Hook会被调用，可在其中 `Flash()` 能力。

---

## 7. 持续时间与递减模式

能力有多种方式实现"持续N回合后消失"的效果：

### 模式A：`PowerCmd.Decrement(this)` — 无条件减少1层

回合结束时减少1层，层数到0后自动移除。**不跳过首次施加的回合**。

```csharp
public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
{
    if (participants.Contains(base.Owner))
    {
        Flash();
        await PowerCmd.Decrement(this);
    }
}
```

适用场景：Blur（模糊）、自定义倒计时能力等

### 模式B：`PowerCmd.TickDownDuration(this)` — 跳过首回合的递减

与Decrement类似，但尊重 `SkipNextDurationTick` 标志。**首次施加当回合不会递减**（游戏会自动为玩家侧生物设置此标志）。

```csharp
public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
{
    if (side == CombatSide.Enemy)
    {
        await PowerCmd.TickDownDuration(this);
    }
}
```

适用场景：脆弱、虚弱、易伤等Debuff（在敌方回合结束时递减）

### 模式C：`PowerCmd.Remove(this)` — 整体移除

回合结束时直接移除整个能力，不管有多少层。

```csharp
public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
{
    if (participants.Contains(base.Owner))
    {
        await PowerCmd.Remove(this);
    }
}
```

适用场景：Rage（愤怒）等持续到回合末的能力

### 模式D：回合开始时递减

```csharp
public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
{
    if (participants.Contains(base.Owner))
    {
        await PowerCmd.Decrement(this);
    }
}
```

适用场景：需要在回合开始而非结束时递减的能力

### 递减时机选择指南

| 时机 | Hook | 适用场景 |
|------|------|----------|
| 玩家回合结束 | `AfterSideTurnEnd` + `participants.Contains(Owner)` | 效果在玩家回合内生效，回合结束时倒计时 |
| 敌方回合结束 | `AfterSideTurnEnd` + `side == CombatSide.Enemy` | Debuff的标准递减时机 |
| 玩家回合开始 | `AfterSideTurnStart` + `participants.Contains(Owner)` | 效果在回合开始时倒计时 |

---

## 8. 在卡牌/遗物中施加能力

### 从卡牌施加能力

```csharp
await CommonActions.Apply<SomePower>(Owner.Creature, this, DynamicVars["VarName"].BaseValue);
// 或对目标施加
await CommonActions.Apply<SomePower>(cardPlay.Target, this, DynamicVars["VarName"].BaseValue);
```

### 从遗物施加能力

```csharp
await PowerCmd.Apply<SomePower>(
    new ThrowingPlayerChoiceContext(),
    base.Owner.Creature,
    base.DynamicVars["Amount"].IntValue,
    base.Owner.Creature, null);
```

### 为卡牌/遗物添加能力悬停提示

```csharp
protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [HoverTipFactory.FromPower<SomePower>()];
```

---

## 9. 其它内容

### DynamicVar类型

能力使用 `CanonicalVars` 声明动态变量：

| 类型 | 用法 | 示例 |
|------|------|------|
| `DynamicVar` | 通用变量 | `new DynamicVar("VarName", 7m)` |
| `PowerVar<T>` | 能力变量 | `new PowerVar<SomePower>(1)` |

### ExtraHoverTips

为能力添加鼠标悬停提示：

```csharp
protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [HoverTipFactory.FromPower<SomePower>()];
// 或使用静态提示
protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [HoverTipFactory.Static(StaticHoverTip.Block)];
```

### 常用命名空间

```csharp
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
```

### ValueProp 常用判断方法

在 `ModifyBlockAdditive` 和 `ModifyDamageAdditive` 等方法中，`ValueProp props` 参数提供了以下判断方法：

| 方法 | 说明 |
|------|------|
| `props.IsPoweredAttack()` | 是否为有来源的攻击（卡牌攻击、怪物攻击） |
| `props.IsPoweredCardOrMonsterMoveBlock()` | 是否为有来源的格挡（卡牌格挡、怪物行动格挡） |

### 常用引用

- Baselib库，该库封装了许多杀戮尖塔2的mod api，源码可查看`%USERPROFILE%\Documents\sts_mods\sts2_lib\BaseLib-StS2`目录
  - 子目录`Abstracts`下可找到能力基类`CustomPowerModel`的实现，其派生自原版代码的`PowerModel`类
- [Baselib官方文档](https://alchyr.github.io/BaseLib-Wiki/docs/Features.html)
- SeleePower基类源码：`SeleeCode/Powers/SeleePower.cs`
- 原版能力代码目录：`%USERPROFILE%\Documents\sts_mods\sts2_src\src\Core\Models\Powers`
- 原版PowerModel基类源码：`%USERPROFILE%\Documents\sts_mods\sts2_src\src\Core\Models\PowerModel.cs`
