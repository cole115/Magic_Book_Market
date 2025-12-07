using UnityEditor;
using CsvHelper;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using System;




public class Editor_SOMaker
{
    const string CSVPath = "Assets/CSVs/";
    const string SavePath = "Assets/ScriptableObjects/";

    /// <summary>
    /// Csv 생성을 위한 제너릭 함수
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="csvFileName"> csv파일의 이름</param>
    /// <param name="groupName"></param>
    /// <param name="fillAssetFields"></param>
    /// <param name="getID"></param>
    private static void GenerateSOFromCSV<T>(string csvFileName, string groupName, string getID,
        Action<CsvReader, T> fillAssetFields) where T : ScriptableObject
    {
        string csvPath = CSVPath + csvFileName;
        string soPath = SavePath + groupName + "/";


        // 폴더 자동 생성
        if (!AssetDatabase.IsValidFolder(SavePath + groupName))
        { 
            AssetDatabase.CreateFolder(SavePath.TrimEnd('/'),groupName);
        }



        using var reader = new StreamReader(csvPath);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        csv.Read();
        csv.ReadHeader();

        while (csv.Read())
        {
            string id = csv.GetField<string>(getID);
            string assetPath = soPath + id + ".asset";

            var asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            if (asset == null)
            {
                // 에셋이 없을 경우 생성
                asset = ScriptableObject.CreateInstance<T>();
                AssetDatabase.CreateAsset(asset, assetPath);
            }

            fillAssetFields(csv, asset);

            EditorUtility.SetDirty(asset);

        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // 어드래서블에 추가
        AddressableUtility.AddToAddressable<T>(groupName, soPath);
    }




    [MenuItem("EditorTool/CSV/Pattern Maker")]
    public static void GeneratePatternSO()
    {

        GenerateSOFromCSV<Enemy_Pattern>(
            "Pattern.csv",
            "Patterns",
            "PatternID",
            (csv, asset) =>
            {
                asset.PatternID = csv.GetField<string>("PatternID");
                asset.MonsterID = csv.GetField<string>("MonsterID");
                asset.HitChanceList = new List<float>();
                asset.DamageMultiplier = csv.GetField<float>("DamageFactor");
                for (int i = 1; i <= 4; i++)
                {
                    if (csv.TryGetField<float>($"HitChance{i}", out var value))
                        asset.HitChanceList.Add(value);
                }
                asset.UseChance = csv.GetField<float>("UseChance");

            }
            );
    }

    [MenuItem("EditorTool/CSV/Monster Maker")]
    public static void GenerateEnemySO()
    {
        GenerateSOFromCSV<EnemySO>(
            "Monster.csv",
            "Enemys",
            "MonsterID",
            (csv, asset) =>
            {
                asset.MonsterID = csv.GetField<string>("MonsterID");
                asset.MonsterName = csv.GetField<string>("MonsterName");
                asset.Hp = csv.GetField<int>("Hp");
                asset.Atk = csv.GetField<int>("Atk");
                asset.Def = csv.GetField<int>("Def");
                asset.StageList = new List<string>();
                for (int i = 1; i <= 8; i++)
                {
                    var value = csv.GetField<string>($"Stage{i}");
                    if (value != "NULL")
                    {
                        asset.StageList.Add(value);
                    }
                    else
                        continue;
                }
                asset.GetGold = csv.GetField<int>("GetGold");
            }
            );

    }

    [MenuItem("EditorTool/CSV/Card Maker")]
    public static void GenerateCardSO()
    {
        GenerateSOFromCSV<CardData>(
            "Card.csv",
            "Cards",
            "CardID",
            (csv, asset) =>
            {
                asset.CardID = csv.GetField<string>("CardID");
                asset.Name = csv.GetField<string>("NameKR");
                asset.Element = (CardElement)Enum.Parse(typeof(CardElement), csv.GetField<string>("Element"));
                asset.Rank = (CardRank)Enum.Parse(typeof(CardRank), csv.GetField<string>("Rarity"));

                // EffectStructure 생성
                asset.Effects = new EffectStructure();
                asset.Effects.Cost = csv.GetField<int>("Cost");

                string activeType1 = csv.GetField<string>("ActiveType1");
                float activeValue1 = csv.GetField<float>("ActiveValue1");
                if (activeType1 != "None")
                {
                    asset.Effects.UprEffectInfos.Add(new EffectInfo
                    {
                        Type = (EffectType)Enum.Parse(typeof(EffectType), activeType1),
                        Value = activeValue1
                    });
                }

                string activeType2 = csv.GetField<string>("ActiveType2");
                float activeValue2 = csv.GetField<float>("ActiveValue2");
                if (activeType2 != "None")
                {
                    asset.Effects.UprEffectInfos.Add(new EffectInfo
                    {
                        Type = (EffectType)Enum.Parse(typeof(EffectType), activeType2),
                        Value = activeValue2
                    });
                }

                // 정방향 공격 횟수
                asset.Effects.AttackCount = csv.GetField<int>("AttackCount");

                string passiveType = csv.GetField<string>("PassiveType");
                float passiveValue = csv.GetField<float>("PassiveValue");

                if (passiveType != "None")
                {
                    asset.Effects.RevEffectInfos.Add(new EffectInfo
                    {
                        Type = (EffectType)Enum.Parse(typeof(EffectType), passiveType),
                        Value = passiveValue
                    });
                }

                asset.Effects.Trigger =
                (RevEffectTrigger)Enum.Parse(typeof(RevEffectTrigger), csv.GetField<string>("Trigger"));


                asset.Effects.UprDescription = csv.GetField<string>("Description_1");
                asset.Effects.RevDescription = csv.GetField<string>("Description_2");

            }
            );
    }

}
