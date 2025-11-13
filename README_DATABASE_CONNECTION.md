# Database Connection Guide for Blazor Application

This guide will walk you through connecting your Blazor application to SQL Server Management Studio (SSMS).

## Prerequisites

- Visual Studio 2022 or later
- SQL Server Management Studio (SSMS) installed
- SQL Server (LocalDB, Express, or Full version) installed and running

---

## Step 1: Install Required NuGet Package

1. Open your project in Visual Studio
2. Right-click on your project in **Solution Explorer**
3. Select **Manage NuGet Packages...**
4. Go to the **Browse** tab
5. Search for: `Microsoft.Data.SqlClient`
6. Select version **5.2.2** (or latest stable version)
7. Click **Install**
8. Accept any license agreements

**Alternative (Package Manager Console):**
```
Install-Package Microsoft.Data.SqlClient -Version 5.2.2
```

---

## Step 2: Create Database in SSMS

1. Open **SQL Server Management Studio (SSMS)**
2. Connect to your SQL Server instance:
   - **Server name**: `localhost` or `(localdb)\MSSQLLocalDB` (for LocalDB)
   - **Authentication**: Windows Authentication (or SQL Server Authentication if configured)
   - Click **Connect**

3. In the **Object Explorer**, right-click on **Databases**
4. Select **New Database...**
5. Enter database name: `DB_RoomReservation_Dumadapat_IT13`
6. Click **OK**

---

## Step 3: Create Tables in SSMS

1. In SSMS, expand your database: `DB_RoomReservation_Dumadapat_IT13`
2. Right-click on **Tables** → **New** → **Table...**

### Create Reservations Table

Use the following SQL script in SSMS (click **New Query**):

```sql
USE [DB_RoomReservation_Dumadapat_IT13]
GO

CREATE TABLE [dbo].[Reservations](
    [ReservationID] [int] IDENTITY(1,1) NOT NULL,
    [CustomerFname] [nvarchar](100) NOT NULL,
    [CustomerLname] [nvarchar](100) NOT NULL,
    [ContactNum] [nvarchar](20) NULL,
    [Email] [nvarchar](100) NULL,
    [RoomType] [nvarchar](50) NOT NULL,
    [CheckInDate] [datetime] NOT NULL,
    [CheckOutDate] [datetime] NOT NULL,
    [Status] [nvarchar](50) NOT NULL,
    [DateCreated] [datetime] NOT NULL,
    [RoomPrice] [decimal](10, 2) NOT NULL,
    CONSTRAINT [PK_Reservations] PRIMARY KEY CLUSTERED ([ReservationID] ASC)
)
GO
```

### Create Admins Table

```sql
USE [DB_RoomReservation_Dumadapat_IT13]
GO

CREATE TABLE [dbo].[Admins](
    [AdminID] [int] IDENTITY(1,1) NOT NULL,
    [Email] [nvarchar](100) NOT NULL,
    [Password] [nvarchar](255) NOT NULL,
    [FullName] [nvarchar](100) NULL,
    CONSTRAINT [PK_Admins] PRIMARY KEY CLUSTERED ([AdminID] ASC)
)
GO
```

### Create RemovedReservations Table

```sql
USE [DB_RoomReservation_Dumadapat_IT13]
GO

CREATE TABLE [dbo].[RemovedReservations](
    [RemovedID] [int] IDENTITY(1,1) NOT NULL,
    [ReservationID] [int] NULL,
    [CustomerFname] [nvarchar](100) NOT NULL,
    [CustomerLname] [nvarchar](100) NOT NULL,
    [ContactNum] [nvarchar](20) NULL,
    [Email] [nvarchar](100) NULL,
    [RoomType] [nvarchar](50) NOT NULL,
    [CheckInDate] [datetime] NULL,
    [CheckOutDate] [datetime] NULL,
    [Status] [nvarchar](50) NOT NULL,
    [ReasonNote] [nvarchar](500) NULL,
    [AdminID] [int] NULL,
    [RemovedBy] [nvarchar](100) NULL,
    [DateRemoved] [datetime] NOT NULL,
    [RoomPrice] [decimal](10, 2) NOT NULL,
    CONSTRAINT [PK_RemovedReservations] PRIMARY KEY CLUSTERED ([RemovedID] ASC),
    CONSTRAINT [FK_RemovedReservations_Admins] FOREIGN KEY ([AdminID]) 
        REFERENCES [dbo].[Admins] ([AdminID])
)
GO
```

