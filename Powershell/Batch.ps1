Function Get-Person {
        $ps = Invoke-RestMethod -Method 'Get' -Uri "http://localhost:51092/api/personne/?ville=tout" 
        $ps
}
Function Test-Connection {
    $values = Invoke-RestMethod -Method 'Get' -Uri "http://localhost:51092/api/personne" 
    if ($values[0] -eq "Value1" -and $values[1] -eq "Value2")
    {
        Write-Host "Ok";
    }
    else 
    {
        Write-Host "Erreur";
    }
}
