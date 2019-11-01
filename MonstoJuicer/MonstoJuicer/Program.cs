using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;
using System.Linq;
using Microsoft.ML;

using System.Data.Common;

namespace MonstoJuicer
{
    class Program
    {
        static void Main(string[] args)
        {
            List<MonsterRaw> RawMonsters;
            RawMonsters = LoadJson();
            string DataPath = @"Resources\Monstos.csv";
            GenerateMonsterCSV(RawMonsters);
            //ResistancesStuff(RawMonsters);
            //foreach(MonsterRaw mon in RawMonsters)
            //{

            //    if (mon.charisma == null)
            //    {
            //        Console.WriteLine(mon.name);
            //    }
            //}

            var ctx = new MLContext();

            IDataView trainData = ctx.Data.LoadFromTextFile<AnalyzeMon>(DataPath, hasHeader: true, separatorChar: ',');
            IDataView testData = ctx.Data.LoadFromTextFile<AnalyzeMon>(DataPath, hasHeader: true, separatorChar: ',');


            var dataProcessPipeline = ctx.Transforms.CopyColumns(outputColumnName: "Label", inputColumnName: nameof(AnalyzeMon.ChallengeRating))
                           .Append(ctx.Transforms.NormalizeMeanVariance(outputColumnName: nameof(AnalyzeMon.ArmorClass)))
                           .Append(ctx.Transforms.NormalizeMeanVariance(outputColumnName: nameof(AnalyzeMon.HitPoints)))
                           .Append(ctx.Transforms.NormalizeMeanVariance(outputColumnName: nameof(AnalyzeMon.Strength)))
                           .Append(ctx.Transforms.NormalizeMeanVariance(outputColumnName: nameof(AnalyzeMon.Dexterity)))
                           .Append(ctx.Transforms.NormalizeMeanVariance(outputColumnName: nameof(AnalyzeMon.Constitution)))
                           .Append(ctx.Transforms.NormalizeMeanVariance(outputColumnName: nameof(AnalyzeMon.Intelligence)))
                           .Append(ctx.Transforms.NormalizeMeanVariance(outputColumnName: nameof(AnalyzeMon.Wisdom)))
                           .Append(ctx.Transforms.NormalizeMeanVariance(outputColumnName: nameof(AnalyzeMon.Charisma)))
                           .Append(ctx.Transforms.NormalizeMeanVariance(outputColumnName: nameof(AnalyzeMon.Actions)))
                           .Append(ctx.Transforms.NormalizeMeanVariance(outputColumnName: nameof(AnalyzeMon.LegendaryActions)))
                           .Append(ctx.Transforms.NormalizeMeanVariance(outputColumnName: nameof(AnalyzeMon.SpecialAbilities)))
                           .Append(ctx.Transforms.Concatenate("Features", nameof(AnalyzeMon.ArmorClass), nameof(AnalyzeMon.HitPoints), 
                                   nameof(AnalyzeMon.Strength), nameof(AnalyzeMon.Dexterity), nameof(AnalyzeMon.Constitution), 
                                   nameof(AnalyzeMon.Intelligence), nameof(AnalyzeMon.Wisdom), nameof(AnalyzeMon.Charisma), nameof(AnalyzeMon.Actions), 
                                   nameof(AnalyzeMon.LegendaryActions), nameof(AnalyzeMon.SpecialAbilities)));

           // ConsoleHelper.PeekDataViewInConsole(ctx, trainData, dataProcessPipeline, 5);
           // ConsoleHelper.PeekVectorColumnDataInConsole(ctx, "Features", trainData, dataProcessPipeline, 5);

            var trainer = ctx.Regression.Trainers.Sdca(labelColumnName: "Label", featureColumnName: "Features");
            var trainingPipeline = dataProcessPipeline.Append(trainer);

            // STEP 4: Train the model fitting to the DataSet
            //The pipeline is trained on the dataset that has been loaded and transformed.
            Console.WriteLine("=============== Training the model ===============");
            var trainedModel = trainingPipeline.Fit(trainData);

            // STEP 5: Evaluate the model and show accuracy stats
            Console.WriteLine("===== Evaluating Model's accuracy with Test data =====");

            IDataView predictions = trainedModel.Transform(testData);
            var metrics = ctx.Regression.Evaluate(predictions, labelColumnName: "Label", scoreColumnName: "Score");

            //Sample: 
            //vendor_id,rate_code,passenger_count,trip_time_in_secs,trip_distance,payment_type,fare_amount
            //VTS,1,1,1140,3.75,CRD,15.5

            var monster = new AnalyzeMon()
            {

                ArmorClass=15,
                HitPoints=32,
                Strength=14,
                Dexterity=16,
                Constitution=15,
                Intelligence=14,
                Wisdom=15,
                Charisma=11,
                Actions=4,
                LegendaryActions=0,
                SpecialAbilities=2
                ,

            };

            ///

            // Create prediction engine related to the loaded trained model
            var predEngine = ctx.Model.CreatePredictionEngine<AnalyzeMon, ChallengePrediction>(trainedModel);

            //Score
            var resultprediction = predEngine.Predict(monster);
            ///

            Console.WriteLine($"**********************************************************************");
            Console.WriteLine($"Predicted CR: {resultprediction.Prediction:0.####}, actual CR: 9");
            Console.WriteLine($"**********************************************************************");
        }

