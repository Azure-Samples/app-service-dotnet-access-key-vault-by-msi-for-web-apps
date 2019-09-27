---
page_type: sample
languages:
- csharp
products:
- azure
extensions:
- services: App-Service
- platforms: dotnet
description: "Azure App Service basic sample for managing web apps."
---

# Getting started on safeguarding Web app secrets in Key Vault using convenience API

 Azure App Service basic sample for managing web apps.
  - Create a Cosmos DB with credentials stored in a Key Vault
  - Create a web app which interacts with the Cosmos DB by first
      reading the secrets from the Key Vault.
      The source code of the web app is located at Asset/documentdb-dotnet-todo-app


## Running this Sample

To run this sample:

Set the environment variable `AZURE_AUTH_LOCATION` with the full path for an auth file. See [how to create an auth file](https://github.com/Azure/azure-libraries-for-net/blob/master/AUTH.md).

    git clone https://github.com/Azure-Samples/app-service-dotnet-access-key-vault-by-msi-for-web-apps.git

    cd app-service-dotnet-access-key-vault-by-msi-for-web-apps

    dotnet build

    bin\Debug\net452\ManageWebAppCosmosDbByMsi.exe

## More information

[Azure Management Libraries for C#](https://github.com/Azure/azure-sdk-for-net/tree/Fluent)
[Azure .Net Developer Center](https://azure.microsoft.com/en-us/develop/net/)
If you don't have a Microsoft Azure subscription you can get a FREE trial account [here](http://go.microsoft.com/fwlink/?LinkId=330212)

---

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.
