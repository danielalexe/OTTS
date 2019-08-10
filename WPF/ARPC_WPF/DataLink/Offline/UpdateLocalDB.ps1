Function pause ($message)
{
    # Check if running Powershell ISE
    if ($psISE)
    {
        Add-Type -AssemblyName System.Windows.Forms
        [System.Windows.Forms.MessageBox]::Show("$message")
    }
    else
    {
        Write-Host "$message" -ForegroundColor Yellow
        $x = $host.ui.RawUI.ReadKey("NoEcho,IncludeKeyDown")
    }
}
Write-Host "This script is intended to update the LocalDB of a windows machine to SQL Server 2017"
pause "Press any key to start"
Write-Host "Stopping MSSQLLocalDB..."
sqllocaldb stop MSSQLLocalDB
Write-Host "MSSQLLocalDB Stopped"
Write-Host "Deleting MSSQLLocalDB..."
sqllocaldb delete MSSQLLocalDB
Write-Host "MSSQLLocalDB Deleted"
Write-Host "Starting SQL Server 2017 LocalDB msi file"
msiexec.exe /I SqlLocalDB.msi
Write-Host "Congratulations your SQL Server LocalDB is now the 2017 version"
pause "Press any key to finish"