using Microsoft.Data.SqlClient;
using RoomReservation_Dumadapat_IT13.Models;
using System.Data;

namespace RoomReservation_Dumadapat_IT13.Services
{
    public class DatabaseService
    {
        // Connection String - UPDATE THIS WITH YOUR SERVER DETAILS
        // 
        // HOW TO GET YOUR CONNECTION STRING FROM SERVER EXPLORER:
        // 1. In Visual Studio, open Server Explorer (View → Server Explorer)
        // 2. Expand "Data Connections"
        // 3. Right-click on your database connection
        // 4. Select "Properties"
        // 5. Find the "Connection String" property and copy it
        // 6. Replace the connectionString value below with your copied connection string
        //
        // OR use one of these templates based on your setup:
        
        // For SQL Server with Windows Authentication (most common):
        // private readonly string connectionString = "Data Source=YOUR_SERVER_NAME;Initial Catalog=YOUR_DATABASE_NAME;Integrated Security=True;TrustServerCertificate=True;";
        
        // For SQL Server Named Instance (e.g., SQLEXPRESS):
        // private readonly string connectionString = "Data Source=YOUR_SERVER_NAME\\SQLEXPRESS;Initial Catalog=YOUR_DATABASE_NAME;Integrated Security=True;TrustServerCertificate=True;";
        
        // For LocalDB:
        // private readonly string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=YOUR_DATABASE_NAME;Integrated Security=True;TrustServerCertificate=True;";
        
        // For SQL Server Authentication:
        // private readonly string connectionString = "Data Source=YOUR_SERVER_NAME;Initial Catalog=YOUR_DATABASE_NAME;User ID=your_username;Password=your_password;TrustServerCertificate=True;";
        
        // Connection string from Server Explorer - Using correct database name
        private readonly string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=DB_RoomReservation_Dumadapat_IT13;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;";

