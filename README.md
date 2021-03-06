# JAHIS Okusuri Unifier

## これは何

[JAHIS電子版お薬手帳データフォーマット](https://www.jahis.jp/standard/detail/id=715) の仕様で、1調剤1ファイルとして出力されたCSVファイルを、1つのファイルにまとめるためのスクリプトです。

## 何のために

お薬手帳のアプリを乗り換えようとしたときに、移行元が1調剤1ファイルとして診療回数分のファイルをエクスポートしてきて、移行先では1ファイルずつしかインポートできなくて発狂したため。

## 動作環境

.NET 6

## 使い方

ビルドして出力された JahisUnifier.exe に、実行時引数として、結合対象の全ファイルを指定する。  
（exeファイルに結合対象ファイルをまとめてドラッグ＆ドロップするのが楽です）

## JAHIS電子版お薬手帳データフォーマットについて

アプリよりエクスポートされたファイルをテキストエディタで開き、1行目に `JAHISTC07,1` のような文字列があれば、JAHISフォーマットです。

`5,` で始まる行から下が、1回の調剤分の情報になります。  
ファイル内に `5,` で始まる行が複数ある場合、すでに複数回数分の調剤データが含まれているファイルということになります。

フォーマット詳細は [こちら](https://www.jahis.jp/standard/detail/id=715) よりダウンロードできるPDFを参照してください。

## 注意事項

このスクリプトは自分の利用ケースを満たす目的でのみ作成してあるため、いろいろ考慮の足りない部分があると思います。  
あくまで自己責任で利用してください。

* `2,` , `3,` , `4,` で始まる行の取り込みは未対応です
* JAHISフォーマットにはバージョン違いがあり、本スクリプトは Ver.2.4（JAHISTC07）に基づき作成していますが、これ以前のバージョンで動作することを全て検証したわけではありません
* 結合対象のファイルに、JAHISバージョンや患者情報（氏名・性別・生年月日等）の異なるファイルが混在している場合、実行が失敗します
