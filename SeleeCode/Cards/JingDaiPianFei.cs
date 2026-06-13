using BaseLib.Extensions;
using Selee.SeleeCode.Extensions;
using Selee.SeleeCode.Powers;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Selee.SeleeCode.Cards;

public class JingDaiPianFei() : SeleeCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromKeyword(CardKeyword.Retain),
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<XingHuanNengJiNextTurnPower>(30),
        new EnergyVar(2),
        new DynamicVar("RetainCount", 1m),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<XingHuanNengJiNextTurnPower>(choiceContext, base.Owner.Creature, DynamicVars["XingHuanNengJiNextTurnPower"].BaseValue, base.Owner.Creature, this);
        await PowerCmd.Apply<EnergyNextTurnPower>(choiceContext, base.Owner.Creature, DynamicVars.Energy.BaseValue, base.Owner.Creature, this);

        var selection = await CardSelectCmd.FromHand(
            prefs: new CardSelectorPrefs(base.SelectionScreenPrompt, 0, DynamicVars["RetainCount"].IntValue),
            context: choiceContext,
            player: base.Owner,
            filter: RetainFilter,
            source: this);

        foreach (var card in selection)
        {
            card.GiveSingleTurnRetain();
        }
    }

    private bool RetainFilter(CardModel card)
    {
        return !card.ShouldRetainThisTurn;
    }

    protected override void OnUpgrade()
    {
        DynamicVars["XingHuanNengJiNextTurnPower"].UpgradeValueBy(5m);
        DynamicVars["RetainCount"].UpgradeValueBy(1m);
    }
}
