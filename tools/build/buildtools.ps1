 function Set-VsixVersion {
    [cmdletbinding()]
    param (
        [Parameter(Mandatory=1)]
        [string]$vsixmanifestPath,

        [Parameter(Mandatory=1)]
        [string]$version
    )
    process {
        Write-Host "Setting version in:" $vsixmanifestPath "to" $version "..." -NoNewline

        [xml]$vsixmanifestXml = Get-Content $vsixmanifestPath
        
        $namespaceManager = New-Object System.Xml.XmlNamespaceManager $vsixmanifestXml.NameTable
        $namespaceManager.AddNamespace("ns", $vsixmanifestXml.DocumentElement.NamespaceURI) | Out-Null

        $versionAttribute = $vsixmanifestXml.SelectSingleNode("//ns:Identity", $namespaceManager).Attributes["Version"]
        $versionAttribute.Value = $version

        $vsixmanifestXml.Save($vsixmanifestPath)

        Write-Host " Done."
    }
}