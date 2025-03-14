# Introduction

This is a template to create **ASP.NET Core MVC / Angular** based startup projects for [ASP.NET Boilerplate](https://aspnetboilerplate.com/Pages/Documents). It has 2 different versions:

1. [ASP.NET Core MVC & jQuery](https://aspnetboilerplate.com/Pages/Documents/Zero/Startup-Template-Core) (server rendered multi-page application).
2. [ASP.NET Core & Angular](https://aspnetboilerplate.com/Pages/Documents/Zero/Startup-Template-Angular) (single page application).
 
User Interface is based on [BSB Admin theme](https://github.com/gurayyarar/AdminBSBMaterialDesign).
 
# Download

Create & download your project from https://aspnetboilerplate.com/Templates

# How to start Development

1. Install the .NET Core 2.2 [.NET Core SDK](https://dotnet.microsoft.com/en-us/download/dotnet/2.2).
2. Open a terminal or Visual Studio. Navigate to the RMA.Web.Host folder of the project or set this project as the startup project on Visual Studio.
3. Create `appsettings.Development.json` file in the `src\RMA.Web.Host` folder and copy the content below to the file. Replace settings with your own settings.
4. Run `dotnet run` command in the terminal or press `F5` in Visual Studio, for hot reload run `dotnet watch` command in the terminal.

```json
{
  "ConnectionStrings": {
    "Default": "Server=WSIIS-SLMS-V08\\SQLEXPRESS; Database=RMALMSDb; User ID=userdb;Password=passdb"
  },
  "App": {
    "ServerRootAddress": "http://localhost:21021/",
    "ClientRootAddress": "http://localhost",
    "CorsOrigins": "http://localhost:4200,http://localhost,http://localhost:8081,http://localhost:21021"
  },
  "Authentication": {
    "JwtBearer": {
        "IsEnabled": "true",
        "SecurityKey": "JWT_SECRET_KEY",
        "TokenValidDays": 7,
        "Issuer": "RMALMS",
        "Audience": "RMALMS"
    },
    "Mezon": {
      "ClientId": "CLIENT_ID",
      "ClientSecret": "CLIENT_SECRET",
      "AppToken": "APP_TOKEN",
      "AuthServerUrl": "https://oauth2.mezon.ai"
    }
  },
  "EnableMultiTenant": false,
  "SercurityCode": "SECURITY_CODE",
  "WWWFolder": "wwwroot",
  "MediaShortFolder": "Resource",
  "SCORMShortFolder": "SCORM",
  "DefaultMedia": "Media",
  "TimeScanFinishCourse": 10

}
```

# Screenshots

#### Sample Dashboard Page
![](_screenshots/module-zero-core-template-ui-home.png)

#### User Creation Modal
![](_screenshots/module-zero-core-template-ui-user-create-modal.png)

#### Login Page

![](_screenshots/module-zero-core-template-ui-login.png)

# Documentation

* [ASP.NET Core MVC & jQuery version.](https://aspnetboilerplate.com/Pages/Documents/Zero/Startup-Template-Core)
* [ASP.NET Core & Angular  version.](https://aspnetboilerplate.com/Pages/Documents/Zero/Startup-Template-Angular)

# License

[MIT](LICENSE).