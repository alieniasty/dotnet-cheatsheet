param(
    [string]$resourceGroup,
    [string]$subscriptionId,
    [string]$logicAppName
)

#Use this script with "Azure Powershell" task in ADO by passing it already created service principal (assuming you can't create your own) to query Azure resources

try {  
    Write-Host "Script started"

    Write-Host "ResourceGroup: ${resourceGroup}"
    Write-Host "SubscriptionId ${subscriptionId}"
    Write-Host "LogicAppName ${logicAppName}"
    
    Write-Host "Fetching callback url ..."

    $response = Invoke-AzRestMethod -Path "/subscriptions/${subscriptionId}/resourceGroups/${resourceGroup}/providers/Microsoft.Logic/workflows/${logicAppName}/triggers/manual/listCallbackUrl?api-version=2016-10-01" -Method POST
    $jsonResponse = $response.Content | ConvertFrom-Json
    $callbackUrl = $jsonResponse.Value

    Write-Host $callbackUrl

    Write-Host "Done" 
}
catch {
    Write-Host "An error occurred:" 
    Write-Error $_
    return
}