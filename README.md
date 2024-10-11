# Setup

To run this function, you will need a `.\src\Defra.Trade.Events.EHCO.GCSubscriber\local.settings.json` file with the following structure:

```jsonc 
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet",
    "ServiceBus:ConnectionString": "<secret>",
    "ConfigurationServer:ConnectionString": "<secret>",
    "ConfigurationServer:TenantId": "<secret>"
  }
}
```

Secrets can be found [here](https://dev.azure.com/defragovuk/DEFRA-TRADE-APIS/_wiki/wikis/DEFRA-TRADE-APIS.wiki/26086))