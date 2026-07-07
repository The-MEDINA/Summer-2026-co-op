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

// This #define enables any warnings involving anything that comes out undefined.
// Comment it out to remove any undefined warnings.
#define WARN_UNDEFINED

using System.Collections.Generic;
using System.IO;
using Unity.Multiplayer.PlayMode;
using UnityEngine;

namespace cardIndex
{
    #region STRUCTS
    // The reason why this is a struct and not a class is because all this needs to contain is the details for card parent.
    // it's up to card constructors themselves to use the data in here.
    // the struct also shouldn't do anything.
    public struct Details
    {
        public Details(string _faction, int _cost, string _name, NewVirtualCardParent.type _type, string _text, int _health, int _damage, MinionParent.effect _ability, string _flavorText, int _nameIndexPosition, SpellParent.spellEffect _spellEffect, SpellParent.spellTarget _spellTarget, int _secondDamage, MinionParent.effect _secondAbility)
        {
            faction = _faction;
            cost = _cost;
            name = _name;
            type = _type;
            description = _text;
            health = _health;
            damage = _damage;
            ability = _ability;
            flavorText = _flavorText;
            nameIndexPosition = _nameIndexPosition;
            spellEffect = _spellEffect;
            spellTarget = _spellTarget;
            secondDamage = _secondDamage;
            secondAbility = _secondAbility;
        }

        public string faction;
        public int cost;
        public string name;
        public NewVirtualCardParent.type type;
        public string description;
        public int health;
        public int damage;
        public int secondDamage;
        public MinionParent.effect ability;
        public MinionParent.effect secondAbility;
        public string flavorText;
        public int nameIndexPosition;
        public SpellParent.spellEffect spellEffect;
        public SpellParent.spellTarget spellTarget;
    }

    public struct Sprites
    {
        public Sprites(Sprite _cardImage, Sprite _DescBackground)
        {
            cardImage = _cardImage;
            DescBackground = _DescBackground;
        }
        public Sprite cardImage;
        public Sprite DescBackground;
    }

    public struct CommanderDetails
    {
        public CommanderDetails(string _faction, string _name, string _description, string _flavorText)
        {
            faction = _faction;
            name = _name;
            description = _description;
            flavorText = _flavorText;
        }
        public string faction;
        public string name;
        public string description;
        public string flavorText;
    }
    #endregion
    // The only point of this class is to have a dictionary of all cards.
    // In theory this should speed up retrieving the card when making one by ONLY name.
    // The index is only made once and not remade every time a card needs to be constructed.
    public static class Index
    {
        // index of all cards.
        private static Dictionary<string, Details> index = new Dictionary<string, Details>();
        private static Dictionary<string, CommanderDetails> commanderIndex = new Dictionary<string, CommanderDetails>();
        private static Dictionary<string, Sprites> spritesIndex = new Dictionary<string, Sprites>();
        private static List<string> nameIndex = new List<string>();
        private static List<Sprite> cardSprites = new List<Sprite>();

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
        /// <param name="location">Location of the card.</param>
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
            if (genericDetails.type == NewVirtualCardParent.type.minion || genericDetails.type == NewVirtualCardParent.type.token)
            {
                // check for an ability with a separate class and create the minion.
                switch (genericDetails.ability)
                {
                    // two attacks.
                    case (MinionParent.effect.twoAttacks):
                        {
                            cardToCreate = new TwoAttackParent(name, location);
                            break;
                        }
                    // regular minion.
                    default:
                        {
                            cardToCreate = new MinionParent(name, location);
                            break;
                        }
                }

            }
            else if (genericDetails.type == NewVirtualCardParent.type.spell)
            {
                // create a spell
                cardToCreate = new SpellParent(name, location);
            }
            else
            {
#if WARN_UNDEFINED
                Debug.LogWarning($"Found no card named {name}! Double check this card exists?");
#endif
            }

            return cardToCreate;
        }

