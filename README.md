ProductsManager - 
is software based on the APS.NET Core web platform with an MSSQL database.
Includes functionality for actions with products: adding, changing, presenting, searching and filtering by attributes.
The project includes three pages, one is a system page (responsible for creating databases, tables, and filling tables with data),
two others for use by users (one is home, for presentation and CRUD operations), another one for reporting and analyzing data.

Tehnologies:
- .NET Core 8.0 / C#
- ASP.NET Core
- MSSQL
- Entity Framework Core 8.0
- JQuery, CSS3, HTML5

Structure and design:
- MVC pattern
- layered architecture, repository pattern, dependency injection
- code, data, UI are split
- code consists of: controllers, models, core service logic and optional 3rd party libs
- Entity Framework supports using MSSQL, easy to create new db instance, create tables and fill with data, customize the CRUD
- data stored by default in MSSQL database
- use of well-known 3rd party libraries: Automapper
- code architecture allows easy extend options:
  - add new system functionalities
  - add new repository and data providers

This technology and architecture were chosen so that in the future it would be possible to easily extend the 
functionality and integrate this project in a more complex system.
