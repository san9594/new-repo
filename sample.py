import asyncio
import aiohttp
import json
import logging
import os
import sys
from datetime import datetime, timedelta
from pathlib import Path

CACHE_FILE = Path("./cache.json")
CACHE_TTL = timedelta(minutes=10)

logging.basicConfig(
    level=logging.INFO,
    format="%(levelname)s | %(asctime)s | %(message)s",
    datefmt="%Y-%m-%d %H:%M:%S",
)

API_ENDPOINTS = {
    "ip": "https://api.ipify.org?format=json",
    "time": "https://worldtimeapi.org/api/ip",
    "btc": "https://api.coindesk.com/v1/bpi/currentprice/BTC.json",
}

# ------------------------------
# Caching Layer
# ------------------------------

def load_cache():
    if not CACHE_FILE.exists():
        return {}
    try:
        with open(CACHE_FILE, "r") as f:
            data = json.load(f)
        ts = datetime.fromisoformat(data.get("timestamp"))
        if datetime.utcnow() - ts > CACHE_TTL:
            logging.info("Cache expired.")
            return {}
        return data
    except Exception:
        return {}

def save_cache(payload):
    data = {
        "timestamp": datetime.utcnow().isoformat(),
        "payload": payload
    }
    with open(CACHE_FILE, "w") as f:
        json.dump(data, f, indent=2)
    logging.info("Cache updated.")

# ------------------------------
# Async API Fetching
# ------------------------------

async def fetch(session, name, url):
    try:
        async with session.get(url, timeout=5) as r:
            r.raise_for_status()
            return name, await r.json()
    except Exception as e:
        logging.error(f"Failed fetching {name}: {e}")
        return name, None

async def fetch_all():
    async with aiohttp.ClientSession() as session:
        tasks = [fetch(session, name, url) for name, url in API_ENDPOINTS.items()]
        results = await asyncio.gather(*tasks)
        return {name: data for name, data in results}

# ------------------------------
# Main Logic
# ------------------------------

def summarize(data):
    """Create a useful summary from mixed API results."""
    return {
        "your_ip": data.get("ip", {}).get("ip"),
        "local_time": data.get("time", {}).get("datetime"),
        "btc_price_usd": (
            data.get("btc", {})
                .get("bpi", {})
                .get("USD", {})
                .get("rate")
        )
    }

def run(force_refresh=False):
    cache = load_cache()

    if cache and not force_refresh:
        logging.info("Using cached data.")
        payload = cache["payload"]
    else:
        logging.info("Fetching fresh data...")
        payload = asyncio.run(fetch_all())
        save_cache(payload)

    summary = summarize(payload)
    print("\n=== Summary ===")
    print(json.dumps(summary, indent=2))

# ------------------------------
# CLI Entry Point
# ------------------------------

def main():
    if len(sys.argv) > 1 and sys.argv[1] == "--refresh":
        run(force_refresh=True)
    else:
        run()

if __name__ == "__main__":
    main()
