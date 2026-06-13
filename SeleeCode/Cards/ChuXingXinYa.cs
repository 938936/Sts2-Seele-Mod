using BaseLib.Abstracts;
using BaseLib.Extensions;
using Selee.SeleeCode.Extensions;
using Selee.SeleeCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Selee.SeleeCode.Cards;

public class ChuXingXinYa() : SeleeCard(0, CardType.Power, CardRarity.Ancient, TargetType.Self)
{

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<LiangZiDieJiaPower>(),
        base.EnergyHoverTip,
        HoverTipFactory.FromPower<XingHuanNengJiPower>(),
        HoverTipFactory.FromPower<YuHePower>(),
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<LiangZiDieJiaPower>(3),
        new EnergyVar(2),
        new PowerVar<YuHePower>(10),
        new PowerVar<XingHuanNengJiPower>(30),
        new PowerVar<ChuXingXinYaPower>(2),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<LiangZiDieJiaPower>(choiceContext, base.Owner.Creature, DynamicVars["LiangZiDieJiaPower"].BaseValue, base.Owner.Creature, this);
        await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, base.Owner);
        await PowerCmd.Apply<YuHePower>(choiceContext, base.Owner.Creature, DynamicVars["YuHePower"].BaseValue, base.Owner.Creature, this);
        await PowerCmd.Apply<XingHuanNengJiPower>(choiceContext, base.Owner.Creature, DynamicVars["XingHuanNengJiPower"].BaseValue, base.Owner.Creature, this);
        await PowerCmd.Apply<ChuXingXinYaPower>(choiceContext, base.Owner.Creature, DynamicVars["ChuXingXinYaPower"].BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Innate);
    }
    
}
