# azure-blob-azure-tables-dot-net-core

## SetUp

1. Install Azurite, preferable npm: `npm install -g azurite`.
2. Run Azurite, i.e. `azurite npm`.
3. Setup `RandomMessageApp.FunctionApp` as startup project in VS, run FunctionApp, wait on new logs. Logs should be generated every minute.
4. Run `RandomMessageApp.WebApi`, open swagger, run request fetch logs.
5. Request individual log with message id.