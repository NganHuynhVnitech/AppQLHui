
$connectionString = "Server=HG-PC\SQLEXPRESS;Database=AppHuiDB;User Id=AccAntiHui;Password=123321;TrustServerCertificate=True;"

# Map of tables and column types
$tableConfigs = @{
    "Users" = @{ 
        Columns = "Id,Username,PasswordHash,FullName,Role,CreatedAt,IsEnabled,BankAccountName,BankAccountNumber,BankName";
        Types = @("int", "string", "string", "string", "int", "date", "bit", "string", "string", "string")
    };
    "Players" = @{
        Columns = "Id,Name,Phone,ZaloName,CreatedAt,BankAccountName,BankAccountNumber,BankName,OwnerId,Notes";
        Types = @("int", "string", "string", "string", "date", "string", "string", "string", "int", "string")
    };
    "Tontines" = @{
        Columns = "Id,Name,Type,BaseAmount,TotalShares,CommissionFee,FeeType,Status,CreatedAt,TotalFeesCollected,CycleType,OwnerId,CycleNotes";
        Types = @("int", "string", "int", "decimal", "int", "decimal", "int", "int", "date", "decimal", "string", "int", "string")
    };
    "TontineShares" = @{
        Columns = "Id,TontineId,Position,PlayerId,Status,WonDrawId,IsSettledEarly";
        Types = @("int", "int", "int", "int", "int", "int", "bit")
    };
    "Draws" = @{
        Columns = "Id,TontineId,SequenceNumber,DrawDate,WinningShareId,BidAmount,CollectedFee,ActualReceived,OldDebtDeduction";
        Types = @("int", "int", "int", "date", "int", "decimal", "decimal", "decimal", "decimal")
    };
    "Transactions" = @{
        Columns = "Id,DrawId,PlayerId,AmountPay,AmountReceive,NetAmount,IsSettled,PaidAmount";
        Types = @("int", "int", "int", "decimal", "decimal", "decimal", "bit", "decimal")
    };
    "ZaloSettings" = @{
        Columns = "Id,SendDelayMs,PasteX,PasteY,PasteShortcut,SendShortcut,IsSingleSendOnly,AutoCopyBill,LastUpdated,OwnerId";
        Types = @("int", "int", "int", "int", "string", "string", "bit", "bit", "date", "int")
    }
}

$tableOrder = @("Users", "Players", "Tontines", "TontineShares", "Draws", "Transactions", "ZaloSettings")

$outputFile = "h:\Code\AppHuiWeb\AppQLHui\AppQLHui\migrate_data.sql"
"USE [xang4739_HuiDB];`nGO`n" | Out-File $outputFile -Encoding utf8

foreach ($tableName in $tableOrder) {
    $config = $tableConfigs[$tableName]
    $columns = $config.Columns
    $types = $config.Types
    
    "PRINT 'Migrating $tableName...';" | Out-File $outputFile -Append -Encoding utf8
    "SET IDENTITY_INSERT [$tableName] ON;" | Out-File $outputFile -Append -Encoding utf8
    
    $query = "SELECT $columns FROM $tableName"
    $data = sqlcmd -S HG-PC\SQLEXPRESS -d AppHuiDB -U AccAntiHui -P 123321 -Q "SET NOCOUNT ON; $query" -W -s "|"
    
    $lines = $data | Select-Object -Skip 2
    
    foreach ($line in $lines) {
        if ([string]::IsNullOrWhiteSpace($line)) { continue }
        $vals = $line.Split("|")
        $formattedVals = New-Object System.Collections.Generic.List[string]
        
        for ($i = 0; $i -lt $vals.Length; $i++) {
            $v = $vals[$i].Trim()
            $type = $types[$i]
            
            if ($v -eq "NULL") {
                $formattedVals.Add("NULL")
            } elseif ($type -eq "string" -or $type -eq "date") {
                $formattedVals.Add("N'" + $v.Replace("'", "''") + "'")
            } elseif ($type -eq "bit") {
                if ($v -eq "True" -or $v -eq "1") { $formattedVals.Add("1") } else { $formattedVals.Add("0") }
            } else {
                # int, decimal
                if ($v -eq "") { $formattedVals.Add("NULL") } else { $formattedVals.Add($v) }
            }
        }
        
        $valString = $formattedVals -join ","
        "INSERT INTO [$tableName] ($columns) VALUES ($valString);" | Out-File $outputFile -Append -Encoding utf8
    }
    
    "SET IDENTITY_INSERT [$tableName] OFF;" | Out-File $outputFile -Append -Encoding utf8
    "GO`n" | Out-File $outputFile -Append -Encoding utf8
}
