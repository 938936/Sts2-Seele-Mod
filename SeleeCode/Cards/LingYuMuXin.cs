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

public class LingYuMuXin() : SeleeCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [SeleeCardKeyword.XingHuanBaoFa];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<XingHuanNengJiPower>(),
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DynamicVar("XingHuanNengJi", 10m),
        new DynamicVar("TriggerCount", 4m),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var power = await PowerCmd.Apply<LingYuMuXinPower>(choiceContext, base.Owner.Creature, DynamicVars["TriggerCount"].BaseValue, base.Owner.Creature, this);
        if (power != null)
        {
            power.DynamicVars["XingHuanNengJi"].BaseValue = DynamicVars["XingHuanNengJi"].BaseValue;
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["XingHuanNengJi"].UpgradeValueBy(5m);
    }
}
