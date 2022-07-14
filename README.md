# FitnessApp
Simple fitness WEB API project written in ASP.NET 6.  </br>
You must have [.NET 6](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) and [Visual Studio 2022](https://visualstudio.microsoft.com/vs/) installed.

## Authors
* **[Dzhengiz Ibryamov](https://github.com/djengo16)** 
* **[Alexander Vladimirov](https://github.com/AlexanderVladimirov9090)**

## Overview
The project's idea is to generate fitness program for specific user based on what he wants to achieve (Lose weight, Gain muscle or Maintain),
how many days he can spend at the gym weekly and how experienced he is. Currently we have training programs for 3, 4 and 5 days with different muscle splits.
Exercises are separated into three groups: easy, medium and hard. Depending on user's experience we get the most suitable ones for him.
The application uses ASP.NET Identity and JWT web token to make it easy for authentication and authorization. 

### Run the app
Before running the application please take these steps:
- Open Powershell and run the command ```dotnet ef database update```
- Add the appsettings.Development.json file to the project with the code below (Jwt requires this key option)
```
"JwtSettings": {
    "Secret": "Key goes here"
  },
  ```
For key generation, you can use [this](http://www.unit-conversion.info/texttools/random-string-generator/).

### Seeding
When we start the app for the first time, our Seeder runs and seeds data as
- Exercises
- Users with different combinations of training programs
- Administrator role
- User with Administrator role
