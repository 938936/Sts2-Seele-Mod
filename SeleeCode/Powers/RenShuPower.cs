using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Selee.SeleeCode.Powers;

public class RenShuPower() : SeleePower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override PowerInstanceType InstanceType { get; } = PowerInstanceType.Instanced;

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
    {
        if (player!=Owner.Player) return;

        var drawPile = PileType.Draw.GetPile(base.Owner.Player!);
        if (drawPile.Cards.Count == 0) return;

        Flash();

        CardModel? selectedCard = (await CardSelectCmd.FromCombatPile(
            choiceContext, PileType.Draw.GetPile(base.Owner.Player), base.Owner.Player!,
            new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 1)
        )).FirstOrDefault();
        decimal yuHeAmount = base.Amount;
        if (selectedCard != null)
        {
            if (selectedCard.Type == CardType.Status || selectedCard.Type == CardType.Curse)
            {
                yuHeAmount *= 2;
            }
            await CardCmd.Exhaust(choiceContext, selectedCard);
        }
        await PowerCmd.Apply<YuHePower>(choiceContext, base.Owner, yuHeAmount, base.Owner, null);
    }
}
