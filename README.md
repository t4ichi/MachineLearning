# 手書き数字の認識アプリ
[ニューラルネットワークをゼロから実装する](https://bizboard.nikkeibp.co.jp/kijiken/summary/20180724/NSW0259H_4124854a.html)を参考にC#とUnityを使って手書き数字の認識アプリを作りました。

## 学習済みモデルの生成と正答率の検証
学習の様子
![Learning](https://user-images.githubusercontent.com/67674781/226420976-4d690233-0bef-4a62-a8fd-9abfb3074bde.gif)

50000回学習させた結果、正答率は95.53%でした✨
![Result](https://user-images.githubusercontent.com/67674781/226421764-f3f9401d-3462-4843-8c21-66fddb806f13.png)

## 自分で書いた文字が認識するか試してみる
稀に1を書いても5になることがあったが、だいたい正解してる
![detect](https://user-images.githubusercontent.com/67674781/226564686-c39ed954-02a7-44c6-a517-80529d5e5fa8.gif)

