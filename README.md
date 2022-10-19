# inOffice - API

## Table of contents
* [General info](#introduction)
* [Technologies](#technologies)
* [Setup locally](#steps-for-running-the-application-locally)
* [Setup on Azure](#setting-up-the-application-on-azure)
* [Setting up multitenancy](#setting-up-multitenancy)
* [How to contribute](#how-to-contribute-to-the-project)

## Introduction

This application is back-end part of inOffice project that enables administrators to model desired offices' and desks' layout and it allows users to reserve those desks/conference rooms for a certain time period. The front-end part of this project can be found on this [link](https://dev.azure.com/ITLabs-LLC/Internship%202022/_git/inOffice%20-%20UI).

## Technologies

This is .NET web API project that uses Entity Framework 6 to communicate with Azure SQL DB. Database will be automatically created, by executing EF migrations, upon application's first launch. Repository contains Azure pipeline scripts that can be used for setting up CI/CD on Azure. The application uses JWT tokens for authentication/authorization. If application cannot be deployed on Azure, the SQL Server RDBMS should be used and application will use self-issued JWT tokens for authentication/authorization.

## Steps for running the application locally

* Install SQL server locally (express version will do)
* Clone repository from Git
* Open the solution in Visual Studio 2022
* In **appsettings.json** file add following properties:
  - **ConnectionString:** Connection string for your database
  - **JwtIssuer:** String that defines valid issuer of the token. If application is deployed on Azure, it should be in format https://login.microsoftonline.com/{your_Azure_tenant_ID}/v2.0
  - **JwtAudience:** String that defines valid audience of the token. If application is deployed on Azure, it should be GUID from **Application (client) ID** from Azure
  - **MetadataAddress (optional):** If application is deployed on Azure, it should be in format https://login.microsoftonline.com/{your_Azure_tenant_ID}/v2.0/.well-known/openid-configuration
  - **SentimentEndpoint (optional):** URL of the external API that will determine sentiment of the reservation's review
  - **CustomBearerTokenSigningKey:** Random string that will be used for signing and validating of self-issued JWT tokens
  - **TenantClaimKey:** Name of custom property in JWT token that will contain name of user's tenant
  - **Tenants:** Serialized dictionary that contains list of all added tenants in format (**key:** tenant name, **value:** connection string to tenant's database)
* Run application locally

## Setting up the application on Azure

In order to setup API on Azure, App Service should be created on Azure portal. When creating the App Service, if **Code** publish type is selected, then connection to Git should be defined and pipelines should be used to enable CI/CD. On the other hand, if **Docker Container** publish type is selected, **inoffice/api** image from **Docker Hub** can be used. In either case, after creating App Service, application parameters described [above](#steps-for-running-the-application-locally) should be added to **Application settings** on **Configuration** menu option.

## Setting up multitenancy

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