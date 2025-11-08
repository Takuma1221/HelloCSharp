# VS Code テーマと外観のおすすめ

白背景が好みの方向けに、視認性の高いライトテーマを中心に提案します。

## ライトテーマ候補
- GitHub Light / Light Default (拡張: `github.github-vscode-theme`)
- Light+ (デフォルト内蔵)
- Quiet Light (デフォルト内蔵)
- Solarized Light (マーケットプレイス)

## アイコンテーマ
- Material Icon Theme (`pkief.material-icon-theme`)

## 追加の体験向上
- IntelliCode (`visualstudioexptteam.vscodeintellicode`) — コード補完の精度向上

## テーマの切り替え方
1. コマンドパレット: "Preferences: Color Theme"
2. 一覧から好みのライトテーマを選択
3. アイコンテーマは "File Icon Theme" から `Material Icon Theme` を選択

## フォント/可読性のヒント
- エディタのフォントサイズを +1〜2pt
- エディタ行間 (`editor.lineHeight`) を少し上げる
- タブ幅を 2 or 4 に統一し、EditorConfigで固定

## 推奨設定例 (settings.json)
```jsonc
{
  "workbench.colorTheme": "GitHub Light Default",
  "workbench.iconTheme": "material-icon-theme",
  "editor.fontSize": 14,
  "editor.lineHeight": 22,
  "editor.renderWhitespace": "selection",
  "files.trimTrailingWhitespace": true
}
```

---
ライトテーマは屋内/屋外の輝度で印象が変わります。状況に応じてコントラストの高いテーマを使い分けてください。
