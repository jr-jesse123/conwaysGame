# Conway's Game of Life

A web-based implementation of Conway's Game of Life using .NET Core.

## Prerequisites

- .NET 8.0 SDK or later
- Git

## Getting Started

1. Clone the repository:

```bash
git clone <repository-url>
cd conways-game
```

2. Install the Entity Framework Core tools (if not already installed):
```bash
dotnet tool install --global dotnet-ef
```

3. Set up the database:
```bash
# Navigate to the Web project
cd ConWaysGame.Web

# Apply migrations to create/update the database
dotnet ef database update
```

## Running the Application

1. Navigate to the web project directory:
```bash
cd ConWaysGame.Web
```

2. Run the application:
```bash
dotnet run
```

The application will be available at `https://localhost:7234` (or `http://localhost:5234`).

## Running Tests

From the root directory:
```bash
dotnet test
```

## Database Management

### Creating New Migrations

When you make changes to the data models, create a new migration:
```bash
cd ConWaysGame.Web
dotnet ef migrations add <MigrationName>
```

### Applying Migrations

To update the database with the latest migrations:
```bash
dotnet ef database update
```


## API Documentation

The API documentation is available at `/swagger` when running the application in development mode.

## Project Structure

- `ConwaysGame.Core` - Core game logic and domain models
- `ConWaysGame.Web` - Web API and frontend
- `ConwaysGame.Tests` - Integration and unit tests

