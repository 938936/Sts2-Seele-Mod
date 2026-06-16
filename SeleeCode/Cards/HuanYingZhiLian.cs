using BaseLib.Cards.Variables;
using BaseLib.Extensions;
using Selee.SeleeCode.Extensions;
using Selee.SeleeCode.Patch;
using Selee.SeleeCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Selee.SeleeCode.Cards;

public class HuanYingZhiLian() : SeleeCard(0, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    public override bool GainsBlock => true;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [SeleeCardKeyword.DieJia];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("DamageBase",3m),
        new DynamicVar("BlockBase",3m),
        new DynamicVar("DamageExtra",2m),
        new DynamicVar("BlockExtra",2m),
        new CustomCalculatedDamageVar("Damage", ValueProp.Move)
            .WithMultiplier((CardModel model, Creature? creature) => model.Owner.HasPower<LiangZiDieJiaPower>() ? 1 : 0),
        new CustomCalculatedBlockVar("Block", ValueProp.Move)
            .WithMultiplier((CardModel model, Creature? creature) => model.Owner.HasPower<LiangZiDieJiaPower>() ? 1 : 0)
    ];

    protected override bool ShouldGlowGoldInternal =>
        base.Owner != null && base.Owner.Creature.HasPower<LiangZiDieJiaPower>();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

        var dieJiaPower = base.Owner.Creature.GetPower<LiangZiDieJiaPower>();

        CustomCalculatedDamageVar damage = (DynamicVars["Damage"] as CustomCalculatedDamageVar)!;
        CustomCalculatedBlockVar block = (DynamicVars["Block"] as CustomCalculatedBlockVar)!;

        await DamageCmd.Attack(damage.CalculateCustom(cardPlay.Target))
            .FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        await CreatureCmd.GainBlock(base.Owner.Creature, block.CalculateCustom(cardPlay.Target), 
            ValueProp.Move, cardPlay);

        if (dieJiaPower != null)
        {
            await SeleeHook.AfterDieJiaTrigger(Owner, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["DamageBase"].UpgradeValueBy(1m);
        DynamicVars["BlockBase"].UpgradeValueBy(1m);
        DynamicVars["DamageExtra"].UpgradeValueBy(1m);
        DynamicVars["BlockExtra"].UpgradeValueBy(1m);
    }
}
