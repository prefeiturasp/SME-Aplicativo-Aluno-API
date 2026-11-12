[CmdletBinding()]
param(
	[ValidateSet('Debug', 'Release')]
	[string]$Configuration = 'Release'
)

$ErrorActionPreference = 'Stop'
$rootPath = $PSScriptRoot
$coveragePath = Join-Path $rootPath 'coverage'
$reportPath = Join-Path $coveragePath 'report'

if (Test-Path $coveragePath) {
	Remove-Item $coveragePath -Recurse -Force
}

New-Item -ItemType Directory -Path $coveragePath -Force | Out-Null

$testProjects = @(Get-ChildItem -Path (Join-Path $rootPath 'teste') -Filter '*.csproj' -Recurse)
if ($testProjects.Count -eq 0) {
	throw 'Nenhum projeto de teste foi encontrado em teste.'
}

foreach ($project in $testProjects) {
	$projectCoveragePath = Join-Path $coveragePath $project.BaseName
	New-Item -ItemType Directory -Path $projectCoveragePath -Force | Out-Null
	$coverletOutput = "$projectCoveragePath$([IO.Path]::DirectorySeparatorChar)"

	Write-Host "Executando testes e cobertura: $($project.BaseName)"
	dotnet test $project.FullName `
		--configuration $Configuration `
		/p:CollectCoverage=true `
		/p:CoverletOutputFormat=opencover `
		"/p:CoverletOutput=$coverletOutput"
	if ($LASTEXITCODE -ne 0) {
		throw "Os testes falharam em $($project.BaseName)."
	}
}

Write-Host 'Gerando relatório HTML consolidado.'
dotnet tool run reportgenerator `
	"-reports:$coveragePath\**\coverage.opencover.xml" `
	"-targetdir:$reportPath" `
	'-reporttypes:Html'
if ($LASTEXITCODE -ne 0) {
	throw 'O ReportGenerator falhou ao gerar o relatório.'
}

Write-Host "Relatório disponível em: $reportPath\index.html"
