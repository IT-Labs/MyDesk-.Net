# inOffice - API

## Table of contents
* [General info](#introduction)
* [Technologies](#technologies)
* [Setup](#steps-for-running-the-application-locally)
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
  - **JwtIssuer:** String in format https://login.microsoftonline.com/{your_Azure_tenant_ID}/v2.0
  - **JwtAudience:** Application (client) ID from Azure
  - **MetadataAddress:** String in format https://login.microsoftonline.com/{your_Azure_tenant_ID}/v2.0/.well-known/openid-configuration
  - **SentimentEndpoint:** URL of the external API that will determine sentiment of the reservation's review
  - **CustomBearerTokenSigningKey:** Random string that will be used for signing and validating of self-issued JWT tokens
* Run application locally

## How to contribute to the project

* Fork this repository
* Clone your forked repository
* Add your changes
* Commit and push
* Create a pull request
* Star this repository
* Wait for pull request to merge