using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;

namespace Business_App_Dev.Services
{
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ScriptService]
    public class AdminAnalytics : WebService
    {
        public class GrowthResult
        {
            public string[] labels;

            // Users
            public int[] monthlyUsers;
            public int[] cumulativeUsers;
            public int premiumUsers;
            public int nonPremiumUsers;

            // Sellers - Approved
            public int[] monthlyApprovedSellers;
            public int[] cumulativeApprovedSellers;

            // Sellers - Rejected
            public int[] monthlyRejectedSellers;
            public int[] cumulativeRejectedSellers;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public GrowthResult GetGrowthData(int months)
        {
            var role = HttpContext.Current?.Session?["UserRole"]?.ToString();
            if (!string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase))
                throw new HttpException(401, "Unauthorized");

            if (months <= 0) months = 6;
            if (months > 24) months = 24;

            string connStr = ConfigurationManager.ConnectionStrings["EcoEatsDb"].ConnectionString;

            // Labels
            List<string> labels = new List<string>();
            DateTime start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-(months - 1));
            for (int i = 0; i < months; i++)
                labels.Add(start.AddMonths(i).ToString("yyyy-MM"));

            var usersMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            var approvedMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            var rejectedMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            int premiumUsers = 0, nonPremiumUsers = 0;

            string sqlUsersMonthly = @"
WITH m AS (
  SELECT DATEADD(MONTH, -(@months-1), DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)) AS StartMonth
)
SELECT FORMAT(DATEFROMPARTS(YEAR(MemberSince), MONTH(MemberSince), 1), 'yyyy-MM') AS YM,
       COUNT(*) AS Cnt
FROM Users
WHERE MemberSince IS NOT NULL
  AND MemberSince >= (SELECT StartMonth FROM m)
GROUP BY FORMAT(DATEFROMPARTS(YEAR(MemberSince), MONTH(MemberSince), 1), 'yyyy-MM')
ORDER BY YM;";

            string sqlUsersPremiumSplit = @"
WITH m AS (
  SELECT DATEADD(MONTH, -(@months-1), DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)) AS StartMonth
)
SELECT IsPremium, COUNT(*) AS Cnt
FROM Users
WHERE MemberSince IS NOT NULL
  AND MemberSince >= (SELECT StartMonth FROM m)
GROUP BY IsPremium;";

            string sqlApprovedMonthly = @"
WITH m AS (
  SELECT DATEADD(MONTH, -(@months-1), DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)) AS StartMonth
)
SELECT FORMAT(DATEFROMPARTS(YEAR(ApprovedAt), MONTH(ApprovedAt), 1), 'yyyy-MM') AS YM,
       COUNT(*) AS Cnt
FROM SellerApplications
WHERE Status = 'APPROVED'
  AND ApprovedAt IS NOT NULL
  AND ApprovedAt >= (SELECT StartMonth FROM m)
GROUP BY FORMAT(DATEFROMPARTS(YEAR(ApprovedAt), MONTH(ApprovedAt), 1), 'yyyy-MM')
ORDER BY YM;";

            string sqlRejectedMonthly = @"
WITH m AS (
  SELECT DATEADD(MONTH, -(@months-1), DATEFROMPARTS(YEAR(GETDATE()), MONTH(GETDATE()), 1)) AS StartMonth
)
SELECT FORMAT(DATEFROMPARTS(YEAR(RejectedAt), MONTH(RejectedAt), 1), 'yyyy-MM') AS YM,
       COUNT(*) AS Cnt
FROM SellerApplications
WHERE Status = 'REJECTED'
  AND RejectedAt IS NOT NULL
  AND RejectedAt >= (SELECT StartMonth FROM m)
GROUP BY FORMAT(DATEFROMPARTS(YEAR(RejectedAt), MONTH(RejectedAt), 1), 'yyyy-MM')
ORDER BY YM;";

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                // monthly users
                using (SqlCommand cmd = new SqlCommand(sqlUsersMonthly, conn))
                {
                    cmd.Parameters.AddWithValue("@months", months);
                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                            usersMap[Convert.ToString(r["YM"])] = Convert.ToInt32(r["Cnt"]);
                    }
                }

                // premium split (within range)
                using (SqlCommand cmd = new SqlCommand(sqlUsersPremiumSplit, conn))
                {
                    cmd.Parameters.AddWithValue("@months", months);
                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            bool isPrem = Convert.ToBoolean(r["IsPremium"]);
                            int cnt = Convert.ToInt32(r["Cnt"]);
                            if (isPrem) premiumUsers = cnt;
                            else nonPremiumUsers = cnt;
                        }
                    }
                }

                // approved sellers
                using (SqlCommand cmd = new SqlCommand(sqlApprovedMonthly, conn))
                {
                    cmd.Parameters.AddWithValue("@months", months);
                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                            approvedMap[Convert.ToString(r["YM"])] = Convert.ToInt32(r["Cnt"]);
                    }
                }

                // rejected sellers
                using (SqlCommand cmd = new SqlCommand(sqlRejectedMonthly, conn))
                {
                    cmd.Parameters.AddWithValue("@months", months);
                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                            rejectedMap[Convert.ToString(r["YM"])] = Convert.ToInt32(r["Cnt"]);
                    }
                }
            }

            int[] monthlyUsers = labels.Select(m => usersMap.TryGetValue(m, out int v) ? v : 0).ToArray();
            int uRun = 0;
            int[] cumulativeUsers = monthlyUsers.Select(x => uRun += x).ToArray();

            int[] monthlyApproved = labels.Select(m => approvedMap.TryGetValue(m, out int v) ? v : 0).ToArray();
            int aRun = 0;
            int[] cumulativeApproved = monthlyApproved.Select(x => aRun += x).ToArray();

            int[] monthlyRejected = labels.Select(m => rejectedMap.TryGetValue(m, out int v) ? v : 0).ToArray();
            int rRun = 0;
            int[] cumulativeRejected = monthlyRejected.Select(x => rRun += x).ToArray();

            return new GrowthResult
            {
                labels = labels.ToArray(),

                monthlyUsers = monthlyUsers,
                cumulativeUsers = cumulativeUsers,
                premiumUsers = premiumUsers,
                nonPremiumUsers = nonPremiumUsers,

                monthlyApprovedSellers = monthlyApproved,
                cumulativeApprovedSellers = cumulativeApproved,

                monthlyRejectedSellers = monthlyRejected,
                cumulativeRejectedSellers = cumulativeRejected
            };
        }
    }
}