        /// <summary>
        /// Attach a commander card to an object.
        /// </summary>
        /// <param name="obj">Object to attach a commander card to.</param>
        /// <param name="name">Name of the commander card.</param>
        /// <param name="battleground">Battleground of the commander card.</param>
        /// <returns>Whether the attachment was successful.</returns>
        public static bool AttachCommanderCard(GameObject obj, string name, Battleground battleground)
        {
            if (obj.GetComponent<CommanderCardScript>() != null)
            {
                // object already has component, exit
                return false;
            }
            switch (name)
            {
                case ("Major Munchkin"):
                    {
                        obj.AddComponent<MajorMunchkinScript>();
                        obj.GetComponent<MajorMunchkinScript>().Name = "Major Munchkin";
                        if (battleground != null)
                        {
                            MajorMunchkinScript playerMajor = (MajorMunchkinScript)battleground.CommanderCard;
                            obj.GetComponent<MajorMunchkinScript>().TokenPrefab = playerMajor.TokenPrefab;
                            obj.GetComponent<MajorMunchkinScript>().BG = battleground;
                            battleground.P.CommanderCard = obj.GetComponent<MajorMunchkinScript>();
                        }
                        break;
                    }
                case ("Sergeant Zoomie"):
                    {
                        obj.AddComponent<SeargentZoomieScript>();
                        obj.GetComponent<SeargentZoomieScript>().Name = "Seargent Zoomie";
                        if (battleground != null)
                        {
                            obj.GetComponent<SeargentZoomieScript>().BG = battleground;
                            battleground.P.CommanderCard = obj.GetComponent<SeargentZoomieScript>();
                        }
                        break;
                    }
                // unimplemented commander card
                default:
                    {
                        Debug.LogWarning($"Could not attach commander card {name}! Double check a case is implemented for it in the switch statement?");
                        return false;
                    }
            }
            return true;
        }

        /// <summary>
        /// Creates the sprites for the requested card.
        /// </summary>
        /// <param name="name">Name of the card to grab sprites</param>
        /// <returns>A sprites struct for the requested card.</returns>
        public static Sprites GetSprites(string name)
        {
            Sprites spritesDetails;
            spritesIndex.TryGetValue(name, out spritesDetails);
            return spritesDetails;
        }

        public static List<Details> GetAllFactionCards(string faction)
        {
            if (nameIndex.Count == 0) GenerateDictionaryIndex();
            List<Details> factionCards = new List<Details>();
            for (int i = 0; i < nameIndex.Count; i++)
            {
                Details cardDetails = GetDetails(nameIndex[i]);
                if (cardDetails.faction == faction)
                {
                    factionCards.Add(cardDetails);
                }
            }
            return factionCards;
        }

