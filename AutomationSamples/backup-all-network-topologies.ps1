param (    
  [Parameter(Mandatory = $true)][string]$ApplicationId,
  [Parameter(Mandatory = $true)][string]$TenantId,
  [Parameter(Mandatory = $true)][string]$ClientSecret,
  [Parameter(Mandatory = $true)][string]$JobName,
  [Parameter(Mandatory = $true)][string]$BuildNumber
)

Set-PSRepository -Name "PSGallery" -InstallationPolicy Trusted

Install-Module -Name Az.Network
Install-Module -Name Az.Resources
Install-Module -Name Az.Accounts
Install-Module -Name Az.Storage

$Credential = New-Object -TypeName System.Management.Automation.PSCredential -ArgumentList $ApplicationId, ($ClientSecret | ConvertTo-SecureString -AsPlainText -Force)
Connect-AzAccount -ServicePrincipal -TenantId $TenantId -Credential $Credential

$mgOne = Get-AzManagementGroup -GroupName "mg-one" -Expand
$mgTwo = Get-AzManagementGroup -GroupName "mg-two" -Expand
$mgThree = Get-AzManagementGroup -GroupName "mg-three" -Expand

$managementGroups = @($mgOne, $mgTwo, $mgThree)
$subscriptions = $managementGroups.Children

$networkWatchers = @()
$topologies = @()

# Iterate over all subscriptions with Network Watchers and generate topologies
foreach ($subscription in $subscriptions) {
  Set-AzContext -SubscriptionId $subscription.Name

  $targetResourceGroups = Get-AzResourceGroup

  if ((-not $targetResourceGroups) -or ($targetResourceGroups.ResourceGroupName -notcontains "NetworkWatcherRG") ) {
    continue
  }

  $networkWatchers += Get-AzNetworkWatcher -Name NetworkWatcher*

  foreach ($networkWatcher in $networkWatchers) {
    foreach ($targetResourceGroup in $targetResourceGroups) {
      $topologies += Get-AzNetworkWatcherTopology -NetworkWatcher $networkWatcher -TargetResourceGroupName $targetResourceGroup.ResourceGroupName
    }
  }
}

$topologies.Resources | ConvertTo-Json -Depth 9 | Out-File -FilePath "topologies.json"

# Upload generated json topologies to a specific storage account

Set-AzContext -SubscriptionId "SUB ID FOR SA"
$key = (Get-AzStorageAccountKey -ResourceGroupName "RG FOR SA" -Name "YOUR SA NAME")[0].Value
$context = New-AzStorageContext -StorageAccountName $SaName -StorageAccountKey $key
$containerName = 'network-topology-backups'

$storageContainer = Get-AzStorageContainer -Name $containerName -Context $context

if (-not $storageContainer) {
  New-AzStorageContainer -Name $containerName -Context $context -Permission Off
}

$blob = @{
  File             = "topologies.json"
  Container        = $containerName
  Blob             = "${JobName}/${BuildNumber}/topologies.json"
  Context          = $context
  StandardBlobTier = "Hot"
}
Set-AzStorageBlobContent @blob
Remove-Item -Path "topologies.json" -Verbose
