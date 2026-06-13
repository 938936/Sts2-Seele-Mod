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

public class LianLiXing() : SeleeCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [SeleeCardKeyword.XingHuanBaoFa];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromKeyword(SeleeCardKeyword.GongMing),
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<LianLiXingPower>(1),
        new DynamicVar("BaoFaBlock", 2m),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var power=await PowerCmd.Apply<LianLiXingPower>(choiceContext, base.Owner.Creature, DynamicVars["LianLiXingPower"].BaseValue, base.Owner.Creature, this);
        if (power != null)
        {
            power.DynamicVars["BaoFaBlock"].BaseValue += DynamicVars["BaoFaBlock"].BaseValue;
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["LianLiXingPower"].UpgradeValueBy(1m);
        DynamicVars["BaoFaBlock"].UpgradeValueBy(1m);
    }
}
