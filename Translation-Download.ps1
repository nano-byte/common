Param ([Parameter(Mandatory=$True)][String]$User, [Parameter(Mandatory=$True)][String]$Password)
$ErrorActionPreference = "Stop"

function get ($relativeUri, $filePath) {
    0install run http://repo.roscidus.com/utils/curl -k -L --user "${User}:${Password}" -o $filePath https://www.transifex.com/api/2/project/0install-win/$relativeUri
}

function download($slug, $pathBase) {
    get "resource/$slug/translation/el/?file" "$pathBase.el.resx"
    get "resource/$slug/translation/tr/?file" "$pathBase.tr.resx"
}

download common "$PSScriptRoot\src\Common\Properties\Resources"

download window-common_winforms_errorreportform "$PSScriptRoot\src\Common.WinForms\Controls\ErrorReportForm"