### Insert Default Admin User

```sql
USE [DB_RoomReservation_Dumadapat_IT13]
GO

INSERT INTO [dbo].[Admins] ([Email], [Password], [FullName])
VALUES ('admin@vanderson.com', 'admin123', 'Admin User')
GO
```

---

## Step 4: Create Database Service File

1. In Visual Studio, right-click on your project
2. Select **Add** → **New Folder** (if it doesn't exist)
3. Name it: `Services`
4. Right-click on the `Services` folder
5. Select **Add** → **Class...**
6. Name it: `DatabaseService.cs`
7. Click **Add**

---

## Step 5: Add Database Service Code

Replace the content of `Services/DatabaseService.cs` with the following:

```csharp
using Microsoft.Data.SqlClient;
using RoomReservation_Dumadapat_IT13.Models;
using System.Data;

namespace RoomReservation_Dumadapat_IT13.Services
{
    public class DatabaseService
    {
        // Connection String - UPDATE THIS WITH YOUR SERVER DETAILS
        private readonly string connectionString = "Data Source=localhost;Initial Catalog=DB_RoomReservation_Dumadapat_IT13;Integrated Security=True;TrustServerCertificate=True;";

        // Alternative connection strings:
        // For LocalDB: "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=DB_RoomReservation_Dumadapat_IT13;Integrated Security=True;TrustServerCertificate=True;"
        // For SQL Server Authentication: "Data Source=localhost;Initial Catalog=DB_RoomReservation_Dumadapat_IT13;User ID=your_username;Password=your_password;TrustServerCertificate=True;"

        public async Task<bool> AddReservationAsync(Reservation reservation)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    var query = @"INSERT INTO Reservations 
                        (CustomerFname, CustomerLname, ContactNum, Email, RoomType, CheckInDate, CheckOutDate, Status, DateCreated, RoomPrice)
                        VALUES (@CustomerFname, @CustomerLname, @ContactNum, @Email, @RoomType, @CheckInDate, @CheckOutDate, @Status, @DateCreated, @RoomPrice)";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CustomerFname", reservation.CustomerFname);
                        command.Parameters.AddWithValue("@CustomerLname", reservation.CustomerLname);
                        command.Parameters.AddWithValue("@ContactNum", (object?)reservation.ContactNum ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Email", (object?)reservation.Email ?? DBNull.Value);
                        command.Parameters.AddWithValue("@RoomType", reservation.RoomType);
                        command.Parameters.AddWithValue("@CheckInDate", reservation.CheckInDate);
                        command.Parameters.AddWithValue("@CheckOutDate", reservation.CheckOutDate);
                        command.Parameters.AddWithValue("@Status", reservation.Status);
                        command.Parameters.AddWithValue("@DateCreated", reservation.DateCreated);
                        command.Parameters.AddWithValue("@RoomPrice", reservation.RoomPrice);

                        await command.ExecuteNonQueryAsync();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding reservation: {ex.Message}");
                return false;
            }
        }

        public async Task<List<Reservation>> GetAllReservationsAsync()
        {
            var reservations = new List<Reservation>();
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    var query = "SELECT * FROM Reservations ORDER BY DateCreated DESC";

                    using (var command = new SqlCommand(query, connection))
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            reservations.Add(new Reservation
                            {
                                ReservationID = reader.GetInt32("ReservationID"),
                                CustomerFname = reader.GetString("CustomerFname"),
                                CustomerLname = reader.GetString("CustomerLname"),
                                ContactNum = reader.IsDBNull("ContactNum") ? null : reader.GetString("ContactNum"),
                                Email = reader.IsDBNull("Email") ? null : reader.GetString("Email"),
                                RoomType = reader.GetString("RoomType"),
                                CheckInDate = reader.GetDateTime("CheckInDate"),
                                CheckOutDate = reader.GetDateTime("CheckOutDate"),
                                Status = reader.GetString("Status"),
                                DateCreated = reader.GetDateTime("DateCreated"),
                                RoomPrice = reader.GetDecimal("RoomPrice")
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting reservations: {ex.Message}");
            }
            return reservations;
        }

        public async Task<bool> UpdateReservationAsync(Reservation reservation)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    var query = @"UPDATE Reservations 
                        SET CustomerFname = @CustomerFname, CustomerLname = @CustomerLname, 
                            ContactNum = @ContactNum, Email = @Email, RoomType = @RoomType,
                            CheckInDate = @CheckInDate, CheckOutDate = @CheckOutDate, 
                            Status = @Status, RoomPrice = @RoomPrice
                        WHERE ReservationID = @ReservationID";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ReservationID", reservation.ReservationID);
                        command.Parameters.AddWithValue("@CustomerFname", reservation.CustomerFname);
                        command.Parameters.AddWithValue("@CustomerLname", reservation.CustomerLname);
                        command.Parameters.AddWithValue("@ContactNum", (object?)reservation.ContactNum ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Email", (object?)reservation.Email ?? DBNull.Value);
                        command.Parameters.AddWithValue("@RoomType", reservation.RoomType);
                        command.Parameters.AddWithValue("@CheckInDate", reservation.CheckInDate);
                        command.Parameters.AddWithValue("@CheckOutDate", reservation.CheckOutDate);
                        command.Parameters.AddWithValue("@Status", reservation.Status);
                        command.Parameters.AddWithValue("@RoomPrice", reservation.RoomPrice);

                        await command.ExecuteNonQueryAsync();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating reservation: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> CancelReservationAsync(int reservationId, string reason, int adminId)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    
                    // Get reservation details
                    var reservation = await GetReservationByIdAsync(reservationId);
                    if (reservation == null) return false;

                    // Get admin name
                    var admin = await GetAdminByIdAsync(adminId);
                    var adminName = admin?.FullName ?? "Admin User";

                    // Insert into RemovedReservations
                    var insertQuery = @"INSERT INTO RemovedReservations 
                        (ReservationID, CustomerFname, CustomerLname, ContactNum, Email, RoomType, 
                         CheckInDate, CheckOutDate, Status, ReasonNote, AdminID, RemovedBy, DateRemoved, RoomPrice)
                        VALUES (@ReservationID, @CustomerFname, @CustomerLname, @ContactNum, @Email, @RoomType,
                                @CheckInDate, @CheckOutDate, @Status, @ReasonNote, @AdminID, @RemovedBy, @DateRemoved, @RoomPrice)";

                    using (var insertCommand = new SqlCommand(insertQuery, connection))
                    {
                        insertCommand.Parameters.AddWithValue("@ReservationID", reservationId);
                        insertCommand.Parameters.AddWithValue("@CustomerFname", reservation.CustomerFname);
                        insertCommand.Parameters.AddWithValue("@CustomerLname", reservation.CustomerLname);
                        insertCommand.Parameters.AddWithValue("@ContactNum", (object?)reservation.ContactNum ?? DBNull.Value);
                        insertCommand.Parameters.AddWithValue("@Email", (object?)reservation.Email ?? DBNull.Value);
                        insertCommand.Parameters.AddWithValue("@RoomType", reservation.RoomType);
                        insertCommand.Parameters.AddWithValue("@CheckInDate", (object?)reservation.CheckInDate ?? DBNull.Value);
                        insertCommand.Parameters.AddWithValue("@CheckOutDate", (object?)reservation.CheckOutDate ?? DBNull.Value);
                        insertCommand.Parameters.AddWithValue("@Status", reservation.Status);
                        insertCommand.Parameters.AddWithValue("@ReasonNote", reason);
                        insertCommand.Parameters.AddWithValue("@AdminID", adminId);
                        insertCommand.Parameters.AddWithValue("@RemovedBy", adminName);
                        insertCommand.Parameters.AddWithValue("@DateRemoved", DateTime.Now);
                        insertCommand.Parameters.AddWithValue("@RoomPrice", reservation.RoomPrice);

                        await insertCommand.ExecuteNonQueryAsync();
                    }

                    // Delete from Reservations
                    var deleteQuery = "DELETE FROM Reservations WHERE ReservationID = @ReservationID";
                    using (var deleteCommand = new SqlCommand(deleteQuery, connection))
                    {
                        deleteCommand.Parameters.AddWithValue("@ReservationID", reservationId);
                        await deleteCommand.ExecuteNonQueryAsync();
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error cancelling reservation: {ex.Message}");
                return false;
            }
        }

        public async Task<Reservation?> GetReservationByIdAsync(int id)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    var query = "SELECT * FROM Reservations WHERE ReservationID = @ReservationID";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ReservationID", id);
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return new Reservation
                                {
                                    ReservationID = reader.GetInt32("ReservationID"),
                                    CustomerFname = reader.GetString("CustomerFname"),
                                    CustomerLname = reader.GetString("CustomerLname"),
                                    ContactNum = reader.IsDBNull("ContactNum") ? null : reader.GetString("ContactNum"),
                                    Email = reader.IsDBNull("Email") ? null : reader.GetString("Email"),
                                    RoomType = reader.GetString("RoomType"),
                                    CheckInDate = reader.GetDateTime("CheckInDate"),
                                    CheckOutDate = reader.GetDateTime("CheckOutDate"),
                                    Status = reader.GetString("Status"),
                                    DateCreated = reader.GetDateTime("DateCreated"),
                                    RoomPrice = reader.GetDecimal("RoomPrice")
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting reservation: {ex.Message}");
            }
            return null;
        }

        public async Task<List<Reservation>> SearchReservationsAsync(string searchTerm)
        {
            var reservations = new List<Reservation>();
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    var query = @"SELECT * FROM Reservations 
                        WHERE CustomerFname LIKE @SearchTerm 
                        OR CustomerLname LIKE @SearchTerm 
                        OR Email LIKE @SearchTerm 
                        OR ContactNum LIKE @SearchTerm 
                        OR RoomType LIKE @SearchTerm 
                        OR Status LIKE @SearchTerm
                        ORDER BY DateCreated DESC";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@SearchTerm", $"%{searchTerm}%");
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                reservations.Add(new Reservation
                                {
                                    ReservationID = reader.GetInt32("ReservationID"),
                                    CustomerFname = reader.GetString("CustomerFname"),
                                    CustomerLname = reader.GetString("CustomerLname"),
                                    ContactNum = reader.IsDBNull("ContactNum") ? null : reader.GetString("ContactNum"),
                                    Email = reader.IsDBNull("Email") ? null : reader.GetString("Email"),
                                    RoomType = reader.GetString("RoomType"),
                                    CheckInDate = reader.GetDateTime("CheckInDate"),
                                    CheckOutDate = reader.GetDateTime("CheckOutDate"),
                                    Status = reader.GetString("Status"),
                                    DateCreated = reader.GetDateTime("DateCreated"),
                                    RoomPrice = reader.GetDecimal("RoomPrice")
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error searching reservations: {ex.Message}");
            }
            return reservations;
        }

        public async Task<List<RemovedReservation>> GetAllRemovedReservationsAsync()
        {
            var removedReservations = new List<RemovedReservation>();
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    var query = @"SELECT r.*, a.FullName as AdminName 
                        FROM RemovedReservations r
                        LEFT JOIN Admins a ON r.AdminID = a.AdminID
                        ORDER BY r.DateRemoved DESC";

                    using (var command = new SqlCommand(query, connection))
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            removedReservations.Add(new RemovedReservation
                            {
                                RemovedID = reader.GetInt32("RemovedID"),
                                ReservationID = reader.IsDBNull("ReservationID") ? null : reader.GetInt32("ReservationID"),
                                CustomerFname = reader.GetString("CustomerFname"),
                                CustomerLname = reader.GetString("CustomerLname"),
                                ContactNum = reader.IsDBNull("ContactNum") ? null : reader.GetString("ContactNum"),
                                Email = reader.IsDBNull("Email") ? null : reader.GetString("Email"),
                                RoomType = reader.GetString("RoomType"),
                                CheckInDate = reader.IsDBNull("CheckInDate") ? null : reader.GetDateTime("CheckInDate"),
                                CheckOutDate = reader.IsDBNull("CheckOutDate") ? null : reader.GetDateTime("CheckOutDate"),
                                Status = reader.GetString("Status"),
                                ReasonNote = reader.IsDBNull("ReasonNote") ? null : reader.GetString("ReasonNote"),
                                AdminID = reader.IsDBNull("AdminID") ? null : reader.GetInt32("AdminID"),
                                RemovedBy = reader.IsDBNull("RemovedBy") ? null : reader.GetString("RemovedBy"),
                                DateRemoved = reader.GetDateTime("DateRemoved"),
                                RoomPrice = reader.GetDecimal("RoomPrice"),
                                AdminName = reader.IsDBNull("AdminName") ? null : reader.GetString("AdminName")
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting removed reservations: {ex.Message}");
            }
            return removedReservations;
        }

        public async Task<Admin?> GetAdminByEmailAsync(string email)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    var query = "SELECT * FROM Admins WHERE Email = @Email";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Email", email);
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return new Admin
                                {
                                    AdminID = reader.GetInt32("AdminID"),
                                    Email = reader.GetString("Email"),
                                    Password = reader.GetString("Password"),
                                    FullName = reader.IsDBNull("FullName") ? null : reader.GetString("FullName")
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting admin: {ex.Message}");
            }
            return null;
        }

        public async Task<Admin?> GetAdminByIdAsync(int id)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    var query = "SELECT * FROM Admins WHERE AdminID = @AdminID";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@AdminID", id);
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return new Admin
                                {
                                    AdminID = reader.GetInt32("AdminID"),
                                    Email = reader.GetString("Email"),
                                    Password = reader.GetString("Password"),
                                    FullName = reader.IsDBNull("FullName") ? null : reader.GetString("FullName")
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting admin: {ex.Message}");
            }
            return null;
        }
    }
}
```

---

## Step 6: Register Database Service

Open `MauiProgram.cs` and add the following:

**Add this using statement at the top:**
```csharp
using RoomReservation_Dumadapat_IT13.Services;
```

**Add this line in the `CreateMauiApp()` method (after `builder.Services.AddMauiBlazorWebView();`):**
```csharp
builder.Services.AddSingleton<DatabaseService>();
```

**Complete `MauiProgram.cs` should look like:**
```csharp
using Microsoft.Extensions.Logging;
using RoomReservation_Dumadapat_IT13.Services;

namespace RoomReservation_Dumadapat_IT13
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();
            builder.Services.AddSingleton<DatabaseService>();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
```

---

## Step 7: Add Using Statement to Imports

Open `Components/_Imports.razor` and add this line:

```razor
@using RoomReservation_Dumadapat_IT13.Services
```

---

## Step 8: Update Connection String

**IMPORTANT:** Update the connection string in `Services/DatabaseService.cs` based on your SQL Server setup:

### For SQL Server (Default Instance):
```csharp
private readonly string connectionString = "Data Source=localhost;Initial Catalog=DB_RoomReservation_Dumadapat_IT13;Integrated Security=True;TrustServerCertificate=True;";
```

### For SQL Server (Named Instance):
```csharp
private readonly string connectionString = "Data Source=localhost\\SQLEXPRESS;Initial Catalog=DB_RoomReservation_Dumadapat_IT13;Integrated Security=True;TrustServerCertificate=True;";
```

### For LocalDB:
```csharp
private readonly string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=DB_RoomReservation_Dumadapat_IT13;Integrated Security=True;TrustServerCertificate=True;";
```

### For SQL Server Authentication:
```csharp
private readonly string connectionString = "Data Source=localhost;Initial Catalog=DB_RoomReservation_Dumadapat_IT13;User ID=your_username;Password=your_password;TrustServerCertificate=True;";
```

**To find your server name:**
1. Open SSMS
2. Look at the server name in the connection dialog
3. Use that exact name in your connection string

---

## Step 9: Update Razor Pages to Use Database

In your Razor pages (e.g., `ReservationManagement.razor`, `RemovedReservations.razor`), add:

```razor
@inject DatabaseService DatabaseService
```

Then replace static data calls with database calls:
- Replace `InitializeSampleData()` with `await DatabaseService.GetAllReservationsAsync()`
- Replace `AddReservation()` with `await DatabaseService.AddReservationAsync(currentReservation)`
- Replace `UpdateReservation()` with `await DatabaseService.UpdateReservationAsync(currentReservation)`
- Replace `CancelReservation()` with `await DatabaseService.CancelReservationAsync(...)`
- Replace `SearchReservations()` with `await DatabaseService.SearchReservationsAsync(searchTerm)`

---

## Step 10: Test the Connection

1. Build your project: **Build** → **Build Solution** (Ctrl+Shift+B)
2. Run your application: **Debug** → **Start Debugging** (F5)
3. Try adding a reservation through the UI
4. Check SSMS to verify the data was inserted:
   - Open SSMS
   - Expand your database
   - Right-click on **Reservations** table → **Select Top 1000 Rows**

---

## Troubleshooting

### Error: "Cannot open database"
- **Solution**: Verify the database name in the connection string matches exactly with SSMS
- Check that SQL Server is running (Services → SQL Server)

### Error: "Login failed"
- **Solution**: 
  - For Windows Authentication: Ensure your Windows account has access
  - For SQL Authentication: Verify username and password

### Error: "A network-related or instance-specific error"
- **Solution**: 
  - Check SQL Server is running
  - Verify server name is correct
  - Check firewall settings

### Error: "Package not found"
- **Solution**: 
  - Restore NuGet packages: Right-click solution → **Restore NuGet Packages**
  - Check internet connection for package download

---

## Files Modified Summary

1. ✅ **NuGet Package**: `Microsoft.Data.SqlClient` (installed)
2. ✅ **New File**: `Services/DatabaseService.cs` (created)
3. ✅ **Modified**: `MauiProgram.cs` (added service registration)
4. ✅ **Modified**: `Components/_Imports.razor` (added using statement)
5. ✅ **Modified**: Razor pages (inject DatabaseService and use database methods)

---

## Connection String Parameters Explained

- **Data Source**: Server name (localhost, (localdb)\MSSQLLocalDB, or server\instance)
- **Initial Catalog**: Database name
- **Integrated Security=True**: Uses Windows Authentication
- **User ID/Password**: Required for SQL Server Authentication
- **TrustServerCertificate=True**: Trusts the server certificate (for development)

---

## Next Steps

After successful connection:
1. Test CRUD operations (Create, Read, Update, Delete)
2. Test search functionality
3. Test cancellation functionality
4. Verify data persistence across application restarts

---

## Support

If you encounter issues:
1. Check SQL Server is running
2. Verify connection string matches your SSMS server name
3. Check database and tables exist
4. Review error messages in Visual Studio Output window

