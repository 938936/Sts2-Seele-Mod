using BaseLib.Extensions;
using Godot;
using Selee.SeleeCode.Extensions;
using Selee.SeleeCode.Patch;
using Selee.SeleeCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Selee.SeleeCode.Cards;

public class FenYangFengLingDeYaoYue() : SeleeCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [SeleeCardKeyword.XingHuanBaoFa];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<XingHuanBaoFaPower>(),
    ];
    

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<FenYangFengLingDeYaoYuePower>(1),
        new EnergyVar(2),
        new CardsVar(2),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var power = await PowerCmd.Apply<FenYangFengLingDeYaoYuePower>(choiceContext, base.Owner.Creature, DynamicVars["FenYangFengLingDeYaoYuePower"].BaseValue, base.Owner.Creature, this);
        if (power != null)
        {
            power.DynamicVars.Energy.BaseValue = DynamicVars["Energy"].BaseValue;
            power.DynamicVars.Cards.BaseValue = DynamicVars.Cards.BaseValue;
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Energy"].UpgradeValueBy(1m);
    }
}
