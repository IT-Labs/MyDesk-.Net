# MyDesk - API

## Table of contents
* [General info](#introduction)
* [Technologies](#technologies)
* [Setup locally](#steps-for-running-the-application-locally)
* [Setup on Azure](#setting-up-the-application-on-azure)
* [Setting up multitenancy](#setting-up-multitenancy)
* [How to contribute](#how-to-contribute-to-the-project)

## Introduction

This application is back-end part of MyDesk project that enables administrators to model desired offices' and desks' layout and it allows users to reserve those desks/conference rooms for a certain time period. The front-end part of this project can be found on this [link](https://github.com/IT-Labs/MyDesk-UI).

## Technologies

This is .NET web API project that uses Entity Framework 6 to communicate with Azure SQL DB. Database will be automatically created, by executing EF migrations, upon application's first launch. Repository contains Azure pipeline scripts that can be used for setting up CI/CD on Azure. The application uses JWT tokens for authentication/authorization. If application cannot be deployed on Azure, the SQL Server RDBMS should be used and application will use self-issued JWT tokens for authentication/authorization.

## Steps for running the application locally

* Install SQL server locally (express version will do)
* Clone repository from Git
* Open the solution in Visual Studio 2022
* In **appsettings.json** file add following properties:
  - **ConnectionString:** Connection string for your database
  - **AdminEmail:** Email address for the initial admin account created during application startup
  - **AdminPassword:** Password for the initial admin account created during application startup
  - **SentimentEndpoint (optional):** URL of the external API that will determine sentiment of the reservation's review
  - **TenantClaimKey:** Name of custom property in JWT token that will contain name of user's tenant
  - **Tenants:** Serialized dictionary that contains list of all added tenants in format (**key:** tenant name, **value:** connection string to tenant's database)
  - **Authentication:Local:CustomBearerTokenSigningKey:** Random string that will be used for signing and validating of self-issued JWT tokens
  - **Authentication:AzureAd:Issuer:** String that defines valid issuer of the Azure AD token. It should be in format https://login.microsoftonline.com/{your_Azure_tenant_ID}/v2.0
  - **Authentication:AzureAd:Audience:** String that defines valid audience of the Azure AD token. It should be GUID from **Application (client) ID** from Azure
  - **Authentication:AzureAd:MetadataAddress:** It should be in format https://login.microsoftonline.com/{your_Azure_tenant_ID}/v2.0/.well-known/openid-configuration
  - **Authentication:Google:Issuer:** String that defines valid issuer of the Google token. It should have the following value: https://accounts.google.com
  - **Authentication:Google:ClientId:** Client Id from the google application

* Run application locally

Note: Multitenancy feature is in progress. Tenants and TenantClaimKey settings are not used in the app.


## Setting up the application on Azure

In order to setup API on Azure, App Service should be created on Azure portal. When creating the App Service, if **Code** publish type is selected, then connection to Git should be defined and pipelines should be used to enable CI/CD. On the other hand, if **Docker Container** publish type is selected, **inoffice/api** image from **Docker Hub** can be used. In either case, after creating App Service, application parameters described [above](#steps-for-running-the-application-locally) should be added to **Application settings** on **Configuration** menu option.
Note that all nested JSON settings will be modified during deployment to Azure (i.e. Authentication:AzureAd:Issuer will be replaced with Authentication_AzureAd_Issuer) since Linux app service does not support ':' character in the app setting name (https://learn.microsoft.com/en-us/azure/app-service/configure-common?tabs=portal#app-settings)

## Setting up multitenancy (In Progress)

The application supports multitenancy. To setup multitenant environment, following steps should be applied:
* Each tenant should have its own database
* Enter values in **TenantClaimKey** and **Tenants** application parameters as described [above](#steps-for-running-the-application-locally)
* If MS SSO login flow is used, then AAD JWT token should be extended by the property that contains tenant's name (property's name should be the same as in **TenantClaimKey** application parameter). In order to do this, follow these steps:
  - Install AzureAD PowerShell module
  - Create extenstion property on user object by using **New-AzureADApplicationExtensionProperty -ObjectId "Object ID of the application on AAD App registrations" -Name "extenstion name" -DataType "String" -TargetObjects "User"**
  - For each user that should use non-default tenant, set the value of this extenstion property to desired tenant's name using **Set-AzureADUserExtension** method
  - Create new ClaimsMappingPolicy that will extend AAD JWT tokens when accessing the application using **New-AzureADPolicy -Definition @('{"ClaimsMappingPolicy":{"Version":1,"IncludeBasicClaimSet":"true", "ClaimsSchema": [{"Source":"user","ExtensionID":"extension property's ID","JwtClaimType":"name of mapped propery in JWT token"}]}}') -DisplayName "policy's name" -Type "ClaimsMappingPolicy"**
  - Assign this policy to the application by using **Add-AzureADServicePrincipalPolicy -Id "Object ID of the application on AAD Enterprise applications" -RefObjectId "Object ID of the Policy"**

## How to contribute to the project

* Fork this repository
* Clone your forked repository
* Add your changes
* Commit and push
* Create a pull request
* Star this repository
* Wait for pull request to merge
