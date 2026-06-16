using BaseLib.Extensions;
using Selee.SeleeCode.Extensions;
using Selee.SeleeCode.Powers;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Selee.SeleeCode.Cards;

public class X10ShiYan() : SeleeCard(0, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Innate];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<LiangZiDieJiaPower>(),
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<LiangZiDieJiaPower>(2),
        new CardsVar(2),
        new PowerVar<X10ShiYanPower>(1),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<LiangZiDieJiaPower>(choiceContext, base.Owner.Creature, DynamicVars["LiangZiDieJiaPower"].BaseValue, base.Owner.Creature, this);

        var selection = await CardSelectCmd.FromCombatPile(
            prefs: new CardSelectorPrefs(base.SelectionScreenPrompt, 0, DynamicVars.Cards.IntValue),
            context: choiceContext,
            pile: PileType.Draw.GetPile(base.Owner),
            player: base.Owner);

        foreach (var card in selection)
        {
            await CardCmd.AutoPlay(choiceContext, card, null);
        }

        await PowerCmd.Apply<X10ShiYanPower>(choiceContext, base.Owner.Creature, DynamicVars["X10ShiYanPower"].BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Cards"].UpgradeValueBy(1m);
    }
}
