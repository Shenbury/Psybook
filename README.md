# WMSP VIP Booking System (Psybook)

A comprehensive VIP experience booking system for West Midlands Safari Park, built with .NET 9, Blazor WebAssembly, and modern web technologies.

## 🌟 Overview

The WMSP VIP Booking System enables customers to book exclusive VIP wildlife experiences at West Midlands Safari Park. The system provides a modern, responsive web interface for managing bookings, with integrated calendar synchronization, real-time status management, and comprehensive administrative features.

## 🚀 Key Features

### 📅 **Calendar Integration**
- **Multi-Platform Support**: Google Calendar, Outlook, Apple Calendar, and iCalendar (.ics)
- **One-Click Addition**: Direct URLs for instant calendar integration
- **Auto-Sync**: Automated synchronization based on user preferences
- **iCalendar Downloads**: Universal .ics files for any calendar application
- **Real-Time Updates**: Booking changes reflect immediately in external calendars

### 🎯 **VIP Experience Management**
- **Multiple Experience Types**: Rhino Keeper, Lion Feeding, Safari Driving, and more
- **Dynamic Booking Calendar**: Interactive calendar with drag-and-drop functionality
- **Status Management**: Pending, Confirmed, Cancelled, Completed, No-Show tracking
- **Customer Information**: Comprehensive contact and address management

### 🎨 **Modern User Interface**
- **Blazor WebAssembly**: Fast, responsive client-side application
- **MudBlazor Components**: Material Design UI components
- **Responsive Design**: Optimized for desktop, tablet, and mobile devices
- **Dark/Light Theme**: User-configurable theme preferences
- **Accessibility**: WCAG-compliant interface with screen reader support

### 🔧 **Administrative Features**
- **Booking Status Workflow**: Complete lifecycle management
- **Customer Communications**: Automated notifications and confirmations
- **Reporting & Analytics**: Comprehensive booking statistics
- **Data Export**: CSV, Excel, and PDF export capabilities

## 🏗️ Architecture

### Solution Structure

```
Psybook/
├── 📁 Psybook.API/                    # RESTful API backend
├── 📁 Psybook.UI/                     # Blazor Server hosting
├── 📁 Psybook.UI.Client/              # Blazor WebAssembly client
├── 📁 Psybook.Services/               # Business logic & external integrations
├── 📁 Psybook.Repositories/           # Data access layer
├── 📁 Psybook.Objects/                # Domain models & DTOs
├── 📁 Psybook.Shared/                 # Shared components & contexts
├── 📁 Psybook.Migrations/             # Database migration service
├── 📁 Psybook.AppHost/                # .NET Aspire orchestration
└── 📁 Psybook.ServiceDefaults/        # Common service configurations
```

### Technology Stack

| Layer | Technologies |
|-------|-------------|
| **Frontend** | Blazor WebAssembly, MudBlazor, HTML5, CSS3, JavaScript |
| **Backend** | ASP.NET Core Web API, Entity Framework Core |
| **Database** | SQL Server with Entity Framework migrations |
| **Authentication** | Azure AD B2C, JWT Bearer tokens, Microsoft Identity |
| **Hosting** | .NET Aspire, Docker containerization |
| **External APIs** | Microsoft Graph, Google Calendar API |
| **Build & Deploy** | .NET 9, CI/CD pipelines, Azure DevOps |

## 📋 Detailed Project Breakdown

### 🌐 **Psybook.API**
RESTful API backend providing:
- **Booking Management**: CRUD operations for VIP bookings
- **Calendar Integration**: iCalendar generation and external calendar sync
- **Status Management**: Booking lifecycle and workflow automation
- **Authentication**: Secure API endpoints with JWT authentication
- **Data Validation**: Comprehensive input validation and sanitization

**Key Controllers:**
- `BookingController`: Core booking operations
- `CalendarIntegrationController`: External calendar sync and file generation

### 🖥️ **Psybook.UI & Psybook.UI.Client**
Modern Blazor application with:
- **Interactive Calendar**: Heron.MudCalendar integration for booking visualization
- **Responsive Components**: Mobile-first design with adaptive layouts
- **Real-Time Updates**: SignalR integration for live booking updates
- **Theme System**: Dark/light mode with user preferences
- **Progressive Web App**: Offline capability and app-like experience

**Key Components:**
- `Home.razor`: Main calendar dashboard
- `Book.razor`: Booking creation and editing
- `BookingDetailsDialog.razor`: Comprehensive booking information
- `CalendarIntegrationComponent.razor`: External calendar sync interface
- `BookingStatusManager.razor`: Status workflow management

