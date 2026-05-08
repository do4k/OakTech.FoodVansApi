# Food Vans API & Slack Notifier

A .NET solution with a food van API and scheduled Slack notifier.

## Projects

- **OakTech.FoodVansApi** - Web API serving food van data
- **OakTech.FoodVansLib** - Shared library (Services, Models, Extensions, Options)
- **OakTech.FoodVansSlackNotifier** - Coravel-based scheduler that sends Slack notifications on weekdays at 8am

## Setup

```bash
cd src
dotnet restore
dotnet build
```

## Configuration

Edit `.env` with your secrets:

```
SlackWebhookPath=your-slack-webhook-path
FoodVanApiUrl=http://localhost:8080
```

## Docker

Build and run:

```bash
docker compose up -d
```

Logs:

```bash
docker compose logs -f slack-notifier
```

## Development

```bash
# Run API
dotnet run --project OakTech.FoodVansApi

# Run Notifier
dotnet run --project OakTech.FoodVansSlackNotifier
```