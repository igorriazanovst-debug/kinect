$repoPath = "C:\Temp\2026\kinect\kinect"
$projectPath = "$repoPath\My project"

Set-Location $repoPath

# Проверяем .gitignore
$gitignorePath = "$repoPath\.gitignore"
if (-not (Test-Path $gitignorePath)) {
    $gitignore = @'
# Unity generated
[Ll]ibrary/
[Tt]emp/
[Oo]bj/
[Bb]uild/
[Bb]uilds/
[Ll]ogs/
[Uu]ser[Ss]ettings/
*.pidb
*.suo
*.user
*.userprefs
*.unityproj
*.booproj
*.svd
*.pdb
*.opendb
*.VC.db
*.pidb.meta
*.pdb.meta
*.mdb.meta

# Unity3D generated meta files
*.meta

# Unity3D Generated File On Crash Reports
sysinfo.txt

# Builds
*.apk
*.aab
*.unitypackage
*.app

# Crashlytics generated file
crashlytics-build.properties

# Packed Addressables
/[Aa]ssets/[Aa]ddressable[Aa]ssets[Dd]ata/*/*.bin*

# Temporary auto-generated Android Assets
/[Aa]ssets/[Ss]treamingAssets/aa.meta
/[Aa]ssets/[Ss]treamingAssets/aa/*
'@
    Set-Content -Path $gitignorePath -Value $gitignore -Encoding UTF8
    Write-Host ".gitignore создан"
}

git add .
git status
git commit -m "Add scripts: MainMenuUI, Views, GameScenes, BuildSettings editor"
git push origin main

Write-Host "Done."