        /// <summary>
        /// Adds a new reservation to the tblReservations table
        /// </summary>
        public async Task<int> AddReservationAsync(Reservation reservation)
        {
            try
            {
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                string query = @"
                    INSERT INTO [dbo].[tblReservations] 
                    (CustomerFname, CustomerLname, ContactNum, Email, RoomType, CheckInDate, CheckOutDate, Status, DateCreated, RoomPrice)
                    VALUES 
                    (@CustomerFname, @CustomerLname, @ContactNum, @Email, @RoomType, @CheckInDate, @CheckOutDate, @Status, @DateCreated, @RoomPrice);
                    SELECT CAST(SCOPE_IDENTITY() as int);";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CustomerFname", (object?)reservation.CustomerFname ?? DBNull.Value);
                command.Parameters.AddWithValue("@CustomerLname", (object?)reservation.CustomerLname ?? DBNull.Value);
                command.Parameters.AddWithValue("@ContactNum", (object?)reservation.ContactNum ?? DBNull.Value);
                command.Parameters.AddWithValue("@Email", (object?)reservation.Email ?? DBNull.Value);
                command.Parameters.AddWithValue("@RoomType", (object?)reservation.RoomType ?? DBNull.Value);
                command.Parameters.AddWithValue("@CheckInDate", (object?)reservation.CheckInDate ?? DBNull.Value);
                command.Parameters.AddWithValue("@CheckOutDate", (object?)reservation.CheckOutDate ?? DBNull.Value);
                command.Parameters.AddWithValue("@Status", (object?)reservation.Status ?? DBNull.Value);
                command.Parameters.AddWithValue("@DateCreated", (object?)reservation.DateCreated ?? DateTime.Now);
                command.Parameters.AddWithValue("@RoomPrice", reservation.RoomPrice);

                var result = await command.ExecuteScalarAsync();
                return result != null ? Convert.ToInt32(result) : 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding reservation: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Seeds 3 default users in the tblUsers table if they don't already exist
        /// </summary>
        public async Task SeedUsersAsync()
        {
            try
            {
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                // Check if users already exist
                string checkQuery = "SELECT COUNT(*) FROM [dbo].[tblUsers]";
                using var checkCommand = new SqlCommand(checkQuery, connection);
                var result = await checkCommand.ExecuteScalarAsync();
                var userCount = result != null ? (int)result : 0;

                // Only seed if table is empty
                if (userCount == 0)
                {
                    string insertQuery = @"
                        INSERT INTO [dbo].[tblUsers] (Fname, Lname, Cnumber, Email, Password)
                        VALUES 
                        (@Fname1, @Lname1, @Cnumber1, @Email1, @Password1),
                        (@Fname2, @Lname2, @Cnumber2, @Email2, @Password2),
                        (@Fname3, @Lname3, @Cnumber3, @Email3, @Password3);";

                    using var command = new SqlCommand(insertQuery, connection);

                    // User 1
                    command.Parameters.AddWithValue("@Fname1", "John");
                    command.Parameters.AddWithValue("@Lname1", "Doe");
                    command.Parameters.AddWithValue("@Cnumber1", "09123456789");
                    command.Parameters.AddWithValue("@Email1", "john.doe@vanderson.com");
                    command.Parameters.AddWithValue("@Password1", "admin123");

                    // User 2
                    command.Parameters.AddWithValue("@Fname2", "Jane");
                    command.Parameters.AddWithValue("@Lname2", "Smith");
                    command.Parameters.AddWithValue("@Cnumber2", "09987654321");
                    command.Parameters.AddWithValue("@Email2", "jane.smith@vanderson.com");
                    command.Parameters.AddWithValue("@Password2", "admin456");

                    // User 3
                    command.Parameters.AddWithValue("@Fname3", "Robert");
                    command.Parameters.AddWithValue("@Lname3", "Johnson");
                    command.Parameters.AddWithValue("@Cnumber3", "09555123456");
                    command.Parameters.AddWithValue("@Email3", "robert.johnson@vanderson.com");
                    command.Parameters.AddWithValue("@Password3", "admin789");

                    await command.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error seeding users: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets all reservations from the tblReservations table
        /// </summary>
        public async Task<List<Reservation>> GetAllReservationsAsync()
        {
            var reservations = new List<Reservation>();

            try
            {
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                string query = "SELECT * FROM [dbo].[tblReservations] ORDER BY DateCreated DESC";

                using var command = new SqlCommand(query, connection);
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    reservations.Add(new Reservation
                    {
                        ReservationID = reader.GetInt32("ReservationID"),
                        CustomerFname = reader.IsDBNull("CustomerFname") ? null : reader.GetString("CustomerFname"),
                        CustomerLname = reader.IsDBNull("CustomerLname") ? null : reader.GetString("CustomerLname"),
                        ContactNum = reader.IsDBNull("ContactNum") ? null : reader.GetString("ContactNum"),
                        Email = reader.IsDBNull("Email") ? null : reader.GetString("Email"),
                        RoomType = reader.IsDBNull("RoomType") ? null : reader.GetString("RoomType"),
                        CheckInDate = reader.IsDBNull("CheckInDate") ? null : reader.GetDateTime("CheckInDate"),
                        CheckOutDate = reader.IsDBNull("CheckOutDate") ? null : reader.GetDateTime("CheckOutDate"),
                        Status = reader.IsDBNull("Status") ? null : reader.GetString("Status"),
                        DateCreated = reader.IsDBNull("DateCreated") ? null : reader.GetDateTime("DateCreated"),
                        RoomPrice = reader.GetDecimal("RoomPrice")
                    });
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting reservations: {ex.Message}", ex);
            }

            return reservations;
        }

        /// <summary>
        /// Updates an existing reservation in the tblReservations table
        /// </summary>
        public async Task<bool> UpdateReservationAsync(Reservation reservation)
        {
            try
            {
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                string query = @"
                    UPDATE [dbo].[tblReservations] 
                    SET CustomerFname = @CustomerFname,
                        CustomerLname = @CustomerLname,
                        ContactNum = @ContactNum,
                        Email = @Email,
                        RoomType = @RoomType,
                        CheckInDate = @CheckInDate,
                        CheckOutDate = @CheckOutDate,
                        Status = @Status,
                        RoomPrice = @RoomPrice
                    WHERE ReservationID = @ReservationID;";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ReservationID", reservation.ReservationID);
                command.Parameters.AddWithValue("@CustomerFname", (object?)reservation.CustomerFname ?? DBNull.Value);
                command.Parameters.AddWithValue("@CustomerLname", (object?)reservation.CustomerLname ?? DBNull.Value);
                command.Parameters.AddWithValue("@ContactNum", (object?)reservation.ContactNum ?? DBNull.Value);
                command.Parameters.AddWithValue("@Email", (object?)reservation.Email ?? DBNull.Value);
                command.Parameters.AddWithValue("@RoomType", (object?)reservation.RoomType ?? DBNull.Value);
                command.Parameters.AddWithValue("@CheckInDate", (object?)reservation.CheckInDate ?? DBNull.Value);
                command.Parameters.AddWithValue("@CheckOutDate", (object?)reservation.CheckOutDate ?? DBNull.Value);
                command.Parameters.AddWithValue("@Status", (object?)reservation.Status ?? DBNull.Value);
                command.Parameters.AddWithValue("@RoomPrice", reservation.RoomPrice);

                int rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating reservation: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets all removed/cancelled reservations from the tblRemovedReservation table
        /// </summary>
        public async Task<List<RemovedReservation>> GetAllRemovedReservationsAsync()
        {
            var removedReservations = new List<RemovedReservation>();

            try
            {
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                string query = @"
                    SELECT r.*, u.Fname + ' ' + u.Lname AS AdminName
                    FROM [dbo].[tblRemovedReservations] r
                    LEFT JOIN [dbo].[tblUsers] u ON r.RemovedBy = CAST(u.AdminID AS VARCHAR(50))
                    ORDER BY r.DateRemoved DESC";

                using var command = new SqlCommand(query, connection);
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    removedReservations.Add(new RemovedReservation
                    {
                        RemovedID = reader.GetInt32("RemovedID"),
                        ReservationID = reader.IsDBNull("ReservationID") ? null : reader.GetInt32("ReservationID"),
                        CustomerFname = reader.IsDBNull("CustomerFname") ? null : reader.GetString("CustomerFname"),
                        CustomerLname = reader.IsDBNull("CustomerLname") ? null : reader.GetString("CustomerLname"),
                        ContactNum = reader.IsDBNull("ContactNum") ? null : reader.GetString("ContactNum"),
                        Email = reader.IsDBNull("Email") ? null : reader.GetString("Email"),
                        RoomType = reader.IsDBNull("RoomType") ? null : reader.GetString("RoomType"),
                        CheckInDate = reader.IsDBNull("CheckinDate") ? null : reader.GetDateTime("CheckinDate"),
                        CheckOutDate = reader.IsDBNull("CheckOutDate") ? null : reader.GetDateTime("CheckOutDate"),
                        Status = reader.IsDBNull("Status") ? null : reader.GetString("Status"),
                        ReasonNote = reader.IsDBNull("ReasonNote") ? null : reader.GetString("ReasonNote"),
                        RemovedBy = reader.IsDBNull("RemovedBy") ? null : reader.GetString("RemovedBy"),
                        DateRemoved = reader.IsDBNull("DateRemoved") ? null : reader.GetDateTime("DateRemoved"),
                        RoomPrice = reader.IsDBNull("RoomPrice") ? 0 : reader.GetDecimal("RoomPrice"),
                        AdminName = reader.IsDBNull("AdminName") ? null : reader.GetString("AdminName")
                    });
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting removed reservations: {ex.Message}", ex);
            }

            return removedReservations;
        }

        /// <summary>
        /// Adds a removed reservation to the tblRemovedReservation table
        /// </summary>
        public async Task<int> AddRemovedReservationAsync(RemovedReservation removedReservation)
        {
            try
            {
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                string query = @"
                    INSERT INTO [dbo].[tblRemovedReservations] 
                    (ReservationID, CustomerFname, CustomerLname, ContactNum, Email, RoomType, CheckinDate, CheckOutDate, Status, ReasonNote, RemovedBy, DateRemoved, RoomPrice)
                    VALUES 
                    (@ReservationID, @CustomerFname, @CustomerLname, @ContactNum, @Email, @RoomType, @CheckinDate, @CheckOutDate, @Status, @ReasonNote, @RemovedBy, @DateRemoved, @RoomPrice);
                    SELECT CAST(SCOPE_IDENTITY() as int);";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ReservationID", (object?)removedReservation.ReservationID ?? DBNull.Value);
                command.Parameters.AddWithValue("@CustomerFname", (object?)removedReservation.CustomerFname ?? DBNull.Value);
                command.Parameters.AddWithValue("@CustomerLname", (object?)removedReservation.CustomerLname ?? DBNull.Value);
                command.Parameters.AddWithValue("@ContactNum", (object?)removedReservation.ContactNum ?? DBNull.Value);
                command.Parameters.AddWithValue("@Email", (object?)removedReservation.Email ?? DBNull.Value);
                command.Parameters.AddWithValue("@RoomType", (object?)removedReservation.RoomType ?? DBNull.Value);
                command.Parameters.AddWithValue("@CheckinDate", (object?)removedReservation.CheckInDate ?? DBNull.Value);
                command.Parameters.AddWithValue("@CheckOutDate", (object?)removedReservation.CheckOutDate ?? DBNull.Value);
                command.Parameters.AddWithValue("@Status", (object?)removedReservation.Status ?? DBNull.Value);
                command.Parameters.AddWithValue("@ReasonNote", (object?)removedReservation.ReasonNote ?? DBNull.Value);
                command.Parameters.AddWithValue("@RemovedBy", (object?)removedReservation.RemovedBy ?? DBNull.Value);
                command.Parameters.AddWithValue("@DateRemoved", (object?)removedReservation.DateRemoved ?? DateTime.Now);
                command.Parameters.AddWithValue("@RoomPrice", removedReservation.RoomPrice);

                var result = await command.ExecuteScalarAsync();
                return result != null ? Convert.ToInt32(result) : 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding removed reservation: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Permanently deletes a removed reservation from tblRemovedReservations by RemovedID
        /// </summary>
        public async Task<bool> DeleteRemovedReservationAsync(int removedId)
        {
            try
            {
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                string query = "DELETE FROM [dbo].[tblRemovedReservations] WHERE RemovedID = @RemovedID";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@RemovedID", removedId);

                int rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting removed reservation: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Permanently deletes multiple removed reservations from tblRemovedReservations by RemovedIDs
        /// </summary>
        public async Task<int> DeleteMultipleRemovedReservationsAsync(List<int> removedIds)
        {
            if (removedIds == null || removedIds.Count == 0)
                return 0;

            try
            {
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                // Create parameterized query for multiple IDs
                var parameters = string.Join(",", removedIds.Select((_, index) => $"@id{index}"));
                string query = $"DELETE FROM [dbo].[tblRemovedReservations] WHERE RemovedID IN ({parameters})";

                using var command = new SqlCommand(query, connection);
                for (int i = 0; i < removedIds.Count; i++)
                {
                    command.Parameters.AddWithValue($"@id{i}", removedIds[i]);
                }

                int rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting removed reservations: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Authenticates a user by email and password from tblUsers
        /// </summary>
        public async Task<Admin?> AuthenticateUserAsync(string email, string password)
        {
            try
            {
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                string query = "SELECT * FROM [dbo].[tblUsers] WHERE Email = @Email AND Password = @Password";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@Password", password);

                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new Admin
                    {
                        AdminID = reader.GetInt32("AdminID"),
                        Fname = reader.IsDBNull("Fname") ? null : reader.GetString("Fname"),
                        Lname = reader.IsDBNull("Lname") ? null : reader.GetString("Lname"),
                        Cnumber = reader.IsDBNull("Cnumber") ? null : reader.GetString("Cnumber"),
                        Email = reader.IsDBNull("Email") ? null : reader.GetString("Email"),
                        Password = reader.IsDBNull("Password") ? null : reader.GetString("Password")
                    };
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error authenticating user: {ex.Message}", ex);
            }

            return null;
        }

        /// <summary>
        /// Deletes a reservation from tblReservations by ReservationID
        /// </summary>
        public async Task<bool> DeleteReservationAsync(int reservationId)
        {
            try
            {
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                string query = "DELETE FROM [dbo].[tblReservations] WHERE ReservationID = @ReservationID";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ReservationID", reservationId);

                int rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting reservation: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets all users from tblUsers
        /// </summary>
        public async Task<List<Admin>> GetAllUsersAsync()
        {
            var users = new List<Admin>();

            try
            {
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                string query = "SELECT * FROM [dbo].[tblUsers]";

                using var command = new SqlCommand(query, connection);
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    users.Add(new Admin
                    {
                        AdminID = reader.GetInt32("AdminID"),
                        Fname = reader.IsDBNull("Fname") ? null : reader.GetString("Fname"),
                        Lname = reader.IsDBNull("Lname") ? null : reader.GetString("Lname"),
                        Cnumber = reader.IsDBNull("Cnumber") ? null : reader.GetString("Cnumber"),
                        Email = reader.IsDBNull("Email") ? null : reader.GetString("Email"),
                        Password = reader.IsDBNull("Password") ? null : reader.GetString("Password")
                    });
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting users: {ex.Message}", ex);
            }

            return users;
        }

        /// <summary>
        /// Gets a user by AdminID from tblUsers
        /// </summary>
        public async Task<Admin?> GetUserByIdAsync(int adminId)
        {
            try
            {
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                string query = "SELECT * FROM [dbo].[tblUsers] WHERE AdminID = @AdminID";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@AdminID", adminId);

                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new Admin
                    {
                        AdminID = reader.GetInt32("AdminID"),
                        Fname = reader.IsDBNull("Fname") ? null : reader.GetString("Fname"),
                        Lname = reader.IsDBNull("Lname") ? null : reader.GetString("Lname"),
                        Cnumber = reader.IsDBNull("Cnumber") ? null : reader.GetString("Cnumber"),
                        Email = reader.IsDBNull("Email") ? null : reader.GetString("Email"),
                        Password = reader.IsDBNull("Password") ? null : reader.GetString("Password")
                    };
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting user: {ex.Message}", ex);
            }

            return null;
        }
    }
}

