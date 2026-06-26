using BaseLib.Extensions;
using Selee.SeleeCode.Extensions;
using Selee.SeleeCode.Patch;
using Selee.SeleeCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Selee.SeleeCode.Cards;

public class KuaGuoHeiYeDeChenXi() : SeleeCard(3, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [SeleeCardKeyword.DieJiaX];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(30m, ValueProp.Move),
        new DynamicVar("DieJiaCount", 3m),
        new DynamicVar("EnergyReduction", 2m),
    ];

    protected override bool ShouldGlowGoldInternal =>
        (base.Owner.Creature.GetPower<LiangZiDieJiaPower>()?.Amount ?? 0) >= DynamicVars["DieJiaCount"].IntValue;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        
        var dieJiaPower = base.Owner.Creature.GetPower<LiangZiDieJiaPower>();
        if (dieJiaPower != null && dieJiaPower.Amount >= DynamicVars["DieJiaCount"].IntValue)
        {
            await PowerCmd.ModifyAmount(choiceContext, dieJiaPower, 1 - DynamicVars["DieJiaCount"].IntValue, Owner.Creature, this);
            await SeleeHook.AfterDieJiaTrigger(Owner, this, choiceContext);
        }


    }

    public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        modifiedCost = originalCost;
        if (card != this) return false;
        var dieJiaPower = base.Owner.Creature.GetPower<LiangZiDieJiaPower>();
        if (dieJiaPower == null || dieJiaPower.Amount < DynamicVars["DieJiaCount"].IntValue) return false;
        if (card.Pile?.Type != PileType.Hand && card.Pile?.Type != PileType.Play) return false;
        modifiedCost = originalCost - DynamicVars["EnergyReduction"].BaseValue;
        if (modifiedCost < 0m) modifiedCost = 0m;
        return true;
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(10m);
    }
}
