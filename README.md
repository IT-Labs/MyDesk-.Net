# inOffice - API

## Table of contents
* [General info](#introduction)
* [Technologies](#technologies)
* [Setup locally](#steps-for-running-the-application-locally)
* [Setup on Azure](#setting-up-the-application-on-Azure)
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
* Run application locally

## Setting up the application on Azure

In order to setup API on Azure, App Service should be created on Azure portal. When creating the App Service, if **Code** publish type is selected, then connection to Git should be defined so Azure would know from where to pull the code. On the other hand, if **Docker Container** publish type is selected, **inoffice/api** image from **Docker Hub** can be used. In either case, after creating App Service, application parameters described [above](#setting-up-the-application-on-Azure) should be added to **Application settings** on **Configuration** menu option.

## How to contribute to the project

* Fork this repository
* Clone your forked repository
* Add your changes
* Commit and push
* Create a pull request
* Star this repository
* Wait for pull request to merge