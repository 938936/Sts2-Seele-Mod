using BaseLib.Extensions;
using Selee.SeleeCode.Extensions;
using Selee.SeleeCode.Patch;
using Selee.SeleeCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;

namespace Selee.SeleeCode.Cards;

public class JingMiLiRen() : SeleeCard(2, CardType.Attack, CardRarity.Rare, TargetType.RandomEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [SeleeCardKeyword.DieJia];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<LiangZiDieJiaPower>()];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(4m, ValueProp.Move),
        new DamageVar("DieJiaDamage", 8m,ValueProp.Move),
    ];

    protected override bool ShouldGlowGoldInternal =>
        base.Owner != null && base.Owner.Creature.HasPower<LiangZiDieJiaPower>();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .WithHitCount(3)
            .FromCard(this)
#pragma warning disable CS8604 // 引用类型参数可能为 null。
            .TargetingRandomOpponents(base.CombatState)
#pragma warning restore CS8604 // 引用类型参数可能为 null。
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        var dieJiaPower = base.Owner.Creature.GetPower<LiangZiDieJiaPower>();
        if (dieJiaPower != null)
        {
            int stackCount = dieJiaPower.Amount;
            await DamageCmd.Attack(DynamicVars["DieJiaDamage"].BaseValue)
                .WithHitCount(stackCount)
                .FromCard(this)
                .TargetingAllOpponents(base.CombatState)
                .WithHitVfxNode(NGrandFinaleImpactVfx.Create)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);

            await SeleeHook.AfterDieJiaTrigger(Owner, this);

            var remainingPower = base.Owner.Creature.GetPower<LiangZiDieJiaPower>();
            if (remainingPower != null)
            {
                await PowerCmd.Remove(remainingPower);
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["DieJiaDamage"].UpgradeValueBy(3m);
    }
}
