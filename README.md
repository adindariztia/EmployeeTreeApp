# EmployeeTreeApp

## Description
Welcome to the EmployeeTree App! This application is designed to store employee information along with their hierarchical structure, allowing users to trace employee managers up to the root of the tree. The application consists of two main components: a backend developed using .NET C# 6 and a frontend folder containing simple frontend files.

## Backend Installation and Setup
The backend of the EmployeeTree App is built using .NET C# 6. To run the backend server, please follow these steps:

1. Navigate to the backend folder: `cd ~/backend/EmployeeTree/EmployeeTree`
2. Run the following command to start the server:
   ```bash
   dotnet run .

## Frontend Installation and Setup
The frontend of the EmployeeTree App is located in the `frontend` folder. To run the frontend interface, perform the following steps:
1. Navigate to the frontend folder: `cd ~/frontend`.
2. Run the following command to start the server:
   ```bash
   python -m http.server 3000

  The frontend server will be hosted on port 3000. Please note that the CORS port configured in the backend is also 3000.

## Usage
Once both the backend and frontend servers are running, you can access the EmployeeTree App through your web browser. Navigate to http://localhost:3000 to interact with the application.
