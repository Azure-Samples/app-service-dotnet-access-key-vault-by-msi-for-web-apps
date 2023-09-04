// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using Azure.Core;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.AppService;
using Azure.ResourceManager.CosmosDB;
using Azure.ResourceManager.CosmosDB.Models;
using Azure.ResourceManager.KeyVault;
using Azure.ResourceManager.KeyVault.Models;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager.Samples.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace ManageWebAppCosmosDbByMsi
{
    public class Program
    {
        /**
         * Azure App Service basic sample for managing web apps.
         *  - Create a Cosmos DB with credentials stored in a Key Vault
         *  - Create a web app which interacts with the Cosmos DB by first
         *      reading the secrets from the Key Vault.
         *
         *      The source code of the web app is located at Asset/documentdb-dotnet-todo-app
         */

        public static async Task RunSample(ArmClient client)
        {
            AzureLocation region = AzureLocation.EastUS;
            string appName = Utilities.CreateRandomName("app");
            string rgName = Utilities.CreateRandomName("rg1NEMV_");
            string vaultName = Utilities.CreateRandomName("vault");
            string vaultSecretName = Utilities.CreateRandomName("vaultsecret");
            string cosmosName = Utilities.CreateRandomName("cosmosdb");
            string appUrl = appName + ".azurewebsites.net";
            var lro =await client.GetDefaultSubscription().GetResourceGroups().CreateOrUpdateAsync(Azure.WaitUntil.Completed, rgName, new ResourceGroupData(AzureLocation.EastUS));
            var resourceGroup = lro.Value;
            try
            {
                //============================================================
                // Create a CosmosDB

                Utilities.Log("Creating a CosmosDB...");
                //var lro = client.GetDefaultSubscription().GetResourceGroups().CreateOrUpdate(Azure.WaitUntil.Completed, rgName, new ResourceGroupData(AzureLocation.EastUS));
                //var resourceGroup = lro.Value;
                CosmosDBAccountCollection cosmosDBAccountCollection = resourceGroup.GetCosmosDBAccounts();
                IEnumerable<CosmosDBAccountLocation> list= new List<CosmosDBAccountLocation> { new CosmosDBAccountLocation() };
                var cosmosDBData = new CosmosDBAccountCreateOrUpdateContent(region, list)
                {
                    Kind = CosmosDBAccountKind.GlobalDocumentDB,
                };
                var cosmosResource_lro = cosmosDBAccountCollection.CreateOrUpdate(Azure.WaitUntil.Completed, cosmosName, cosmosDBData);
                var cosmosDBAccount = cosmosResource_lro.Value;

                Utilities.Log("Created CosmosDB");
                Utilities.Log(cosmosDBAccount);

                //============================================================
                // Create a key vault

                var keyVaultCollection = resourceGroup.GetKeyVaults();
                var keyVaultData = new KeyVaultCreateOrUpdateContent(region, new KeyVaultProperties(new Guid("72f988bf-86f1-41af-91ab-2d7cd011db47"), new KeyVaultSku(KeyVaultSkuFamily.A, KeyVaultSkuName.Standard)) { } )
                {
                };
                var keyVault_lro = keyVaultCollection.CreateOrUpdate(Azure.WaitUntil.Completed, cosmosName, keyVaultData);
                var keyVault = keyVault_lro.Value;
                Thread.Sleep(10000);

                //============================================================
                // Store Cosmos DB credentials in Key Vault

                var keyvaultSecretCollection = keyVault.GetKeyVaultSecrets();
                var secretData1 = new KeyVaultSecretCreateOrUpdateContent(new SecretProperties()
                {
                    Value = cosmosDBAccount.Data.DocumentEndpoint
                });
                {
                };
                var secret1_lro = keyvaultSecretCollection.CreateOrUpdate(Azure.WaitUntil.Completed, "azure-documentdb-uri", secretData1);
                var secretData2 = new KeyVaultSecretCreateOrUpdateContent(new SecretProperties()
                {
                    Value = "tododb"
                });
                {
                };
                var secret2_lro = keyvaultSecretCollection.CreateOrUpdate(Azure.WaitUntil.Completed, "azure-documentdb-key", secretData2);
                var secretData3 = new KeyVaultSecretCreateOrUpdateContent(new SecretProperties()
                {
                    Value = cosmosDBAccount.GetKeys().Value.PrimaryMasterKey
                });
                {
                };
                var secret3_lro = keyvaultSecretCollection.CreateOrUpdate(Azure.WaitUntil.Completed, "azure-documentdb-key", secretData2);
                var secret1 = secret1_lro.Value;
                var secret2 = secret2_lro.Value;
                var secret3 = secret3_lro.Value;
                //============================================================
                // Create a web app with a new app service plan

                Utilities.Log("Creating web app " + appName + " in resource group " + rgName + "...");

                var webSiteCollection = resourceGroup.GetWebSites();
                var webSiteData = new WebSiteData(region)
                {
                   SiteConfig = new Azure.ResourceManager.AppService.Models.SiteConfigProperties()
                   {
                       WindowsFxVersion = "PricingTier.StandardS1",
                       NetFrameworkVersion = "NetFrameworkVersion.V4_6",
                   }
                };
                var webSite_lro = webSiteCollection.CreateOrUpdate(Azure.WaitUntil.Completed, appName, webSiteData);
                var webSite = webSite_lro.Value;

                Utilities.Log("Created web app " + webSite.Data.Name);
                Utilities.Log(webSite);

                //============================================================
                // Update vault to allow the web app to access

                keyVault.Update(new KeyVaultPatch()
                {
                    Properties = new KeyVaultPatchProperties()
                    {
                        AccessPolicies =
                        {
                            new KeyVaultAccessPolicy(new Guid(""), "webSite.Data.Properties.SystemAssignedManagedServiceIdentityPrincipalId" ,new IdentityAccessPermissions())
                        }
                    }
                });

                //============================================================
                // Deploy to web app through local Git

                Utilities.Log("Deploying a local asp.net application to " + appName + " through Git...");

                var profile = webSite.Data.HostingEnvironmentProfile;
                //Utilities.DeployByGit(profile, "documentdb-dotnet-todo-app");
                var extension = webSite.GetSiteExtension();
                var deploy = await extension.CreateOrUpdateAsync(Azure.WaitUntil.Completed, new Azure.ResourceManager.AppService.Models.WebAppMSDeploy());

                Utilities.Log("Deployment to web app " + webSite.Data.Name + " completed");
                Utilities.Print(webSite);

                // warm up
                Utilities.Log("Warming up " + appUrl + "...");
                Utilities.CheckAddress("http://" + appUrl);
                Thread.Sleep(5000);
                Utilities.Log("CURLing " + appUrl + "...");
                Utilities.Log(Utilities.CheckAddress("http://" + appUrl));
            }
            finally
            {
                try
                {
                    Utilities.Log("Deleting Resource Group: " + rgName);
                    resourceGroup.Delete(Azure.WaitUntil.Completed);
                    Utilities.Log("Deleted Resource Group: " + rgName);
                }
                catch (NullReferenceException)
                {
                    Utilities.Log("Did not create any resources in Azure. No clean up is necessary");
                }
                catch (Exception g)
                {
                    Utilities.Log(g);
                }
            }
        }

        public static async Task Main(string[] args)
        {
            try
            {
                //=================================================================
                // Authenticate
                var clientId = Environment.GetEnvironmentVariable("CLIENT_ID");
                var clientSecret = Environment.GetEnvironmentVariable("CLIENT_SECRET");
                var tenantId = Environment.GetEnvironmentVariable("TENANT_ID");
                var subscription = Environment.GetEnvironmentVariable("SUBSCRIPTION_ID");
                ClientSecretCredential credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
                ArmClient client = new ArmClient(credential, subscription);

                // Print selected subscription
                Utilities.Log("Selected subscription: " + client.GetSubscriptions().Id);

                await RunSample(client);
            }
            catch (Exception e)
            {
                Utilities.Log(e);
            }
        }
    }
}