using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonstoJuicer
{
    class Monster
    {
        public string _id { get; set; }
        public int index { get; set; }
        public string name { get; set; }
        public double challenge_rating { get; set; }
        public MonSize size { get; set; }
        public string type { get; set; }
        public string subtype { get; set; }
        public string alignment { get; set; }
        
        //Stats
        #region Stats
        public int armor_class { get; set; }
        public int hit_points { get; set; }
        public double hit_dice { get; set; }
        public string speed { get; set; }
        public int strength { get; set; }
        public int dexterity { get; set; }
        public int constitution { get; set; }
        public int intelligence { get; set; }
        public int wisdom { get; set; }
        public int charisma { get; set; }
        #endregion stats

        //saving Throws
        #region Saves
        public int? strength_save { get; set; }
        public int? dexterity_save { get; set; }
        public int constitution_save { get; set; }
        public int intelligence_save { get; set; }
        public int wisdom_save { get; set; }
        public int? charisma_save { get; set; }
        
        #endregion

        //skills
        #region skills
        public int history { get; set; }
        public int perception { get; set; }
        public int? medicine { get; set; }
        public int? religion { get; set; }
        public int? stealth { get; set; }
        public int? persuasion { get; set; }
        public int? insight { get; set; }
        public int? deception { get; set; }
        public int? arcana { get; set; }
        public int? athletics { get; set; }
        public int? acrobatics { get; set; }
        public int? survival { get; set; }
        public int? investigation { get; set; }
        public int? nature { get; set; }
        public int? intimidation { get; set; }
        public int? performance { get; set; }
        #endregion

        //Resistances
        #region Resistances
        public bool necrotic_resist { get; set; }
        public bool lightning_resist { get; set; }
        public bool thunder_resist { get; set; }
        public bool bludgeoning_physical_resist { get; set; }
        public bool piercing_physical_resist { get; set; }
        public bool slashing_physical_resist { get; set; }
        public bool spells_resist { get; set; }
        public bool poison_resist { get; set; }
        public bool cold_resist { get; set; }
        public bool radiant_resist { get; set; }
        public bool acid_resist { get; set; }
        public bool adamantine_exception { get; set; }
        public bool silvered_exception { get; set; }
        #endregion


        public string damage_vulnerabilities { get; set; }
        public string damage_resistances { get; set; }
        public string damage_immunities { get; set; }
        public string condition_immunities { get; set; }
        public string senses { get; set; }
        public string languages { get; set; }
        public List<SpecialAbility> special_abilities { get; set; }
        public List<Action> actions { get; set; }
        public List<LegendaryAction> legendary_actions { get; set; }
        public string url { get; set; }
        
       
        
        public List<Reaction> reactions { get; set; }
        



        Monster(MonsterRaw rawMon)
        {
            _id = rawMon._id;
            index = rawMon.index;
            name = rawMon.name;
            challenge_rating = rawMon.challenge_rating;
            size = FixSize(rawMon.size);
            //type
            //subtype
            //alignmnet
            
            //stats;
            #region Stats
            armor_class = rawMon.armor_class;
            hit_points = rawMon.hit_points;
            hit_dice = Program.DiceToNum(rawMon.hit_dice);

            //speed

            strength = rawMon.strength;
            dexterity = rawMon.dexterity;
            constitution = rawMon.constitution;
            intelligence = rawMon.intelligence;
            wisdom = rawMon.wisdom;
            charisma = rawMon.charisma;
            #endregion

            //saving throws
            #region Saves
            strength_save = rawMon.strength_save;
            dexterity_save = rawMon.dexterity_save;
            constitution_save = rawMon.constitution_save;
            intelligence_save = rawMon.intelligence_save;
            wisdom_save = rawMon.wisdom_save;           
            charisma_save = rawMon.charisma_save;
            #endregion
            
            
            //public int history { get; set; }
            //public int perception { get; set; }
        }
        private MonSize FixSize(string strSize)
        {
            List<MonSize> sizes = Enum.GetValues(typeof(MonSize)).Cast<MonSize>().ToList();
            foreach (MonSize ms in sizes.Where(x => x.ToString().ToLower() == strSize.ToLower()))
            {
                return ms;
            }
            //if it couldn't assign a size then error
            return MonSize.error;
        }
    }

    
}
