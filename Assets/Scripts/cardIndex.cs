/*
 * cardIndex.cs allows you to pull out details of cards.
 * It's a static class, so it's not tied to any object. It's essentially a singleton.
 *
 * In order to access it from other scripts, you'll need to include the namespace.
 * using cardIndex; <--- Like this
 *
 * Then to access something from it, you'll need to use the keyword 'cardIndex.Index'.
 * cardIndex.Index.something <--- Methods, properties, etc
 * 
 * Index on its own theoretically works, but it causes issues. Avoid that if possible.
 * 
 *  - Dave :>
 */

using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace cardIndex
{
    // The reason why this is a struct and not a class is because all this needs to contain is the details for card parent.
    // it's up to card constructors themselves to use the data in here.
    // the struct also shouldn't do anything.
    public struct Details
    {
        public Details(string _faction, int _cost, string _name, NewVirtualCardParent.type _type, string _text, int _health, int _damage, MinionParent.effect _ability, string _flavorText, int _nameIndexPosition, SpellParent.spellEffect _spellEffect, SpellParent.spellTarget _spellTarget)
        {
            faction = _faction;
            cost = _cost;
            name = _name;
            type = _type;
            text = _text;
            health = _health;
            damage = _damage;
            ability = _ability;
            flavorText = _flavorText;
            nameIndexPosition = _nameIndexPosition;
            spellEffect = _spellEffect;
            spellTarget = _spellTarget;
        }

        public string faction;
        public int cost;
        public string name;
        public NewVirtualCardParent.type type;
        public string text;
        public int health;
        public int damage;
        public MinionParent.effect ability;
        public string flavorText;
        public int nameIndexPosition;
        public SpellParent.spellEffect spellEffect;
        public SpellParent.spellTarget spellTarget;
    }
    // The only point of this class is to have a dictionary of all cards.
    // In theory this should speed up retrieving the card when making one by ONLY name.
    // The index is only made once and not remade every time a card needs to be constructed.
    public static class Index
    {
        // index of all cards.
        private static Dictionary<string, Details> index = new Dictionary<string, Details>();
        private static List<string> nameIndex = new List<string>();

        /// <summary>
        /// Return a details struct for CardParent to instantiate with. Also creates the index if it's not made already.
        /// </summary>
        /// <param name="name">name of card to return</param>
        /// <returns>a details struct containing all the information about the card.</returns>
        public static Details GetDetails(string name)
        {
            if (index.Count == 0) GenerateDictionaryIndex();
            Details returnDetails;
            index.TryGetValue(name, out returnDetails);
            return returnDetails;
        }

        /// <summary>
        /// Return a details struct for CardParent to instantiate with. Also creates the index if it's not made already.
        /// </summary>
        /// <param name="name">name of card to return</param>
        /// <returns>a details struct containing all the information about the card.</returns>
        public static Details GetDetails(int i)
        {
            if (index.Count == 0) GenerateDictionaryIndex();
            Details returnDetails;
            string name = nameIndex[i];
            index.TryGetValue(name, out returnDetails);
            return returnDetails;
        }

        /// <summary>
        /// Returns ONLY the name of the card given by index. Use this if you only need the name.
        /// </summary>
        /// <param name="i">index of the card.</param>
        /// <returns>name of the card at that index.</returns>
        public static string GetName(int i)
        {
            return nameIndex[i];
        }

        /// <summary>
        /// Creates the requested card.
        /// </summary>
        /// <param name="name">Name of the card to create</param>
        /// <param name="location">location of the card.</param>
        /// <returns>The requested card.</returns>
        public static NewVirtualCardParent CreateCard(string name, NewVirtualCardParent.location location)
        {
            NewVirtualCardParent cardToCreate = null;

            // Check for any exceptions here.
            // Exceptions meaning any card that has their own class.
            // if one is found, create an instance of that class and return it immediately.
            switch (name)
            {
                // (there currently aren't any)
            }

            // create a card from the details.
            Details genericDetails = GetDetails(name);
            if (genericDetails.type == NewVirtualCardParent.type.minion)
            {
                // create a minion
                cardToCreate = new MinionParent(name, location);
            }
            else if (genericDetails.type == NewVirtualCardParent.type.spell)
            {
                // create a spell
                cardToCreate = new SpellParent(name, location);
            }
            else
            {
                Debug.LogWarning($"Found no card named {name}! Double check this card exists?");
            }

            return cardToCreate;
        }

        /// <summary>
        /// Creates the index of cards from the provided allCards.tsv file.
        /// This function should ONLY be called in the worst case scenario that the index is not ready by the time a card gets instantiated.
        /// Ideally, an async version of this function should get called when the game starts.
        /// </summary>
        private static void GenerateDictionaryIndex()
        {
            // setup
            StreamReader reader = new StreamReader("Assets/Database/allCards.tsv");
            string raw;

            // while allCards.tsv has data.
            // for loop is only here because I need an int for the nameIndex.
            for (int i = 0; (raw = reader.ReadLine()) != null; i++)
            {
                string[] rawDetails = raw.Split('\t');

                // parse the stuff that's not a string.
                int _cost = -1;
                int _health = -1;
                int _damage = -1;
                NewVirtualCardParent.type _type = NewVirtualCardParent.type.minion;
                MinionParent.effect _ability = MinionParent.effect.none;
                SpellParent.spellEffect _spellEffect = (SpellParent.spellEffect) 0;
                SpellParent.spellTarget _spellTarget = (SpellParent.spellTarget) 0;

                int.TryParse(rawDetails[1], out _cost);
                int.TryParse(rawDetails[5], out _health);
                int.TryParse(rawDetails[6], out _damage);
                // type of card
                switch (rawDetails[3].Trim().ToLower())
                {
                    case("minion"):
                    {
                        _type = NewVirtualCardParent.type.minion;
                        break;
                    }
                    case("spell"):
                    {
                        _type = NewVirtualCardParent.type.spell;
                        break;
                    }
                    default:
                    {
                        Debug.LogWarning($"Unimplemented or unknown card type {rawDetails[3].Trim().ToLower()}! Please add it to cardIndex.cs. Otherwise assuming it's a minion.");
                        break;
                    }
                }
                // minion effect
                switch (rawDetails[7].Trim().ToLower())
                {
                    case ("none"):
                        {
                            _ability = MinionParent.effect.none;
                            break;
                        }
                    case ("na"):
                        {
                            _ability = MinionParent.effect.none;
                            break;
                        }
                    case ("explode"):
                        {
                            _ability = MinionParent.effect.explode;
                            break;
                        }
                    case ("deathtouch"):
                        {
                            _ability = MinionParent.effect.deathtouch;
                            break;
                        }
                    case ("coordinate"):
                        {
                            _ability = MinionParent.effect.coordinate;
                            break;
                        }
                    case ("overkill"):
                        {
                            _ability = MinionParent.effect.overkill;
                            break;
                        }
                    case ("haste"):
                        {
                            _ability = MinionParent.effect.haste;
                            break;
                        }
                    case ("sloth"):
                        {
                            _ability = MinionParent.effect.sloth;
                            break;
                        }
                    case ("two attacks"):
                        {
                            _ability = MinionParent.effect.twoAttacks;
                            break;
                        }
                    default:
                        {
                            Debug.LogWarning($"Unimplemented or unknown card ability {rawDetails[7].Trim().ToLower()}! Please add it to cardIndex.cs. Otherwise assuming no ability.");
                            break;
                        }
                }
                if (_type == NewVirtualCardParent.type.spell)
                {
                    // spell effect
                    switch (rawDetails[9].Trim().ToLower())
                    {
                        case ("damage"):
                        {
                            _spellEffect = SpellParent.spellEffect.damage;
                            break;
                        }
                        case ("heal"):
                        {
                            _spellEffect = SpellParent.spellEffect.heal;
                            break;
                        }
                        case ("unique"):
                        {
                            _spellEffect = SpellParent.spellEffect.unique;
                            break;
                        }
                        case ("equipment"):
                        {
                            _spellEffect = SpellParent.spellEffect.equipment;
                            break;
                            }
                        default:
                        {
                                Debug.LogWarning($"Unimplemented or unknown spell effect {rawDetails[9].Trim().ToLower()}! Please add it to cardIndex.cs. Otherwise assuming damage.");
                            break;
                        }
                    }
                    // spell target
                    switch (rawDetails[10].Trim().ToLower())
                    {
                        case ("allies"):
                            {
                                _spellTarget = SpellParent.spellTarget.allyCards;
                                break;
                            }
                        case ("enemies"):
                            {
                                _spellTarget = SpellParent.spellTarget.enemyCards;
                                break;
                            }
                        case ("player"):
                            {
                                _spellTarget = SpellParent.spellTarget.owner;
                                break;
                            }
                        default:
                            {
                                Debug.LogWarning($"Unimplemented or unknown spell target {rawDetails[10].Trim().ToLower()}! Please add it to cardIndex.cs. Otherwise assuming enemyCards.");
                                break;
                            }
                    }
                }
                // create the struct and add.
                Details cardToAdd = new Details(rawDetails[0], _cost, rawDetails[2], _type, rawDetails[4], _health, _damage, _ability, rawDetails[8], i, _spellEffect, _spellTarget);
                index.Add(rawDetails[2], cardToAdd);
                nameIndex.Add(rawDetails[2]);
            }
            reader.Close();
        }
    }
}
