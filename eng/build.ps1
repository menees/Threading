param(
    [bool] $build = $true,
    [bool] $test = $false,
    [string[]] $configurations = @('Debug', 'Release'),
	[bool] $publish = $false,
    [string] $msBuildVerbosity = 'minimal',
    [string] $nugetApiKey = $null
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$scriptPath = [IO.Path]::GetDirectoryName($MyInvocation.MyCommand.Definition)
$repoPath = Resolve-Path (Join-Path $scriptPath '..')
$slnPath = @(Get-ChildItem -Path $repoPath -Filter '*.sln')
if (!$slnPath)
{
	throw "Solution not found at $repoPath."
}

$slnPath = $slnPath[0].FullName
$productName = [IO.Path]::GetFileNameWithoutExtension($slnPath)

function GetXmlPropertyValue($fileName, $propertyName)
{
	$result = Get-Content $fileName |`
		Where-Object {$_ -like "*<$propertyName>*</$propertyName>*"} |`
		ForEach-Object {$_.Replace("<$propertyName>", '').Replace("</$propertyName>", '').Trim()}
	return $result
}

if ($build)
{
    foreach ($configuration in $configurations)
    {
        # Restore NuGet packages first
        dotnet restore $slnPath /p:Configuration=$configuration /v:$msBuildVerbosity /nologo
        dotnet build $slnPath /p:Configuration=$configuration /v:$msBuildVerbosity /nologo
    }
}

if ($test)
{
    foreach ($configuration in $configurations)
    {
    	dotnet test $slnPath /p:Configuration=$configuration
    }
}

if ($publish)
{
	$buildPropsFile = "$repoPath\Directory.Build.props"
	$version = GetXmlPropertyValue "$repoPath\Directory.Version.props" 'Version'

    $published = $false
    if ($version)
    {
        $artifactsPath = "$repoPath\artifacts"
		if (Test-Path $artifactsPath)
		{
			Remove-Item -Recurse -Force $artifactsPath
		}

		$ignore = mkdir $artifactsPath
		if ($ignore) { } # For PSUseDeclaredVarsMoreThanAssignments

        if ($configurations -contains 'Release')
        {
            $configuration = 'Release'

            Write-Host "Publishing version $version $configuration packages to $artifactsPath"
            $packages = @(Get-ChildItem -r "$repoPath\src\**\*.$version.nupkg" | Where-Object {$_.Directory -like "*\bin\$configuration"})
            foreach ($package in $packages)
            {
                Write-Host "Copying $package"
                Copy-Item -Path $package -Destination $artifactsPath -Force

                if ($nugetApiKey)
                {
                    $artifactPackage = Join-Path $artifactsPath (Split-Path -Leaf $package)
                    dotnet nuget push $artifactPackage -k $nugetApiKey -s https://api.nuget.org/v3/index.json --skip-duplicate
                    $published = $true
                }
            }
        }
    }

    if ($published)
    {
		Write-Host "`n`n****** REMEMBER TO ADD A GITHUB RELEASE! ******"
    }
}
