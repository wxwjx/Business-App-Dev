using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Script.Serialization;

namespace Business_App_Dev.Services
{
    public static class EmbeddingStore
    {
        // Reads from dbo.ProductAIEmbeddings: (ProductId, VectorJson, Dim, UpdatedAt)
        public static Dictionary<int, float[]> GetProductEmbeddings(string connStr, IEnumerable<int> productIds)
        {
            var ids = (productIds ?? Enumerable.Empty<int>()).Distinct().ToList();
            var result = new Dictionary<int, float[]>();
            if (ids.Count == 0) return result;

            // Build safe IN clause params: @p0,@p1,...
            var paramNames = ids.Select((id, i) => "@p" + i).ToList();

            string sql = $@"
SELECT ProductId, VectorJson
FROM dbo.ProductAIEmbeddings
WHERE ProductId IN ({string.Join(",", paramNames)});
";

            using (var cn = new SqlConnection(connStr))
            using (var cmd = new SqlCommand(sql, cn))
            {
                for (int i = 0; i < ids.Count; i++)
                    cmd.Parameters.AddWithValue(paramNames[i], ids[i]);

                cn.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    var js = new JavaScriptSerializer();

                    while (rd.Read())
                    {
                        int pid = Convert.ToInt32(rd["ProductId"]);
                        string json = rd["VectorJson"]?.ToString() ?? "";

                        if (string.IsNullOrWhiteSpace(json)) continue;

                        try
                        {
                            // stored as JSON array: [0.12, -0.04, ...]
                            var vec = js.Deserialize<List<double>>(json)
                                       .Select(x => (float)x)
                                       .ToArray();

                            if (vec.Length > 0)
                                result[pid] = vec;
                        }
                        catch
                        {
                            // ignore bad row
                        }
                    }
                }
            }

            return result;
        }
    }
}