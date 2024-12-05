﻿using JRPG_Game.Characters.Skills;
using JRPG_Game.Enums;
using JRPG_Game.Interfaces;

namespace JRPG_Game.Characters.Classes;

/// <summary>
/// Represents a priest character specialized in magical attacks and team healing abilities.
/// </summary>
/// <remarks>
/// Inherits from <see cref="Character"/> and implements the <see cref="IMana"/> interface.
/// The priest is a support-focused character capable of dealing magical damage and healing allies.
/// </remarks>
public class Priest : Character, IMana
{
    public int MaxMana => 100;
    public int CurrentMana { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Priest"/> class.
    /// </summary>
    /// <param name="name">The name of the priest.</param>
    /// <param name="team">The team to which the priest belongs.</param>
    /// <remarks>
    /// Sets the priest's stats and initializes its skill set with magical damage and healing abilities.
    /// </remarks>
    public Priest(string name, Team team)
        : base(
            name: name,
            team: team,
            maxHealth: 70,
            speed: 70,
            armor: ArmorType.Textile,
            physicalAttack: 0,
            magicalAttack: 65,
            dodgeChance: 0.15m,
            paradeChance: 0.10m,
            spellResistanceChance: 0.30m,
            skills: [])
    {
        CurrentMana = MaxMana;
        Skills.AddRange([
            new Attack<Character>(
                name: "Châtiment",
                description: () =>
                    $"Inflige 75% de la puissance d’attaque magique ({(int)(MagicalAttack * 0.75m)}) à la cible.\n" +
                    $"Inflige 150% ({(int)(MagicalAttack * 1.50m)}) à la cible si celle ci n'est ni un {nameof(Priest)} ni un {nameof(Paladin)}.",
                owner: this,
                targetType: TargetType.Enemy,
                reloadTime: 1,
                manaCost: 15,
                damage: target =>
                    (int)(MagicalAttack * (target.GetType() == typeof(Priest) || target.GetType() == typeof(Paladin)
                        ? 0.75m
                        : 1.50m)),
                attackType: DamageType.Magical),
            new SpecialAbility<Team>(
                name: "Cercle de soins",
                description: () => $"Soigne toute l'équipe sélectionné de {(int)(MagicalAttack * 0.75m)} PV.",
                owner: this,
                targetType: TargetType.TeamAllied,
                reloadTime: 2,
                manaCost: 30,
                effect: target =>
                {
                    target.Heal((int)(MagicalAttack * 0.75m));
                    return "";
                }),
            ((IMana)this).Drink(this)
        ]);
    }

    /// <summary>
    /// Handles the priest's defense against incoming attacks.
    /// </summary>
    /// <typeparam name="TTarget">The type of the target being attacked.</typeparam>
    /// <param name="from">The incoming attack.</param>
    /// <param name="damageParameter">The character responsible for the damage.</param>
    /// <returns>The amount of damage taken after applying defensive effects.</returns>
    public override int Defend<TTarget>(Attack<TTarget> from, Character damageParameter)
    {
        from.StatusInfo.Set(from, (false, false, false));
        from.StatusInfo.SetDamage(from, damageParameter);

        return TakeDamage((int)from.StatusInfo.Damage);
    }

    protected override void ApplyEndTurn()
    {
        throw new NotImplementedException();
    }

    //

    /// <summary>
    /// Returns a string that represents the <see cref="Priest"/>.
    /// </summary>
    /// <returns>A string that represents the <c>Priest</c></returns>
    public override string ToString()
    {
        return base.ToString() + "\n" +
               $" ~ Mana: {CurrentMana}/{MaxMana}";
    }
}