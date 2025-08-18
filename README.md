# Unity API Dashboard

## Overview
Unity API Dashboard is a hybrid **Unity + .NET project** that combines **real-time API integration** with **interactive visualization**.  
It was designed as both a **gameplay prototype** and a **data engineering showcase**, featuring an internal **ETL pipeline** (Extract, Transform, Load) for external API data.

- **Extract**: Fetch event data from APIs using `EventsApiClient` and `UnityHttpClient`.
- **Transform**: Normalize, log, and manage data with `EventService` and domain models.
- **Load**: Persist data locally via `EventPersistentService` and display live metrics inside a Unity dashboard.

This project demonstrates skills in:
- Clean software architecture (Interfaces, Services, DI Installer, Config-driven design).
- Backend-style ETL workflows.
- Gameplay programming (FPS controls, scoring, object pooling).
- Unity UI integration for real-time data visualization.

---

## Architecture

- **Core/Components/Api** → API client logic  
- **Core/Services/Events** → Service layer (business logic, persistence)  
- **Core/Data** → Configs and ScriptableObjects for runtime tuning  
- **Core/Controllers** → Gameplay input and event handling  
- **Core/Utilities** → Object pooling and DI setup  

---

## Features
- 🌐 **Real-time API integration** with configurable endpoints.  
- ⚙️ **ETL pipeline** (extract → transform → load) implemented in Unity.  
- 🎮 **Gameplay layer**: FPS-style shooting targets, score system, object pooling.  
- 🧩 **Config-driven design** using ScriptableObjects.  
- 🧪 **Test harness** (`ApiTestRunner`) for validating API endpoints.  
- 📊 **UI Dashboard** displaying real-time updates.  

---

## Setup & Installation
1. Clone repo and open in Unity (2021 LTS or higher recommended).
2. Configure API settings in `ApiConfiguration.cs` or via ScriptableObject assets.
3. Run `ApiTestRunner` to validate connectivity.
4. Enter Play Mode → interact with the dashboard & gameplay while real data streams in.

---
---

## Roadmap
- Add unit tests for ETL pipeline.  
- Dockerize backend components for CI/CD demos.  
- Expand dashboard UI with graphs and filters.  
- Optional WebGL build for lightweight demos.  

---

## License
MIT
