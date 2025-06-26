# Thermocron Dashboard

Application Blazor WebAssembly pour visualiser les données de température collectées par le job Thermocron.

## Architecture

- **ThermocronApi** : API REST backend (ASP.NET Core)
- **ThermocronWeb** : Frontend Blazor WebAssembly avec graphiques ApexCharts
- **Thermocron** : Job de collecte de données (existant)

## Prérequis

- .NET 8.0 SDK
- PostgreSQL (via Docker)
- Base de données avec des données de température

## Démarrage

### 1. Démarrer la base de données

```bash
cd Thermocron
docker-compose up -d db
```

### 2. Démarrer l'API

```bash
cd ThermocronApi
dotnet run
```

L'API sera disponible sur :
- HTTPS: https://localhost:7001
- HTTP: http://localhost:5001
- Swagger: https://localhost:7001/swagger

### 3. Démarrer le frontend

```bash
cd ThermocronWeb
dotnet run
```

Le frontend sera disponible sur :
- HTTPS: https://localhost:7000
- HTTP: http://localhost:5000

## Fonctionnalités

### Dashboard Principal
- Sélection de capteur (tous ou spécifique)
- Sélection de période (heure, jour, semaine, mois)
- Sélection d'intervalle d'agrégation (brut, heure, jour)
- Statistiques résumées (min, max, moyenne)

### Graphiques Interactifs
- Courbes de température mesurée, cible et extérieure
- Zoom et navigation temporelle
- Tooltips détaillés
- Export PNG/SVG/CSV
- Légende interactive

## API Endpoints

### Devices
- `GET /api/device` - Liste des capteurs

### Temperature
- `GET /api/temperature/measures` - Mesures de température
  - `?deviceId={id}` - Filtrer par capteur
  - `?from={date}&to={date}` - Période
  - `?interval={hour|day|raw}` - Agrégation
- `GET /api/temperature/latest` - Dernière mesure
- `GET /api/temperature/stats` - Statistiques

## Configuration

### API (ThermocronApi/appsettings.json)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost:15432;Database=thermocron_dev;Username=thermocron_dev;Password=thermocron"
  }
}
```

### Frontend (ThermocronWeb/Services/ApiService.cs)
```csharp
_httpClient.BaseAddress = new Uri("https://localhost:7001/api/");
```

## Développement

### Structure des projets

```
ThermocronApi/
├── Controllers/          # Contrôleurs API
├── Services/             # Logique métier
├── Data/                 # Modèles de données
├── DTOs/                 # Objets de transfert
└── Program.cs            # Configuration

ThermocronWeb/
├── Components/Pages/     # Pages Blazor
├── Services/             # Services API
├── Models/               # DTOs partagés
└── Program.cs            # Configuration
```

### Ajout de nouvelles fonctionnalités

1. **Nouveau endpoint API** : Ajouter dans `Controllers/`
2. **Nouvelle page** : Créer dans `Components/Pages/`
3. **Nouveau service** : Ajouter dans `Services/`

## Troubleshooting

### Erreur de connexion API
- Vérifier que l'API est démarrée sur le bon port
- Vérifier la configuration CORS dans `Program.cs`

### Pas de données
- Vérifier que la base de données contient des mesures
- Vérifier la chaîne de connexion
- Lancer le job Thermocron pour collecter des données

### Erreur de compilation
- Vérifier que tous les packages NuGet sont installés
- Nettoyer et rebuilder : `dotnet clean && dotnet build`
