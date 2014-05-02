param($installPath, $toolsPath, $package, $project)

# Set "extern alias" (http://msdn.microsoft.com/en-us/library/ms173212.aspx) for LinqBridge to prevent name conflicts with System.Core
$project.Object.References.Find("LinqBridge").Aliases = "LinqBridge"
