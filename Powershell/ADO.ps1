Function Execute-SQL {
    Param(
        [string]$sconstring
    )
    Process
    {
        $scon = New-Object System.Data.SqlClient.SqlConnection
        $scon.ConnectionString = "$sconstring"
    
        $cmd = New-Object System.Data.SqlClient.SqlCommand
        $cmd.Connection = $scon
        $cmd.CommandText = "CREATE TABLE HelloWorld (ID INT)"
    
        $scon.Open()
        $cmd.ExecuteNonQuery()
    }
}

Execute-SQL -sconstring "Data Source=.\SQLEXPRESS;Initial Catalog=AdventureWorks2016;Integrated Security=true"