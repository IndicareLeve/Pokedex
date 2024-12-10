# Pokedex API

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/get-started)

## Running the Solution
All commands are supposed to be executed from the root of the repository. If you just cloned it, simply run 
```
cd Pokedex
```


### Running with Docker

1. **Build the Docker image:**

   Open a terminal in the directory containing the Dockerfile and run:

   ```sh
   docker build -t pokedex-api .
   ```

2. **Run the Docker container:**

   ```sh
   docker run -d -p 5280:8080 --name pokedex-container pokedex-api
   ```

   This will run the container in detached mode and map port `5280` on your host to port `8080` in the container.

3. **Access the application:**

   APIs are available at `http://localhost:5280`.

### Running Directly

1. **Restore dependencies:**

   Open a terminal in the root directory of the project and run:

   ```sh
   dotnet restore
   ```

2. **Build the project:**

   ```sh
   dotnet build
   ```

3. **Run the application:**

   Navigate to the `Pokedex.API` project directory and run:

   ```sh
   dotnet run --project ./src/Pokedex.API/Pokedex.API.csproj
   ```

   By default, the application will run on `http://localhost:5280`.

## Running Tests

 **Run the tests:**

   ```sh
   dotnet test
   ```

   This will execute all the tests in the solution `Pokedex` and display the results in the terminal.