# GermanClimateTracker

## Description
A full-stack weather monitoring application built with .NET 9, MSSQL, and Vue.js that fetches real-time weather data from German cities using the OpenWeather API and displays it in a responsive dashboard.

## Prerequisites
Before you begin, ensure you have the following installed:
- .NET 9 SDK
- Node.js (v18 or later)
- SQL Server Express (2019 or later) and SQL Server Management Studio (Recommended)

## Required API Keys
- OpenWeather API key (Get it from [OpenWeather](https://openweathermap.org/api)) or, for testing purposes, you can use mine:
```bash
OPENWEATHER_API_KEY=ea2b9cc4f254b14038effdf8ec1ab86a
```

## Installation Steps

### 1. Clone the Repository
```bash
git clone https://github.com/Fernando9200/GermanClimateTracker
cd GermanClimateTracker
```

### 2. Database Setup
1. Open SQL Server Management Studio
2. Connect to your local SQL Server instance
3. Note your SQL Server instance name (you'll need it for connection string)

### 3. Backend Configuration
1. Navigate to the WebApi project directory and update `appsettings.json` with your database connection:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=CleanArchitectureDemo;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True",
    "HangfireConnection": "Server=YOUR_SERVER_NAME;Database=CleanArchitectureDemo;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  }
}
```
*Replace `YOUR_SERVER_NAME` with your SQL Server instance name (e.g., `.\SQLEXPRESS01`)*

3. Navigate to the WorkerService project directory and do the same updates on `appsettings.json`

4. Create a `.env` file in the root of the application:
```env
OPENWEATHER_API_KEY=your_api_key_here
```

5. On WebApi directory, apply database migrations (be sure to have the donet tools installed first):
```bash
dotnet ef database update
```

6. On first terminal, run the WebApi:
```bash
cd WebApi
dotnet run
```

7. On second terminal, run the WorkerService:
```bash
cd WorkerService
dotnet run
```
*The API should start at `http://localhost:5102/api/weather`*

### 4. Frontend Setup
1. On third terminal, navigate to the frontend directory:
```bash
cd frontend
```

2. Install dependencies:
```bash
npm install
```

3. Start the development server:
```bash
npm run dev
```
*The frontend should start at `http://localhost:5173`*

## Common Issues and Solutions

### Database Connection Issues
- Verify SQL Server is running
- Check the connection string in `appsettings.json`
- Ensure the database exists

### API Not Working
- Check if OpenWeather API key is correctly set in `.env`
- Verify the API is running on the correct port
- Check the API logs for any errors

### Frontend Issues
- Make sure Node.js is installed correctly
- Clear npm cache if needed: `npm cache clean --force`
- Verify all dependencies are installed
- Check console for any errors

## Support
If you encounter any issues while testing this application, please feel free to contact me at `fernando91.sosa@hotmail.com`.
