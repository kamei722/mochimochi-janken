// using UnityEngine;

// [CreateAssetMenu(fileName = "LeafEffect", menuName = "Items/Effects/LeafEffect")]
// public class LeafEffect : ItemEffect
// {
//     public override void ApplyEffect(GameController gameController)
//     {
//         Debug.Log("LeafEffect.ApplyEffect called.");

//         int playerType = gameController.GetPlayerChoice(); // 現在のプレイヤーの選択タイプ
//         int[,] typeAdvantage = gameController.GetTypeAdvantage(); // 全体の相性表
//         bool[,] knownTypeAdvantages = gameController.GetKnownTypeAdvantages(); // 現在の既知情報
//         int typeCount = gameController.TypeCount;

//         // 未知の相性を探す
//         for (int targetType = 0; targetType < typeCount; targetType++)
//         {
//             // 未知の相性かつ引き分けではないものを探す
//             if (!knownTypeAdvantages[playerType, targetType] && typeAdvantage[playerType, targetType] != 0)
//             {
//                 // 相性を1つ学習
//                 gameController.UpdateKnownTypeAdvantages(playerType, targetType);

//                 Debug.Log($"LeafEffect: Revealed type advantage between {playerType} and {targetType}.");
//                 return; // 最初に見つかった未知の相性を公開して終了
//             }
//         }

//         // 未知の相性がない場合
//         Debug.Log("LeafEffect: No unknown type advantages left to reveal.");
//     }
// }
