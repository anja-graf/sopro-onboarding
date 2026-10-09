# Short Description of this project:

An employee onboarding ASP.NET Core web application built with C# and .NET 6 using Razor Pages pattern.
Features user authentication, role management, reusable onboarding process templates, task management, database persistence, CI/CD pipeline and docker support.

# More detailed description of all functions
- Onboarding process templates: Define reusable process blueprints consisting of tasks and specify which roles are permitted to use each process.
- Task management: Define tasks with instructions, responsible roles, permitted departments and contract types, and deadlines relative to a process's due date.
- Process tracking: Create individual process instances with due dates, assigned responsible users, reference users, and associated tasks. Process instances can also be archived.
- User and role management: Manage users, roles, and department memberships, with role-based relationships between users and onboarding processes.
- Authentication: Uses ASP.NET Core Identity and includes password hashing and validation in the application's user model.
- Database persistence: Stores application data using Entity Framework Core, with SQLite configured in the application startup code.

# Architecture and technologies
- Language and framework: C#, ASP.NET Core, .NET 6
- Web architecture: Razor Pages with paired .cshtml and .cshtml.cs files
- Data access: Entity Framework Core and SQLite
- Authentication: ASP.NET Core Identity
- Deployment: Docker support
- CI/CD: GitLab pipeline configured
