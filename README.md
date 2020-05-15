---
page_type: sample
languages:
  - csharp
products:
  - azure
  - dotnet
  - azure-app-service
  - azure-key-vault
  - azure-cosmosDB
description: "This sample show how to use Azure Key Vault store Cosmos DB credential, and then create a web app interacts with the Cosmos DB."
urlFragment: app-service-dotnet-access-key-vault-by-msi-for-web-apps
---

# Getting started on safeguarding Web app secrets in Key Vault using convenience API #

 Azure App Service basic sample for managing web apps.
  - Create a Cosmos DB with credentials stored in a Key Vault
  - Create a web app which interacts with the Cosmos DB by first
      reading the secrets from the Key Vault.
      The source code of the web app is located at Asset/documentdb-dotnet-todo-app

## Prerequisites

To complete this tutorial:

* Install .NET Core 2.0 version for [Linux] or [Windows]

If you don't have an Azure subscription, create a [free account] before you begin.

### Create an auth file

This project requires a auth file be stored in an environment variable securely on the machine running the sample. You can generate this file using Azure CLI 2.0 through the following command. Make sure you selected your subscription by az account set --subscription <name or id> and you have the privileges to create service principals.

```azure-cli
az ad sp create-for-rbac --sdk-auth > my.azureauth
```

### Set the auth file path to an environment variable

Follow one of the examples below depending on your operating system to create the environment variable. If using Windows close your open IDE or shell and restart it to be able to read the environment variable.

```bash
export AZURE_AUTH_LOCATION="<YourAuthFilePath>"
```

Windows

```cmd
setx AZURE_AUTH_LOCATION "<YourAuthFilePath>"
```

## Run the application
First, clone the repository on your machine:

```bash
git clone https://github.com/Azure-Samples/app-service-dotnet-access-key-vault-by-msi-for-web-apps.git
```

Then, switch to the appropriate folder:
```bash
cd app-service-dotnet-access-key-vault-by-msi-for-web-apps
```

Finally, run the application with the `dotnet run` command.

```console
dotnet run

## More information ##

[Azure Management Libraries for C#](https://github.com/Azure/azure-sdk-for-net/tree/Fluent)
[Azure .Net Developer Center](https://azure.microsoft.com/en-us/develop/net/)

---

This project has adopted the [Microsoft Open Source Code of Conduct]. For more information see the [Code of Conduct FAQ] or contact [opencode@microsoft.com] with any additional questions or comments.

<!-- LINKS -->
[Linux]: https://dotnet.microsoft.com/download
[Windows]: https://dotnet.microsoft.com/download
[free account]: https://azure.microsoft.com/free/?WT.mc_id=A261C142F
[Microsoft Open Source Code of Conduct]: https://opensource.microsoft.com/codeofconduct/
[Code of Conduct FAQ]: https://opensource.microsoft.com/codeofconduct/faq/
[opencode@microsoft.com]: mailto:opencode@microsoft.com
