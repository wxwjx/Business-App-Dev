using System;
using System.Configuration;
using System.Data.SqlClient;

public static class NotificationHelper
{
    private static readonly string ConnStr =
        ConfigurationManager.ConnectionStrings["EcoEatsDb"].ConnectionString;

    public static void Add(string type, string title, string message, string linkTab, int? refId = null)
    {
        using (SqlConnection conn = new SqlConnection(ConnStr))
        using (SqlCommand cmd = new SqlCommand(@"
            INSERT INTO AdminNotifications (Type, Title, Message, LinkTab, RefId)
            VALUES (@Type, @Title, @Message, @LinkTab, @RefId);", conn))
        {
            cmd.Parameters.AddWithValue("@Type", type);
            cmd.Parameters.AddWithValue("@Title", title);
            cmd.Parameters.AddWithValue("@Message", (object)message ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@LinkTab", (object)linkTab ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@RefId", (object)refId ?? DBNull.Value);

            conn.Open();
            cmd.ExecuteNonQuery();
        }
    }

    public static int GetUnreadCount()
    {
        using (SqlConnection conn = new SqlConnection(ConnStr))
        using (SqlCommand cmd = new SqlCommand(
            "SELECT COUNT(*) FROM AdminNotifications WHERE IsRead = 0;", conn))
        {
            conn.Open();
            return Convert.ToInt32(cmd.ExecuteScalar());
        }
    }
}
