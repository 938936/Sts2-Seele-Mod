using BaseLib.Extensions;
using Selee.SeleeCode.Extensions;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Selee.SeleeCode.Cards;

public class FuYuGuiChao() : SeleeCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CardsVar(2),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var selection = await CardSelectCmd.FromCombatPile(
            prefs: new CardSelectorPrefs(base.SelectionScreenPrompt, 0, DynamicVars.Cards.IntValue),
            context: choiceContext,
            pile: PileType.Discard.GetPile(base.Owner),
            player: base.Owner);

        foreach (var card in selection)
        {
            card.EnergyCost.SetUntilPlayed(0);
            card.SetStarCostUntilPlayed(0);
            await CardPileCmd.Add(card, PileType.Draw, CardPilePosition.Random);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}
