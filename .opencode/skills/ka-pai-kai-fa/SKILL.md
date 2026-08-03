---
name: ka-pai-kai-fa
description: 创建、修改Selee角色mod专属卡牌，添加卡牌到对应卡池。Use ONLY when the user asks to create or modify a card for the Selee mod. Triggers on keywords: 卡牌, card, 新卡, 添加卡牌, 卡池.
---

# 卡牌开发

本skill指导如何为Selee角色模组创建和修改卡牌。

## 目录

1. [创建卡牌代码文件](#1-创建卡牌代码文件)
2. [实现卡牌类](#2-实现卡牌类)
3. [添加本地化文本](#3-添加本地化文本)
4. [实现卡牌效果](#4-实现卡牌效果)
5. [其它内容](#5-其它内容)
6. [创建能力](#6-新增能力power状态效果)
7. [修改卡牌注意事项](#7-修改卡牌注意事项)
8. [其它引用](#8-其它引用)

所有代码操作默认在 `SeleeCode/` 目录下进行。

---

## 1. 创建卡牌代码文件

在 `SeleeCode/Cards/` 目录下创建卡牌的 `.cs` 文件。

**命名规则：**
- 中文卡牌名 → 拼音命名，如"打击"→ `DaJi.cs`
- 覆写原版卡牌 → `{原始名称}Patched`，如覆写飞跃(Leap)→ `LeapPatched.cs`

---

## 2. 实现卡牌类

参考模板文件 `SeleeCode/Cards/SeleeCard.cs` 创建。Selee角色mod的卡牌基类为 `SeleeCard`，其继承自 `CustomCardModel` 并已处理了卡牌图像路径和卡池注解。

### 2.1 构造函数

卡牌需继承 `SeleeCard` 类，调用其构造函数初始化：

```csharp
SeleeCard(int cost, CardType type, CardRarity rarity, TargetType target)
```

**参数说明：**

| 参数 | 说明 |
|------|------|
| cost | 卡牌耗能 |
| type | 卡牌类型（见下枚举） |
| rarity | 卡牌稀有度（见下枚举） |
| target | 目标类型（见下枚举） |

**CardType 枚举：**
```
None, Attack, Skill, Power, Status, Curse, Quest
```

**CardRarity 枚举：**
```
None, 
Basic, //基础卡
Common, //普通，白卡
Uncommon, //罕见，蓝卡
Rare, //稀有，金卡
Ancient, //先古卡
Event, //事件卡
Token, //临时生成卡、选项卡
Status, //状态卡
Curse, //诅咒卡
Quest //宝物卡
```

**TargetType 枚举：**
```
None, Self, AnyEnemy, AllEnemies, RandomEnemy, AnyPlayer, AnyAlly, AllAllies, TargetedNoCreature, Osty
```

**示例：**
```csharp
// 1耗能攻击牌，罕见，目标所有敌人
public class MyCard(): SeleeCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
```

注意：`SeleeCard` 基类已通过 `[Pool(typeof(SeleeCardPool))]` 注解将所有派生卡牌自动添加到Selee卡池，无需手动设置卡池。

### 2.2 必须重载的项

每个卡牌类**必须**重载以下方法：

| 项目 | 签名 | 说明 |
|------|------|------|
| 卡牌打出方法 | `override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)` | 卡牌被打出时执行的效果 |
| 卡牌升级方法 | `override void OnUpgrade()` | 卡牌升级时的效果 |

**卡牌图像路径：** `SeleeCard` 基类已处理 `CustomPortraitPath`，无需重载。若用户指定自定义图像名称，则重载：
```csharp
public override string CustomPortraitPath => $"{user_image_name}.png".BigCardImagePath();
```

### 2.3 模板代码

```csharp
using BaseLib.Extensions;
using Selee.SeleeCode.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Selee.SeleeCode.Cards;

public class _TemplateCard(): SeleeCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DynamicVar("var", 1m),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {

    }

    protected override void OnUpgrade()
    {
        DynamicVars["var"].UpgradeValueBy(1);
    }
}
```

---

## 3. 添加本地化文本

打开 `Selee/localization/zhs/cards.json`，在JSON对象中添加两项：

```json
"SELEE-{CARD_NAME}.description": "卡牌描述文本",
"SELEE-{CARD_NAME}.title": "卡牌中文名称"
```

**CARD_NAME 命名规则：** 类名转为大写+下划线格式。
- 类名 `DaJi` → `DA_JI`
- 类名 `CuiLianDuSu` → `CUI_LIAN_DU_SU`

**描述文本中的动态变量格式：**
- `{VarName:diff()}` — 显示数值，升级时显示差值
- `{VarName:energyIcons()}` — 基于变量值动态显示对应数量的能量图标，VarName可为任意变量名（能力层数可用 `Amount`），`energyIcons()` 不能包含参数。代码中需使用对应类型的变量（如 `EnergyVar`）
- `{energyPrefix:energyIcons(N)}` — 显示固定N个能量图标，前缀必须为 `energyPrefix`
- `[gold]文本[/gold]` — 金色高亮文本

**变量名必须与代码一致：** 描述中的变量名必须与 `CanonicalVars` 中声明的键名完全匹配。特别注意 `PowerVar<T>` 的键名是能力类名 `T`，而非自定义名称：
```csharp
// 代码：new PowerVar<XingHuanYingYuPower>(20)  → 键名 "XingHuanYingYuPower"
// 描述：{XingHuanYingYuPower:diff()}            → ✅ 正确
// 描述：{XingHuanNengJi:diff()}                 → ❌ 不匹配，无法显示
```

各类型变量的键名：
| 类型 | 键名 |
|------|------|
| `PowerVar<T>` | `T`（能力类名） |
| `DamageVar` | `"Damage"` |
| `BlockVar` | `"Block"` |
| `EnergyVar` | `"Energy"` |
| `CardsVar` | `"Cards"` |
| `DynamicVar("Name", ...)` | `"Name"` |

**示例：**
```json
"SELEE-CUI_LIAN_DU_SU.description": "回合结束时，若敌人拥有至少{PoisonCount:diff()}层[gold]中毒[/gold]，下回合获得{Energy:energyIcons()}。",
"SELEE-CUI_LIAN_DU_SU.title": "淬炼毒素"
```

---

## 4. 实现卡牌效果

实现卡牌效果时，若遇到实现方法未知的需求，应先查阅 `docs/开发经验.md` 寻找是否有相关经验记录，没有相关经验再去查阅游戏本体代码以找到实现方法。


### 步骤

1. **查阅开发经验**：打开 `docs/开发经验.md`，搜索是否有与目标效果相关的经验记录

2. **查找原版卡牌描述**：打开 `C:\Users\joyce\Documents\sts_mods\sts2_src\localization\zhs\cards.json`，搜索与目标效果相关的描述，并找到对应的原版卡牌名称

3. **找到对应代码文件**：根据卡牌名称转为驼峰命名，在 `C:\Users\joyce\Documents\sts_mods\sts2_src\src\Core\Models\Cards` 目录下查找对应的 `.cs` 文件
   - 例如：`STRIKE_DEFECT` → `StrikeDefect.cs`

4. **参考实现**：阅读原版卡牌的代码 ，参考以下内容：
   - `DynamicVars` 的初始化（`CanonicalVars`）和应用
   - 卡牌打出效果、升级效果实现（`OnPlay`,`OnUpgrade`）
   - 条件打出、无法打出效果实现
   - 其它卡牌机制

5. **也可参考本mod内其它卡牌**：`SeleeCode/Cards/` 目录下的现有卡牌
   - 修改卡牌时应当优先读取mod内卡牌的本地化描述，找到卡牌类名并读取被修改的卡牌的代码

### 常用效果参考

**造成伤害：**
```csharp
await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
    .FromCard(this).Targeting(cardPlay.Target)
    .Execute(choiceContext);
```

**对全体敌人造成2次伤害：**
```csharp
await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
    .WithHitCount(2).FromCard(this).TargetingAllOpponents(base.CombatState)
    .WithHitFx("vfx/vfx_attack_slash")
    .Execute(choiceContext);
```
- TargetingAllOpponents(base.CombatState)表示攻击敌方全体
- WithHitCount(2)表示造成2次伤害
- WithHitFx("vfx/vfx_attack_slash")表示播放平滑攻击声音

**获得格挡：**
```csharp
await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);
```

**施加能力(Power)：**
```csharp
await CommonActions.Apply<SomePower>(Owner.Creature, this, DynamicVars["VarName"].BaseValue);
// 或对目标施加
await CommonActions.Apply<SomePower>(cardPlay.Target, this, DynamicVars["VarName"].BaseValue);
```

**抽牌：**
```csharp
await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner);
```

**播放动画：**
```csharp
await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
```

### 常用DynamicVar类型

| 类型 | 用法 | 示例 |
|------|------|------|
| `DynamicVar` | 通用变量 | `new DynamicVar("VarName", 7m)` |
| `DamageVar` | 伤害变量 | `new DamageVar(6m, ValueProp.Move)` |
| `BlockVar` | 格挡变量 | `new BlockVar(4m, ValueProp.Move)` |
| `EnergyVar` | 能量变量 | `new EnergyVar(3)` |
| `CardsVar` | 抽牌变量 | `new CardsVar(3)` |
| `PowerVar<T>` | 能力变量 | `new PowerVar<SomePower>(1)` |

### 升级效果常用操作

```csharp
DynamicVars["VarName"].UpgradeValueBy(1m);     // 变量增加
DynamicVars.Damage.UpgradeValueBy(2);           // 伤害增加
DynamicVars.Block.UpgradeValueBy(3);            // 格挡增加
EnergyCost.UpgradeBy(-1);                       // 耗能减少
```

---

## 5. 其它内容

### 叠加（DieJia）效果卡牌

具有"叠加"效果的卡牌（拥有量子叠加时触发额外效果），必须遵循以下规范：

1. **添加叠加关键字**：`public override IEnumerable<CardKeyword> CanonicalKeywords => [SeleeCardKeyword.DieJia];`

2. **叠加效果触发后必须使用 `SeleeHook.AfterDieJiaTrigger`**：不得直接调用 `PowerCmd.Decrement` 减少量子叠加层数。`SeleeHook.AfterDieJiaTrigger` 会先通知所有 `ISeleeHook` 监听者（如"蝶舞"卡牌），然后自动减少一层量子叠加。

```csharp
var dieJiaPower = base.Owner.Creature.GetPower<LiangZiDieJiaPower>();
if (dieJiaPower != null)
{
    // 执行叠加额外效果...
    await SeleeHook.AfterDieJiaTrigger(Owner, this);
}
```

3. **条件发光**：叠加卡牌应重载 `ShouldGlowGoldInternal`，在玩家拥有量子叠加时发出金色光芒：
```csharp
protected override bool ShouldGlowGoldInternal =>
    base.Owner != null && base.Owner.Creature.HasPower<LiangZiDieJiaPower>();
```

### 监听叠加事件的卡牌（ISeleeHook）

某些卡牌的效果是"当叠加效果触发时"被动响应（如"蝶舞"——触发叠加效果时将这张牌放入手牌）。这类卡牌应实现 `ISeleeHook` 接口：

```csharp
public class DieWu() : SeleeCard(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy), ISeleeHook
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [SeleeCardKeyword.DieJia];

    public async Task AfterDieJiaTrigger(Player owner, CardModel? triggerCard)
    {
        if (Pile?.Type != PileType.Hand)
        {
            await CardPileCmd.Add(this, PileType.Hand);
        }
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 正常打出效果，不含叠加逻辑
    }
}
```

关键点：
- `ISeleeHook.AfterDieJiaTrigger` 返回 `Task`，支持异步操作
- "将这张牌放入手牌"指将卡牌自身移入手牌，使用 `CardPileCmd.Add(this, PileType.Hand)`，而非生成复制
- 需判断卡牌当前不在手牌中才移入，使用 `Pile?.Type != PileType.Hand` 检查
- 只要卡牌描述中提到叠加效果，就应当添加 `DieJia` 关键字，以便玩家在游玩时看到对应的说明文本提示
- 需引入命名空间 `MegaCrit.Sts2.Core.Models` 和 `MegaCrit.Sts2.Core.Entities.Players`

### X耗能卡牌

若卡牌耗能为X（消耗所有能量，基于消耗能量数量产生效果）：
- 设置卡牌耗能为 `0`
- 重载属性 `override bool HasEnergyCostX => true;`

### 无法打出的卡牌

若卡牌默认无法被打出（如诅咒、状态牌）：
- 设置卡牌耗能为 `-1`
- 添加卡牌关键字"无法被打出"(`Unplayable`)

### 卡牌关键字
- 可以为卡牌添加"消耗"，"保留"，"虚无"等关键字
- 重载属性`override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain]`
- 关键字枚举：
```csharp
public enum CardKeyword
{
	None,
	Exhaust, //消耗
	Ethereal, //虚无
	Innate, //固有
	Unplayable, //无法被打出
	Retain, //保留
	Sly, //奇巧
	Eternal  //永恒 
}
```
- 添加关键字后会自动增加对应描述，无需在卡牌描述中说明。

### 多人游戏限定卡牌

若卡牌仅在多人游戏模式下可用：
```csharp
public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;
```
需要引入命名空间 `MegaCrit.Sts2.Core.GameActions.Multiplayer`。


## 6. 新增能力（Power,状态效果）

有时候卡牌需要新增能力来实现效果，请参考 **能力开发skill** (`neng-li-kai-fa`) 获取完整的能力开发指南。

### 快速参考

- 能力类命名规则：中文能力名 → 拼音 + `Power` 后缀，如"量子叠加"→ `LiangZiDieJiaPower`
- 能力类放在 `SeleeCode/Powers/` 目录下，继承 `SeleePower`
- 能力本地化填写在 `Selee/localization/zhs/powers.json` 中，**必须包含 `title`、`description`、`smartDescription` 三项**
- 本地化键名规则：类名转大写+下划线，如 `LiangZiDieJiaPower` → `SELEE-LIANG_ZI_DIE_JIA_POWER`
- 描述中代表能力层数的变量使用 `{Amount}`（无需 `:diff()`）

### 从卡牌施加能力

```csharp
await CommonActions.Apply<SomePower>(Owner.Creature, this, DynamicVars["VarName"].BaseValue);
// 或对目标施加
await CommonActions.Apply<SomePower>(cardPlay.Target, this, DynamicVars["VarName"].BaseValue);
```

## 7. 修改卡牌注意事项
- 修改卡牌时，注意修改卡牌的本地化描述。
- 修改卡牌时，请先找到被修改的卡牌的代码，再根据修改需求去原版目录中查找需要参考的代码。
- 修改含有叠加效果的卡牌时，叠加效果触发后必须使用 `SeleeHook.AfterDieJiaTrigger(Owner, this)` 而非 `PowerCmd.Decrement`，以确保监听者（如蝶舞）能正确响应叠加事件。

## 8. 根据设计文档开发

当用户要求"根据设计文档开发"卡牌时，使用设计文档读取脚本获取卡牌设计数据：

1. **列出所有sheet页**：`python .opencode/scripts/read_design_doc.py`
2. **读取卡牌sheet全部内容**：`python .opencode/scripts/read_design_doc.py 卡牌`
3. **搜索特定卡牌**：`python .opencode/scripts/read_design_doc.py 卡牌 {卡牌名称}`

搜索结果为JSON数组，每个元素是一个字典，key为表头（名称、费用、稀有度、类型、效果描述、升级效果、目标类型、关键字、Tags、能力提示、备注）。

**"能力提示"列的使用：** 该列指定卡牌需要在 `ExtraHoverTips` 中添加哪些能力的悬浮提示。基于设计文档开发时，HoverTips应根据此列添加，而非为所有施加的能力都添加提示。例如"坍缩攻击"涉及量子叠加能力，但"能力提示"列为空，因此无需添加量子叠加的HoverTip。

**注意：** 脚本已内置 `sys.stdout.reconfigure(encoding='utf-8')` 确保中文输出正确。若在其他Python脚本中输出中文到Bash工具时遇到乱码，也需在脚本开头添加此行。

读取设计数据后，按照上述步骤1-6实现卡牌。

## 9. 其它引用
- Baselib库，该库封装了许多杀戮尖塔2的mod api，源码可查看`C:\Users\joyce\Documents\sts_mods\sts2_lib\BaseLib-StS2-master`目录。   
  子目录`Abstracts`下可找到卡牌基类`CustomCardModel`的实现，其派生自原版代码的`CardModel`类
- [Baselib官方文档](https://alchyr.github.io/BaseLib-Wiki/docs/Features.html)
- SeleeCard基类源码：`SeleeCode/Cards/SeleeCard.cs`
