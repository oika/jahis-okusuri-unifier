using System.Diagnostics;
using System.Text;

var pathes = Environment.GetCommandLineArgs().Skip(1).ToList();
if (pathes.Count == 0)
{
    Console.WriteLine("結合するファイルを指定してください");
    return;
}
if (pathes.Count == 1)
{
    Console.WriteLine("結合対象のファイルが1つしか指定されていないため、なにもせず終了します");
    return;
}

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
var encoding = Encoding.GetEncoding("Shift_JIS");

try
{
    BasicInfo? basicInfo = null;
    var allCsvRows = new List<string>();
    foreach (var path in pathes)
    {
        var lines = File.ReadAllLines(path, encoding);

        var (tmpInfo, msg, restLines) = BasicInfo.Parse(lines);
        if (tmpInfo == null)
        {
            Console.WriteLine($"基本情報の解析に失敗しました。 {path} {msg ?? ""}");
            return;
        }
        if (basicInfo == null)
        {
            basicInfo = tmpInfo;
        }
        else
        {
            var merged = basicInfo.Merge(tmpInfo);
            if (merged.info == null)
            {
                Console.WriteLine($"患者情報またはフォーマットバージョンの異なるデータを結合することはできません。({merged.msg ?? ""}) " + path);
                return;
            }
            basicInfo = merged.info;
        }

        allCsvRows.AddRange(restLines);
    }

    Debug.Assert(basicInfo != null);
    allCsvRows.InsertRange(0, basicInfo.ToCsvRows());


    File.WriteAllLines($"jahis_result_{DateTime.Now:yyyyMMddHHmmss}.csv", allCsvRows, encoding);
    Console.WriteLine("完了");
}
catch(Exception ex)
{
    Console.WriteLine(ex);
}
finally
{
    Console.ReadKey();
}

