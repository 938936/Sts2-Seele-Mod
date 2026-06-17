using Selee.SeleeCode.Patch;
using Selee.SeleeCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Selee.SeleeCode.Cards;

public class CunZaiPaoYing() : SeleeCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [SeleeCardKeyword.DieJia];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromKeyword(SeleeCardKeyword.DieJia),
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<CunZaiPaoYingPower>(5),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<CunZaiPaoYingPower>(choiceContext, base.Owner.Creature, DynamicVars["CunZaiPaoYingPower"].BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["CunZaiPaoYingPower"].UpgradeValueBy(2m);
    }
}
