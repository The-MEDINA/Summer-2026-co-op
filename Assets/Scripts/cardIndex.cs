
using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace cardIndex
{
    // I hate C# structs.
    // I *really* hope that this is actually a good way of doing this.
    // The reason why this is a struct and not a class is because all this needs to contain is the details for card parent.
    // it's up to card parent itself to use the data in here.
    // the struct also shouldn't do anything.
    public struct Details
    {
        public Details(string _faction, int _cost, string _name, CardParent.type _type, string _text, int _health, int _damage, CardParent.effect _ability, string _flavorText, int _nameIndexPosition)
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
            
        }
        public string faction;
        public int cost;
        public string name;
        public CardParent.type type;
        public string text;
        public int health;
        public int damage;
        public CardParent.effect ability;
        public string flavorText;
        public int nameIndexPosition;
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
                
                CardParent.type _type = CardParent.type.minion;
                CardParent.effect _ability = CardParent.effect.none;
                int.TryParse(rawDetails[1], out _cost);
                int.TryParse(rawDetails[5], out _health);
                int.TryParse(rawDetails[6], out _damage);
                switch (rawDetails[3].Trim().ToLower())
                {
                    case("minion"):
                    {
                        _type = CardParent.type.minion;
                        break;
                    }
                    case("spell"):
                    {
                        _type = CardParent.type.spell;
                        break;
                    }
                    default:
                    {
                        Debug.LogWarning($"Unimplemented or unknown card type {rawDetails[3].Trim().ToLower()}! Please add it to cardIndex.cs. Otherwise assuming it's a minion.");
                        break;
                    }
                }

                switch (rawDetails[7].Trim().ToLower())
                {
                    case("none"):
                    {
                        _ability = CardParent.effect.none;
                        break;
                    }
                    case("explode"):
                    {
                        _ability = CardParent.effect.explode;
                        break;
                    }
                    case("deathtouch"):
                    {
                        _ability = CardParent.effect.deathtouch;
                        break;
                    }
                    default:
                    {
                        Debug.LogWarning($"Unimplemented or unknown card ability {rawDetails[7].Trim().ToLower()}! Please add it to cardIndex.cs. Otherwise assuming no ability.");
                        break;
                    }
                }
                // create the struct and add.
                Details cardToAdd = new Details(rawDetails[0], _cost, rawDetails[2], _type, rawDetails[4], _health, _damage, _ability, rawDetails[8], i);
                index.Add(rawDetails[2], cardToAdd);
                nameIndex.Add(rawDetails[2]);
            }
            reader.Close();
        }
    }
}
