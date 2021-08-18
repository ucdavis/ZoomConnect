# Calls url passed as first argument.
# Passes CmdKey in header.
# Write a header plus the results to output.
#
$dt = Get-Date -Format "ddd yyyy-MM-dd hh:mm:ss tt"
Write-Host ""
Write-Host "==========================================="
Write-Host $dt
Write-Host "==========================================="

add-type @"
using System.Net;
using System.Security.Cryptography.X509Certificates;
public class TrustAllCertsPolicy : ICertificatePolicy {
    public bool CheckValidationResult(
        ServicePoint srvPoint, X509Certificate certificate,
        WebRequest request, int certificateProblem) {
        return true;
    }
}
"@
[System.Net.ServicePointManager]::CertificatePolicy = New-Object TrustAllCertsPolicy

$header = @{"X-Cmd-Key"="your_command_key_here"};
$result = Invoke-WebRequest -TimeoutSec 600 -Uri $args[0] -Method POST -Headers $header;
Write-Host $result;
