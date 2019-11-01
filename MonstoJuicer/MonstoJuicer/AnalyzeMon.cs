using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace MonstoJuicer
{
    class AnalyzeMon
    {
        [LoadColumn(0)]
        public string Name { get; set; }
        [LoadColumn(1)]
        public float ArmorClass { get; set; }
        [LoadColumn(2)]
        public float HitPoints { get; set; }
        [LoadColumn(3)]
        public float Strength { get; set; }
        [LoadColumn(4)]
        public float Dexterity { get; set; }
        [LoadColumn(5)]
        public float Constitution { get; set; }
        [LoadColumn(6)]
        public float Intelligence { get; set; }
        [LoadColumn(7)]
        public float Wisdom { get; set; }
        [LoadColumn(8)]
        public float Charisma { get; set; }
        [LoadColumn(9)]
        public float ConstitutioSave { get; set; }
        [LoadColumn(10)]
        public float IntelligenceSave { get; set; }
        [LoadColumn(11)]
        public float WisdomSave { get; set; }
        [LoadColumn(12)]
        public float SpecialAbilities { get; set; }
        [LoadColumn(13)]
        public float Actions { get; set; }
        [LoadColumn(14)]
        public float LegendaryActions { get; set; }
        [LoadColumn(15)]
        public float ChallengeRating { get; set; }
    }

    public class ChallengePrediction
    {
        [ColumnName("Score")]
        public float Prediction { get; set; }
        //public float Probability { get; set; }
        //public float Score { get; set; }
    }
}
