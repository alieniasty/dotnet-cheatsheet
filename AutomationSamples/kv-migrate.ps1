$secretNames = (Get-AzKeyVaultSecret -VaultName $sourceVaultName).Name

$secretNames.foreach{
    Set-AzKeyVaultSecret -VaultName $destVaultName -Name $_ `
        -SecretValue (Get-AzKeyVaultSecret -VaultName $sourceVaultName -Name $_).SecretValue
}
