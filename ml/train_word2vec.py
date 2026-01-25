import json
from collections import defaultdict

import pyodbc
from gensim.models import Word2Vec

# ✅ MDF path (yours)
MDF_PATH = r"C:\Users\wanju\source\repos\wxwjx\Business-App-Dev\App_Data\EcoEats.mdf"

# ✅ LocalDB attach connection
CONN_STR = fr"""
Driver={{ODBC Driver 17 for SQL Server}};
Server=(LocalDB)\MSSQLLocalDB;
AttachDbFilename={MDF_PATH};
Trusted_Connection=yes;
"""

VECTOR_DIM = 64


def main():
    cn = pyodbc.connect(CONN_STR)
    cur = cn.cursor()

    # ✅ Train on ALL orders (no PayStatus filter)
    cur.execute("""
        SELECT oi.OrderID, oi.ProductID
        FROM dbo.Orders o
        JOIN dbo.OrderItems oi ON oi.OrderID = o.OrderID
        ORDER BY oi.OrderID
    """)

    # Build baskets: each order = list of productIDs
    baskets = defaultdict(list)
    rows = cur.fetchall()
    for order_id, product_id in rows:
        baskets[order_id].append(str(product_id))

    sentences = list(baskets.values())

    # Basic sanity checks
    if len(sentences) < 5:
        print("⚠️ Not enough orders to train Word2Vec (need at least ~5 orders).")
        cur.close()
        cn.close()
        return

    unique_products = set()
    for s in sentences:
        unique_products.update(s)

    print(f"📦 Orders used for training: {len(sentences)}")
    print(f"🍱 Unique products seen in orders: {len(unique_products)}")

    # Train Word2Vec
    model = Word2Vec(
        sentences=sentences,
        vector_size=VECTOR_DIM,
        window=5,
        min_count=1,   # keep even rare products (must appear at least once)
        workers=4,
        sg=1,          # skip-gram
        epochs=30
    )

    # Save vectors into SQL
    # If your table is dbo.ProductEmbeddings, keep it explicit.
    cur.execute("DELETE FROM dbo.ProductEmbeddings")
    cn.commit()

    insert_sql = "INSERT INTO dbo.ProductEmbeddings(ProductId, VectorJson, Dim) VALUES (?, ?, ?)"

    count = 0
    for key in model.wv.index_to_key:
        pid = int(key)
        vec = model.wv[key].tolist()
        cur.execute(insert_sql, pid, json.dumps(vec), VECTOR_DIM)
        count += 1

    cn.commit()
    cur.close()
    cn.close()

    print(f"✅ Done. Stored embeddings for {count} products in dbo.ProductEmbeddings.")


if __name__ == "__main__":
    main()
