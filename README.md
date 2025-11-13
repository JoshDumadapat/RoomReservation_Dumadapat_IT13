# Room Reservation System

A Blazor-based room reservation management system built with .NET MAUI.

## Features

- **Landing Page**: Beautiful landing page with hero section, popular spots, and about section
- **Booking System**: Room browsing and filtering with detailed room information
- **Reservation Management**: Full CRUD operations for reservations
- **Admin Dashboard**: Dashboard with statistics and reservation management
- **Cancelled Reservations**: View and filter cancelled reservations
- **Authentication**: Admin login system

## Prerequisites

- **Visual Studio 2022** (17.8 or later) with the following workloads:
  - .NET Multi-platform App UI development
  - ASP.NET and web development
- **.NET 9.0 SDK** or later
- **Windows 10/11** (for Windows development)

## Getting Started

### Clone the Repository

```bash
git clone https://github.com/JoshDumadapat/RoomReservation_Dumadapat_IT13.git
cd RoomReservation_Dumadapat_IT13
```

### Open in Visual Studio

1. Double-click `RoomReservation_Dumadapat_IT13.sln` to open the solution in Visual Studio
2. Wait for Visual Studio to restore NuGet packages (automatic)
3. Select your target platform (Windows, Android, iOS, etc.) from the dropdown
4. Click the **Run** button (▶️) or press **F5** to build and run the application

### First Run

1. The application will build automatically
2. If prompted, allow Visual Studio to restore NuGet packages
3. The application will launch in the selected platform

## Project Structure

```
RoomReservation_Dumadapat_IT13/
├── Components/
│   ├── Layout/          # Layout components
│   └── Pages/           # Page components
│       ├── Landing.razor
│       ├── Booking.razor
│       ├── Login.razor
│       ├── Dashboard.razor
│       ├── ReservationManagement.razor
│       └── RemovedReservations.razor
├── Models/              # Data models
├── wwwroot/             # Static files (CSS, images)
└── RoomReservation_Dumadapat_IT13.sln
```

## Default Login Credentials

- **Email**: `admin@vanderson.com`
- **Password**: `admin123`

## Database Connection (Optional)

If you want to connect to SQL Server, follow the instructions in `README_DATABASE_CONNECTION.md`.

## Technologies Used

- **.NET MAUI** - Cross-platform framework
- **Blazor** - Web UI framework
- **C#** - Programming language
- **CSS** - Styling

## Features Overview

### Landing Page
- Hero section with search filters
- Popular Spots & Rooms gallery
- About Us section
- Footer with contact information

### Booking Page
- Room type filtering
- Price range filtering
- Guest capacity filtering
- Room cards with detailed information
- Reservation modal

### Admin Dashboard
- Statistics overview
- Reservation management (CRUD operations)
- Cancelled reservations view
- Search and filter functionality

## Troubleshooting

### Build Errors
- Ensure all NuGet packages are restored: Right-click solution → **Restore NuGet Packages**
- Clean and rebuild: **Build** → **Clean Solution**, then **Build** → **Rebuild Solution**

### Run Button Not Showing
- Make sure the solution file (`.sln`) is opened, not just the project file
- Ensure Visual Studio has the required workloads installed
- Try closing and reopening Visual Studio

### Missing Images
- Ensure all image files in `wwwroot/images/` are present
- Check that image paths in Razor files match the actual file locations

## License

This project is for educational purposes.

## Author

Josh Dumadapat - IT13

