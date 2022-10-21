# ref 1: https://stackoverflow.com/questions/53771022/how-to-create-and-install-x-509-self-signed-certificates-in-windows-10-without-u
# ref 2: https://www.cloudnotes.io/x509certificate-is-immutable-on-this-platform-use-the-equivalent-constructor-instead/
#
# This script will create and install two certificates:
#     1. `MyCA.cer`: A self-signed root authority certificate. 
#     2. `MySPC.cer`: The cerificate to sign code in 
#         a development environment (signed with `MyCA.cer`).
# 
# No user interaction is needed (unattended). 
# Powershell 4.0 or higher is required.
#

if (!([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator")) { 
    $p = Start-Process powershell.exe "-NoProfile -ExecutionPolicy Bypass -File `"$PSCommandPath`"" -Verb RunAs  -Wait; 
    exit 
}

# Define the expiration date for certificates.
$notAfter = (Get-Date).AddYears(10)

# Create a self-signed root Certificate Authority (CA).
$rootCert = New-SelfSignedCertificate -KeyExportPolicy Exportable -CertStoreLocation Cert:\CurrentUser\My -DnsName "TruthShield" -NotAfter $notAfter -TextExtension @("2.5.29.37={text}1.3.6.1.5.5.7.3.3", "2.5.29.19={text}CA=1") -KeyusageProperty All -KeyUsage DataEncipherment

# Export the CA private key.
[System.Security.SecureString] $password = ConvertTo-SecureString -String "password*8" -Force -AsPlainText
[String] $rootCertPath = Join-Path -Path cert:\CurrentUser\My\ -ChildPath "$($rootcert.Thumbprint)"
Export-PfxCertificate -Cert $rootCertPath -FilePath "MyCA.pfx" -Password $password
Export-Certificate -Cert $rootCertPath -FilePath "MyCA.crt"

# Create an end certificate signed by our CA.
$cert = New-SelfSignedCertificate -CertStoreLocation Cert:\LocalMachine\My -DnsName "TruthShield" -NotAfter $notAfter -Signer $rootCert -Type DocumentEncryptionCert -TextExtension @("2.5.29.37={text}1.3.6.1.5.5.7.3.3", "2.5.29.19={text}CA=0&pathlength=0")

# Save the signed certificate with private key into a PFX file and just the public key into a CRT file.
[String] $certPath = Join-Path -Path cert:\LocalMachine\My\ -ChildPath "$($cert.Thumbprint)"
Export-PfxCertificate -Cert $certPath -FilePath "MySPC.pfx" -Password $password
Export-Certificate -Cert $certPath -FilePath "MySPC.crt"

# Add MyCA certificate to the Trusted Root Certification Authorities.
$pfx = New-Object -TypeName System.Security.Cryptography.X509Certificates.X509Certificate2("$PSScriptRoot\MyCA.pfx", $password, "Exportable,PersistKeySet")

$store = new-object System.Security.Cryptography.X509Certificates.X509Store(
    [System.Security.Cryptography.X509Certificates.StoreName]::Root,
    "localmachine"
)
$store.open("MaxAllowed")
$store.add($pfx)
$store.close()

# Remove MyCA from CurrentUser to avoid issues when signing with "signtool.exe /a ..."
Remove-Item -Force "cert:\CurrentUser\My\$($rootCert.Thumbprint)"

# Import certificate.
Import-PfxCertificate -FilePath MySPC.pfx cert:\CurrentUser\My -Password $password -Exportable

Write-Output "Thumbprint:" + $cert.Thumbprint

# Update Master Config with Thumbprint
$pathToJson = ".\master-config.json"
$a = Get-Content $pathToJson | ConvertFrom-Json
$a.CertificateKey = $cert.Thumbprint
$a | ConvertTo-Json | set-content $pathToJson