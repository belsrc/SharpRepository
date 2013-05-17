# Check each nuspec file to see if we need to set the file dependency to the new build version
# rdv = Repository Dependency Version
param([string]$rdv = "1.3.2")

function Get-Directory([string[]]$path, [string[]]$include, [switch]$recurse) 
{ 
    Get-ChildItem -Path $path -Include $include -Recurse:$recurse | `
         Where-Object { $_.PSIsContainer } 
} 

$version_holder = "%Repository.Dependency.Version%"

Write-Host ""

$directories = Get-Directory $(Get-Location), "SharpRepository.*"

# not sure what is going on here but this is a HACK for sure
$uniqueDirectories = @()
foreach($directory in $directories)
{
	$dir_fullname = $directory.fullname
	
	if (!($uniqueDirectories -contains $dir_fullname) )
	{ 
		$nuspec_path = ($directory.fullname + "\" + $directory + ".nuspec")
		
		Write-Host $nuspec_path
		
		#see if there is a supec file here
		if (Test-Path ($nuspec_path))
		{
			Write-Host "nuspec exists, doing replace"
			((Get-Content $nuspec_path) -creplace $version_holder, $rdv) | Set-Content $nuspec_path
		}
		
		# add the full path to the list of already processed directories, this is because the array for this loop has dups
		$uniqueDirectories += $directory.fullname
	}
}