        public static List<MonsterRaw> LoadJson()
        {
            using (StreamReader r = new StreamReader(@"Resources\Monstos.txt"))
            {
                string json = r.ReadToEnd();
                List<MonsterRaw> items = JsonConvert.DeserializeObject<List<MonsterRaw>>(json);
                return items;
            }
        }

        public static void ResistancesStuff(List<MonsterRaw> RawMonsters)
        {
            List<string> resistances = new List<string>();
            foreach (MonsterRaw monster in RawMonsters)
            {
                List<string> lines = new List<string>(monster.damage_resistances.Split(';'));
                if (monster.damage_resistances != "")
                {
                    //Console.WriteLine(monster.damage_resistances);
                }
                foreach (string line in lines)
                {
                    List<string> splitStr = new List<string>(line.Split(","));
                    resistances.AddRange(splitStr);
                }

                List<string> tmpList = new List<string>();
                foreach (string str in resistances)
                {
                    tmpList.Add(str.Trim());
                }
                resistances = tmpList;
            }
            foreach (string line in resistances.Distinct())
            {
                //Console.WriteLine(line);
            }

        }


        //Convert a dice roll string to an average ex: 2d6 = 5
        //Must be in the form (int)D(int)
        public static double DiceToNum(string dice)
        {
            //remove whitespace if any
            dice.Replace(" ", string.Empty);

            dice = dice.ToLower();
            string[] diceNum = dice.Split('d');

            int diceSize;
            int diceCount;

            if (diceNum.Count() == 2)
            {
                if (int.TryParse(diceNum[0], out diceCount) &&
                    int.TryParse(diceNum[1], out diceSize)){
                    int difference = (diceSize * diceCount) - diceCount;
                    return (((double)difference) / 2);
                }
                return 0;
            }

            return 0;
        }

        public static void GenerateMonsterCSV(List<MonsterRaw> RawMons)
        {
            string csvLine =
                    "name," +
                    "armor class," +
                    "hit points," +
                    "strength," +
                    "dexterity," +
                    "constitution," +
                    "intelligence," +
                    "wisdom," +
                    "charisma," +
                    "constitution_save," +
                    "intelligence_save," +
                    "wisdom_save," +
                    "Special Abilities," +
                    "actions," +
                    "legendary_actions," +
                    "challenge_rating";
            using (StreamWriter MonstoCSV = new StreamWriter(@"Resources\Monstos.csv"))
            {
                MonstoCSV.WriteLine(csvLine);
                foreach (MonsterRaw mon in RawMons)
                {
                    csvLine = "";
                    csvLine += mon.name + ",";
                    csvLine += mon.armor_class + ",";
                    csvLine +=mon.hit_points+",";
                    csvLine += mon.strength + ",";
                    csvLine += mon.dexterity+",";
                    csvLine += mon.constitution + ",";
                    csvLine += mon.intelligence + ",";
                    csvLine += mon.wisdom + ",";
                    csvLine += mon.charisma + ",";
                    csvLine += mon.constitution_save + ",";
                    csvLine += mon.intelligence_save + ",";
                    csvLine += mon.wisdom_save + ",";
                    if (mon.special_abilities == null)
                    {
                        csvLine += 0 + ",";
                    }
                    else
                    {
                        csvLine += mon.special_abilities.Count + ",";
                    }

                    if (mon.actions == null)
                    {
                        csvLine += 0 + ",";
                    }
                    else
                    {
                        csvLine += mon.actions.Count + ",";
                    }

                    if (mon.legendary_actions == null)
                    {
                        csvLine += 0 + ",";
                    }
                    else
                    {
                        csvLine += mon.legendary_actions.Count + ",";
                    }
                    csvLine += mon.challenge_rating+"";

                    MonstoCSV.WriteLine(csvLine);

                }
            }
        }


    }
}
