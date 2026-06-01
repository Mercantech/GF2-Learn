$path = Join-Path $PSScriptRoot '..\content\opgaver\begynder\01-variabler.md'
$text = Get-Content -Path $path -Raw -Encoding UTF8
$pattern = '(?ms)(:::exercise[^\r\n]*\r?\n[\s\S]*?\r?\n:::)\r?\n\r?\n```csharp\r?\n([\s\S]*?)\r?\n```\r?\n\r?\n(:::solution)'
$nl = [Environment]::NewLine
$replacement = '${1}' + $nl + $nl + ':::code-playground' + $nl + '```csharp' + $nl + '${2}' + $nl + '```' + $nl + ':::' + $nl + $nl + '${3}'
$newText = [regex]::Replace($text, $pattern, $replacement)
if ($newText -eq $text) { throw 'No matches replaced' }
Set-Content -Path $path -Value $newText -Encoding UTF8 -NoNewline
Write-Host 'Replaced exercise code blocks with code-playground'
