using BaseLib.Extensions;
using Selee.SeleeCode.Extensions;
using Selee.SeleeCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Selee.SeleeCode.Cards;

public class ShenFenHuHuan() : SeleeCard(1, CardType.Skill, CardRarity.Rare, TargetType.AnyAlly)
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [base.EnergyHoverTip];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new EnergyVar(2),
        new CardsVar(2),
        new DynamicVar("BlockPercent",50m)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        Player? target = cardPlay.Target.Player;
        if (target is { Creature.IsAlive: true })
        {
            await PlayerCmd.GainEnergy(base.DynamicVars.Energy.IntValue, target);        
            await CardPileCmd.Draw(choiceContext, base.DynamicVars.Cards.BaseValue, target);
            var power = await PowerCmd.Apply<ShenFenHuHuanPower>(choiceContext, cardPlay.Target, 1, base.Owner.Creature, this);
            if (power != null)
            {
                power.DynamicVars["BlockPercent"].BaseValue=DynamicVars["BlockPercent"].BaseValue;
            }
        }
        PlayerCmd.EndTurn(base.Owner, canBackOut: false);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Energy.UpgradeValueBy(1m);
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}