### 🔧 **Psybook.Services**
Business logic and external integrations:
- **External Calendar Services**: Multi-platform calendar integration
- **Data Loaders**: Efficient data retrieval with caching
- **UI Clients**: HTTP client abstractions for API communication
- **User Preferences**: Calendar sync settings and user customization

**Key Services:**
- `IExternalCalendarService`: Calendar integration abstraction
- `GoogleCalendarApiService`: Google Calendar API client
- `IBookingLoaderService`: Optimized data loading with pagination
- `UserCalendarPreferencesService`: User setting management

### 💾 **Psybook.Repositories**
Data access layer with:
- **Repository Pattern**: Clean separation of data access logic
- **Entity Framework**: Code-first approach with migrations
- **Connection Pooling**: Optimized database connections
- **Query Optimization**: Efficient data retrieval strategies

### 📊 **Psybook.Objects**
Domain models and data structures:
- **Database Models**: Entity Framework entities
- **DTOs**: Data transfer objects for API communication
- **Enums**: Strongly-typed constants and status values
- **Validation Attributes**: Data annotation validation

### 🔄 **Psybook.Migrations**
Database management service:
- **Background Service**: Automated migration execution
- **Environment-Aware**: Different strategies per environment
- **Rollback Support**: Safe migration rollback capabilities
- **Health Checks**: Database connectivity monitoring

## 🛠️ Setup & Installation

### Prerequisites

- **.NET 9 SDK** (latest version)
- **SQL Server** (LocalDB, Express, or full version)
- **Visual Studio 2024** or **Visual Studio Code**
- **Node.js** (for JavaScript tooling)
- **Git** for version control

### Quick Start

1. **Clone the Repository**
```bash
git clone https://github.com/Shenbury/Psybook.git
cd Psybook
```

2. **Configure Database Connection**
```json
// appsettings.json
{
  "ConnectionStrings": {
    "wmsp-db": "Server=(localdb)\\mssqllocaldb;Database=Psybook;Trusted_Connection=true;"
  }
}
```

3. **Run Database Migrations**
```bash
dotnet ef database update --project Psybook.Shared
```

4. **Start the Application**
```bash
dotnet run --project Psybook.AppHost
```

5. **Access the Application**
- **Web Interface**: https://localhost:7031
- **API Documentation**: https://localhost:7032/swagger
- **Health Checks**: https://localhost:7033/health

### Development Setup

#### Using .NET Aspire (Recommended)

```bash
# Install .NET Aspire workload
dotnet workload install aspire

# Run the complete solution
dotnet run --project Psybook.AppHost
```

#### Manual Setup

```bash
# Start API
dotnet run --project Psybook.API

# Start UI (separate terminal)
dotnet run --project Psybook.UI

# Start Migration Service (separate terminal)
dotnet run --project Psybook.Migrations
```

## 🔗 External Calendar Integration

### Supported Platforms

| Platform | Integration Type | Features |
|----------|-----------------|----------|
| **Google Calendar** | URL + API | Quick add, full CRUD with OAuth |
| **Outlook Calendar** | URL + Graph API | Web integration, Teams sync |
| **Apple Calendar** | iCalendar files | iOS/macOS native support |
| **Universal** | .ics downloads | Any RFC 5545 compatible app |

### API Endpoints

#### Calendar Integration
```http
GET /api/calendarintegration/booking/{id}/icalendar
GET /api/calendarintegration/booking/{id}/urls
POST /api/calendarintegration/booking/{id}/sync
GET /api/calendarintegration/providers
```

#### Booking Management
```http
GET /api/booking/slots
GET /api/booking/slot/{id}
POST /api/booking/slot
PUT /api/booking/slot/{id}
DELETE /api/booking/slot/{id}
POST /api/booking/slot/{id}/status
```

### Usage Examples

#### Basic Calendar Integration
```razor
@using Psybook.Services.ExternalCalendar

<CalendarIntegrationComponent Booking="@currentBooking" 
                            ShowAutoSyncOption="true"
                            OnAutoSyncConfigured="HandleSync" />
```

#### Programmatic Calendar Operations
```csharp
// Generate calendar URLs
var calendarEvent = ExternalCalendarEvent.FromCalendarSlot(booking);
var urls = calendarService.GenerateCalendarUrls(calendarEvent);

// Create iCalendar file
var icalData = await calendarService.GenerateICalendarFileAsync(calendarEvent);

// Sync with multiple providers
var options = new CalendarIntegrationOptions
{
    EnabledProviders = { CalendarProvider.GoogleCalendar, CalendarProvider.OutlookCalendar }
};
var results = await calendarService.SyncBookingAsync(booking, options);
```

## 🧪 Testing

### Test Pages

