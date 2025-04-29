# Weather App

Aplicación web que muestra el clima actual y el pronóstico del clima para los próximos 5 días en San Salvador. Esta aplicación utiliza la API de OpenWeatherMap para obtener los datos meteorológicos y está construida con ASP.NET Core y Blazor Server.

## Tecnologías Utilizadas

- **ASP.NET Core**: Framework para crear la API backend.
- **Blazor Server**: Framework para el frontend.
- **OpenWeatherMap API**: API externa para obtener los datos meteorológicos.
- **Serilog**: Para el logging de la aplicación.
- **Entity Framework**: Para gestionar la base de datos relacional (si es necesario en el futuro).

## Funcionalidades

- **Clima Actual**: Muestra la temperatura actual, descripción y el clima en San Salvador.
- **Pronóstico de Clima**: Muestra el pronóstico de clima para los siguientes 5 días, con temperaturas mínimas y máximas por día.

## Requisitos Previos

- Tener una suscripción válida para **OpenWeatherMap API** (se requiere una clave de API para acceder a los datos).
- Tener instalado .NET 8 SDK o superior.

## Configuración

### 1. Crear un archivo `appsettings.json` en el proyecto Backend (API) con tu clave de API de OpenWeatherMap:

```json
{
  "OpenWeatherMap": {
    "ApiKey": "KEY_VALUE"
  }
}
