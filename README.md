# PackageSync

PackageSync is a web application that is designed to streamline package management for users. It provides an intuitive interface and robust backend functionality to perform CRUD (Create, Read, Update, Delete) operations on packages efficiently. Besides the basic CRUD operations, this application allows users to filter their packages based on their name and status. The application was built using ASP.NET Web API and Blazor WebAssembly.

# How to run the PackageSync

To run PackageSync on your local machine follow these steps:
1. Run the PackageSyncWebAPI project - this will start up the backend server on http://localhost:5142 (When the backend server starts up it will automatically redirect to http://localhost:5142/swagger/index.html, this is the Swagger UI which contains the API documentation.)
2. Run the PackageSyncWASM project - this will start up the client application on http://localhost:5020

# PackageSync structure
The PackageSync solution contains three different projects:
1. PackageSync.Domain - This is a class library which contains the domain models
2. PackageSyncWebAPI - This is a ASP.NET Web API project which represents the backend of the PackageSync web application
3. PackageSyncWASM - This is a Blazor WebAssemby project which represents the frontend of the PackageSync web application

The PackageSyncWebAPI is devided in to:
1. Controllers - Represent the classes which contain the endpoints that are exposed to the users
2. Services - Represent the classes which contain the bussiness logic of the application
3. Infrastructure - Contains the DbContext and the UserSeeder class
4. Validators - Represent the classes for request validation
5. Middleware - Contains the LoggerMiddleware class which is responsible for logging

The PackageSyncWASM is devided in to:
1. Pages - Represent the presentation layer of the application (UI)
2. Components - Represent reusable components which are used in Pages
3. Services - Handle communication with the backend

# Libraries which PackageSync uses

PackageSyncWebAPI uses the following libraries:
1. FluentValidation.AspNetCore - request validation
2. Microsoft.AspNetCore.Authentication.JwtBearer - jwt based authentication
3. Microsoft.EntityFrameworkCore - persistance layer
4. Microsoft.EntityFrameworkCore.InMemory - in memory database
5. Microsoft.AspNetCore.Identity.EntityFrameworkCore - Identity framework for authentication
6. Serilog.AspNetCore - logging
7. Serilog.AspNetCore.Console - logging to console
8. Swashbuckle.AspNetCore - swagger documentation and testing
9. System.IdentityModel.Tokens.Jwt - working with jwt

PackageSyncWASM uses the following libraries:
1. Blazored.LocalStorage - working with localStorage on the browser
2. Blazored.Toast - toast notifications
