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

public class SuHuiHaiYuDeYaoLan() : SeleeCard(0, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [SeleeCardKeyword.XingHuanBaoFa];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<JianBingDongNengPower>(),
        HoverTipFactory.FromPower<XingHuanNengJiPower>(),
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(20m, ValueProp.Move),
        new DynamicVar("NengJiPerDongNeng", 10m),
        new CardsVar(1)
    ];

    protected override bool IsPlayable =>
        base.Owner != null && base.Owner.Creature.HasPower<XingHuanBaoFaPower>();

    protected override bool ShouldGlowGoldInternal =>
        base.Owner != null && base.Owner.Creature.HasPower<XingHuanBaoFaPower>();

    public int dongNengAmount = 0;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .FromCard(this).TargetingAllOpponents(base.CombatState!)
            .WithHitVfxNode(NGrandFinaleImpactVfx.Create)
            .WithHitFx(null, null, "blunt_attack.mp3")
            .Execute(choiceContext);

        var dongNengPower = base.Owner.Creature.GetPower<JianBingDongNengPower>();
        dongNengAmount = dongNengPower?.Amount ?? 0;

        await PowerCmd.Remove<XingHuanBaoFaPower>(base.Owner.Creature);
        if (dongNengAmount > 0)
        {
            decimal nengJiGain = dongNengAmount * DynamicVars["NengJiPerDongNeng"].BaseValue;
            dongNengAmount = 0;
            await SeleeHook.AfterGongMingTrigger(base.Owner, this, choiceContext);
            await PowerCmd.Apply<XingHuanNengJiPower>(choiceContext, base.Owner.Creature, nengJiGain, base.Owner.Creature, this);
            await CardPileCmd.Draw(choiceContext, dongNengAmount * DynamicVars.Cards.IntValue, Owner);
        }
    }
    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(8m);
    }
}
