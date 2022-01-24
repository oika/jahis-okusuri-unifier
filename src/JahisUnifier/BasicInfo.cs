using System.Text;

public class BasicInfo
{
    const string VersionPrefix = "JAHISTC";

    public int Version { get; }
    public int? OutputType { get; }
    public string Name { get; }
    public int Sex { get; }
    public string Birthday { get; }

    public string? PostalCode { get; init; }
    public string? Address { get; init; }
    public string? Tel { get; init; }
    public string? EmergencyCall { get; init; }
    public string? Blood { get; init; }
    public string? Weight { get; init; }
    public string? Kana { get; init; }

    //TODO 2,3,4

    public BasicInfo(int version, int? outputType, string name, int sex, string birthday)
    {
        this.Version = version;
        this.OutputType = outputType;
        this.Name = name;
        this.Sex = sex;
        this.Birthday = birthday;
    }

    public static (BasicInfo? info, string? msg, string[] restLines) Parse(string[] lines)
    {
        if (lines.Length < 2) return (null, "必要情報なし", lines);

        var vVals = lines[0].Split(',');
        var vText = vVals.ElementAtOrDefault(0) ?? "";
        if (!vText.StartsWith(VersionPrefix, StringComparison.OrdinalIgnoreCase)
            || !int.TryParse(vText.Substring(VersionPrefix.Length), out int vNum))
        {
            return (null, "バージョン情報不明 " + vText, lines);
        }
        var outTypeVal = vVals.ElementAtOrDefault(1) ?? "";
        int? outType = null;
        if (outTypeVal != "")
        {
            if (int.TryParse(outTypeVal, out int n))
            {
                outType = n;
            }
            else
            {
                return (null, "出力タイプ不明 " + outTypeVal, lines);
            }
        }

        var userLineIdxes = lines.Select((l, i) => new { Idx = i, IsUserLine = l.StartsWith("1,") })
                                .Where(a => a.IsUserLine)
                                .Select(a => a.Idx)
                                .ToArray();
        if (userLineIdxes.Length == 0)
        {
            return (null, "患者基本情報なし", lines);
        }
        if (userLineIdxes.Length != 1)
        {
            return (null, "患者基本情報複数件あり", lines);
        }

        var userLineIndex = userLineIdxes[0];
        var userVals = lines[userLineIndex].Split(',').Skip(1).ToArray();
        var name = userVals.ElementAtOrDefault(0);
        if (string.IsNullOrWhiteSpace(name))
        {
            return (null, "氏名不明 " + name, lines);
        }
        var sexVal = userVals.ElementAtOrDefault(1);
        if (!int.TryParse(sexVal, out int sex))
        {
            return (null, "性別不明 " + sexVal, lines);
        }
        var birthday = userVals.ElementAtOrDefault(2);
        if (string.IsNullOrWhiteSpace(birthday))
        {
            return (null, "生年月日なし", lines);
        }
        var postal = userVals.ElementAtOrDefault(3);
        var address = userVals.ElementAtOrDefault(4);
        var tel = userVals.ElementAtOrDefault(5);
        var emergency = userVals.ElementAtOrDefault(6);
        var blood = userVals.ElementAtOrDefault(7);
        var weight = userVals.ElementAtOrDefault(8);
        var kana = userVals.ElementAtOrDefault(9);

        var info = new BasicInfo(vNum, outType, name, sex, birthday)
        {
            PostalCode = string.IsNullOrWhiteSpace(postal) ? null : postal,
            Address = string.IsNullOrWhiteSpace(address) ? null : address,
            Tel = string.IsNullOrWhiteSpace(tel) ? null : tel,
            EmergencyCall = string.IsNullOrWhiteSpace(emergency) ? null : emergency,
            Blood = string.IsNullOrWhiteSpace(blood) ? null : blood,
            Weight = string.IsNullOrWhiteSpace(weight) ? null : weight,
            Kana = string.IsNullOrWhiteSpace(kana) ? null : kana,
        };

        var restLines = lines.Where((l, i) => i != 0 && i != userLineIndex).ToArray();
        return (info, null, restLines);
    }

    public (BasicInfo? info, string? msg) Merge(BasicInfo another)
    {
        string? failMsg;

        (var v, failMsg) = MergeItem("バージョン", this.Version, another.Version);
        if (failMsg != null) return (null, failMsg);

        (var outType, failMsg) = MergeItem("出力タイプ", this.OutputType, another.OutputType);
        if (failMsg != null) return (null, failMsg);

        (var name, failMsg) = MergeItem("氏名", this.Name, another.Name);
        if (failMsg != null) return (null, failMsg);

        (var sex, failMsg) = MergeItem("性別", this.Sex, another.Sex);
        if (failMsg != null) return (null, failMsg);

        (var birthday, failMsg) = MergeItem("生年月日", this.Birthday, another.Birthday);
        if (failMsg != null) return (null, failMsg);

        (var postal, failMsg) = MergeItem("郵便番号", this.PostalCode, another.PostalCode);
        if (failMsg != null) return (null, failMsg);

        (var address,  failMsg) = MergeItem("住所", this.Address, another.Address);
        if (failMsg != null) return (null, failMsg);

        (var tel, failMsg) = MergeItem("電話番号", this.Tel, another.Tel);
        if (failMsg != null) return (null, failMsg);

        (var emergecy, failMsg) = MergeItem("緊急連絡先", this.EmergencyCall, another.EmergencyCall);
        if (failMsg != null) return (null, failMsg);

        (var blood, failMsg) = MergeItem("血液型", this.Blood, another.Blood);
        if (failMsg != null) return (null, failMsg);

        (var weight, failMsg) = MergeItem("体重", this.Weight, another.Weight);
        if (failMsg != null) return (null, failMsg);

        (var kana, failMsg) = MergeItem("カナ", this.Kana, another.Kana);
        if (failMsg != null) return (null, failMsg);

        var info = new BasicInfo(v, outType, name!, sex, birthday!)
        {
            PostalCode = postal,
            Address = address,
            Tel = tel,
            EmergencyCall = emergecy,
            Blood = blood,
            Weight = weight,
            Kana = kana,
        };
        return (info, null);
    }

    private static (T? val, string? onFail) MergeItem<T>(string itemName, T? val1, T? val2)
    {
        if (val1 == null && val2 == null) return (default, null);
        if (val1 == null) return (val2, null);
        if (val2 == null) return (val1, null);

        if (val1.Equals(val2)) return (val1, null);

        return (default(T), $"{itemName}不一致: {val1} / {val2}");
    }

    public string[] ToCsvRows()
    {
        var ver = VersionPrefix + Version.ToString("D2");
        if (OutputType.HasValue)
        {
            ver += "," + OutputType.Value;
        }

        var userVals = new[]
        {
            "1",
            Name,
            Sex.ToString(),
            Birthday,
            PostalCode ?? "",
            Address ?? "",
            Tel ?? "",
            EmergencyCall ?? "",
            Blood ?? "",
            Weight ?? "",
            Kana ?? "",
        };

        return new[]
        {
            ver,
            string.Join(',', userVals)
        };
    }
}