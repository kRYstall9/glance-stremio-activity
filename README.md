# GlanceStremioActivity

A [Glance](https://github.com/glanceapp/glance) widget that displays your Stremio watching activity.

> [!NOTE] 
> This service requires your Stremio credentials (email and password) to authenticate with Stremio's API and fetch your activity. Since the service is **self-hosted**, your credentials never leave your machine.

---

## Configuration

### Users Configuration

The service loads Stremio credentials in order of priority:

1. **Environment variable `STREMIO_USERS`** (checked first — recommended for Docker)
2. **File `users.json`** (fallback)

#### Option 1 — Environment Variable

Set `STREMIO_USERS` with a JSON string:

    export STREMIO_USERS='{"Users":[{"Email":"user@example.com","Password":"pass","DisplayName":"User 1"}]}'

#### Option 2 — JSON File

Create a `users.json` file in the application root:

    {
      "Users": [
        {
          "Email": "your-email@example.com",
          "Password": "your-password",
          "DisplayName": "User 1"
        },
        {
          "Email": "another-email@example.com",
          "Password": "another-password",
          "DisplayName": "User 2"
        }
      ]
    }

> `DisplayName` is optional.
> Make sure `users.json` is in `.gitignore` to avoid committing credentials.

### Other Environment Variables

| Variable | Description | Default |
|---|---|---|
| `API_KEY` | **Required.** API key for securing the `/activity` endpoint | — |
| `UsersFilePath` | Custom path to the users JSON file | `users.json` |

---

## Docker Deployment

### Docker Compose (recommended)

A ready-to-use `docker-compose.yml` is included in the repository.

**Option A — Credentials via environment variable:**

    services:
      glance-stremio-activity:
        build: .
        ports:
          - "5000:8080"
        environment:
          - API_KEY=your-secret-api-key
          - STREMIO_USERS={"Users":[{"Email":"user@example.com","Password":"pass","DisplayName":"User 1"}]}

Then run:

    docker compose up -d

**Option B — Credentials via volume mount:**

Create a `users.json` file on your host, then mount it read-only:

    services:
      glance-stremio-activity:
        build: .
        ports:
          - "5000:8080"
        environment:
          - API_KEY=your-secret-api-key
        volumes:
          - ./users.json:/app/users.json:ro

Then run:

    docker compose up -d

### Docker Run

**With environment variable:**

    docker run -d \
      -p 5000:8080 \
      -e API_KEY=your-secret-api-key \
      -e 'STREMIO_USERS={"Users":[{"Email":"user@example.com","Password":"pass"}]}' \
      glance-stremio-activity

**With volume mount:**

    docker run -d \
      -p 5000:8080 \
      -e API_KEY=your-secret-api-key \
      -v $(pwd)/users.json:/app/users.json:ro \
      glance-stremio-activity

---

## API Endpoints

### GET `/activity`

Fetches the watching activity for all configured users.

**Query Parameters:**
- `type`: Activity type — `0` for Watching, `1` for Last Show Watched

**Headers:**
- `token`: API key

**Example:**

    curl -H "token: your-api-key" "http://localhost:5287/activity?type=0"
