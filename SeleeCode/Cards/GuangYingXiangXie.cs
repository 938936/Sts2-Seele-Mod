using BaseLib.Extensions;
using Selee.SeleeCode.Extensions;
using Selee.SeleeCode.Patch;
using Selee.SeleeCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Selee.SeleeCode.Cards;

public class GuangYingXiangXie() : SeleeCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [SeleeCardKeyword.DieJiaX];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        base.EnergyHoverTip,
        HoverTipFactory.FromCard<LuoYing>(),
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new EnergyVar(2),
        new DynamicVar("DieJiaCount",2m)
    ];

    protected override bool ShouldGlowGoldInternal =>
        base.Owner != null && (base.Owner.Creature.GetPower<LiangZiDieJiaPower>()?.Amount ?? 0) >=2;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var dieJiaPower = base.Owner.Creature.GetPower<LiangZiDieJiaPower>();

        if (dieJiaPower != null && dieJiaPower.Amount>=2)
        {
            await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, base.Owner);
            var luoYing = base.CombatState!.CreateCard<LuoYing>(base.Owner);
            await CardPileCmd.AddGeneratedCardToCombat(luoYing, PileType.Draw, base.Owner, CardPilePosition.Random);
            //先减少X-1层，然后触发Hook再减少一层
            await PowerCmd.ModifyAmount(choiceContext,dieJiaPower , 1-DynamicVars["DieJiaCount"].BaseValue, Owner.Creature, this);
            await SeleeHook.AfterDieJiaTrigger(Owner, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Energy.UpgradeValueBy(1m);
    }
}
