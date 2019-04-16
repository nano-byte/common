Param ([Parameter(Mandatory=$True)][String]$User, [Parameter(Mandatory=$True)][String]$Password)
$ErrorActionPreference = "Stop"

function put ($relativeUri, $filePath) {
    0install run http://repo.roscidus.com/utils/curl -k -L --user "${User}:${Password}" -i -X PUT -F "file=@$filePath" "https://www.transifex.com/api/2/project/0install-win/$relativeUri"
}

function upload($slug, $pathBase) {
    put "resource/$slug/content/" "$pathBase.resx"
    put "resource/$slug/translation/de/" "$pathBase.de.resx"
}

Add-Type -AssemblyName System.Windows.Forms
function upload_filtered($slug, $pathBase) {
    $filteredKeys = New-Object Collections.Generic.List[string]
    $reader = New-Object Resources.ResXResourceReader -ArgumentList "$pathBase.de.resx"
    $dict = $reader.GetEnumerator();
    while ($dict.MoveNext()) {
        $filteredKeys.Add($dict.Key.ToString());
    }
    $reader.Dispose()

    $reader = New-Object Resources.ResXResourceReader -ArgumentList "$pathBase.resx"
    $writer = New-Object Resources.ResXResourceWriter -ArgumentList "$pathBase.filtered.resx"
    $dict = $reader.GetEnumerator()
    while ($dict.MoveNext()) {
        $key = $dict.Key.ToString();
        if ($filteredKeys.Contains($key)) {
            $writer.AddResource($key, $dict.Value)
        }
    }
    $writer.Dispose()
    $reader.Dispose()

    put "resource/$slug/content/" "$pathBase.filtered.resx"
    put "resource/$slug/translation/de/" "$pathBase.de.resx"
}

upload common "$PSScriptRoot\src\Common\Properties\Resources"

upload_filtered window-common_winforms_errorreportform "$PSScriptRoot\src\Common.WinForms\Controls\ErrorReportForm"