        /// <summary>
        /// Creates the index of cards from the provided allCards.tsv file.
        /// This function should ONLY be called in the worst case scenario that the index is not ready by the time a card gets instantiated.
        /// Ideally, an async version of this function should get called when the game starts.
        /// </summary>
        private static void GenerateDictionaryIndex()
        {
            // setup
            string fileText = "allCards.tsv";
            string filepath = System.IO.Path.Combine(Application.streamingAssetsPath, fileText);
            if (System.IO.File.Exists(filepath))
            {
                fileText = System.IO.File.ReadAllText(filepath);
            }
            else
            {
#if WARN_UNDEFINED
                Debug.LogError($"Could not find the file needed for cardIndex at {filepath}! Cards cannot be spawned. Please return to the editor and find a fix.");
#endif
            }
            string[] reader = fileText.Split("\n");
            int catCardsOffset = 0;
            Sprite[] catSpritesheet = Resources.LoadAll<Sprite>($"spritesheet Cat");
            Sprite[] descBackgrounds = Resources.LoadAll<Sprite>($"DescBackgrounds");

            // while allCards.tsv has data.
            // for loop is only here because I need an int for the nameIndex.
            for (int i = 0; i < reader.Length; i++)
            {
                string[] rawDetails = reader[i].Split('\t');

                // parse the stuff that's not a string.
                int _cost = -1;
                int _health = -1;
                int _damage = -1;
                int _secondDamage = -1;
                NewVirtualCardParent.type _type = NewVirtualCardParent.type.none;
                MinionParent.effect _ability = MinionParent.effect.none;
                MinionParent.effect _secondAbility = MinionParent.effect.none;
                SpellParent.spellEffect _spellEffect = (SpellParent.spellEffect) 0;
                SpellParent.spellTarget _spellTarget = (SpellParent.spellTarget) 0;
                Sprite cardImage = null;
                Sprite descBackground = null;

                int.TryParse(rawDetails[1], out _cost);
                int.TryParse(rawDetails[5], out _health);
                int.TryParse(rawDetails[6], out _damage);
                // type of card
                switch (rawDetails[3].Trim().ToLower())
                {
                    case("minion"):
                    {
                        _type = NewVirtualCardParent.type.minion;
                        descBackground = descBackgrounds[(int) NewVirtualCardParent.type.minion];
                        break;
                    }
                    case("spell"):
                    {
                        _type = NewVirtualCardParent.type.spell;
                        descBackground = descBackgrounds[(int)NewVirtualCardParent.type.spell];
                        break;
                    }
                    case("token"):
                    {
                        _type = NewVirtualCardParent.type.token;
                        break;
                    }
                    case ("commander"):
                    {
                        descBackground = descBackgrounds[2];
                        break;
                    }
                    default:
                    {
#if WARN_UNDEFINED
                        Debug.LogWarning($"Unimplemented or unknown card type {rawDetails[3].Trim().ToLower()}! Please add it to cardIndex.cs. Otherwise assuming it's a minion.");
#endif
                        break;
                    }
                }
                // minion effect
                string[] effects = rawDetails[7].Trim().Split('/');
                for (int j = 0; j < effects.Length; j++)
                {
                    switch (effects[j].ToLower())
                    {
                        case ("none"):
                            {
                                if (j == 0) _ability = MinionParent.effect.none;
                                else _secondAbility = MinionParent.effect.none;
                                break;
                            }
                        case ("na"):
                            {
                                if (j == 0) _ability = MinionParent.effect.none;
                                else _secondAbility = MinionParent.effect.none;
                                break;
                            }
                        case ("explode"):
                            {
                                if (j == 0) _ability = MinionParent.effect.explode;
                                else _secondAbility = MinionParent.effect.explode;
                                break;
                            }
                        case ("deathtouch"):
                            {
                                if (j == 0) _ability = MinionParent.effect.deathtouch;
                                else _secondAbility = MinionParent.effect.deathtouch;
                                break;
                            }
                        case ("coordinate"):
                            {
                                if (j == 0) _ability = MinionParent.effect.coordinate;
                                else _secondAbility = MinionParent.effect.coordinate;
                                break;
                            }
                        case ("overkill"):
                            {
                                if (j == 0) _ability = MinionParent.effect.overkill;
                                else _secondAbility = MinionParent.effect.overkill;
                                break;
                            }
                        case ("haste"):
                            {
                                if (j == 0) _ability = MinionParent.effect.haste;
                                else _secondAbility = MinionParent.effect.haste;
                                break;
                            }
                        case ("sloth"):
                            {
                                if (j == 0) _ability = MinionParent.effect.sloth;
                                else _secondAbility = MinionParent.effect.sloth;
                                break;
                            }
                        case ("aoe"):
                            {
                                if (j == 0) _ability = MinionParent.effect.aoe;
                                else _secondAbility = MinionParent.effect.aoe;
                                break;
                            }
                        case ("heal"):
                            {
                                if (j == 0) _ability = MinionParent.effect.heal;
                                else _secondAbility = MinionParent.effect.heal;
                                break;
                            }
                        case ("thorns"):
                            {
                                if (j == 0) _ability = MinionParent.effect.thorns;
                                else _secondAbility = MinionParent.effect.thorns;
                                break;
                            }
                        case ("guard"):
                            {
                                if (j == 0) _ability = MinionParent.effect.guard;
                                else _secondAbility = MinionParent.effect.guard;
                                break;
                            }
                        case ("spawntokens"):
                            {
                                if (j == 0) _ability = MinionParent.effect.spawnToken;
                                else _secondAbility = MinionParent.effect.spawnToken;
                                break;
                            }
                        case ("duplicate"):
                            {
                                if (j == 0) _ability = MinionParent.effect.duplicate;
                                else _secondAbility = MinionParent.effect.duplicate;
                                break;
                            }
                        case ("twoattacks"):
                            {
                                if (j == 0)
                                {
                                    _ability = MinionParent.effect.twoAttacks;
                                    string[] twoAttackDamages = rawDetails[6].Split("/");
                                    _damage = int.Parse(twoAttackDamages[0]);
                                    _secondDamage = int.Parse(twoAttackDamages[1]);
                                }
                                else
                                {
                                    Debug.LogWarning($"Minion {rawDetails[2]} has TwoAttacks set twice! Please double check this minion.");
                                }
                                break;
                            }
                        default:
                            {
#if WARN_UNDEFINED
                                Debug.LogWarning($"Minion: Unimplemented or unknown card ability {rawDetails[7].Trim().ToLower()}! Please add it to cardIndex.cs. Otherwise assuming no ability.");
#endif
                                break;
                            }
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
                        case ("spawntokens"):
                        {
                            _spellEffect = SpellParent.spellEffect.spawnTokens;
                            break;
                        }
                        default:
                        {
#if WARN_UNDEFINED
                            Debug.LogWarning($"Spell: Unimplemented or unknown spell effect {rawDetails[9].Trim().ToLower()}! Please add it to cardIndex.cs. Otherwise assuming damage.");
#endif
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
                        case ("owner"):
                            {
                                _spellTarget = SpellParent.spellTarget.owner;
                                break;
                            }
                        case ("allenemies"):
                            {
                                _spellTarget = SpellParent.spellTarget.allEnemies;
                                break;
                            }
                        case ("none"):
                            {
                                _spellTarget = SpellParent.spellTarget.none;
                                break;
                            }
                        default:
                            {
#if WARN_UNDEFINED
                                Debug.LogWarning($"Unimplemented or unknown spell target {rawDetails[10].Trim().ToLower()}! Please add it to cardIndex.cs. Otherwise assuming enemyCards.");
#endif
                                break;
                            }
                    }
                }

                // load the sprites
                switch (rawDetails[0].Trim().ToLower())
                {
                    case ("cat"):
                        {
                            if (catCardsOffset < catSpritesheet.Length)
                            {
                                cardImage = catSpritesheet[catCardsOffset];
                            }
#if WARN_UNDEFINED
                            else
                            {
                                Debug.LogWarning($"More cat cards than sprites found! Double check the size of both the spritesheet and allCards.tsv?");
                            }
                            if (cardImage == null)
                            {
                                Debug.LogWarning($"Could not find sprite at spritesheet Cat_{catCardsOffset}! card will have fallback sprite.");
                            }
#endif
                            cardSprites.Add(cardImage);
                            catCardsOffset++;
                            break;
                        }
                }
                // create the struct and add.
                Details cardToAdd = new Details(rawDetails[0], _cost, rawDetails[2], _type, rawDetails[4], _health, _damage, _ability, rawDetails[8], i, _spellEffect, _spellTarget, _secondDamage, _secondAbility);
                Sprites spritesToAdd = new Sprites(cardImage, descBackground);
                spritesIndex.Add(rawDetails[2], spritesToAdd);
                index.Add(rawDetails[2], cardToAdd);
                nameIndex.Add(rawDetails[2]);
            }
        }
    }
}
