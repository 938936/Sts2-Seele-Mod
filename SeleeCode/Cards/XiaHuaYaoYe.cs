using BaseLib.Extensions;
using Selee.SeleeCode.Extensions;
using Selee.SeleeCode.Patch;
using Selee.SeleeCode.Powers;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Selee.SeleeCode.Cards;

public class XiaHuaYaoYe() : SeleeCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [SeleeCardKeyword.DieJia];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust),
        HoverTipFactory.FromCard<LuoYing>(),
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CardsVar(1),
        new DynamicVar("DieJiaTransformCount", 3m),
    ];

    protected override bool ShouldGlowGoldInternal =>
        base.Owner != null && base.Owner.Creature.HasPower<LiangZiDieJiaPower>();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, base.Owner);

        var dieJiaPower = base.Owner.Creature.GetPower<LiangZiDieJiaPower>();
        int transformCount = dieJiaPower != null ? DynamicVars["DieJiaTransformCount"].IntValue : 1;

        var selection = await CardSelectCmd.FromHand(
            prefs: new CardSelectorPrefs(CardSelectorPrefs.TransformSelectionPrompt, 1, transformCount),
            context: choiceContext,
            player: base.Owner,
            filter: null,
            source: this);

        foreach (var selectedCard in selection)
        {
            var luoYing = base.CombatState!.CreateCard<LuoYing>(base.Owner);
            await CardCmd.Transform(selectedCard, luoYing);
        }

        if (dieJiaPower != null)
        {
            await SeleeHook.AfterDieJiaTrigger(Owner, this, choiceContext);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}
