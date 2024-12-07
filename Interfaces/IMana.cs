﻿using JRPG_Game.Characters;
using JRPG_Game.Characters.Skills;
using JRPG_Game.Enums;

namespace JRPG_Game.Interfaces;

/// <summary>
/// Interface representing entities that have mana and can perform mana-related actions such as regenerating or losing mana.
/// </summary>
public interface IMana
{
    int MaxMana { get; }
    int CurrentMana { get; set; }

    /// <summary>
    /// Creates a special ability to drink a potion and regenerate mana by 50%.
    /// </summary>
    /// <param name="character">The character performing the action of drinking the potion.</param>
    /// <returns>A special ability to drink a potion and regenerate mana.</returns>
    public SpecialAbility<ITarget> Drink(Character character) => new(
        name: "Boire une potion",
        description: () =>
            $"Régénère le mana de 50% ({CurrentMana} -> {Math.Min(MaxMana, CurrentMana + MaxMana / 2)}/{MaxMana})",
        owner: character,
        targetType: TargetType.Self,
        reloadTime: 1,
        manaCost: 0,
        effect: target =>
        {
            var output = "";
            var oldMana = CurrentMana;
            CurrentMana += Math.Min(MaxMana - CurrentMana, MaxMana / 2);
            output += oldMana != CurrentMana
                ? $"{target.Name} régénère son mana de {CurrentMana - oldMana} ({CurrentMana}/{MaxMana})"
                : $"{target.Name} a déja son mana au maximum : {CurrentMana}/{MaxMana}";

            return output;
        });

    /// <summary>
    /// Reduces the current mana by the specified amount, and returns the actual amount of mana lost.
    /// </summary>
    /// <param name="manaLost">The amount of mana to lose.</param>
    /// <returns>The amount of mana actually lost.</returns>
    public int LoseMana(int manaLost)
    {
        var manaUsed = Math.Min(CurrentMana, manaLost);
        CurrentMana -= manaUsed;
        return manaUsed;
    }
}