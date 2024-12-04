﻿using JRPG_Game.Characters;
using JRPG_Game.Interfaces;
using JRPG_Game.Utils;

namespace JRPG_Game.Team;

/// <summary>
/// Represents a team of characters in the game.
/// The <see cref="Team"/> class manages a group of characters.
/// </summary>
public class Team : ITarget
{
    private Team(string name)
    {
        Name = name;
        List.Add(this);
    }

    public List<Character> Characters { get; } = [];
    public string Name { get; }

    /// <summary>
    /// A static list containing all the teams created in the game.
    /// </summary>
    /// <value>A list of <see cref="Team"/> objects.</value>
    public static readonly List<Team> List = [];

    //

    /// <summary>
    /// Adds a character to the team.
    /// </summary>
    /// <param name="character">The <see cref="Character"/> to add to the team.</param>
    /// <remarks>
    /// This method is used internally during team creation to populate the team's character list.
    /// </remarks>
    private void Add(Character character) => Characters.Add(character);

    //

    /// <summary>
    /// Creates a new team with a specified number of characters.
    /// </summary>
    /// <param name="size">The number of characters to add to the team.</param>
    /// <returns>The newly created <see cref="Team"/> instance.</returns>
    /// <remarks>
    /// This method performs the following actions:
    /// <list type="bullet">
    ///     <item>
    ///         <description>Prompts the user to enter the team's name.</description>
    ///     </item>
    ///     <item>
    ///         <description>Iteratively creates characters and adds them to the team. (see <see cref="Character.Create"/>)</description>
    ///     </item>
    /// </list>
    /// </remarks>
    /// <example>
    /// <code>
    /// Team myTeam = Team.Create(3);
    /// </code>
    /// </example>
    public static Team Create(int size)
    {
        Team newTeam = new(Prompt.Get<string>("Entrez le nom de votre équipe :"));

        for (var i = 0; i < size; i++)
        {
            Console.WriteLine();
            Console.WriteLine($"Creation du {i + 1}{(i + 1 == 1 ? "er" : "ème")} personnage...");
            newTeam.Add(Character.Create(newTeam));
        }

        return newTeam;
    }

    /// <summary>
    /// Determines whether combat can continue based on the number of teams with alive characters.
    /// </summary>
    /// <returns>
    /// <c>true</c> if at least two teams have characters that are still alive; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsCombatOn() =>
        List.Count(team => team.Characters.Any(character => character.IsAlive(false))) >= 2;

    //

    /// <summary>
    /// Provides a string representation of the team, including its name and characters.
    /// </summary>
    /// <returns>
    /// A string describing the team's name and its members.
    /// </returns>
    /// <remarks>
    /// If the team has no characters, it returns "Aucun personnage" for the member list.
    /// </remarks>
    public override string ToString()
    {
        var charactersInfo = Characters.Count != 0
            ? string.Join(", ", Characters.Select(character => character.Name))
            : "Aucun personnage";

        return $"Équipe : {Name}\n" +
               $"Membres : {charactersInfo}";
    }
}