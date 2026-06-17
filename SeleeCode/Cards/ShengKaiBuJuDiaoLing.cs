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

public class ShengKaiBuJuDiaoLing() : SeleeCard(0, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override bool HasEnergyCostX => true;

    protected override bool ShouldGlowGoldInternal => (Owner.PlayerCombatState?.Energy ?? 0) >= 2 ;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<YuHePower>(),
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DynamicVar("DieJiaDraw", 1m),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int xValue = ResolveEnergyXValue();
        int drawCount = xValue >= 4 ? 2 : (xValue >= 2 ? 1 : 0);

        var power = await PowerCmd.Apply<ShengKaiBuJuDiaoLingPower>(choiceContext, base.Owner.Creature, xValue, base.Owner.Creature, this);
        
        if (power != null && drawCount > 0)
        {
            power.DynamicVars["DrawCount"].BaseValue += drawCount;
            power.InvokeDisplayAmountChanged();
        }
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}