- **Calendar Test**: `/calendar-test` - Comprehensive calendar integration testing
- **Booking Test**: Built-in booking creation and management testing
- **API Testing**: Swagger UI at `/swagger` for API endpoint testing

### Running Tests

```bash
# Unit tests
dotnet test

# Integration tests with database
dotnet test --filter Category=Integration

# End-to-end tests
dotnet test --filter Category=E2E
```

## 🚀 Deployment

### Production Deployment

#### Using Docker

```dockerfile
# Build and deploy with Docker
docker build -f Psybook.AppHost/Dockerfile -t psybook:latest .
docker run -p 8080:8080 psybook:latest
```

#### Azure Deployment

```bash
# Deploy to Azure Container Apps
az containerapp create --name psybook --resource-group rg-psybook --image psybook:latest
```

### Environment Configuration

#### Production Settings
```json
{
  "ConnectionStrings": {
    "wmsp-db": "{{AZURE_SQL_CONNECTION_STRING}}"
  },
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "Domain": "wmsp.onmicrosoft.com",
    "TenantId": "{{TENANT_ID}}",
    "ClientId": "{{CLIENT_ID}}"
  },
  "GoogleCalendar": {
    "ClientId": "{{GOOGLE_CLIENT_ID}}",
    "ClientSecret": "{{GOOGLE_CLIENT_SECRET}}"
  }
}
```

## 📊 Performance & Monitoring

### Built-in Monitoring

- **Application Insights**: Comprehensive telemetry and analytics
- **Health Checks**: Database, API, and external service monitoring
- **Structured Logging**: Serilog with multiple sinks
- **Performance Counters**: Custom metrics and KPIs

### Performance Optimizations

- **Blazor WebAssembly**: Client-side rendering for optimal performance
- **Connection Pooling**: Efficient database connection management
- **HTTP Caching**: Strategic caching for static and semi-static content
- **Lazy Loading**: Component and data lazy loading strategies
- **Compression**: Response compression for reduced bandwidth

## 🔒 Security

### Authentication & Authorization

- **Azure AD B2C**: Enterprise-grade identity management
- **JWT Tokens**: Secure API authentication
- **Role-Based Access**: Fine-grained permission system
- **HTTPS Enforcement**: TLS encryption for all communications

### Data Protection

- **Data Encryption**: At-rest and in-transit encryption
- **Input Validation**: Comprehensive sanitization and validation
- **CORS Configuration**: Secure cross-origin resource sharing
- **Rate Limiting**: API throttling and abuse prevention

## 🤝 Contributing

### Development Guidelines

1. **Code Style**: Follow .NET coding conventions and EditorConfig
2. **Testing**: Maintain test coverage above 80%
3. **Documentation**: Update documentation for all public APIs
4. **Performance**: Profile performance-critical code paths
5. **Security**: Security review for all authentication changes

### Pull Request Process

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📞 Support & Contact

### Documentation

- **API Documentation**: Available at `/swagger` when running locally
- **Calendar Integration**: See `CALENDAR_INTEGRATION.md` for detailed guide
- **Architecture Decisions**: Documented in `/docs/architecture/`

### Support Channels

- **Issues**: GitHub Issues for bug reports and feature requests
- **Discussions**: GitHub Discussions for general questions
- **Wiki**: Comprehensive documentation and tutorials

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

### Technologies & Libraries

- **Microsoft**: .NET, Blazor, Azure services
- **MudBlazor**: Material Design components for Blazor
- **Heron.MudCalendar**: Calendar component integration
- **Entity Framework**: Object-relational mapping
- **Serilog**: Structured logging framework

### Contributors

- **Development Team**: West Midlands Safari Park IT Department  
- **UI/UX Design**: Safari Park Marketing Team
- **Testing**: Quality Assurance Team
- **Infrastructure**: DevOps and Cloud Operations Team

---

## 🔧 Development Status

| Feature | Status | Version |
|---------|--------|---------|
| **Core Booking System** | ✅ Complete | v1.0.0 |
| **Calendar Integration** | ✅ Complete | v1.1.0 |
| **Mobile Optimization** | ✅ Complete | v1.0.0 |
| **API Documentation** | ✅ Complete | v1.0.0 |
| **Authentication** | ✅ Complete | v1.0.0 |
| **Reporting Dashboard** | 🚧 In Progress | v1.2.0 |
| **Mobile App** | 📅 Planned | v2.0.0 |
| **Advanced Analytics** | 📅 Planned | v2.1.0 |

**Current Version**: v1.1.0  
**Next Release**: v1.2.0 (Q2 2024)  
**LTS Support**: .NET 9 through November 2026

---

*Built with ❤️ by the West Midlands Safari Park development team for creating unforgettable VIP wildlife experiences.*