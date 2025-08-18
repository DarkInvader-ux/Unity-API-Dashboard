import sqlite3
import json
import os

BASE_DIR = os.path.dirname(__file__)
CONFIG_FILE = os.path.join(BASE_DIR, "config.json")

with open(CONFIG_FILE, "r") as f:
    config = json.load(f)

JSON_FILE = os.path.join(BASE_DIR, config["json_file"])
DB_FILE = os.path.join(BASE_DIR, config["db_file"])


def create_connection(db_file):
    """Create a database connection to SQLite."""
    try:
        conn = sqlite3.connect(db_file)
        print(f"Connected to database: {db_file}")
        return conn
    except sqlite3.Error as e:
        print(f"DB connection error: {e}")
        return None


def create_table(conn):
    """Create the GameEvents table if it doesn't exist."""
    create_table_sql = """
    CREATE TABLE IF NOT EXISTS GameEvents (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        playerId TEXT,
        eventType TEXT,
        timestamp TEXT,
        posX REAL,
        posY REAL,
        posZ REAL,
        metadata TEXT
    );
    """
    cursor = conn.cursor()
    cursor.execute(create_table_sql)
    conn.commit()


def insert_event(conn, event):
    """Insert a single event into the GameEvents table."""
    sql = """
    INSERT INTO GameEvents (playerId, eventType, timestamp, posX, posY, posZ, metadata)
    VALUES (?, ?, ?, ?, ?, ?, ?)
    """
    cursor = conn.cursor()
    cursor.execute(sql, (
        event.get("playerId"),
        event.get("eventType"),
        event.get("timestamp"),
        event["position"].get("x") if event.get("position") else None,
        event["position"].get("y") if event.get("position") else None,
        event["position"].get("z") if event.get("position") else None,
        json.dumps(event.get("metadata", {}))
    ))
    conn.commit()


def etl_process():
    """Extract events from Unity JSON, transform, and load into SQLite."""
    if not os.path.exists(JSON_FILE):
        print(f"No JSON file found at {JSON_FILE}")
        return

    conn = create_connection(DB_FILE)
    if conn is None:
        return

    create_table(conn)

    try:
        with open(JSON_FILE, "r") as f:
            data = json.load(f)

        events = data.get("events", [])
        if not events:
            print("No events found in the JSON file")
            conn.close()
            return

        print(f"Found {len(events)} events to process")

        for i, event in enumerate(events):
            try:
                insert_event(conn, event)
            except Exception as e:
                print(f"Error inserting event {i + 1}: {e}")
                print(f"Event data: {event}")

        conn.close()
        print(f"ETL complete ✅ {len(events)} events inserted into {DB_FILE}")

    except json.JSONDecodeError as e:
        print(f"Error parsing JSON file: {e}")
        conn.close()
    except Exception as e:
        print(f"Unexpected error: {e}")
        conn.close()


if __name__ == "__main__":
    etl_process()
