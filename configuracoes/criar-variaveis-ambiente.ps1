[Environment]::SetEnvironmentVariable("CoreSSOConnection", "Server=10.49.19.159;Database=CoreSSO;User Id=UserCoreSSO;Password=Sgp1234;", "Machine")
[Environment]::SetEnvironmentVariable("ConexaoRedis", "localhost:6379", "Machine")
[Environment]::SetEnvironmentVariable("EolConnection", "Server=db_educacao.rede.sp;Database=se1426;User Id=user_se1426_cotic;Password=3ol_c0tic@;", "Machine")
[Environment]::SetEnvironmentVariable("AEConnection", "User ID=postgres;Password=postgres;Host=localhost;Port=5433;Database=ae_db;Pooling=true", "Machine")