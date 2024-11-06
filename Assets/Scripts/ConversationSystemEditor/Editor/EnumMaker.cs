#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace TeamB.Editor
{
    public class EnumMaker : MonoBehaviour
    {
        private static StringBuilder _code = new();
        private static StringBuilder _tab = new();

        //初期化し、enumの上部部分を作成
        private static void Init(string exportPath, string fileName, string nameSpace)
        {
            //既にファイルがある場合は削除
            if (File.Exists(exportPath + $"/{fileName}.cs"))
            {
                File.Delete(exportPath + $"/{fileName}.cs");
            }

            //コード全文とタブ文字をリセット
            _code.Clear();
            _tab.Clear();

            //ネームスペースが入力されていれば設定
            if (!string.IsNullOrEmpty(nameSpace))
            {
                _code.AppendLine("namespace " + nameSpace);
                _code.AppendLine("{");
                _tab.Append("\t");
            }
        }

        /// <summary>
        /// Enumを生成する
        /// </summary>
        public static void Create(string fileName, string enumName, List<string> itemNameList, string exportPath,
            string summary = "",
            string nameSpace = "")
        {
            Init(exportPath, fileName, nameSpace);
            GenerateEnum(enumName, itemNameList, summary);
            Export(exportPath, fileName, nameSpace, enumName);
        }

        /// <summary>
        /// 複数のEnumを生成する
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="enumDatas">enumName, (itemNameList, summary)</param>
        /// <param name="exportPath"></param>
        /// <param name="nameSpace"></param>
        public static void Create(string fileName,
            Dictionary<string, (List<string> itemNameList, string summary)> enumDatas, string exportPath,
            string nameSpace = "")
        {
            Init(exportPath, fileName, nameSpace);
            var exportLog = new StringBuilder();
            for (int i = 0; i < enumDatas.Count; i++)
            {
                var enumData = enumDatas.ElementAt(i);
                if (i != 0) _code.AppendLine();
                exportLog.Append($"[{enumData.Key}]");
                GenerateEnum(enumData.Key, enumData.Value.itemNameList, enumData.Value.summary);
            }

            Export(exportPath, fileName, nameSpace, exportLog.ToString());
        }

        private static void GenerateEnum(string enumName, List<string> itemNameList, string summary)
        {
            //概要が入力されていれば設定
            if (!string.IsNullOrEmpty(summary))
            {
                _code.AppendLine(_tab + "/// <summary>");
                _code.AppendLine(_tab + "/// " + summary);
                _code.AppendLine(_tab + "/// </summary>");
            }

            //enum名を設定
            _code.AppendLine(_tab + "public enum " + enumName);
            _code.AppendLine(_tab + "{");
            //インデントを下げる
            _tab.Append("\t");
            foreach (var item in itemNameList)
            {
                _code.AppendLine(_tab + item + ",");
            }

            //インデントを戻す
            _tab.Remove(0, 1);
            _code.AppendLine(_tab + "}");
        }

        //enumを書き出し
        private static void Export(string exportPath, string fileName, string nameSpace, string enumName)
        {
            if (!string.IsNullOrEmpty(nameSpace)) _code.AppendLine("}");
            File.WriteAllText(exportPath + $"/{fileName}.cs", _code.ToString(), Encoding.UTF8);
            AssetDatabase.Refresh(ImportAssetOptions.ImportRecursive);
            Debug.Log(enumName + "のEnumを作成しました");
        }
    }
}
#endif