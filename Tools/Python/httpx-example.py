"""
httpx — modern HTTP client. Async-capable; requests-like sync API.

uv add httpx
"""
import asyncio
import httpx


def fetch_sync(url: str) -> dict:
    with httpx.Client(timeout=5.0, follow_redirects=True) as client:
        r = client.get(url, headers={"Accept": "application/json"})
        r.raise_for_status()
        return r.json()


async def fetch_many(urls: list[str]) -> list[dict]:
    async with httpx.AsyncClient(timeout=5.0) as client:
        tasks = [client.get(u) for u in urls]
        responses = await asyncio.gather(*tasks, return_exceptions=True)
        results: list[dict] = []
        for r in responses:
            if isinstance(r, Exception):
                continue
            r.raise_for_status()
            results.append(r.json())
        return results


def post_with_retry(url: str, body: dict, idempotency_key: str) -> dict:
    transport = httpx.HTTPTransport(retries=3)
    with httpx.Client(transport=transport, timeout=10.0) as client:
        r = client.post(url, json=body, headers={"Idempotency-Key": idempotency_key})
        r.raise_for_status()
        return r.json()


if __name__ == "__main__":
    print(fetch_sync("https://httpbin.org/get"))
    print(asyncio.run(fetch_many(["https://httpbin.org/get?a=1", "https://httpbin.org/get?a=2"])))
