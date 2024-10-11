using UnityEditor;
using UnityEngine;
using System.Linq;

public class BuildCommand
{
    [MenuItem("Assets/Build Application")]
    public static void Build()
    {
        //プラットフォーム、オプション
        var isDevelopment = true;
        var platform = BuildTarget.StandaloneWindows;

        // 出力名とか
        var exeName = PlayerSettings.productName;
        var ext = ".exe";
        string outpath = default;

        // ビルド対象シーンリスト
        var scenes = EditorBuildSettings.scenes
            .Where(scene => scene.enabled)
            .Select(scene => scene.path)
            .ToArray();

        //コマンドライン引数をパース
				//NOTE: iOSはプロジェクトが生成されるだけなので、別途処理する
        var args = System.Environment.GetCommandLineArgs();
        for (var i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "-projectPath":
                    outpath = args[i + 1] + "\\Build";
                    break;
                case "-devmode":
                    isDevelopment = args[i + 1] == "true";
                    break;
                case "-platform":
                    switch(args[i + 1])
                    {
                        case "Android":
                            platform = BuildTarget.Android;
                            ext = ".apk";
                            break;

                        case "Windows":
                            platform = BuildTarget.StandaloneWindows;
                            ext = ".exe";
                            break;

						case "WebGL":
                            platform = BuildTarget.WebGL;
                            PlayerSettings.WebGL.decompressionFallback = true; //動かない環境があるのでチェック入れる
                            ext = "";
                            break;

                        case "Switch":
                            platform = BuildTarget.Switch;
                            ext = "";
                            break;
                    }
                    break;
            }
        }

        //ビルドオプションの成型
        var option = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = outpath + "\\" + exeName + ext
        };
        
        if (isDevelopment)
        {
            //optionsはビットフラグなので、|で追加していくことができる
            option.options = BuildOptions.Development | BuildOptions.AllowDebugging;
        }
        option.target = platform; //ビルドターゲットを設定. 今回はWin64

        // 実行
        var report = BuildPipeline.BuildPlayer(option);

        // 結果出力
        if (report.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
        {
            Debug.Log("BUILD SUCCESS");
            EditorApplication.Exit(0);
        }
        else
        {
            Debug.LogError("BUILD FAILED");

            foreach(var step in report.steps)
            {
                Debug.Log(step.ToString());
            }

            Debug.LogError("Error Count: " + report.summary.totalErrors);
            EditorApplication.Exit(1);
        }
    }
}